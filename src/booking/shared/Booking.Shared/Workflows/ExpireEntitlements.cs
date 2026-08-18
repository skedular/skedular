using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

[Workflow]
public sealed class ExpireEntitlements
{
    [WorkflowRun]
    public async Task ExecuteAsync()
    {
        while (true)
        {
            await Workflow.ExecuteActivityAsync(
                (EntitlementExpiryIntegrations activity) => activity.ExpireDueEntitlementsAsync(),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(10),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        InitialInterval = TimeSpan.FromSeconds(10),
                        MaximumAttempts = 0,
                        MaximumInterval = TimeSpan.FromMinutes(1),
                    },
                });

            await Workflow.DelayAsync(TimeSpan.FromHours(24));
        }
    }
}
