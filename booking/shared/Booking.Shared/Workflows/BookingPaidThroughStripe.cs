using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookingPaidThroughStripeInput(string BookingId);

[Workflow]
public class BookingPaidThroughStripe
{
    [WorkflowRun]
    public async Task ExecuteAsync(BookingPaidThroughStripeInput args)
    {
    }
}
