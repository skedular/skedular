using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox.Publishers;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Location.Shared.Workflows.PrecomputeLocationProductRelationships;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.Publishers;

public interface ITemporalOutboxPublisher
{
    void StartWorkflowLocationDailyAnalytics(GenerateLocationDailyAnalyticsInput args, IUnitOfWork unitOfWork);

    void StartComputeOrganizationLocationsAndProductsRelationships(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        IUnitOfWork unitOfWork);
}

public class TemporalOutboxPublisher(
    TemporalConfiguration temporalConfiguration,
    ITemporalHelperService temporalHelperService,
    ITemporalOutboxWorkflowExecutor<GenerateLocationDailyAnalytics> temporalOutboxLocationDailyAnalyticsExecutor,
    ITemporalOutboxWorkflowExecutor<ComputeOrganizationLocationsAndProductsRelationships>
        temporalOutboxComputeOrganizationLocationsAndProductsRelationshipsExecutor)
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

    public void StartComputeOrganizationLocationsAndProductsRelationships(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        IUnitOfWork unitOfWork) =>
        temporalOutboxComputeOrganizationLocationsAndProductsRelationshipsExecutor.Execute(
            args,
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.ComputeLocationProductRelationshipsPrefix}-{args.OrganizationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning
            },
            unitOfWork);
}
