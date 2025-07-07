using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("ConfirmBookingPaymentInput")]
public class ConfirmBookingPaymentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; }
}
