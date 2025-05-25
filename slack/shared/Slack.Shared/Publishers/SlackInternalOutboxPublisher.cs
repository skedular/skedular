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
    void PublishRefreshWorkspace(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork);
    void PublishRefreshWorkspaceMembers(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork);
    void PublishRefreshWorkspaceChannels(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork);
}

public class SlackInternalOutboxPublisher(
    ApplicationConfiguration applicationConfiguration,
    IContext context,
    IKafkaOutboxEventPublisher<Key, Event> publisher)
    : ISlackInternalOutboxPublisher
{
    public void PublishRefreshWorkspaceMembers(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork)
    {
        foreach (var workspaceId in workspaceIds)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }

    public void PublishRefreshWorkspaceChannels(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork)
    {
        foreach (var workspaceId in workspaceIds)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }

    public void PublishRefreshWorkspace(IEnumerable<string> workspaceIds, IUnitOfWork unitOfWork)
    {
        foreach (var workspaceId in workspaceIds)
        {
            publisher.Publish(
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
                unitOfWork);
        }
    }
}
