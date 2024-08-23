using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Confluent.Kafka;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using MsTeams.Processors.Mappers;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;
using Customer = Api.Shared.Services.Grpc.UnityHub.Organization.V1.Customer;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;
using CustomerConfiguration = MsTeams.Shared.Configurations.CustomerConfiguration;
using Location = MsTeams.Shared.Database.Entities.Location;
using OrganizationConfiguration = MsTeams.Shared.Configurations.OrganizationConfiguration;
using LocationConfiguration = MsTeams.Shared.Configurations.LocationConfiguration;
using Member = Api.Shared.Services.Grpc.UnityHub.Organization.V1.Member;
using MembershipType = Api.Shared.Services.Grpc.UnityHub.Organization.V1.MembershipType;
using OrderDirection = Api.Shared.Services.Grpc.UnityHub.Location.V1.OrderDirection;
using Organization = MsTeams.Shared.Database.Entities.Organization;

namespace MsTeams.Processors.Subscribers;

public class MsTeamsInternalSubscriber(
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    OrganizationConfiguration organizationConfiguration,
    TimeProvider timeProvider,
    IMsGraphService msGraphService,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    CustomerService.CustomerServiceClient customerServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRandomHelper randomHelper)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RefreshTenantMembers:
                await HandleRefreshAzureTenantMembersAsync(@event.TenantId, cancellationToken);
                break;

            default:
                return;
        }
    }

    private async Task HandleRefreshAzureTenantMembersAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var tenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return;
        }

        var users = await msGraphService.GetUsersAsync(tenantId, cancellationToken);
        var itemsToRemove = tenant.AzureTenantMembers
            .Where(tenantMember => users.All(item => item.Id != tenantMember.Id))
            .ToList();
        var updatedItems = tenant.AzureTenantMembers
            .Where(tenantMember => users.Any(item => item.Id == tenantMember.Id))
            .ToList();
        var addedItems = users
            .Where(tenantMember => tenant.AzureTenantMembers.All(item => item.Id != tenantMember.Id))
            .Select(user => repositoryFactory.AzureTenantMemberRepository.Add(mapper.MapToEntity(user))).ToList();

        repositoryFactory.AzureTenantMemberRepository.RemoveRange(itemsToRemove);
        tenant.AzureTenantMembers = addedItems.Concat(updatedItems).ToList();

        await SyncCustomersAndOrganizationMembersAsync(tenant, cancellationToken);

        tenant.MembersLastRefreshedAt = timeProvider.GetUtcNow();
        repositoryFactory.AzureTenantRepository.Update(tenant);

        await repositoryFactory.AzureTenantMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.AzureTenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task SyncCustomersAndOrganizationMembersAsync(
        AzureTenant azureTenant,
        CancellationToken cancellationToken)
    {
        var getPaginatedLocationsInput = new Admin_GetPaginatedLocationsInput
        {
            First = -1, Last = -1, Where = new LocationWhereInput { OrganizationId = azureTenant.Organization.Id }
        };
        getPaginatedLocationsInput.OrderBy.AddRange([
            new LocationOrderInput { Direction = OrderDirection.Ascending, Field = LocationOrderField.Name }
        ]);
        var getLocationsResponse = await locationServiceClient.Admin_GetPaginatedLocationsAsync(
            getPaginatedLocationsInput,
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        var customerIdsTenantMembersPair = new List<(string, AzureTenantMember)>();

        foreach (var tenantMember in azureTenant.AzureTenantMembers)
        {
            var anyCustomerExistByVerifiableTokenResponse =
                await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
                    new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = tenantMember.Id },
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);
            if (anyCustomerExistByVerifiableTokenResponse.Exist)
            {
                customerIdsTenantMembersPair.Add(
                    (anyCustomerExistByVerifiableTokenResponse.Customer.Id, tenantMember));

                if (string.IsNullOrWhiteSpace(
                        anyCustomerExistByVerifiableTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = azureTenant.Organization.Id,
                            CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                continue;
            }

            var anyCustomerExistByEmailTokenResponse =
                await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
                    new Admin_AnyCustomerExistByEmailInput { Email = tenantMember.Email },
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);
            if (anyCustomerExistByEmailTokenResponse.Exist)
            {
                customerIdsTenantMembersPair.Add(
                    (anyCustomerExistByEmailTokenResponse.Customer.Id, tenantMember));
                await customerServiceClient.Admin_AddIdentityAsync(
                    mapper.MapTo(tenantMember, anyCustomerExistByEmailTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(
                        anyCustomerExistByEmailTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = azureTenant.Organization.Id,
                            CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddDefaultLocationAsync(
                        new Admin_AddDefaultLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id,
                            CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                continue;
            }

            var customerId = randomHelper.Generate();
            customerIdsTenantMembersPair.Add((customerId, tenantMember));
            await customerServiceClient.Admin_AddAsync(
                mapper.MapTo(
                    tenantMember,
                    customerId,
                    new Organization { Id = azureTenant.Organization.Id },
                    getLocationsResponse.TotalCount == 1
                        ? [new Location { Id = getLocationsResponse.Edges.First().Node.Id }]
                        : []),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
        }

        await customerIdsTenantMembersPair.Select(customerIdsTenantMemberPair =>
        {
            var customerId = customerIdsTenantMemberPair.Item1;
            var organizationMember =
                azureTenant.Organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customerId);

            if (organizationMember is null)
            {
                return new Member
                {
                    Id = randomHelper.Generate(),
                    Customer = new Customer { Id = customerId },
                    MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                        ? MembershipType.Owner
                        : MembershipType.Member
                };
            }

            return new Member
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                    ? MembershipType.Owner
                    : MembershipType.Member
            };
        }).ForEachAsync(async (member, ct) =>
        {
            await organizationServiceClient.Admin_AddMemberAsync(
                new Admin_AddMemberInput { Id = azureTenant.Organization.Id, Member = member },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: ct);
        }, cancellationToken);
    }
}
