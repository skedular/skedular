using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Booking.Shared.Services;

public interface IMarketplaceStripeSubscriptionPriceService
{
    Task<bool> UpdatePriceAsync(
        string marketplaceBookingSubscriptionId,
        string stripePriceId,
        long quantity,
        CancellationToken cancellationToken);

    Task<bool> UpdatePriceForEntitlementAsync(
        string entitlementPurchaseId,
        string stripePriceId,
        long quantity,
        CancellationToken cancellationToken);

    Task<bool> PrepareCurrentPriceAsync(
        string stripeAccountId,
        string stripeSubscriptionId,
        CancellationToken cancellationToken);
}

public sealed class MarketplaceStripeSubscriptionPriceService(
    IRepositoryFactory repositoryFactory,
    SubscriptionService subscriptionService,
    IProductVersionHelperService productVersionHelperService,
    IStripeProductPricingService stripeProductPricingService,
    IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
    ILogger<MarketplaceStripeSubscriptionPriceService> logger) : IMarketplaceStripeSubscriptionPriceService
{
    public async Task<bool> PrepareCurrentPriceAsync(
        string stripeAccountId,
        string stripeSubscriptionId,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetByStripeSubscriptionAsync(stripeAccountId, stripeSubscriptionId, cancellationToken);
        if (subscription is not null)
        {
            var productVersion = await repositoryFactory.ProductVersionRepository
                .GetByIdAsync(subscription.MarketplaceBooking.ProductVersion.Id, cancellationToken);
            var pricing = productVersion?.PricingOptions is null
                ? null
                : productVersionHelperService.FindMatchingPricing([.. productVersion.PricingOptions],
                    subscription.MarketplaceBooking.ProductPricing);
            if (productVersion is null || pricing is null || !pricing.SupportsSubscriptionAutoRenewal)
            {
                return false;
            }

            var quantity = subscription.MarketplaceBooking.Quantity <= 0 ? 1 : subscription.MarketplaceBooking.Quantity;
            var fallbackTotal = pricing.Price * quantity;
            var totalAmount = fallbackTotal;
            var schedule = recurringInvoiceBillingScheduleService.GetSchedule(
                pricing,
                totalAmount,
                productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
            var stripePriceId = await stripeProductPricingService.EnsureRecurringPriceAsync(
                productVersion,
                pricing,
                stripeAccountId,
                schedule.InvoiceAmount / quantity,
                schedule.MembershipTerm,
                cancellationToken);
            return !string.IsNullOrWhiteSpace(stripePriceId) && await UpdatePriceAsync(
                subscription.Id, stripePriceId, subscription.MarketplaceBooking.Quantity, subscription, cancellationToken);
        }

        var purchase = await repositoryFactory.EntitlementPurchaseRepository
            .GetByStripeSubscriptionAsync(stripeAccountId, stripeSubscriptionId, cancellationToken);
        if (purchase is null)
        {
            return false;
        }

        var entitlementProductVersion = await repositoryFactory.ProductVersionRepository
            .GetByIdAsync(purchase.ProductVersionId, cancellationToken);
        var entitlementPricing = entitlementProductVersion?.PricingOptions is null
            ? null
            : productVersionHelperService.FindMatchingPricing([.. entitlementProductVersion.PricingOptions], purchase.ProductPricing);
        if (entitlementProductVersion is null || entitlementPricing is null || !entitlementPricing.SupportsSubscriptionAutoRenewal)
        {
            return false;
        }

        var entitlementSchedule = recurringInvoiceBillingScheduleService.GetSchedule(
            entitlementPricing,
            entitlementPricing.Price,
            entitlementProductVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
        var entitlementStripePriceId = await stripeProductPricingService.EnsureRecurringPriceAsync(
            entitlementProductVersion,
            entitlementPricing,
            stripeAccountId,
            entitlementSchedule.InvoiceAmount,
            entitlementSchedule.MembershipTerm,
            cancellationToken);
        return !string.IsNullOrWhiteSpace(entitlementStripePriceId) && await UpdatePriceAsync(
            purchase.Id, entitlementStripePriceId, 1, purchase, cancellationToken);
    }

    public async Task<bool> UpdatePriceAsync(
        string marketplaceBookingSubscriptionId,
        string stripePriceId,
        long quantity,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetByIdAsync(marketplaceBookingSubscriptionId, cancellationToken);
        return await UpdatePriceAsync(marketplaceBookingSubscriptionId, stripePriceId, quantity, subscription, cancellationToken);
    }

    public async Task<bool> UpdatePriceForEntitlementAsync(
        string entitlementPurchaseId,
        string stripePriceId,
        long quantity,
        CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository
            .GetByIdAsync(entitlementPurchaseId, cancellationToken);
        return await UpdatePriceAsync(entitlementPurchaseId, stripePriceId, quantity, purchase, cancellationToken);
    }

    private async Task<bool> UpdatePriceAsync(
        string sourceId,
        string stripePriceId,
        long quantity,
        MarketplaceBookingSubscription? subscription,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stripePriceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (subscription is null || string.IsNullOrWhiteSpace(subscription.StripeSubscriptionId) ||
            string.Equals(subscription.StripeSubscriptionStatus, "disconnected", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(subscription.StripePriceId, stripePriceId, StringComparison.Ordinal))
        {
            return true;
        }

        try
        {
            var requestOptions = new RequestOptions
            {
                StripeAccount = subscription.StripeAccountId,
                IdempotencyKey = $"marketplace-subscription-price:{sourceId}:{stripePriceId}",
            };
            var stripeSubscription = await subscriptionService.GetAsync(subscription.StripeSubscriptionId, null, requestOptions, cancellationToken);
            var item = stripeSubscription.Items.Data.SingleOrDefault();
            if (item is null)
            {
                logger.LogWarning(
                    "Could not update marketplace Stripe subscription price because no subscription item exists. MarketplaceSubscriptionId={MarketplaceSubscriptionId}, StripeSubscriptionId={StripeSubscriptionId}",
                    sourceId,
                    subscription.StripeSubscriptionId);
                return false;
            }

            await subscriptionService.UpdateAsync(
                subscription.StripeSubscriptionId,
                new SubscriptionUpdateOptions
                {
                    Items =
                    [
                        new SubscriptionItemOptions
                        {
                            Id = item.Id,
                            Price = stripePriceId,
                            Quantity = quantity,
                        },
                    ],
                    ProrationBehavior = "none",
                },
                requestOptions,
                cancellationToken);
            subscription.StripePriceId = stripePriceId;
            repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Updated marketplace Stripe subscription to the current recurring price. MarketplaceSubscriptionId={MarketplaceSubscriptionId}, StripeSubscriptionId={StripeSubscriptionId}, StripePriceId={StripePriceId}",
                sourceId,
                subscription.StripeSubscriptionId,
                stripePriceId);
            return true;
        }
        catch (StripeException exception)
        {
            logger.LogWarning(
                exception,
                "Could not update marketplace Stripe subscription price. MarketplaceSubscriptionId={MarketplaceSubscriptionId}, StripeAccountId={StripeAccountId}, StripeSubscriptionId={StripeSubscriptionId}, StripePriceId={StripePriceId}",
                sourceId,
                subscription.StripeAccountId,
                subscription.StripeSubscriptionId,
                stripePriceId);
            return false;
        }
    }

    private async Task<bool> UpdatePriceAsync(
        string sourceId,
        string stripePriceId,
        long quantity,
        EntitlementPurchase? purchase,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stripePriceId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (purchase is null || string.IsNullOrWhiteSpace(purchase.StripeSubscriptionId) ||
            string.Equals(purchase.StripeSubscriptionStatus, "disconnected", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(purchase.StripePriceId, stripePriceId, StringComparison.Ordinal))
        {
            return true;
        }

        try
        {
            var requestOptions = new RequestOptions
            {
                StripeAccount = purchase.StripeAccountId,
                IdempotencyKey = $"marketplace-subscription-price:{sourceId}:{stripePriceId}",
            };
            var stripeSubscription = await subscriptionService.GetAsync(purchase.StripeSubscriptionId, null, requestOptions, cancellationToken);
            var item = stripeSubscription.Items.Data.SingleOrDefault();
            if (item is null)
            {
                return false;
            }

            await subscriptionService.UpdateAsync(
                purchase.StripeSubscriptionId,
                new SubscriptionUpdateOptions
                {
                    Items =
                    [
                        new SubscriptionItemOptions
                        {
                            Id = item.Id,
                            Price = stripePriceId,
                            Quantity = quantity,
                        },
                    ],
                    ProrationBehavior = "none",
                },
                requestOptions,
                cancellationToken);
            purchase.StripePriceId = stripePriceId;
            repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (StripeException exception)
        {
            logger.LogWarning(exception,
                "Could not update marketplace Stripe entitlement subscription price. PurchaseId={PurchaseId}, StripeSubscriptionId={StripeSubscriptionId}, StripePriceId={StripePriceId}",
                sourceId, purchase.StripeSubscriptionId, stripePriceId);
            return false;
        }
    }
}
