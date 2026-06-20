using Booking.Shared.Database.Entities;

namespace Booking.Shared.Models;

public sealed record MarketplaceBookingFailureSummary(
    string Id,
    string Category,
    string Scope,
    DateTimeOffset FinalizedAt,
    DateTimeOffset? RequestedFrom,
    DateTimeOffset? RequestedUntil,
    string CustomerAction)
{
    public static implicit operator MarketplaceBookingFailureSummary(MarketplaceBookingFailure failure) =>
        new(failure.Id, failure.Category, failure.Scope, failure.FinalizedAt, failure.RequestedFrom, failure.RequestedUntil,
            failure.CustomerAction ?? string.Empty);
}
