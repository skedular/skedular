using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record GenerateInitialArrearsBookingInvoiceInput(string BookingId, ICollection<string> InvoiceEmailList);

[Workflow]
public class GenerateInitialArrearsBookingInvoice
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateInitialArrearsBookingInvoiceInput args) =>
        await Workflow.ExecuteActivityAsync(
            (InvoiceIntegrations activity) =>
                activity.GenerateAndSendInvoiceAsync(new GenerateAndSendInvoiceInput(args.BookingId, false, args.InvoiceEmailList)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });
}
