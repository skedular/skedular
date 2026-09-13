using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Shared.Services;

public interface IMarketplaceAutomaticPaymentStatusService
{
    Task<MarketplaceAutomaticPaymentStatus?> GetForSubscriptionAsync(string marketplaceBookingSubscriptionId, CancellationToken cancellationToken);
    Task<MarketplaceAutomaticPaymentStatus?> GetForEntitlementAsync(string entitlementPurchaseId, CancellationToken cancellationToken);
}

public sealed class MarketplaceAutomaticPaymentStatusService(IRepositoryFactory repositoryFactory) : IMarketplaceAutomaticPaymentStatusService
{
    public async Task<MarketplaceAutomaticPaymentStatus?> GetForSubscriptionAsync(string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken) =>
        ToModel(await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            marketplaceBookingSubscriptionId, cancellationToken));

    public async Task<MarketplaceAutomaticPaymentStatus?> GetForEntitlementAsync(string entitlementPurchaseId, CancellationToken cancellationToken) =>
        ToModel(await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(entitlementPurchaseId, cancellationToken));

    private static MarketplaceAutomaticPaymentStatus? ToModel(MarketplaceBookingSubscription? subscription) =>
        subscription?.StripeSubscriptionId is null
            ? null
            : new MarketplaceAutomaticPaymentStatus(
                true,
                subscription.StripeSubscriptionStatus ?? "pending",
                subscription.StripeCurrentPeriodEndsAt,
                subscription.StripeCancelAtPeriodEnd);

    private static MarketplaceAutomaticPaymentStatus? ToModel(EntitlementPurchase? purchase) =>
        purchase?.StripeSubscriptionId is null
            ? null
            : new MarketplaceAutomaticPaymentStatus(
                true,
                purchase.StripeSubscriptionStatus ?? "pending",
                purchase.StripeCurrentPeriodEndsAt,
                purchase.StripeCancelAtPeriodEnd);
}
