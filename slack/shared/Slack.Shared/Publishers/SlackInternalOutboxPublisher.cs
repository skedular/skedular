using Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Key;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Event = Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Type;

namespace Slack.Shared.Publishers;

public interface ISlackInternalOutboxPublisher
{
    Task PublishRefreshWorkspaceAsync(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
    Task PublishRefreshWorkspaceMembersAsync(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
    Task PublishRefreshWorkspaceChannelsAsync(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken);
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
        CancellationToken cancellationToken)
    {
        foreach (var workspaceId in workspaceIds)
        {
            await publisher.PublishAsync(
                new Key { WorkspaceId = workspaceId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.RefreshWorkspaceMembers,
                        context.GetCorrelationId()),
                    WorkspaceId = workspaceId
                },
                unitOfWork,
                cancellationToken);
        }
    }

    public async Task PublishRefreshWorkspaceChannelsAsync(
        IEnumerable<string> workspaceIds,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        foreach (var workspaceId in workspaceIds)
        {
            await publisher.PublishAsync(
                new Key { WorkspaceId = workspaceId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.RefreshWorkspaceChannels,
                        context.GetCorrelationId()),
                    WorkspaceId = workspaceId
                },
                unitOfWork,
                cancellationToken);
        }
    }

    public async Task PublishRefreshWorkspaceAsync(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork, CancellationToken cancellationToken)
    {
        foreach (var workspaceId in workspaceIds)
        {
            await publisher.PublishAsync(
                new Key { WorkspaceId = workspaceId },
                new Event
                {
                    Metadata = Event.NewMetadata(
                        applicationConfiguration.DomainSource,
                        applicationConfiguration.AppSource,
                        Type.RefreshWorkspace,
                        context.GetCorrelationId()),
                    WorkspaceId = workspaceId
                },
                unitOfWork,
                cancellationToken);
        }
    }
}
