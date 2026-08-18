using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ConfirmBankTransferRefundReceivedInput")]
public class ConfirmBankTransferRefundReceivedInput
{
    public string Id { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
