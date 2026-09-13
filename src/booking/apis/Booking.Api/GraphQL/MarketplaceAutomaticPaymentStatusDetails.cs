using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL;

[GraphQLName("MarketplaceAutomaticPaymentStatusDetails")]
public sealed class MarketplaceAutomaticPaymentStatusDetails
{
    public bool Configured { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset? CurrentPeriodEndsAt { get; init; }
    public bool CancelAtPeriodEnd { get; init; }

    public static MarketplaceAutomaticPaymentStatusDetails From(MarketplaceAutomaticPaymentStatus source) => new()
    {
        Configured = source.Configured,
        Status = source.Status,
        CurrentPeriodEndsAt = source.CurrentPeriodEndsAt,
        CancelAtPeriodEnd = source.CancelAtPeriodEnd,
    };
}
