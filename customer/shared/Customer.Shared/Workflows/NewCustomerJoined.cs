using Customer.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Customer.Shared.Workflows;

public record NewCustomerJoinedInput(string CustomerId);

[Workflow]
public class NewCustomerJoined
{
    [WorkflowRun]
    public async Task ExecuteAsync(NewCustomerJoinedInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) => activity.SendNewCustomerJoinedEmailAsync(args.CustomerId),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(1),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromMinutes(1) }
            });
}
