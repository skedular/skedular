using Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Key;
using Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value;
using Enterprise.Shared.Kafka.Consume;
using Slack.Shared.Services;
using Type = Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Type;

namespace Slack.Processors.Subscribers;

public class SlackInternalSubscriber(
    IWorkspaceService workspaceService,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    ILocationDailyUpdaterService locationDailyUpdaterService,
    ITeamDailyUpdaterService teamDailyUpdaterService)
    : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RefreshWorkspace:
                await workspaceService.RefreshWorkspaceAsync(@event.WorkspaceId, cancellationToken);
                break;

            case Type.RefreshWorkspaceMembers:
                await workspaceMemberService.RefreshWorkspaceMembersAsync(@event.WorkspaceId, cancellationToken);
                break;

            case Type.RefreshWorkspaceChannels:
                await workspaceChannelService.RefreshWorkspaceChannelsAsync(@event.WorkspaceId, cancellationToken);
                break;

            case Type.SendWorkspaceLocationDailyUpdateMessage:
                await locationDailyUpdaterService.SendDailyUpdateAsync(@event.LocationId, cancellationToken);
                break;

            case Type.SendWorkspaceTeamDailyUpdateMessage:
                await teamDailyUpdaterService.SendDailyUpdateAsync(@event.TeamId, cancellationToken);
                break;

            case Type.UpdateWorkspaceMemberProfileStatus:
                await workspaceMemberService.UpdateWorkspaceMemberProfileStatusAsync(@event.WorkspaceMemberId, cancellationToken);
                break;

            case Type.DeactivateOrganizationMembersNotFoundOnSlack:
                break;
        }

        return EventSubscriberResults.Success;
    }
}
