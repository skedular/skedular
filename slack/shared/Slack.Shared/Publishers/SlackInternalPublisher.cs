using Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Type;

namespace Slack.Shared.Publishers;

public interface ISlackInternalPublisher
{
    Task PublishRefreshWorkspaceAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken);
    Task PublishRefreshWorkspaceMembersAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken);
    Task PublishRefreshWorkspaceChannelsAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken);
    Task PublishSendWorkspaceLocationDailyUpdateMessageAsync(IEnumerable<string> locationIds, CancellationToken cancellationToken);
    Task PublishSendWorkspaceTeamDailyUpdateMessageAsync(IEnumerable<string> teamIds, CancellationToken cancellationToken);
    Task PublishUpdateWorkspaceMemberProfileStatusAsync(IEnumerable<string> workspaceMemberIds, CancellationToken cancellationToken);
}

public class SlackInternalPublisher(ApplicationConfiguration applicationConfiguration, IContext context, IKafkaPublisher<Key, Event> publisher)
    : ISlackInternalPublisher
{
    public async Task PublishRefreshWorkspaceAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceIds.Select(async workspaceId =>
        {
            var key = new Key { WorkspaceId = workspaceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshWorkspace,
                    context.GetCorrelationId()),
                WorkspaceId = workspaceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishRefreshWorkspaceMembersAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceIds.Select(async workspaceId =>
        {
            var key = new Key { WorkspaceId = workspaceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshWorkspaceMembers,
                    context.GetCorrelationId()),
                WorkspaceId = workspaceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishRefreshWorkspaceChannelsAsync(IEnumerable<string> workspaceIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceIds.Select(async workspaceId =>
        {
            var key = new Key { WorkspaceId = workspaceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshWorkspaceChannels,
                    context.GetCorrelationId()),
                WorkspaceId = workspaceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishSendWorkspaceLocationDailyUpdateMessageAsync(IEnumerable<string> locationIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(locationIds.Select(async locationId =>
        {
            var key = new Key { LocationId = locationId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.SendWorkspaceLocationDailyUpdateMessage,
                    context.GetCorrelationId()),
                LocationId = locationId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishSendWorkspaceTeamDailyUpdateMessageAsync(IEnumerable<string> teamIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(teamIds.Select(async teamId =>
        {
            var key = new Key { TeamId = teamId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.SendWorkspaceTeamDailyUpdateMessage,
                    context.GetCorrelationId()),
                TeamId = teamId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishUpdateWorkspaceMemberProfileStatusAsync(IEnumerable<string> workspaceMemberIds, CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceMemberIds.Select(async workspaceMemberId =>
        {
            var key = new Key { WorkspaceMemberId = workspaceMemberId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.UpdateWorkspaceMemberProfileStatus,
                    context.GetCorrelationId()),
                WorkspaceMemberId = workspaceMemberId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
