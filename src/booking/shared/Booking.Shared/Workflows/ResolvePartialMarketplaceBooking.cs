using Booking.Shared.Activities;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record ResolvePartialMarketplaceBookingInput(string FailureId, DateTimeOffset DeadlineAt);

[Workflow]
public class ResolvePartialMarketplaceBooking
{
    [WorkflowRun]
    public async Task ExecuteAsync(ResolvePartialMarketplaceBookingInput input)
    {
        var delay = input.DeadlineAt - Workflow.UtcNow;
        if (delay > TimeSpan.Zero)
        {
            await Workflow.DelayAsync(delay);
        }

        await Workflow.ExecuteActivityAsync(
            (MarketplaceBookingFailureResolutionIntegrations activity) =>
                activity.ResolveExpiredAsync(new ResolveExpiredMarketplaceBookingFailureInput(input.FailureId)),
            new ActivityOptions { StartToCloseTimeout = TimeSpan.FromMinutes(10), TaskQueue = Workflow.Info.TaskQueue });
    }
}
