using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateLocationDailyAnalyticsAsync(GenerateLocationDailyAnalyticsInput args, CancellationToken cancellationToken);

    Task StartOrSignalWorkflowRecomputeLocationBookingDerivedStateAsync(
        RecomputeLocationBookingDerivedStateInput args,
        CancellationToken cancellationToken);

    Task StartComputeOrganizationLocationsAndProductsRelationshipsAsync(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    IWorkflowIdService workflowIdService) : ITemporalService
{
    public async Task StartWorkflowGenerateLocationDailyAnalyticsAsync(
        GenerateLocationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.GenerateLocationDailyAnalytics(args.LocationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartOrSignalWorkflowRecomputeLocationBookingDerivedStateAsync(
        RecomputeLocationBookingDerivedStateInput args,
        CancellationToken cancellationToken)
    {
        var workflowOptions = new WorkflowOptions
        {
            Id = workflowIdService.RecomputeLocationBookingDerivedState(args.LocationId),
            TaskQueue = temporalConfiguration.Worker.TaskQueue,
            RetryPolicy = null,
            IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
            Rpc = new RpcOptions { CancellationToken = cancellationToken }
        };

        workflowOptions.SignalWithStart((RecomputeLocationBookingDerivedState workflow) => workflow.BookingChangedAsync());

        await temporalClient.StartWorkflowAsync(
            (RecomputeLocationBookingDerivedState workflow) => workflow.ExecuteAsync(args),
            workflowOptions);
    }

    public async Task StartComputeOrganizationLocationsAndProductsRelationshipsAsync(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ComputeOrganizationLocationsAndProductsRelationships workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = workflowIdService.ComputeOrganizationLocationsAndProductsRelationships(args.OrganizationId),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
