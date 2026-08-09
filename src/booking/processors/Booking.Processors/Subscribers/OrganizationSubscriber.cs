using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Processors.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Kafka.Consume;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;
using OrganizationType = Api.Shared.Services.Models.OrganizationType;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Type;

namespace Booking.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IEventMapper eventMapper,
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    ICachedOrganizationService cachedOrganizationService)
    : IEventSubscriber<Key, Event>
{
    private const int FreePlanCode = 1;
    private const int LegacyEarlyBirdPlanCode = 4;
    private const int GrowthPlanCode = 5;
    private const int BusinessPlanCode = 6;
    private const int ContactUsPlanCode = 7;

    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = eventMapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                        organization.Id,
                        null,
                        true,
                        true,
                        cancellationToken);
                    if (existingOrganization is not null && existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    existingOrganization ??= await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organization.Id, cancellationToken);

                    await HandleOrganizationUpsertedEventAsync(organization, existingOrganization,
                        cancellationToken);
                }
                break;

            case Type.OrganizationDeleted:
                {
                    var organization = eventMapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                        organization.Id,
                        null,
                        true,
                        true,
                        cancellationToken);
                    if (existingOrganization is not null && existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingOrganization is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationDeletedEventAsync(existingOrganization, cancellationToken);
                }
                break;

            case Type.OrganizationOfferingUpdated:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleOrganizationUpsertedEventAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var hadMarketplaceBillingWorkflow = existingOrganization.Type == OrganizationTypeConstants.Marketplace;

        existingOrganization = repositoryFactory.OrganizationRepository.Update(eventMapper.MergeToEntity(organization, existingOrganization));

        existingOrganization = RebuildOrganizationTags(organization, existingOrganization);
        existingOrganization = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        _ = RebuildOrganizationSsoSettings(organization.OrganizationSsoSettings, existingOrganization);
        SaveSpacesOfferingState(organization, existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await QueueArrearsBillingWorkflowAsync(
            existingOrganization,
            hadMarketplaceBillingWorkflow,
            cancellationToken);
        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(existingOrganization.Id, existingOrganization.CustomDomain, cancellationToken);
    }

    private void SaveSpacesOfferingState(
        Shared.Models.Organization organization,
        Organization existingOrganization)
    {
        if (organization.Type != OrganizationType.Marketplace || organization.Offering is null
                                                              || !IsSpacesOffering(organization.Offering.Code))
        {
            return;
        }

        var existingOffering = existingOrganization.Offering ?? new Offering();
        var planCode = GetPlanCode(organization.Offering.Code);
        var quotaLimit = GetQuotaLimit(organization.Offering);
        var customCapacity = organization.Offering.Code == OfferingCode.SpacesContactUsV1
            ? organization.Offering.PurchasedTeamCapacity
            : null;

        existingOffering.SpacesPlanCode = planCode;
        existingOffering.SpacesQuotaLimit = quotaLimit;
        existingOffering.SpacesCustomCapacity = customCapacity;
        existingOffering.SpacesPeriodStart = organization.Offering.Start;
        existingOffering.SpacesPeriodEnd = organization.Offering.End;

        existingOrganization.Offering = existingOffering;
        repositoryFactory.OrganizationRepository.Update(existingOrganization);
    }

    private static bool IsSpacesOffering(OfferingCode offeringCode) =>
        offeringCode is OfferingCode.EarlyBirdV1 or OfferingCode.SpacesFreeTierV1 or OfferingCode.SpacesGrowthV1
            or OfferingCode.SpacesBusinessV1 or OfferingCode.SpacesContactUsV1;

    private static int GetPlanCode(OfferingCode offeringCode) =>
        offeringCode switch
        {
            OfferingCode.EarlyBirdV1 => LegacyEarlyBirdPlanCode,
            OfferingCode.SpacesFreeTierV1 => FreePlanCode,
            OfferingCode.SpacesGrowthV1 => GrowthPlanCode,
            OfferingCode.SpacesBusinessV1 => BusinessPlanCode,
            OfferingCode.SpacesContactUsV1 => ContactUsPlanCode,
            _ => FreePlanCode,
        };

    private static int? GetQuotaLimit(Offering offering) =>
        offering.Code switch
        {
            OfferingCode.EarlyBirdV1 => null,
            OfferingCode.SpacesContactUsV1 => offering.PurchasedTeamCapacity,
            _ => offering.PurchasedTeamCapacity ?? offering.Code.GetOffering().MaxBookingInstanceCount,
        };

    private async Task QueueArrearsBillingWorkflowAsync(
        Organization organization,
        bool hadMarketplaceBillingWorkflow,
        CancellationToken cancellationToken)
    {
        if (organization.Type != OrganizationTypeConstants.Marketplace)
        {
            if (hadMarketplaceBillingWorkflow)
            {
                await temporalService.SignalRunOrganizationArrearsBillingWorkflowStopAsync(organization.Id, cancellationToken);
            }

            return;
        }

        var configuration = new OrganizationArrearsBillingConfiguration(
            organization.Id,
            organization.BillingCycle.ToOrganizationBillingCycle());

        // Always push the current configuration for marketplace organizations; the signal path
        // is responsible for starting the workflow if it is not already running.
        await temporalService.SignalRunOrganizationArrearsBillingWorkflowUpdateConfigurationAsync(
            organization.Id,
            configuration,
            cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        if (existingOrganization.Type == OrganizationTypeConstants.Marketplace)
        {
            await temporalService.SignalRunOrganizationArrearsBillingWorkflowStopAsync(existingOrganization.Id, cancellationToken);
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers);
        existingOrganization.CustomDomain = null;
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(existingOrganization.Id, existingOrganization.CustomDomain, cancellationToken);
    }

    private async Task<Organization> RebuildOrganizationMembersAsync(
        Shared.Models.Organization organization,
        Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        var organizationMembers = await repositoryFactory.OrganizationMemberRepository.GetByOrganizationIdAsync(
            existingOrganization.Id,
            cancellationToken);
        var itemsToRemove = organizationMembers
            .Where(organizationMember => organization.OrganizationMembers.All(item => item.Id != organizationMember.Id))
            .ToList();
        var updatedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organizationMembers
                     .Where(organizationMember => organization.OrganizationMembers.Any(item => item.Id == organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id, false, cancellationToken);
            var updatedOrganizationMember = eventMapper.MergeToEntity(
                organization.OrganizationMembers.First(item => item.Id == organizationMember.Id),
                organizationMember,
                existingOrganization,
                customer);
            updatedOrganizationMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(updatedOrganizationMember));
        }

        var addedItems = new List<OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers
                     .Where(organizationMember => organizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(organizationMember.Customer.Id, false, cancellationToken);
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(eventMapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = [.. addedItems, .. updatedItems, .. itemsToRemove];

        return existingOrganization;
    }

    private Organization RebuildOrganizationTags(Shared.Models.Organization organization, Organization existingOrganization)
    {
        var itemsToRemove = existingOrganization.Tags.Where(tag => organization.Tags.All(item => item.Id != tag.Id)).ToList();
        var updatedItems = existingOrganization.Tags
            .Where(tag => organization.Tags.Any(item => item.Id == tag.Id))
            .Select(organizationTag =>
            {
                var updatedOrganizationTag = eventMapper.MergeToEntity(
                    organization.Tags.First(item => item.Id == organizationTag.Id),
                    organizationTag,
                    existingOrganization);

                updatedOrganizationTag.DeletedAt = null;
                return repositoryFactory.OrganizationTagRepository.Update(updatedOrganizationTag);
            })
            .ToList();
        var addedItems = organization.Tags
            .Where(tag => existingOrganization.Tags.All(item => item.Id != tag.Id))
            .Select(organizationTag =>
                repositoryFactory.OrganizationTagRepository.Add(eventMapper.MapToEntity(organizationTag, existingOrganization)))
            .ToList();

        repositoryFactory.OrganizationTagRepository.RemoveRange(itemsToRemove);
        existingOrganization.Tags = [.. addedItems, .. updatedItems, .. itemsToRemove];

        return existingOrganization;
    }

    private Organization RebuildOrganizationSsoSettings(OrganizationSsoSetting? ssoSettings, Organization organization)
    {
        switch (ssoSettings)
        {
            case null when organization.OrganizationSsoSettings is null:
                // No need to do anything
                break;

            case null when organization.OrganizationSsoSettings is not null:
                repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
                break;

            default:
                {
                    if (ssoSettings is not null && organization.OrganizationSsoSettings is null)
                    {
                        repositoryFactory.OrganizationSsoSettingRepository.Add(eventMapper.MapTo(ssoSettings, organization));
                    }
                    else if (ssoSettings is not null && organization.OrganizationSsoSettings is not null)
                    {
                        if (ssoSettings.Id == organization.OrganizationSsoSettings.Id)
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Update(
                                eventMapper.MergeTo(ssoSettings, organization.OrganizationSsoSettings, organization));
                        }
                        else
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
                            repositoryFactory.OrganizationSsoSettingRepository.Add(eventMapper.MapTo(ssoSettings, organization));
                        }
                    }

                    break;
                }
        }

        return organization;
    }
}
