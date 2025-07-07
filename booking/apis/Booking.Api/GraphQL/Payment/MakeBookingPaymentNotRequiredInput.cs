using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("MakeBookingPaymentNotRequiredInput")]
public class MakeBookingPaymentNotRequiredInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; }
}
