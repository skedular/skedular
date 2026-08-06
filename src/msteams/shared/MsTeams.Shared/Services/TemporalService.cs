using Enterprise.Shared.Temporal.Configurations;
using MsTeams.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace MsTeams.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowReSyncMsTeamsAsync(ReSyncMsTeamsInput args, CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService) : ITemporalService
{
    public async Task StartWorkflowReSyncMsTeamsAsync(ReSyncMsTeamsInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncMsTeams workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.ReSyncMsTeams(args.TenantId),
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
