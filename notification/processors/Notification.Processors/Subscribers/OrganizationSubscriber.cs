using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Notification.Processors.Mappers;
using Notification.Shared.Models;
using Notification.Shared.Repositories;
using Organization = Notification.Shared.Database.Entities.Organization;
using Type = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Type;

namespace Notification.Processors.Subscribers;

public class OrganizationSubscriber(
    ILogger<OrganizationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
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
                    var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
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

        _ = RebuildOrganizationSsoSettings(organization.OrganizationSsoSettings, existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
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
