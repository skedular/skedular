using Booking.Shared.Activities;
using Booking.Shared.Models;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record GenerateInitialArrearsBookingInvoiceInput(string BookingId, IReadOnlyList<string> InvoiceEmailList);

[Workflow]
public class GenerateInitialArrearsBookingInvoice
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateInitialArrearsBookingInvoiceInput args)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (InvoiceIntegrations activity) =>
                    activity.GenerateAndSendInvoiceAsync(new GenerateAndSendInvoiceInput(args.BookingId, args.InvoiceEmailList)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromSeconds(5),
                    },
                });
        }
        catch (Exception)
        {
            await ReleaseBookingResourcesAsync(args.BookingId);
        }
    }

    private static async Task ReleaseBookingResourcesAsync(string bookingId)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                    new ReleaseBookingResourcesInput(bookingId, MarketplaceBookingFailureCategoryConstants.PaymentFailed)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
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
        catch
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingCleanupIntegrations activity) => activity.EnqueueAsync(
                    new EnqueueMarketplaceBookingCleanupInput(bookingId, null, null, MarketplaceBookingFailureCategoryConstants.PaymentFailed)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                });
        }
    }
}
