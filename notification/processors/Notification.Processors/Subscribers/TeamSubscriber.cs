using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Notification.Processors.Mappers;
using Notification.Shared.Repositories;
using Team = Notification.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Notification.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.Team.OrganizationId);

                    var team = mapper.MapTo(@event);
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
                    var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(team.Id, organization, cancellationToken);
                    if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingTeam is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamDeletedEventAsync(existingTeam, cancellationToken);
                }
                break;

            case Type.InvitationToJoinTeamUpserted:
                {
                    var notification = mapper.MapInvitationToJoinTeamToNotification(@event);
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

            case Type.InvitationToJoinTeamDeleted:
                {
                    var notification = mapper.MapInvitationToJoinTeamToNotification(@event);
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
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team existingTeam,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
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

        var team = string.IsNullOrWhiteSpace(notification.Team?.Id)
            ? null
            : await repositoryFactory.TeamRepository.UpsertNakedAsync(
                notification.Team.Id,
                null,
                cancellationToken);

        _ = existingNotification is null
            ? repositoryFactory.NotificationRepository.Add(mapper.MapToEntity(notification, invitedBy, invitee, null, null, team))
            : repositoryFactory.NotificationRepository.Update(
                mapper.MergeToEntity(notification, existingNotification, invitedBy, invitee, null, null, team));

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
