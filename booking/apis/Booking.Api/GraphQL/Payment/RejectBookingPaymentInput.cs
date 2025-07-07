using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("RejectBookingPaymentInput")]
public class RejectBookingPaymentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; }
}
