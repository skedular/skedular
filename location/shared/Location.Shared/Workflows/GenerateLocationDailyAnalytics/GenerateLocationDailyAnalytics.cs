using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows.GenerateLocationDailyAnalytics;

public record GenerateLocationDailyAnalyticsInput(string LocationId, DateTimeOffset? GenerationTime);

[Workflow]
public class GenerateLocationDailyAnalytics
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateLocationDailyAnalyticsInput args)
    {
        if (args.GenerationTime.HasValue)
        {
            var delayDuration = args.GenerationTime.Value - Workflow.UtcNow;
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        do
        {
            if (!await Workflow.ExecuteActivityAsync(
                    (LocationDailyAnalytics activity) => activity.RecordLocationDesksCountAsync(args.LocationId),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(1),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                    }))
            {
                break;
            }

            if (!await Workflow.ExecuteActivityAsync(
                    (LocationDailyAnalytics activity) => activity.RecordLocationRoomsCountAsync(args.LocationId),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(1),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                    }))
            {
                break;
            }

            await Workflow.DelayAsync(TimeSpan.FromDays(1), Workflow.CancellationToken);
        } while (true);
    }
}
