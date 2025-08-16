using Booking.Shared.Workflows.LocationResource;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowLocationResourceSlotGenerationAsync(LocationResourceSlotGenerationInput args, CancellationToken cancellationToken);
    Task StartWorkflowResourceSlotGenerationAsync(ResourceSlotGenerationInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, IRandomHelper randomHelper, ITemporalClient temporalClient)
    : ITemporalService
{
    public async Task StartWorkflowLocationResourceSlotGenerationAsync(
        LocationResourceSlotGenerationInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((LocationResourceSlotGeneration workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowResourceSlotGenerationAsync(ResourceSlotGenerationInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ResourceSlotGeneration workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
