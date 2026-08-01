using Booking.Shared.Database.Entities;
using Booking.Shared.Models;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundOwnershipService
{
    MarketplaceRefundOwnership Resolve(MarketplaceBookingFailure failure);
}

public sealed class MarketplaceRefundOwnershipService : IMarketplaceRefundOwnershipService
{
    public MarketplaceRefundOwnership Resolve(MarketplaceBookingFailure failure)
    {
        if (!string.IsNullOrWhiteSpace(failure.BookingId))
        {
            return new MarketplaceRefundOwnership(MarketplaceRefundOwnershipScope.OneTimeBooking,
                MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                failure.BookingId, failure.BookingId, failure.RecurringBookingId, failure.MarketplaceBookingSubscriptionId);
        }

        if (!string.IsNullOrWhiteSpace(failure.MarketplaceBookingSubscriptionId))
        {
            return new MarketplaceRefundOwnership(MarketplaceRefundOwnershipScope.SubscriptionBillingWindow,
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                failure.MarketplaceBookingSubscriptionId, null, failure.RecurringBookingId, failure.MarketplaceBookingSubscriptionId);
        }

        if (!string.IsNullOrWhiteSpace(failure.RecurringBookingId))
        {
            return new MarketplaceRefundOwnership(MarketplaceRefundOwnershipScope.RecurringBillingWindow,
                MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                failure.RecurringBookingId, null, failure.RecurringBookingId, null);
        }

        throw new InvalidOperationException($"Marketplace failure {failure.Id} has no billable refund owner.");
    }
}
