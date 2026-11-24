using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;

namespace Booking.Api.Services;

public sealed record MarketplaceCancellationAvailability(
    bool CanCancel,
    bool RequiresReason,
    bool IsPolicyOverride,
    string? UnavailableReason,
    bool IsCreditFunded = false,
    string? CreditOutcome = null);

public sealed record MarketplaceSubscriptionCancellationAvailability(
    MarketplaceCancellationAvailability Immediate,
    MarketplaceCancellationAvailability AtPeriodEnd);

public interface IMarketplaceCancellationAvailabilityService
{
    Task<MarketplaceCancellationAvailability> GetBookingAsync(string bookingId, CancellationToken cancellationToken);
    Task<MarketplaceSubscriptionCancellationAvailability> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, MarketplaceCancellationAvailability>> GetBookingsAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>> GetSubscriptionsAsync(
        IReadOnlyCollection<string> subscriptionIds,
        CancellationToken cancellationToken);
}

/// <summary>
///     Produces the caller-specific cancellation capability used by every UI surface.
///     The mutation remains the authority at execution time; this prevents offering an
///     action that the current caller cannot complete under the current policy.
/// </summary>
public sealed class MarketplaceCancellationAvailabilityService(
    IRepositoryFactory repositoryFactory,
    IOrganizationAuthorizationService organizationAuthorizationService,
    MarketplaceRefundPolicyService marketplaceRefundPolicyService,
    TimeProvider timeProvider,
    ICachedCustomerService cachedCustomerService)
    : IMarketplaceCancellationAvailabilityService
{
    public async Task<MarketplaceCancellationAvailability> GetBookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null || booking.MarketplaceBooking is null)
        {
            return Unavailable("This booking is not available for cancellation.");
        }

        if (booking.DeletedByCustomer is not null)
        {
            return Unavailable("This booking has already been cancelled.");
        }

        var isCreditFunded = booking.MarketplaceBooking.EntitlementId is not null;

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var productVersion =
            await repositoryFactory.ProductVersionRepository.GetByIdAsync(booking.MarketplaceBooking.ProductVersion.Id, cancellationToken);
        if (productVersion is null)
        {
            return Unavailable("This booking is not available for cancellation.");
        }

        var quote = marketplaceRefundPolicyService.GetQuote(booking.MarketplaceBooking.ProductPricing, booking.From, timeProvider.GetUtcNow());
        if (quote.CanCancel)
        {
            return Available(isCreditFunded);
        }

        var canOverride = await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
            productVersion.Product.Organization.Id,
            customerId,
            cancellationToken);
        return canOverride
            ? new MarketplaceCancellationAvailability(true, true, true, null, isCreditFunded,
                isCreditFunded ? "Credit will be forfeited because the cancellation policy window has closed." : null)
            : Unavailable("This booking cannot be cancelled because its cancellation window has closed or the product does not allow cancellation.");
    }

    public async Task<MarketplaceSubscriptionCancellationAvailability> GetSubscriptionAsync(string subscriptionId,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null || subscription.MarketplaceBooking is null)
        {
            var unavailable = Unavailable("This subscription is not available for cancellation.");
            return new MarketplaceSubscriptionCancellationAvailability(unavailable, unavailable);
        }

        if (subscription.Status != MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus())
        {
            var unavailable = Unavailable("This subscription is no longer active and cannot be cancelled.");
            return new MarketplaceSubscriptionCancellationAvailability(unavailable, unavailable);
        }

        var atPeriodEnd = subscription.AutoRenew
            ? Available()
            : Unavailable("This subscription is already set not to renew.");
        var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;
        var quote = marketplaceRefundPolicyService.GetQuote(subscription.MarketplaceBooking.ProductPricing, referenceTime, timeProvider.GetUtcNow());
        if (quote.CanCancel)
        {
            return new MarketplaceSubscriptionCancellationAvailability(Available(), atPeriodEnd);
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var productOwnerOrganizationId = subscription.ProductVersion?.Product?.Organization?.Id;
        var canOverride = !string.IsNullOrWhiteSpace(productOwnerOrganizationId) &&
                          await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(productOwnerOrganizationId, customerId,
                              cancellationToken);
        var immediate = canOverride
            ? new MarketplaceCancellationAvailability(true, true, true, null)
            : Unavailable("This subscription cannot be cancelled now because its cancellation policy does not allow it.");
        return new MarketplaceSubscriptionCancellationAvailability(immediate, atPeriodEnd);
    }

    public async Task<IReadOnlyDictionary<string, MarketplaceCancellationAvailability>> GetBookingsAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken)
    {
        var ids = bookingIds.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<string, MarketplaceCancellationAvailability>();
        }

        var bookings = await repositoryFactory.BookingRepository
            .GetByIdsForCancellationAvailabilityAsync(ids, cancellationToken);
        var bookingsById = bookings.ToDictionary(item => item.Id);
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var overrideByOrganizationId = new Dictionary<string, bool>();
        var result = new Dictionary<string, MarketplaceCancellationAvailability>(ids.Count);
        foreach (var id in ids)
        {
            if (!bookingsById.TryGetValue(id, out var booking) || booking.MarketplaceBooking is null)
            {
                result[id] = Unavailable("This booking is not available for cancellation.");
                continue;
            }

            if (booking.DeletedByCustomer is not null)
            {
                result[id] = Unavailable("This booking has already been cancelled.");
                continue;
            }

            var quote = marketplaceRefundPolicyService.GetQuote(
                booking.MarketplaceBooking.ProductPricing,
                booking.From,
                timeProvider.GetUtcNow());
            var isCreditFunded = booking.MarketplaceBooking.EntitlementId is not null;
            if (quote.CanCancel)
            {
                result[id] = Available(isCreditFunded);
                continue;
            }

            var organizationId = booking.MarketplaceBooking.ProductVersion?.Product?.Organization?.Id;
            var canOverride = !string.IsNullOrWhiteSpace(organizationId) &&
                              await CanOverrideCachedAsync(organizationId, customerId, overrideByOrganizationId, cancellationToken);
            result[id] = canOverride
                ? new MarketplaceCancellationAvailability(true, true, true, null, isCreditFunded,
                    isCreditFunded ? "Credit will be forfeited because the cancellation policy window has closed." : null)
                : Unavailable(
                    "This booking cannot be cancelled because its cancellation window has closed or the product does not allow cancellation.");
        }

        return result;
    }

    public async Task<IReadOnlyDictionary<string, MarketplaceSubscriptionCancellationAvailability>> GetSubscriptionsAsync(
        IReadOnlyCollection<string> subscriptionIds,
        CancellationToken cancellationToken)
    {
        var ids = subscriptionIds.Where(item => !string.IsNullOrWhiteSpace(item)).Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<string, MarketplaceSubscriptionCancellationAvailability>();
        }

        var subscriptions = await repositoryFactory.MarketplaceBookingSubscriptionRepository
            .GetByIdsAsync(ids, cancellationToken);
        var subscriptionsById = subscriptions.ToDictionary(item => item.Id);
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var overrideByOrganizationId = new Dictionary<string, bool>();
        var result = new Dictionary<string, MarketplaceSubscriptionCancellationAvailability>(ids.Count);
        foreach (var id in ids)
        {
            if (!subscriptionsById.TryGetValue(id, out var subscription) || subscription.MarketplaceBooking is null)
            {
                var unavailable = Unavailable("This subscription is not available for cancellation.");
                result[id] = new MarketplaceSubscriptionCancellationAvailability(unavailable, unavailable);
                continue;
            }

            if (subscription.Status != MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus())
            {
                var unavailable = Unavailable("This subscription is no longer active and cannot be cancelled.");
                result[id] = new MarketplaceSubscriptionCancellationAvailability(unavailable, unavailable);
                continue;
            }

            var atPeriodEnd = subscription.AutoRenew
                ? Available()
                : Unavailable("This subscription is already set not to renew.");
            var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;
            var quote = marketplaceRefundPolicyService.GetQuote(
                subscription.MarketplaceBooking.ProductPricing,
                referenceTime,
                timeProvider.GetUtcNow());
            if (quote.CanCancel)
            {
                result[id] = new MarketplaceSubscriptionCancellationAvailability(Available(), atPeriodEnd);
                continue;
            }

            var organizationId = subscription.ProductVersion?.Product?.Organization?.Id;
            var canOverride = !string.IsNullOrWhiteSpace(organizationId) &&
                              await CanOverrideCachedAsync(organizationId, customerId, overrideByOrganizationId, cancellationToken);
            var immediate = canOverride
                ? new MarketplaceCancellationAvailability(true, true, true, null)
                : Unavailable("This subscription cannot be cancelled now because its cancellation policy does not allow it.");
            result[id] = new MarketplaceSubscriptionCancellationAvailability(immediate, atPeriodEnd);
        }

        return result;
    }

    private async Task<bool> CanOverrideCachedAsync(
        string organizationId,
        string customerId,
        IDictionary<string, bool> cache,
        CancellationToken cancellationToken)
    {
        if (!cache.TryGetValue(organizationId, out var canOverride))
        {
            canOverride = await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
                organizationId,
                customerId,
                cancellationToken);
            cache[organizationId] = canOverride;
        }

        return canOverride;
    }

    private static MarketplaceCancellationAvailability Available(bool isCreditFunded = false) =>
        new(true, false, false, null, isCreditFunded, isCreditFunded ? "The booking credit will be restored when cancellation is completed." : null);

    private static MarketplaceCancellationAvailability Unavailable(string reason) => new(false, false, false, reason);
}
