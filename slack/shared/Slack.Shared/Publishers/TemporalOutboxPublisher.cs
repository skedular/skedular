using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowNewSlackWorkspaceJoined(string workspaceId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<NewSlackWorkspaceJoined> temporalOutboxNewSlackWorkspaceJoinedWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowNewSlackWorkspaceJoined(string workspaceId, IUnitOfWork unitOfWork) =>
        temporalOutboxNewSlackWorkspaceJoinedWorkflowExecutor.Execute(
            new NewSlackWorkspaceJoinedInput(workspaceId),
            new WorkflowOptions
            {
                Id = $"{Workflows.Constants.NewSlackWorkspaceJoinedPrefix}-{workspaceId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
