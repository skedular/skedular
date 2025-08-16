using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.ReSyncAzureTenant;

public record ReSyncAzureTenantInput(string TenantId, DateTimeOffset? ReSyncTime);

public class ReSyncAzureTenant
{
    [WorkflowRun]
    public async Task ExecuteAsync(ReSyncAzureTenantInput args)
    {
        if (args.ReSyncTime.HasValue)
        {
            var delayDuration = args.ReSyncTime.Value - TimeProvider.System.GetUtcNow();
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        if (!await Workflow.ExecuteActivityAsync(
                (AzureTenantIntegrations activity) => activity.ReSyncTenantAsync(args.TenantId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                }))
        {
            return;
        }

        await Workflow.ExecuteActivityAsync((AzureTenantIntegrations activity) => activity.ExecuteNextReSyncAzureTenantWorkflowAsync(args.TenantId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
    }
}
