using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;

public record GenerateOrganizationDailyAnalyticsInput(string OrganizationId, DateTimeOffset? GenerationTime);

[Workflow]
public class GenerateOrganizationDailyAnalytics
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateOrganizationDailyAnalyticsInput args)
    {
        if (args.GenerationTime.HasValue)
        {
            var delayDuration = args.GenerationTime.Value - TimeProvider.System.GetUtcNow();
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        do
        {
            if (!await Workflow.ExecuteActivityAsync(
                    (OrganizationDailyAnalytics activity) => activity.RecordOrganizationMembersCountAsync(args.OrganizationId),
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
