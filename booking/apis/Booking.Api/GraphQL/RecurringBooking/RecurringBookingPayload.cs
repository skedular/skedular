using HotChocolate;

namespace Booking.Api.GraphQL.RecurringBooking;

[GraphQLName("RecurringBookingPayload")]
public class RecurringBookingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("recurringBooking")] public RecurringBookingDetails RecurringBooking { get; set; } = new();
}
