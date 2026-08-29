using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record MarketplaceBookingCleanupInput(string FailureId);

[Workflow]
public class MarketplaceBookingCleanup
{
    [WorkflowRun]
    public async Task ExecuteAsync(MarketplaceBookingCleanupInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplaceBookingCleanupIntegrations activity) => activity.CleanupAsync(input),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 5,
                    InitialInterval = TimeSpan.FromSeconds(2),
                    MaximumInterval = TimeSpan.FromSeconds(30),
                    BackoffCoefficient = 2,
                },
            });
}
