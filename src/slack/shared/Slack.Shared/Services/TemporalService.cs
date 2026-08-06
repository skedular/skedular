using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowReSyncSlackWorkspaceAsync(ReSyncSlackWorkspaceInput args, CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService) : ITemporalService
{
    public async Task StartWorkflowReSyncSlackWorkspaceAsync(ReSyncSlackWorkspaceInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncSlackWorkspace workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.ReSyncSlackWorkspace(args.WorkspaceId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions
                {
                    CancellationToken = cancellationToken,
                },
            });
}
