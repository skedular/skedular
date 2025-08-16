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
            var delayDuration = args.ReSyncTime.Value - TimeProvider.System.GetUtcNow();
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        if (!await Workflow.ExecuteActivityAsync(
                (MsTeamsIntegrations activity) => activity.ReSyncTeamsAndChannelsAsync(args.TenantId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(10),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                }))
        {
            return;
        }

        await Workflow.ExecuteActivityAsync(
            (MsTeamsIntegrations activity) => activity.ExecuteNextReSyncMsTeamsWorkflowAsync(args.TenantId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
    }
}
