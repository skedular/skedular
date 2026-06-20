using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

[Workflow]
public class RolloverSpacesBookingUsage
{
    [WorkflowRun]
    public async Task ExecuteAsync()
    {
        while (true)
        {
            var nextRunAt = GetNextFirstDayOfMonth(Workflow.UtcNow);
            var delay = nextRunAt - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delay);
            }

            await Workflow.ExecuteActivityAsync(
                (SpacesBookingUsageRolloverIntegrations activity) => activity.RolloverCurrentPeriodsAsync(),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
        }
    }

    private static DateTime GetNextFirstDayOfMonth(DateTime now)
    {
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return currentMonthStart <= now ? currentMonthStart.AddMonths(1) : currentMonthStart;
    }
}
