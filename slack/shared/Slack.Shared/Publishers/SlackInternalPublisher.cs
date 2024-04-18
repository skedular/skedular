using Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Kafka.Produce;
using Event = Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Type;

namespace Slack.Shared.Publishers;

public interface ISlackInternalPublisher
{
    Task PublishRefreshWorkspaceMembersAsync(
        IEnumerable<string> workspaceIds,
        CancellationToken cancellationToken);

    Task PublishRefreshWorkspaceChannelsAsync(
        IEnumerable<string> workspaceIds,
        CancellationToken cancellationToken);

    Task PublishWorkspaceLocationDailyUpdateMessageAsync(
        IEnumerable<string> locationIds,
        CancellationToken cancellationToken);

    Task PublishWorkspaceTeamDailyUpdateMessageAsync(
        IEnumerable<string> teamIds,
        CancellationToken cancellationToken);

    Task PublishUpdateWorkspaceMemberProfileStatusAsync(
        IEnumerable<string> workspaceMemberIds,
        CancellationToken cancellationToken);
}

public class SlackInternalPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaPublisher<Key, Event> publisher)
    : ISlackInternalPublisher
{
    public async Task PublishRefreshWorkspaceMembersAsync(
        IEnumerable<string> workspaceIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceIds.Select(async workspaceId =>
        {
            var key = new Key { WorkspaceId = workspaceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshWorkspaceMembers,
                    context.PropertyBag.CorrelationId),
                WorkspaceId = workspaceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishRefreshWorkspaceChannelsAsync(
        IEnumerable<string> workspaceIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceIds.Select(async workspaceId =>
        {
            var key = new Key { WorkspaceId = workspaceId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.RefreshWorkspaceChannels,
                    context.PropertyBag.CorrelationId),
                WorkspaceId = workspaceId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishWorkspaceLocationDailyUpdateMessageAsync(
        IEnumerable<string> locationIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(locationIds.Select(async locationId =>
        {
            var key = new Key { LocationId = locationId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.SendWorkspaceLocationDailyUpdateMessage,
                    context.PropertyBag.CorrelationId),
                LocationId = locationId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishWorkspaceTeamDailyUpdateMessageAsync(
        IEnumerable<string> teamIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(teamIds.Select(async teamId =>
        {
            var key = new Key { TeamId = teamId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.SendWorkspaceTeamDailyUpdateMessage,
                    context.PropertyBag.CorrelationId),
                TeamId = teamId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));

    public async Task PublishUpdateWorkspaceMemberProfileStatusAsync(
        IEnumerable<string> workspaceMemberIds,
        CancellationToken cancellationToken) =>
        await Task.WhenAll(workspaceMemberIds.Select(async workspaceMemberId =>
        {
            var key = new Key { WorkspaceMemberId = workspaceMemberId };
            var @event = new Event
            {
                Metadata = Event.NewMetadata(
                    applicationConfiguration.DomainSource,
                    applicationConfiguration.AppSource,
                    Type.UpdateWorkspaceMemberProfileStatus,
                    context.PropertyBag.CorrelationId),
                WorkspaceMemberId = workspaceMemberId
            };

            await publisher.PublishAsync(key, @event, cancellationToken);
        }));
}
