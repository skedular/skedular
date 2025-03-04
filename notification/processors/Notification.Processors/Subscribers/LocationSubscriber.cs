using Api.Shared.Clients.Events.Skedular.Location.V1.Key;
using Enterprise.Shared.Kafka.Consume;
using Notification.Shared.Repositories;
using Event = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event;
using IMapper = Notification.Processors.Mappers.IMapper;
using Location = Notification.Shared.Database.Entities.Location;
using Type = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Type;

namespace Notification.Processors.Subscribers;

public class LocationSubscriber(
    ILogger<LocationSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.LocationUpserted:
                {
                    var location = mapper.MapTo(@event);
                    var organization = location.Organization is null
                        ? null
                        : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(location.Organization.Id, cancellationToken);
                    var existingLocation = await repositoryFactory.LocationRepository.UpsertNakedAsync(location.Id, organization, cancellationToken);
                    if (existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationUpsertedEventAsync(location, existingLocation, cancellationToken);
                }
                break;

            case Type.LocationDeleted:
                {
                    var location = mapper.MapTo(@event);
                    var existingLocation = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);
                    if (existingLocation is not null && existingLocation.EventRaisedAt > location.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Location event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingLocation is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleLocationDeletedEventAsync(existingLocation, cancellationToken);
                }
                break;

            case Type.InvitationToJoinLocationUpserted:
                {
                    var notification = mapper.MapInvitationToJoinLocationToNotification(@event);
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

            case Type.InvitationToJoinLocationDeleted:
                {
                    var notification = mapper.MapInvitationToJoinLocationToNotification(@event);
                    var existingNotification =
                        await repositoryFactory.NotificationRepository.GetBySourceIdAsync(notification.SourceId, cancellationToken);
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
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleLocationUpsertedEventAsync(
        Shared.Models.Location location,
        Location? existingLocation,
        CancellationToken cancellationToken)
    {
        _ = existingLocation is null
            ? repositoryFactory.LocationRepository.Add(mapper.MapToEntity(location))
            : repositoryFactory.LocationRepository.Update(mapper.MergeToEntity(location, existingLocation));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleLocationDeletedEventAsync(Location existingLocation, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.LocationRepository.Remove(existingLocation);
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

        var location = string.IsNullOrWhiteSpace(notification.Location?.Id)
            ? null
            : await repositoryFactory.LocationRepository.UpsertNakedAsync(notification.Location.Id, null, cancellationToken);

        _ = existingNotification is null
            ? repositoryFactory.NotificationRepository.Add(mapper.MapToEntity(notification, invitedBy, invitee, null, location, null))
            : repositoryFactory.NotificationRepository.Update(
                mapper.MergeToEntity(notification, existingNotification, invitedBy, invitee, null, location, null));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleNotificationDeletedEventAsync(
        Shared.Database.Entities.Notification existingNotification,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.NotificationRepository.Remove(existingNotification);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
