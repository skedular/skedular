using Booking.Shared.Workflows;
using Booking.Shared.Workflows.ResourcesSlots;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateLocationResourcesSlotsAsync(GenerateLocationResourcesSlotsInput args, CancellationToken cancellationToken);
    Task StartWorkflowGenerateResourcesSlotsAsync(string locationId, GenerateResourcesSlotsInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, ITemporalClient temporalClient) : ITemporalService
{
    public async Task StartWorkflowGenerateLocationResourcesSlotsAsync(
        GenerateLocationResourcesSlotsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateLocationResourcesSlots workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = $"{Constants.GenerateLocationResourcesSlotsPrefix}-{args.LocationId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowGenerateResourcesSlotsAsync(
        string locationId,
        GenerateResourcesSlotsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateResourcesSlots workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = $"{Constants.GenerateResourcesSlotsPrefix}-{locationId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
