using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record NotifyMarketplaceBookingModificationInput(string ModificationId);

[Workflow]
public class NotifyMarketplaceBookingModification
{
    [WorkflowRun]
    public async Task ExecuteAsync(NotifyMarketplaceBookingModificationInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplaceBookingModificationNotificationIntegrations activity) =>
                activity.DispatchMarketplaceBookingModificationAsync(
                    new DispatchMarketplaceBookingModificationNotificationInput(input.ModificationId)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 0,
                    MaximumInterval = TimeSpan.FromHours(1),
                },
            });
}
