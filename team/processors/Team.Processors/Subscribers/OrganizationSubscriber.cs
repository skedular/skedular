using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Enterprise.Shared.Kafka.Consume;
using Team.Processors.Mappers;
using Team.Shared.Models;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Organization = Team.Shared.Database.Entities.Organization;
using OrganizationMember = Team.Shared.Database.Entities.OrganizationMember;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Type;

namespace Team.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICachedOrganizationService cachedOrganizationService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.OrganizationUpserted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organization.Id, cancellationToken);
                    if (existingOrganization.EventRaisedAt > organization.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Organization event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleOrganizationUpsertedEventAsync(organization, existingOrganization, cancellationToken);
                }
                break;

            case Type.OrganizationDeleted:
                {
                    var organization = mapper.MapTo(@event);
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                        organization.Id,
                        null,
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
        existingOrganization = repositoryFactory.OrganizationRepository.Update(mapper.MergeToEntity(organization, existingOrganization));

        existingOrganization = await RebuildOrganizationMembersAsync(organization, existingOrganization, cancellationToken);
        _ = RebuildOrganizationSsoSettings(organization.OrganizationSsoSettings, existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(
            existingOrganization.Id,
            existingOrganization.CustomDomain,
            cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        repositoryFactory.OrganizationMemberRepository.RemoveRange(existingOrganization.OrganizationMembers.ToList());
        existingOrganization.CustomDomain = null;
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(
            existingOrganization.Id,
            existingOrganization.CustomDomain,
            cancellationToken);
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
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                organizationMember.Customer.Id,
                cancellationToken);
            var updatedOrganizationMember = mapper.MergeToEntity(
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
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                organizationMember.Customer.Id,
                cancellationToken);
            addedItems.Add(
                repositoryFactory.OrganizationMemberRepository.Add(
                    mapper.MapToEntity(organizationMember, existingOrganization, customer)));
        }

        repositoryFactory.OrganizationMemberRepository.RemoveRange(itemsToRemove);
        existingOrganization.OrganizationMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

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
                        repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapTo(ssoSettings, organization));
                    }
                    else if (ssoSettings is not null && organization.OrganizationSsoSettings is not null)
                    {
                        if (ssoSettings.Id == organization.OrganizationSsoSettings.Id)
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Update(
                                mapper.MergeTo(ssoSettings, organization.OrganizationSsoSettings, organization));
                        }
                        else
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
                            repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapTo(ssoSettings, organization));
                        }
                    }

                    break;
                }
        }

        return organization;
    }
}
