using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Entitlements;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public sealed record PrepareEntitlementRenewalActivityInput(
    string EntitlementId,
    DateTimeOffset PaymentExpiry);

public sealed class EntitlementRenewalIntegrations(
    IEntitlementRenewalService entitlementRenewalService)
{
    [Activity]
    public Task<EntitlementPurchase?> PreparePendingRenewalAsync(PrepareEntitlementRenewalActivityInput input) =>
        entitlementRenewalService.CreatePendingRenewalAsync(
            input.EntitlementId,
            input.PaymentExpiry,
            ActivityExecutionContext.Current.CancellationToken);
}
