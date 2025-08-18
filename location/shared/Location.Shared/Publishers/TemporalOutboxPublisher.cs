using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows;
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
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor<GenerateLocationDailyAnalytics> temporalOutboxLocationDailyAnalyticsExecutor)
    : ITemporalOutboxPublisher
{
    public void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork) =>
        temporalOutboxLocationDailyAnalyticsExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateLocationDailyAnalyticsPrefix}-{args.LocationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);
}
