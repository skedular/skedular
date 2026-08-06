using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("ConfirmRecurringBookingPaymentInput")]
public class ConfirmRecurringBookingPaymentInput
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
