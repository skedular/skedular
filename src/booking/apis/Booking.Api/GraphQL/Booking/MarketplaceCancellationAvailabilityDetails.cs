using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceCancellationAvailabilityDetails")]
public sealed class MarketplaceCancellationAvailabilityDetails
{
    [GraphQLName("canCancel")]
    public bool CanCancel { get; set; }

    [GraphQLName("requiresReason")]
    public bool RequiresReason { get; set; }

    [GraphQLName("isPolicyOverride")]
    public bool IsPolicyOverride { get; set; }

    [GraphQLName("unavailableReason")]
    public string? UnavailableReason { get; set; }

    public bool IsCreditFunded { get; set; }
    public string? CreditOutcome { get; set; }
}

[GraphQLName("MarketplaceSubscriptionCancellationAvailabilityDetails")]
public sealed class MarketplaceSubscriptionCancellationAvailabilityDetails
{
    [GraphQLName("immediate")]
    public required MarketplaceCancellationAvailabilityDetails Immediate { get; set; }

    [GraphQLName("atPeriodEnd")]
    public required MarketplaceCancellationAvailabilityDetails AtPeriodEnd { get; set; }
}
