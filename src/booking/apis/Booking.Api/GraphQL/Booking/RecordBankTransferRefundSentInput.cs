using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("RecordBankTransferRefundSentInput")]
public class RecordBankTransferRefundSentInput
{
    public string Id { get; set; } = string.Empty;
    public string BankTransferReference { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
