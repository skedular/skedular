using Booking.Shared.Database.Entities;
using Stripe;
using Stripe.Checkout;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchasePaymentCancellationService
{
    Task CancelAsync(EntitlementPurchase purchase, CancellationToken cancellationToken);
}

public sealed class EntitlementPurchasePaymentCancellationService(SessionService sessionService) : IEntitlementPurchasePaymentCancellationService
{
    public async Task CancelAsync(EntitlementPurchase purchase, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(purchase.StripeCheckoutSessionId))
        {
            return;
        }

        await sessionService.ExpireAsync(
            purchase.StripeCheckoutSessionId,
            new SessionExpireOptions(),
            new RequestOptions
            {
                StripeAccount = purchase.StripeAccountId,
            },
            cancellationToken);
    }
}
