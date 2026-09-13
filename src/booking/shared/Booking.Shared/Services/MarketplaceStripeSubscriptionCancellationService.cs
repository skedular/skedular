using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
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
    ILogger<MarketplaceStripeSubscriptionCancellationService> logger,
    ITemporalService temporalService)
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
            // Preserve the failed provider transition locally so the operator recovery
            // path can retry it instead of silently losing the cancellation request.
            subscription.StripeSubscriptionStatus = "cancellation_pending";
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await temporalService.StartWorkflowMarketplaceStripeSubscriptionCancellationAsync(
                new MarketplaceStripeSubscriptionCancellationInput(
                    "subscription", marketplaceBookingSubscriptionId, cancelAtPeriodEnd),
                cancellationToken);
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
