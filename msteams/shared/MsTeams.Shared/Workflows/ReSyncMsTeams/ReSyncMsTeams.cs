using MsTeams.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace MsTeams.Shared.Workflows.ReSyncMsTeams;

public record ReSyncMsTeamsInput(string TenantId, DateTimeOffset? ReSyncTime);

[Workflow]
public class ReSyncMsTeams
{
    [WorkflowRun]
    public async Task ExecuteAsync(ReSyncMsTeamsInput args)
    {
        if (args.ReSyncTime.HasValue)
        {
            var delayDuration = args.ReSyncTime.Value - Workflow.UtcNow;
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        do
        {
            if (!await Workflow.ExecuteActivityAsync(
                    (MsTeamsIntegrations activity) => activity.ReSyncTeamsAndChannelsAsync(args.TenantId),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(10),
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
