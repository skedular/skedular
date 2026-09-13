using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record ProvisionMarketplacePaidInvoiceInput(
    string? MarketplaceBookingSubscriptionId,
    string? EntitlementPurchaseId,
    string StripeAccountId,
    string InvoiceId,
    DateTimeOffset InvoicePeriodStart,
    DateTimeOffset InvoicePeriodEnd,
    string? PaymentIntentId);

[Workflow]
public sealed class ProvisionMarketplacePaidInvoice
{
    private static readonly RetryPolicy s_retryPolicy = new()
    {
        InitialInterval = TimeSpan.FromSeconds(5),
        BackoffCoefficient = 2,
        MaximumInterval = TimeSpan.FromMinutes(1),
        // Cycle materialization is reconciled daily. Keep this durable retry
        // alive long enough for that workflow to create the missing cycle.
        MaximumAttempts = 0,
    };

    [WorkflowRun]
    public async Task ExecuteAsync(ProvisionMarketplacePaidInvoiceInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplacePaidInvoiceIntegrations activity) => activity.ProvisionAsync(input),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                ScheduleToCloseTimeout = TimeSpan.FromHours(26),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = s_retryPolicy,
            });
}
