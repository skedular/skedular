using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceCancellationAvailabilityDetails")]
public sealed class MarketplaceCancellationAvailabilityDetails
{
    [GraphQLName("canCancel")] public bool CanCancel { get; init; }
    [GraphQLName("requiresReason")] public bool RequiresReason { get; init; }
    [GraphQLName("isPolicyOverride")] public bool IsPolicyOverride { get; init; }
    [GraphQLName("unavailableReason")] public string? UnavailableReason { get; init; }
}

[GraphQLName("MarketplaceSubscriptionCancellationAvailabilityDetails")]
public sealed class MarketplaceSubscriptionCancellationAvailabilityDetails
{
    [GraphQLName("immediate")] public required MarketplaceCancellationAvailabilityDetails Immediate { get; init; }
    [GraphQLName("atPeriodEnd")] public required MarketplaceCancellationAvailabilityDetails AtPeriodEnd { get; init; }
}
