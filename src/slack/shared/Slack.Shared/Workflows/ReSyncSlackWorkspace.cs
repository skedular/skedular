using Slack.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Slack.Shared.Workflows;

public record ReSyncSlackWorkspaceInput(string WorkspaceId, DateTimeOffset? ReSyncTime);

[Workflow]
public class ReSyncSlackWorkspace
{
    [WorkflowRun]
    public async Task ExecuteAsync(ReSyncSlackWorkspaceInput args)
    {
        if (args.ReSyncTime.HasValue)
        {
            var delayDuration = args.ReSyncTime.Value - Workflow.UtcNow;
            if (delayDuration > TimeSpan.Zero)
            {
                await Workflow.DelayAsync(delayDuration, Workflow.CancellationToken);
            }
        }

        if (!await Workflow.ExecuteActivityAsync(
                (SlackIntegrations activity) => activity.ReSyncWorkspaceAsync(args.WorkspaceId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                }))
        {
            return;
        }

        if (!await Workflow.ExecuteActivityAsync(
                (SlackIntegrations activity) => activity.ReSyncWorkspaceMembersAsync(args.WorkspaceId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(10),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                }))
        {
            return;
        }

        if (!await Workflow.ExecuteActivityAsync(
                (SlackIntegrations activity) => activity.ReSyncWorkspaceChannelsAsync(args.WorkspaceId),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(5),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
                }))
        {
            return;
        }

        throw Workflow.CreateContinueAsNewException((ReSyncSlackWorkspace workflow) =>
            workflow.ExecuteAsync(new ReSyncSlackWorkspaceInput(args.WorkspaceId, Workflow.UtcNow.AddDays(1))));
    }
}
