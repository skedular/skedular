using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Location.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Location.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateLocationDailyAnalyticsAsync(GenerateLocationDailyAnalyticsInput args, CancellationToken cancellationToken);

    Task StartComputeOrganizationLocationsAndProductsRelationshipsAsync(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        CancellationToken cancellationToken);
}

public class TemporalService(
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient,
    ITemporalHelperService temporalHelperService) : ITemporalService
{
    public async Task StartWorkflowGenerateLocationDailyAnalyticsAsync(
        GenerateLocationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.GenerateLocationDailyAnalyticsPrefix}-{args.LocationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartComputeOrganizationLocationsAndProductsRelationshipsAsync(
        ComputeOrganizationLocationsAndProductsRelationshipsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ComputeOrganizationLocationsAndProductsRelationships workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = temporalHelperService.ToId($"{Constants.ComputeLocationProductRelationshipsPrefix}-{args.OrganizationId}"),
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicate,
                IdConflictPolicy = WorkflowIdConflictPolicy.TerminateExisting,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
