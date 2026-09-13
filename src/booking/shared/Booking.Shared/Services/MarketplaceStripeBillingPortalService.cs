using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Stripe;
using BillingPortalSessionCreateOptions = Stripe.BillingPortal.SessionCreateOptions;
using BillingPortalSessionService = Stripe.BillingPortal.SessionService;

namespace Booking.Shared.Services;

public interface IMarketplaceStripeBillingPortalService
{
    Task<string?> CreateForSubscriptionAsync(string marketplaceBookingSubscriptionId, string returnUrl, CancellationToken cancellationToken);
    Task<string?> CreateForEntitlementAsync(string entitlementPurchaseId, string returnUrl, CancellationToken cancellationToken);
}

public sealed class MarketplaceStripeBillingPortalService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    BillingPortalSessionService sessionService) : IMarketplaceStripeBillingPortalService
{
    public async Task<string?> CreateForSubscriptionAsync(string marketplaceBookingSubscriptionId, string returnUrl,
        CancellationToken cancellationToken) =>
        await CreateAsync(
            await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
                marketplaceBookingSubscriptionId, cancellationToken),
            returnUrl,
            cancellationToken);

    public async Task<string?> CreateForEntitlementAsync(string entitlementPurchaseId, string returnUrl, CancellationToken cancellationToken) =>
        await CreateAsync(
            await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(
                entitlementPurchaseId, cancellationToken),
            returnUrl,
            cancellationToken);

    private async Task<string?> CreateAsync(
        MarketplaceBookingSubscription? subscription,
        string returnUrl,
        CancellationToken cancellationToken)
    {
        var safeReturnUrl = GetSafeReturnUrl(returnUrl);
        if (subscription is null || string.IsNullOrWhiteSpace(subscription.StripeSubscriptionId) ||
            string.Equals(subscription.StripeSubscriptionStatus, "disconnected", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var session = await sessionService.CreateAsync(
            new BillingPortalSessionCreateOptions
            {
                Customer = subscription.StripeCustomerId,
                ReturnUrl = safeReturnUrl,
            },
            new RequestOptions
            {
                StripeAccount = subscription.StripeAccountId,
                IdempotencyKey = $"marketplace-billing-portal:subscription:{subscription.Id}:{Guid.NewGuid():N}",
            },
            cancellationToken);

        return session.Url;
    }

    private async Task<string?> CreateAsync(
        EntitlementPurchase? purchase,
        string returnUrl,
        CancellationToken cancellationToken)
    {
        var safeReturnUrl = GetSafeReturnUrl(returnUrl);
        if (purchase is null || string.IsNullOrWhiteSpace(purchase.StripeSubscriptionId) ||
            string.Equals(purchase.StripeSubscriptionStatus, "disconnected", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var session = await sessionService.CreateAsync(
            new BillingPortalSessionCreateOptions
            {
                Customer = purchase.StripeCustomerId,
                ReturnUrl = safeReturnUrl,
            },
            new RequestOptions
            {
                StripeAccount = purchase.StripeAccountId,
                IdempotencyKey = $"marketplace-billing-portal:entitlement:{purchase.Id}:{Guid.NewGuid():N}",
            },
            cancellationToken);

        return session.Url;
    }

    private string GetSafeReturnUrl(string requestedReturnUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedReturnUrl);
        if (!applicationConfiguration.WebAppBaseDomain.IsAbsoluteUri)
        {
            throw new InvalidOperationException("The marketplace billing portal return URL is not configured.");
        }

        // The browser-provided URL is deliberately not forwarded to Stripe. A Billing
        // Portal URL is a capability URL, so allowing an arbitrary return origin would
        // turn it into an open redirect after payment recovery.
        return applicationConfiguration.WebAppBaseDomain.ToString();
    }
}
