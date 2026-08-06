using Customer.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Customer.Shared.Workflows;

public record SubmitCustomerFeedbackInput(string CustomerFeedbackId);

[Workflow]
public class SubmitCustomerFeedback
{
    [WorkflowRun]
    public async Task ExecuteAsync(SubmitCustomerFeedbackInput args) =>
        await Workflow.ExecuteActivityAsync(
            (EmailIntegrations activity) => activity.SendCustomerFeedbackReceivedEmailAsync(args.CustomerFeedbackId),
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
}
