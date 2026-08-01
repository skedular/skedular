using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("RecordBankTransferRefundSentInput")]
public class RecordBankTransferRefundSentInput
{
    public string Id { get; set; } = null!;
    public string BankTransferReference { get; set; } = null!;
    public string? ClientMutationId { get; set; }
}
