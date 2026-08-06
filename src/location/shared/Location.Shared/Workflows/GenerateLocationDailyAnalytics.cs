using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows;

public record GenerateLocationDailyAnalyticsInput(string LocationId, DateTimeOffset? GenerationTime, DateTimeOffset? SnapshotDateOverride);

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

        if (!await Workflow.ExecuteActivityAsync(
                (LocationDailyAnalytics activity) => activity.RecordLocationDesksCountAsync(args.LocationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromMinutes(1),
                    },
                }))
        {
            return;
        }

        if (!await Workflow.ExecuteActivityAsync(
                (LocationDailyAnalytics activity) => activity.RecordLocationRoomsCountAsync(args.LocationId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromMinutes(1),
                    },
                }))
        {
            return;
        }

        await Workflow.ExecuteActivityAsync(
            (LocationDailyAnalytics activity) =>
                activity.RecordResourceAvailabilitySnapshotForDateAsync(args.LocationId, args.SnapshotDateOverride),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 3,
                    MaximumInterval = TimeSpan.FromMinutes(1),
                },
            });

        throw Workflow.CreateContinueAsNewException((GenerateLocationDailyAnalytics workflow) =>
            workflow.ExecuteAsync(new GenerateLocationDailyAnalyticsInput(args.LocationId, Workflow.UtcNow.AddDays(1), null)));
    }
}
