using Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Offering;
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
using Customer = Organization.Shared.Models.Customer;
using LocationConfiguration = Organization.Shared.Configurations.LocationConfiguration;
using CustomerConfiguration = Organization.Shared.Configurations.CustomerConfiguration;
using Event = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event;
using Location = Organization.Shared.Database.Entities.Location;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using Type = Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Type;

namespace Organization.Processors.Subscribers;

public class OrganizationInternalSubscriber(
    CustomerConfiguration customerConfiguration,
    LocationConfiguration locationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IMapper mapper,
    IGraphService graphService,
    CustomerService.CustomerServiceClient customerServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationMemberService organizationMemberService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
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
        }

        return EventSubscriberResults.Success;
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
        var existingTenant = await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (existingTenant is null)
        {
            return;
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
                    existingTenant);
                updatedAzureTenantMembers.DeletedAt = null;
                return repositoryFactory.AzureTenantMemberRepository.Update(updatedAzureTenantMembers);
            })
            .ToList();
        var addedItems = azureTenantMembers
            .Where(azureTenantMember => existingAzureTenantMembers.All(item => item.Id != azureTenantMember.Id))
            .Select(item => repositoryFactory.AzureTenantMemberRepository.Add(mapper.MapTo(item, existingTenant)))
            .ToList();

        repositoryFactory.AzureTenantMemberRepository.RemoveRange(itemsToRemove);
        existingTenant.AzureTenantMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        await SyncCustomersAndOrganizationMembersAsync(existingTenant, cancellationToken);

        existingTenant.MembersLastRefreshedAt = timeProvider.GetUtcNow();
        repositoryFactory.AzureTenantRepository.Update(existingTenant);

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
                    mapper.MapToUpdateIdentityInput(
                        tenantMember,
                        anyCustomerExistByVerifiableTokenResponse.Customer.Id),
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

                if (getLocationsResponse.TotalCount == 1)
                {
                    await customerServiceClient.Admin_AddDefaultLocationAsync(
                        new Admin_AddDefaultLocationInput
                        {
                            LocationId = getLocationsResponse.Edges.First().Node.Id,
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
                    Status = OrganizationMemberStatus.Active,
                    MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                        ? OrganizationMembershipType.Owner
                        : OrganizationMembershipType.Member,
                    IsOrganizationOnboardingDone = true
                };
            }

            return new OrganizationMember
            {
                Id = organizationMember.Id,
                Customer = new Customer { Id = customerId },
                Status = OrganizationMemberStatus.Active,
                MembershipType = customerIdsTenantMemberPair.Item2.Id == azureTenant.InstalledByUserId
                    ? OrganizationMembershipType.Owner
                    : OrganizationMembershipType.Member,
                IsOrganizationOnboardingDone = true
            };
        }).ToList();

        await organizationMemberService.AddMembersAsync(azureTenant.Organization.Id, members, cancellationToken);
    }
}
