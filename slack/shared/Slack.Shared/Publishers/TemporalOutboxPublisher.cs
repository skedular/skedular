using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowNewSlackWorkspaceJoined(NewSlackWorkspaceJoinedInput args, IUnitOfWork unitOfWork);
    void StartWorkflowReSyncSlackWorkspace(ReSyncSlackWorkspaceInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor temporalOutboxWorkflowExecutor) : ITemporalOutboxPublisher
{
    public void StartWorkflowNewSlackWorkspaceJoined(NewSlackWorkspaceJoinedInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<NewSlackWorkspaceJoined, NewSlackWorkspaceJoinedInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Workflows.Constants.NewSlackWorkspaceJoinedPrefix}-{args.WorkspaceId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);

    public void StartWorkflowReSyncSlackWorkspace(ReSyncSlackWorkspaceInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxWorkflowExecutor.Execute<ReSyncSlackWorkspace, ReSyncSlackWorkspaceInput>(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Workflows.Constants.ReSyncSlackWorkspacePrefix}-{args.WorkspaceId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly
            },
            unitOfWork);
}
