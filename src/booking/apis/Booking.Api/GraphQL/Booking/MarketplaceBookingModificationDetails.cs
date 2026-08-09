using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceBookingModificationDetails")]
public sealed class MarketplaceBookingModificationDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("bookingId")]
    public string BookingId { get; set; } = string.Empty;

    [GraphQLName("occurredAt")]
    public DateTimeOffset OccurredAt { get; set; }

    [GraphQLName("actorKind")]
    public MarketplaceBookingModificationActorKind ActorKind { get; set; }

    [GraphQLName("reason")]
    public string? Reason { get; set; }

    [GraphQLName("originalFrom")]
    public DateTimeOffset OriginalFrom { get; set; }

    [GraphQLName("originalUntil")]
    public DateTimeOffset OriginalUntil { get; set; }

    [GraphQLName("resultFrom")]
    public DateTimeOffset ResultFrom { get; set; }

    [GraphQLName("resultUntil")]
    public DateTimeOffset ResultUntil { get; set; }

    [GraphQLName("originalResourceIds")]
    public IReadOnlyCollection<string> OriginalResourceIds { get; set; } = [];

    [GraphQLName("resultResourceIds")]
    public IReadOnlyCollection<string> ResultResourceIds { get; set; } = [];

    [GraphQLName("originalResourceNames")]
    public IReadOnlyCollection<string> OriginalResourceNames { get; set; } = [];

    [GraphQLName("resultResourceNames")]
    public IReadOnlyCollection<string> ResultResourceNames { get; set; } = [];

    [GraphQLName("subscriptionOccurrenceOverride")]
    public bool SubscriptionOccurrenceOverride { get; set; }
}

[GraphQLName("MarketplaceBookingModificationEligibilityErrorDetails")]
public sealed class MarketplaceBookingModificationEligibilityErrorDetails
{
    [GraphQLName("code")]
    public MarketplaceBookingModificationErrorCode Code { get; set; }

    [GraphQLName("message")]
    public string Message { get; set; } = string.Empty;
}

[GraphQLName("MarketplaceBookingResourceSelectionDetails")]
public sealed class MarketplaceBookingResourceSelectionDetails
{
    [GraphQLName("canSelectResources")]
    public bool CanSelectResources { get; set; }

    [GraphQLName("maximumResourceCount")]
    public int MaximumResourceCount { get; set; }

    [GraphQLName("eligibleResources")]
    public IReadOnlyList<BookingResourceDetails> EligibleResources { get; set; } = [];

    [GraphQLName("availableResourceIds")]
    public IReadOnlyList<string> AvailableResourceIds { get; set; } = [];

    [GraphQLName("eligibleLocations")]
    public IReadOnlyList<LocationDetails> EligibleLocations { get; set; } = [];
}

[GraphQLName("MarketplaceBookingModificationConflictErrorDetails")]
public sealed class MarketplaceBookingModificationConflictErrorDetails
{
    [GraphQLName("code")]
    public MarketplaceBookingModificationErrorCode Code { get; set; }

    [GraphQLName("message")]
    public string Message { get; set; } = string.Empty;

    [GraphQLName("currentBooking")]
    public BookingDetails? CurrentBooking { get; set; }
}

[GraphQLName("ModifyMarketplaceBookingPayload")]
public sealed class ModifyMarketplaceBookingPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("booking")]
    public BookingDetails? Booking { get; set; }

    [GraphQLName("modification")]
    public MarketplaceBookingModificationDetails? Modification { get; set; }

    [GraphQLName("eligibilityError")]
    public MarketplaceBookingModificationEligibilityErrorDetails? EligibilityError { get; set; }

    [GraphQLName("availabilityError")]
    public BookingAvailabilityErrorDetails? AvailabilityError { get; set; }

    [GraphQLName("conflictError")]
    public MarketplaceBookingModificationConflictErrorDetails? ConflictError { get; set; }

    [GraphQLName("accessError")]
    public SpacesAccessErrorDetails? AccessError { get; set; }
}
