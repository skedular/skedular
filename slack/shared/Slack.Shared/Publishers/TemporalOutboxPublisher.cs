using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowNewSlackWorkspaceJoined(string workspaceId, IUnitOfWork unitOfWork);
    void StartWorkflowReSyncSlackWorkspace(string workspaceId, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    IRandomHelper randomHelper,
    ITemporalOutboxWorkflowExecutor<NewSlackWorkspaceJoined> temporalOutboxNewSlackWorkspaceJoinedWorkflowExecutor,
    ITemporalOutboxWorkflowExecutor<ReSyncSlackWorkspace> temporalOutboxReSyncSlackWorkspaceWorkflowExecutor) : ITemporalOutboxPublisher
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

    public void StartWorkflowReSyncSlackWorkspace(string workspaceId, IUnitOfWork unitOfWork) =>
        temporalOutboxReSyncSlackWorkspaceWorkflowExecutor.Execute(
            new ReSyncSlackWorkspaceInput(workspaceId, null),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate
            },
            unitOfWork);
}
