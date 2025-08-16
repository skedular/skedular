using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Shared.Services;

public interface ITemporalService
{
    Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(GenerateOrganizationDailyAnalyticsInput args, CancellationToken cancellationToken);
    Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken);
}

public class TemporalService(TemporalConfiguration temporalConfiguration, ITemporalClient temporalClient) : ITemporalService
{
    public async Task StartWorkflowGenerateOrganizationDailyAnalyticsAsync(
        GenerateOrganizationDailyAnalyticsInput args,
        CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((GenerateOrganizationDailyAnalytics workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = $"{Constants.GenerateOrganizationDailyAnalyticsPrefix}-{args.OrganizationId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

    public async Task StartWorkflowReSyncAzureTenantAsync(ReSyncAzureTenantInput args, CancellationToken cancellationToken) =>
        await temporalClient.StartWorkflowAsync((ReSyncAzureTenant workflow) => workflow.ExecuteAsync(args),
            new WorkflowOptions
            {
                Id = $"{Constants.ReSyncAzureTenantPrefix}-{args.TenantId}",
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.TerminateIfRunning,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });
}
