using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Notification.Processors.Mappers;
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

            case Type.InvitationToJoinOrganizationUpserted:
                {
                    var notification = mapper.MapInvitationToJoinOrganizationToNotification(@event);
                    var existingNotification = await repositoryFactory.NotificationRepository.GetBySourceIdAsync(
                        notification.SourceId,
                        cancellationToken);
                    if (existingNotification is not null && existingNotification.EventRaisedAt > notification.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Notification event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleNotificationUpsertedEventAsync(notification, existingNotification, cancellationToken);
                }
                break;

            case Type.InvitationToJoinOrganizationDeleted:
                {
                    var notification = mapper.MapInvitationToJoinOrganizationToNotification(@event);
                    var existingNotification = await repositoryFactory.NotificationRepository.GetBySourceIdAsync(
                        notification.SourceId,
                        cancellationToken);
                    if (existingNotification is not null && existingNotification.EventRaisedAt > notification.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Notification event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingNotification is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleNotificationDeletedEventAsync(existingNotification, cancellationToken);
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

        _ = RebuildOrganizationSsoSettings(organization, existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleOrganizationDeletedEventAsync(Organization existingOrganization, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.OrganizationRepository.Remove(existingOrganization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleNotificationUpsertedEventAsync(
        Shared.Models.Notification notification,
        Shared.Database.Entities.Notification? existingNotification,
        CancellationToken cancellationToken)
    {
        var invitedBy = string.IsNullOrWhiteSpace(notification.InvitedBy?.Id)
            ? null
            : await repositoryFactory.CustomerRepository.UpsertNakedAsync(notification.InvitedBy.Id, cancellationToken);

        var invitee = string.IsNullOrWhiteSpace(notification.Invitee?.Id)
            ? null
            : await repositoryFactory.CustomerRepository.UpsertNakedAsync(notification.Invitee.Id, cancellationToken);

        var organization = string.IsNullOrWhiteSpace(notification.Organization?.Id)
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(notification.Organization.Id, cancellationToken);

        _ = existingNotification is null
            ? repositoryFactory.NotificationRepository.Add(mapper.MapToEntity(notification, invitedBy, invitee, organization, null, null))
            : repositoryFactory.NotificationRepository.Update(
                mapper.MergeToEntity(notification, existingNotification, invitedBy, invitee, organization, null, null));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleNotificationDeletedEventAsync(
        Shared.Database.Entities.Notification existingNotification,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.NotificationRepository.Remove(existingNotification);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private Organization RebuildOrganizationSsoSettings(Shared.Models.Organization organization, Organization existingOrganization)
    {
        switch (organization.OrganizationSsoSettings)
        {
            case null when existingOrganization.OrganizationSsoSettings is null:
                // No need to do anything
                break;

            case null when existingOrganization.OrganizationSsoSettings is not null:
                repositoryFactory.OrganizationSsoSettingRepository.Remove(existingOrganization.OrganizationSsoSettings);
                break;

            default:
                {
                    if (organization.OrganizationSsoSettings is not null && existingOrganization.OrganizationSsoSettings is null)
                    {
                        repositoryFactory.OrganizationSsoSettingRepository.Add(
                            mapper.MapTo(organization.OrganizationSsoSettings, existingOrganization));
                    }
                    else if (organization.OrganizationSsoSettings is not null && existingOrganization.OrganizationSsoSettings is not null)
                    {
                        if (organization.OrganizationSsoSettings.Id == existingOrganization.OrganizationSsoSettings.Id)
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Update(
                                mapper.MergeTo(
                                    organization.OrganizationSsoSettings,
                                    existingOrganization.OrganizationSsoSettings,
                                    existingOrganization));
                        }
                        else
                        {
                            repositoryFactory.OrganizationSsoSettingRepository.Remove(existingOrganization.OrganizationSsoSettings);
                            repositoryFactory.OrganizationSsoSettingRepository.Add(
                                mapper.MapTo(organization.OrganizationSsoSettings, existingOrganization));
                        }
                    }

                    break;
                }
        }

        return existingOrganization;
    }
}
