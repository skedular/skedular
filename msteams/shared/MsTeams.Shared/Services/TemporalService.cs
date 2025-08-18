using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using MsTeams.Shared.Workflows;
using MsTeams.Shared.Workflows.ReSyncMsTeams;
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
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    public async Task StartWorkflowReSyncMsTeamsAsync(ReSyncMsTeamsInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncMsTeams workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.ReSyncMsTeamsPrefix}-{args.TenantId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
