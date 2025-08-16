using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using MsTeams.Shared.Workflows.ReSyncMsTeams;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace MsTeams.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowReSyncMsTeamsAsync(ReSyncMsTeamsInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, IRandomHelper randomHelper, ITemporalClient temporalClient)
    : ITemporalService
{
    public async Task StartWorkflowReSyncMsTeamsAsync(ReSyncMsTeamsInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncMsTeams workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
