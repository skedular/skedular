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

    private static MarketplaceCancellationAvailability Available(bool isCreditFunded = false) =>
        new(true, false, false, null, isCreditFunded, isCreditFunded ? "The booking credit will be restored when cancellation is completed." : null);

    private static MarketplaceCancellationAvailability Unavailable(string reason) => new(false, false, false, reason);
}
