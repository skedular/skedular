using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record MarketplaceStripeSubscriptionCancellationInput(
    string SourceType,
    string SourceId,
    bool CancelAtPeriodEnd);

[Workflow]
public sealed class MarketplaceStripeSubscriptionCancellation
{
    [WorkflowRun]
    public async Task ExecuteAsync(MarketplaceStripeSubscriptionCancellationInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplaceStripeSubscriptionCancellationIntegrations activity) => activity.SynchronizeAsync(input),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 0,
                    MaximumInterval = TimeSpan.FromHours(1),
                },
            });
}
