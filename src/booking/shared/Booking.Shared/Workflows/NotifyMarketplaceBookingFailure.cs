using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record NotifyMarketplaceBookingFailureInput(string FailureId);

[Workflow]
public class NotifyMarketplaceBookingFailure
{
    [WorkflowRun]
    public async Task ExecuteAsync(NotifyMarketplaceBookingFailureInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplaceBookingFailureNotificationIntegrations activity) =>
                activity.DispatchMarketplaceBookingFailureAsync(new DispatchMarketplaceBookingFailureNotificationsInput(input.FailureId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                // Delivery state is durable. Keep retrying transport failures so an outage
                // cannot leave a finalized failure permanently undispatched.
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 0,
                    MaximumInterval = TimeSpan.FromHours(1),
                },
            });
}
