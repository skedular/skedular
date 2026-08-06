using HotChocolate;

namespace Booking.Api.GraphQL.Payment;

[GraphQLName("MakeRecurringBookingPaymentNotRequiredInput")]
public class MakeRecurringBookingPaymentNotRequiredInput
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }
}
