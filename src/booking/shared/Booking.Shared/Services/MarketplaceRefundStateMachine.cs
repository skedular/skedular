using Booking.Shared.Models;

namespace Booking.Shared.Services;

public static class MarketplaceRefundStateMachine
{
    public static bool CanTransition(string current, string next)
    {
        if (current == next)
        {
            return true;
        }

        if (current is MarketplaceRefundStatusConstants.Rejected
            or MarketplaceRefundStatusConstants.Cancelled)
        {
            return false;
        }

        return current switch
        {
            MarketplaceRefundStatusConstants.Requested => next is MarketplaceRefundStatusConstants.UnderReview
                or MarketplaceRefundStatusConstants.Processing or MarketplaceRefundStatusConstants.Cancelled,
            MarketplaceRefundStatusConstants.UnderReview => next is MarketplaceRefundStatusConstants.Approved
                or MarketplaceRefundStatusConstants.Rejected or MarketplaceRefundStatusConstants.Cancelled,
            MarketplaceRefundStatusConstants.Approved => next is MarketplaceRefundStatusConstants.Processing
                or MarketplaceRefundStatusConstants.Cancelled,
            MarketplaceRefundStatusConstants.ProviderPending => next is MarketplaceRefundStatusConstants.Completed
                or MarketplaceRefundStatusConstants.Failed or MarketplaceRefundStatusConstants.ReconciliationRequired,
            MarketplaceRefundStatusConstants.Processing => next is MarketplaceRefundStatusConstants.Completed
                or MarketplaceRefundStatusConstants.Failed or MarketplaceRefundStatusConstants.ReconciliationRequired,
            MarketplaceRefundStatusConstants.Failed => next is MarketplaceRefundStatusConstants.Processing
                or MarketplaceRefundStatusConstants.ReconciliationRequired,
            MarketplaceRefundStatusConstants.ReconciliationRequired => next is MarketplaceRefundStatusConstants.Completed
                or MarketplaceRefundStatusConstants.Failed,
            // A provider can report a post-settlement failure after the refund was
            // recorded as completed. Keep that completed record visible and route
            // it back to reconciliation instead of silently leaving an invalid
            // post-payout decision in place.
            MarketplaceRefundStatusConstants.Completed => next is MarketplaceRefundStatusConstants.ReconciliationRequired,
            _ => false,
        };
    }

    public static void EnsureAllowed(string current, string next)
    {
        if (!CanTransition(current, next))
        {
            throw new InvalidOperationException($"This refund can't be moved from {current} to {next}.");
        }
    }
}
