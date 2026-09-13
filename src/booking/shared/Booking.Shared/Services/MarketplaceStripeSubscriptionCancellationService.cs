using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Booking.Shared.Services;

public interface IMarketplaceStripeSubscriptionCancellationService
{
    Task SynchronizeAsync(
        string marketplaceBookingSubscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken);
}

public sealed class MarketplaceStripeSubscriptionCancellationService(
    IRepositoryFactory repositoryFactory,
    SubscriptionService subscriptionService,
    ILogger<MarketplaceStripeSubscriptionCancellationService> logger)
    : IMarketplaceStripeSubscriptionCancellationService
{
    public async Task SynchronizeAsync(
        string marketplaceBookingSubscriptionId,
        bool cancelAtPeriodEnd,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetByIdAsync(marketplaceBookingSubscriptionId, cancellationToken);
        if (subscription is null || string.IsNullOrWhiteSpace(subscription.StripeSubscriptionId) ||
            string.Equals(subscription.StripeSubscriptionStatus, "disconnected", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            var requestOptions = new RequestOptions
            {
                StripeAccount = subscription.StripeAccountId,
                IdempotencyKey = $"marketplace-subscription-cancellation:{marketplaceBookingSubscriptionId}:{cancelAtPeriodEnd}",
            };

            if (cancelAtPeriodEnd)
            {
                await subscriptionService.UpdateAsync(
                    subscription.StripeSubscriptionId,
                    new SubscriptionUpdateOptions
                    {
                        CancelAtPeriodEnd = true,
                    },
                    requestOptions,
                    cancellationToken);
            }
            else
            {
                await subscriptionService.CancelAsync(
                    subscription.StripeSubscriptionId,
                    new SubscriptionCancelOptions(),
                    requestOptions,
                    cancellationToken);
            }
        }
        catch (StripeException exception)
        {
            // Local cancellation remains authoritative. The webhook projection and the
            // operator recovery path can retry the provider synchronization later.
            logger.LogWarning(
                exception,
                "Could not synchronize Stripe marketplace subscription cancellation. MarketplaceSubscriptionId={MarketplaceSubscriptionId}, StripeAccountId={StripeAccountId}, StripeSubscriptionId={StripeSubscriptionId}, CancelAtPeriodEnd={CancelAtPeriodEnd}",
                marketplaceBookingSubscriptionId,
                subscription.StripeAccountId,
                subscription.StripeSubscriptionId,
                cancelAtPeriodEnd);
        }
    }
}
