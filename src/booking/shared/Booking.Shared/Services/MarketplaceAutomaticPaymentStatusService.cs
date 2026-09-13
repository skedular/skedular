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
                MapStatus(subscription.StripeSubscriptionStatus),
                subscription.StripeCurrentPeriodEndsAt,
                subscription.StripeCancelAtPeriodEnd);

    private static MarketplaceAutomaticPaymentStatus? ToModel(EntitlementPurchase? purchase) =>
        purchase?.StripeSubscriptionId is null
            ? null
            : new MarketplaceAutomaticPaymentStatus(
                true,
                MapStatus(purchase.StripeSubscriptionStatus),
                purchase.StripeCurrentPeriodEndsAt,
                purchase.StripeCancelAtPeriodEnd);

    private static MarketplaceAutomaticPaymentStatusType MapStatus(string? status) =>
        status?.ToLowerInvariant() switch
        {
            "pending" => MarketplaceAutomaticPaymentStatusType.Pending,
            "active" or "trialing" => MarketplaceAutomaticPaymentStatusType.Active,
            "paused" => MarketplaceAutomaticPaymentStatusType.Paused,
            "past_due" => MarketplaceAutomaticPaymentStatusType.PastDue,
            "action_required" => MarketplaceAutomaticPaymentStatusType.ActionRequired,
            "finalization_failed" => MarketplaceAutomaticPaymentStatusType.FinalizationFailed,
            "incomplete" => MarketplaceAutomaticPaymentStatusType.Incomplete,
            "canceled" or "cancelled" => MarketplaceAutomaticPaymentStatusType.Canceled,
            "disconnected" => MarketplaceAutomaticPaymentStatusType.Disconnected,
            "failed" or "unpaid" or "incomplete_expired" => MarketplaceAutomaticPaymentStatusType.Failed,
            _ => MarketplaceAutomaticPaymentStatusType.Unknown,
        };
}
