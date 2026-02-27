using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("DeleteMarketplaceRecurringBookingInput")]
public class DeleteMarketplaceRecurringBookingInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public required string Id { get; set; }
}
