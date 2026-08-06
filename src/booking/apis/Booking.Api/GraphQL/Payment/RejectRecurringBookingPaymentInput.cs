using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("RejectRecurringBookingPaymentInput")]
public class RejectRecurringBookingPaymentInput
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
