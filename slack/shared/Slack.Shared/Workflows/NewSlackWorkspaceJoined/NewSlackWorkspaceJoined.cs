using Slack.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Slack.Shared.Workflows.NewSlackWorkspaceJoined;

public record NewSlackWorkspaceJoinedInput(string WorkspaceId);

[Workflow]
public class NewSlackWorkspaceJoined
{
    [WorkflowRun]
    public async Task ExecuteAsync(NewSlackWorkspaceJoinedInput args)
    {
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) => activity.SendNewSlackWorkspaceJoinedEmailAsync(args.WorkspaceId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });

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

        await Workflow.ExecuteActivityAsync((SlackIntegrations activity) => activity.StartWorkflowReSyncSlackWorkspaceAsync(args.WorkspaceId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
    }
}
