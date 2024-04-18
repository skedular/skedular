using Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.SlackInternal.V1.Value.Type;

namespace Slack.Shared.Publishers;

public interface ISlackInternalOutboxPublisher
{
    Task PublishRefreshWorkspaceMembersAsync(
        IEnumerable<string> workspaceIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);

    Task PublishRefreshWorkspaceChannelsAsync(
        IEnumerable<string> workspaceIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken);
}

public class SlackInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IOutboxEventPublisher<Key, Event> publisher)
    : ISlackInternalOutboxPublisher
{
    public async Task PublishRefreshWorkspaceMembersAsync(
        IEnumerable<string> workspaceIds,
        IUnitOfWork unitOfWork,
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

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));

    public async Task PublishRefreshWorkspaceChannelsAsync(
        IEnumerable<string> workspaceIds,
        IUnitOfWork unitOfWork,
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

            await publisher.PublishAsync(key, @event, unitOfWork, cancellationToken);
        }));
}
