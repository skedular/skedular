using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Stripe;
using Stripe.Checkout;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchasePaymentCancellationService
{
    Task CancelAsync(EntitlementPurchase purchase, CancellationToken cancellationToken);
}

public sealed class EntitlementPurchasePaymentCancellationService(
    IRepositoryFactory repositoryFactory,
    SessionService sessionService,
    SubscriptionService subscriptionService,
    ITemporalService temporalService) : IEntitlementPurchasePaymentCancellationService
{
    public async Task CancelAsync(EntitlementPurchase purchase, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(purchase.StripeSubscriptionId))
        {
            try
            {
                await subscriptionService.CancelAsync(
                    purchase.StripeSubscriptionId,
                    new SubscriptionCancelOptions(),
                    new RequestOptions
                    {
                        StripeAccount = purchase.StripeAccountId,
                        IdempotencyKey = $"marketplace-entitlement-cancellation:{purchase.Id}",
                    },
                    cancellationToken);
            }
            catch (StripeException)
            {
                purchase.StripeSubscriptionStatus = "cancellation_pending";
                repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await temporalService.StartWorkflowMarketplaceStripeSubscriptionCancellationAsync(
                    new MarketplaceStripeSubscriptionCancellationInput("entitlement", purchase.Id, false),
                    cancellationToken);
                throw;
            }

            return;
        }

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
