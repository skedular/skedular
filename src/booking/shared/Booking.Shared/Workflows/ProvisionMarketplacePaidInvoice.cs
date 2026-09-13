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
    string? PaymentIntentId);

[Workflow]
public sealed class ProvisionMarketplacePaidInvoice
{
    private static readonly RetryPolicy s_retryPolicy = new()
    {
        InitialInterval = TimeSpan.FromSeconds(5),
        BackoffCoefficient = 2,
        MaximumInterval = TimeSpan.FromMinutes(1),
        MaximumAttempts = 12,
    };

    [WorkflowRun]
    public async Task ExecuteAsync(ProvisionMarketplacePaidInvoiceInput input) =>
        await Workflow.ExecuteActivityAsync(
            (MarketplacePaidInvoiceIntegrations activity) => activity.ProvisionAsync(input),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(5),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = s_retryPolicy,
            });
}
