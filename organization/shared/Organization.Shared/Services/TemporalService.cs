using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, IRandomHelper randomHelper, ITemporalClient temporalClient)
    : ITemporalService
{
    public async Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncAzureTenant workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.RejectDuplicate,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
