using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record GenerateInitialArrearsRecurringBookingInvoiceInput(string RecurringBookingId, ICollection<string> InvoiceEmailList);

[Workflow]
public class GenerateInitialArrearsRecurringBookingInvoice
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateInitialArrearsRecurringBookingInvoiceInput args) =>
        await Workflow.ExecuteActivityAsync(
            (InvoiceIntegrations activity) =>
                activity.GenerateAndSendRecurringInvoiceAsync(
                    new GenerateAndSendRecurringInvoiceInput(args.RecurringBookingId, false, args.InvoiceEmailList)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });
}
