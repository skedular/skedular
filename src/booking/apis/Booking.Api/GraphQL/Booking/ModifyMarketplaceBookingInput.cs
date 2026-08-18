using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ModifyMarketplaceBookingInput")]
public sealed class ModifyMarketplaceBookingInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("bookingId")]
    public required string BookingId { get; set; }

    [GraphQLName("entitlementId")]
    public string? EntitlementId { get; set; }

    [GraphQLName("expectedVersion")]
    public required int ExpectedVersion { get; set; }

    [GraphQLName("from")]
    public required DateTimeOffset From { get; set; }

    [GraphQLName("until")]
    public required DateTimeOffset Until { get; set; }

    [GraphQLName("resourceIds")]
    public IReadOnlyCollection<string>? ResourceIds { get; set; }

    [GraphQLName("reason")]
    public required string Reason { get; set; }

    [GraphQLName("actorKind")]
    public MarketplaceBookingModificationActorKind ActorKind { get; set; } = MarketplaceBookingModificationActorKind.Customer;
}
