using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Slack.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowReSyncSlackWorkspaceAsync(ReSyncSlackWorkspaceInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, IRandomHelper randomHelper, ITemporalClient temporalClient)
    : ITemporalService
{
    public async Task StartWorkflowReSyncSlackWorkspaceAsync(ReSyncSlackWorkspaceInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncSlackWorkspace workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
