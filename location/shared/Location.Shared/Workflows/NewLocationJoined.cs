using Location.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Location.Shared.Workflows;

public record NewLocationJoinedInput(string LocationId);

[Workflow]
public class NewLocationJoined
{
    [WorkflowRun]
    public async Task ExecuteAsync(NewLocationJoinedInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) => activity.SendNewLocationJoinedEmailAsync(args.LocationId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
