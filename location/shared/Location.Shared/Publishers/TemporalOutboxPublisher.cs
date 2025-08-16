using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    IRandomHelper randomHelper,
    ITemporalOutboxWorkflowExecutor<GenerateLocationDailyAnalytics> temporalOutboxLocationDailyAnalyticsExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxLocationDailyAnalyticsExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = randomHelper.Generate(),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);
}
