using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Activities;
using Customer = Organization.Shared.Models.Customer;
using Location = Organization.Shared.Database.Entities.Location;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using CustomerService = Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService;
using LocationConfiguration = Api.Shared.Clients.Configurations.Grpc.LocationConfiguration;
using CustomerConfiguration = Api.Shared.Clients.Configurations.Grpc.CustomerConfiguration;

namespace Organization.Shared.Activities;

public class AzureTenantIntegrations(
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IMapper mapper,
    IGraphService graphService,
    CustomerService.CustomerServiceClient customerServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    IOrganizationMemberService organizationMemberService,
    ITemporalService temporalService)
{
    [Activity]
    public async Task<bool> ReSyncTenantAsync(string tenantId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var tenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null || tenant.IsDeleted())
        {
            return false;
        }

        var azureTenantMembers = await graphService.GetAzureTenantMembersAsync(tenantId, cancellationToken);
        var existingAzureTenantMembers = await repositoryFactory.AzureTenantMemberRepository.GetByTenantIdAsync(
            tenantId,
            cancellationToken);
        var itemsToRemove = existingAzureTenantMembers
            .Where(azureTenantMember => azureTenantMembers.All(item => item.Id != azureTenantMember.Id))
            .ToList();
        var updatedItems = existingAzureTenantMembers
            .Where(azureTenantMember => azureTenantMembers.Any(item => item.Id == azureTenantMember.Id))
            .Select(azureTenantMember =>
            {
                var updatedAzureTenantMembers = mapper.MergeToEntity(
                    azureTenantMembers.First(item => item.Id == azureTenantMember.Id),
                    azureTenantMember,
                    tenant);
                updatedAzureTenantMembers.DeletedAt = null;
                return repositoryFactory.AzureTenantMemberRepository.Update(updatedAzureTenantMembers);
            })
            .ToList();
        var addedItems = azureTenantMembers
            .Where(azureTenantMember => existingAzureTenantMembers.All(item => item.Id != azureTenantMember.Id))
            .Select(item => repositoryFactory.AzureTenantMemberRepository.Add(mapper.MapTo(item, tenant)))
            .ToList();

        repositoryFactory.AzureTenantMemberRepository.RemoveRange(itemsToRemove);
        tenant.AzureTenantMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        await SyncCustomersAndOrganizationMembersAsync(tenant, cancellationToken);

        repositoryFactory.AzureTenantRepository.Update(tenant);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    [Activity]
    public async Task ExecuteNextReSyncAzureTenantWorkflowAsync(string tenantId)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        await temporalService.StartWorkflowReSyncAzureTenantAsync(
            new ReSyncAzureTenantInput(tenantId, timeProvider.GetUtcNow().AddDays(1)),
            cancellationToken);
    }

    private async Task SyncCustomersAndOrganizationMembersAsync(AzureTenant azureTenant, CancellationToken cancellationToken)
    {
        var getPaginatedLocationsInput = new Admin_GetPaginatedLocationsInput
        {
            First = ((int?)null).ToNullInt(),
            Last = ((int?)null).ToNullInt(),
            Where = new LocationWhereInput { OrganizationId = azureTenant.Organization.Id }
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
            var anyCustomerExistByVerifiableTokenResponse = await customerServiceClient.Admin_AnyCustomerExistByVerifiableTokenAsync(
                new Admin_AnyCustomerExistByVerifiableTokenInput { VerifiableToken = tenantMember.Id },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
            if (anyCustomerExistByVerifiableTokenResponse.Exist)
            {
                customerIdsTenantMembersPair.Add((anyCustomerExistByVerifiableTokenResponse.Customer.Id, tenantMember));

                await customerServiceClient.Admin_UpdateIdentityAsync(
                    mapper.MapToUpdateIdentityInput(tenantMember, anyCustomerExistByVerifiableTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(anyCustomerExistByVerifiableTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = azureTenant.Organization.Id, CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddPreferredLocationAsync(
                        new Admin_AddPreferredLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id,
                            CustomerId = anyCustomerExistByVerifiableTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                continue;
            }

            var anyCustomerExistByEmailTokenResponse = await customerServiceClient.Admin_AnyCustomerExistByEmailAsync(
                new Admin_AnyCustomerExistByEmailInput { Email = tenantMember.Email },
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
            if (anyCustomerExistByEmailTokenResponse.Exist)
            {
                customerIdsTenantMembersPair.Add((anyCustomerExistByEmailTokenResponse.Customer.Id, tenantMember));

                await customerServiceClient.Admin_AddIdentityAsync(
                    mapper.MapTo(tenantMember, anyCustomerExistByEmailTokenResponse.Customer.Id),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                if (string.IsNullOrWhiteSpace(anyCustomerExistByEmailTokenResponse.Customer.DefaultOrganization?.Id))
                {
                    await customerServiceClient.Admin_SetDefaultOrganizationAsync(
                        new Admin_SetDefaultOrganizationInput
                        {
                            OrganizationId = azureTenant.Organization.Id, CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
                        },
                        customerConfiguration.ApiKey.CreateMetadata(),
                        cancellationToken: cancellationToken);
                }

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddPreferredLocationAsync(
                        new Admin_AddPreferredLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id, CustomerId = anyCustomerExistByEmailTokenResponse.Customer.Id
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
                    new Database.Entities.Organization { Id = azureTenant.Organization.Id },
                    getLocationsResponse.TotalCount == 1 ? [new Location { Id = getLocationsResponse.Edges.First().Node.Id }] : []),
                customerConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
        }

        var members = customerIdsTenantMembersPair.Select(customerIdsTenantMemberPair =>
        {
            var customerId = customerIdsTenantMemberPair.Item1;
            var organizationMember = azureTenant.Organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customerId);

            if (organizationMember is null)
            {
                return new OrganizationMember
                {
                    Id = randomHelper.Generate(),
                    Customer = new Customer { Id = customerId },
                    Status = OrganizationMemberStatus.Active,
                    Role = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                        ? OrganizationMemberRole.Owner
                        : OrganizationMemberRole.Member,
                    IsOrganizationOnboardingDone = true
                };
            }

            return new OrganizationMember
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                Status = OrganizationMemberStatus.Active,
                Role = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                    ? OrganizationMemberRole.Owner
                    : OrganizationMemberRole.Member,
                IsOrganizationOnboardingDone = true
            };
        }).ToList();

        await organizationMemberService.AddMembersAsync(azureTenant.Organization.Id, members, cancellationToken);
    }
}
