using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateLocationDailyAnalyticsAsync(GenerateLocationDailyAnalyticsInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, IRandomHelper randomHelper, ITemporalClient temporalClient)
    : ITemporalService
{
    public async Task StartWorkflowGenerateLocationDailyAnalyticsAsync(
        GenerateLocationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
