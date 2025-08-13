using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowNewSlackWorkspaceJoined(NewSlackWorkspaceJoinedInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalOutboxWorkflowExecutor<NewSlackWorkspaceJoined> temporalOutboxNewSlackWorkspaceJoinedWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowNewSlackWorkspaceJoined(NewSlackWorkspaceJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxNewSlackWorkspaceJoinedWorkflowExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = $"{Workflows.Constants.NewSlackWorkspaceJoinedPrefix}-{args.WorkspaceId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
