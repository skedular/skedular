using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Stripe;
using Stripe.Checkout;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public sealed class MarketplaceStripeSubscriptionCancellationIntegrations(
    IRepositoryFactory repositoryFactory,
    SubscriptionService subscriptionService,
    SessionService sessionService)
{
    [Activity]
    public async Task SynchronizeAsync(MarketplaceStripeSubscriptionCancellationInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        if (input.SourceType == "subscription")
        {
            var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
                                   .GetByIdAsync(input.SourceId, cancellationToken) ??
                               throw new InvalidOperationException("The marketplace subscription could not be found.");
            if (string.IsNullOrWhiteSpace(subscription.StripeSubscriptionId))
            {
                return;
            }

            var requestOptions = new RequestOptions
            {
                StripeAccount = subscription.StripeAccountId,
                IdempotencyKey = $"marketplace-subscription-cancellation:{subscription.Id}:{input.CancelAtPeriodEnd}",
            };
            if (input.CancelAtPeriodEnd)
            {
                await subscriptionService.UpdateAsync(subscription.StripeSubscriptionId,
                    new SubscriptionUpdateOptions
                    {
                        CancelAtPeriodEnd = true,
                    }, requestOptions, cancellationToken);
            }
            else
            {
                await subscriptionService.CancelAsync(subscription.StripeSubscriptionId,
                    new SubscriptionCancelOptions(), requestOptions, cancellationToken);
            }

            subscription.StripeSubscriptionStatus = input.CancelAtPeriodEnd ? "active" : "canceled";
            subscription.StripeCancelAtPeriodEnd = input.CancelAtPeriodEnd;
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var purchase = await repositoryFactory.EntitlementPurchaseRepository
            .GetByIdAsync(input.SourceId, cancellationToken) ?? throw new InvalidOperationException("The entitlement purchase could not be found.");
        if (!string.IsNullOrWhiteSpace(purchase.StripeSubscriptionId))
        {
            var requestOptions = new RequestOptions
            {
                StripeAccount = purchase.StripeAccountId,
                IdempotencyKey = $"marketplace-entitlement-cancellation:{purchase.Id}:{input.CancelAtPeriodEnd}",
            };
            if (input.CancelAtPeriodEnd)
            {
                await subscriptionService.UpdateAsync(purchase.StripeSubscriptionId,
                    new SubscriptionUpdateOptions
                    {
                        CancelAtPeriodEnd = true,
                    }, requestOptions, cancellationToken);
            }
            else
            {
                await subscriptionService.CancelAsync(purchase.StripeSubscriptionId,
                    new SubscriptionCancelOptions(), requestOptions, cancellationToken);
            }
        }
        else if (!string.IsNullOrWhiteSpace(purchase.StripeCheckoutSessionId))
        {
            await sessionService.ExpireAsync(purchase.StripeCheckoutSessionId, new SessionExpireOptions(),
                new RequestOptions
                {
                    StripeAccount = purchase.StripeAccountId,
                }, cancellationToken);
        }

        purchase.StripeSubscriptionStatus = input.CancelAtPeriodEnd ? "active" : "canceled";
        purchase.StripeCancelAtPeriodEnd = input.CancelAtPeriodEnd;
        repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
