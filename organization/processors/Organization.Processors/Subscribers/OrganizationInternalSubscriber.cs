using Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Key;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Offering;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Kafka.Consume;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Organization.Processors.Mappers;
using Organization.Processors.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Customer = Organization.Shared.Models.Customer;
using LocationConfiguration = Organization.Shared.Configurations.LocationConfiguration;
using CustomerConfiguration = Organization.Shared.Configurations.CustomerConfiguration;
using Event = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Event;
using Location = Organization.Shared.Database.Entities.Location;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using Type = Api.Shared.Clients.Events.UnityHub.OrganizationInternal.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationInternalSubscriber(
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IMapper mapper,
    IMsGraphService msGraphService,
    CustomerService.CustomerServiceClient customerServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationMemberService organizationMemberService)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RenewOrganizationOffering:
                await HandleRenewOrganizationOfferingEventAsync(@event, cancellationToken);
                break;

            case Type.RecordDailyMemberCount:
                await HandleRecordDailyMemberCountEventAsync(@event, cancellationToken);
                break;

            case Type.RefreshAzureTenantMembers:
                await HandleRefreshAzureTenantMembersAsync(@event.AzureTenantId, cancellationToken);
                break;

            default:
                return;
        }
    }

    private async Task HandleRenewOrganizationOfferingEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(@event.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var expiredOfferingsRequireAutoRenew = await repositoryFactory.OrganizationOfferingRepository
            .Query(new Specification<OrganizationOffering>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Organization.Id == @event.OrganizationId && query.End <= now &&
                    query.AutoRenew
            }.ApplyOrderByDescending(query => query.End))
            .ToListAsync(cancellationToken);

        if (expiredOfferingsRequireAutoRenew.Count == 0)
        {
            return;
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.OrganizationOfferingRepository.UnitOfWork,
                cancellationToken);

        var expiredOfferingRequireAutoRenew = expiredOfferingsRequireAutoRenew.First();
        var offering = expiredOfferingRequireAutoRenew.Code.GetOffering();
        var start = expiredOfferingRequireAutoRenew.End.GetNextOfferingPeriodStart();

        _ = repositoryFactory.OrganizationOfferingRepository.Add(new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            Code = expiredOfferingRequireAutoRenew.Code,
            Start = start,
            End = start.GetOfferingPeriodEnd(),
            AutoRenew = expiredOfferingRequireAutoRenew.AutoRenew,
            UnitPrice = offering.UnitPrice,
            Organization = organization
        });
        repositoryFactory.OrganizationOfferingRepository.RemoveRange(expiredOfferingsRequireAutoRenew);

        var mappedOrganization = mapper.MapTo(organization);
        mappedOrganization.OrganizationOfferings =
        [
            mappedOrganization.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
                .OrderByDescending(item => item.End).First()
        ];

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mappedOrganization],
            repositoryFactory.OrganizationOfferingRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationOfferingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task HandleRecordDailyMemberCountEventAsync(Event @event, CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(@event.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return;
        }

        var startOfToday = timeProvider.GetUtcNow().StartOfDay();
        if (await repositoryFactory.DailyMemberCountRecordingRepository
                .Query(new Specification<DailyMemberCountRecording>
                {
                    Criteria = query =>
                        !query.DeletedAt.HasValue && query.Organization.Id == @event.OrganizationId &&
                        query.Date == startOfToday
                }).AnyAsync(cancellationToken))
        {
            return;
        }

        _ = repositoryFactory.DailyMemberCountRecordingRepository.Add(new DailyMemberCountRecording
        {
            Id = randomHelper.Generate(),
            Count = organization.OrganizationMembers.Count(item => item.DeletedAt is null),
            Date = startOfToday,
            Organization = organization
        });

        organization.DailyMemberCountLastRecordedAt = timeProvider.GetUtcNow();
        _ = repositoryFactory.OrganizationRepository.Update(organization);

        await repositoryFactory.DailyMemberCountRecordingRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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

                await customerServiceClient.Admin_UpdateIdentityAsync(
                    mapper.MapToUpdateIdentityInput(tenantMember, anyCustomerExistByVerifiableTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

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
                    new Shared.Database.Entities.Organization { Id = azureTenant.Organization.Id },
                    getLocationsResponse.TotalCount == 1
                        ? [new Location { Id = getLocationsResponse.Edges.First().Node.Id }]
                        : []),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
        }

        var members = customerIdsTenantMembersPair.Select(customerIdsTenantMemberPair =>
        {
            var customerId = customerIdsTenantMemberPair.Item1;
            var organizationMember =
                azureTenant.Organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customerId);

            if (organizationMember is null)
            {
                return new OrganizationMember
                {
                    Id = randomHelper.Generate(),
                    Customer = new Customer { Id = customerId },
                    MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                        ? OrganizationMembershipType.Owner
                        : OrganizationMembershipType.Member
                };
            }

            return new OrganizationMember
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                    ? OrganizationMembershipType.Owner
                    : OrganizationMembershipType.Member
            };
        }).ToList();

        await organizationMemberService.AddMembersAsync(azureTenant.Organization.Id, members, cancellationToken);
    }
}
