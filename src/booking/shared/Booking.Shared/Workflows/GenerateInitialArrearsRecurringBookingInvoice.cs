using Booking.Shared.Activities;
using Booking.Shared.Models;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record GenerateInitialArrearsRecurringBookingInvoiceInput(string RecurringBookingId, IReadOnlyList<string> InvoiceEmailList);

[Workflow]
public class GenerateInitialArrearsRecurringBookingInvoice
{
    [WorkflowRun]
    public async Task ExecuteAsync(GenerateInitialArrearsRecurringBookingInvoiceInput args)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (InvoiceIntegrations activity) =>
                    activity.GenerateAndSendRecurringInvoiceAsync(
                        new GenerateAndSendRecurringInvoiceInput(args.RecurringBookingId, args.InvoiceEmailList)),
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
            await ReleaseRecurringBookingResourcesAsync(args.RecurringBookingId);
        }
    }

    private static async Task ReleaseRecurringBookingResourcesAsync(string recurringBookingId)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingSubscriptionIntegrations activity) => activity.ReleaseRecurringBookingResourcesAsync(
                    new ReleaseRecurringBookingResourcesInput(recurringBookingId, MarketplaceBookingFailureCategoryConstants.PaymentFailed)),
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
                    new EnqueueMarketplaceBookingCleanupInput(null, recurringBookingId, null,
                        MarketplaceBookingFailureCategoryConstants.PaymentFailed)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                });
        }
    }
}
