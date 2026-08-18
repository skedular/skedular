using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CreatePartialMarketplaceRefundInput")]
public class CreatePartialMarketplaceRefundInput
{
    public string AllocationId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? ClientMutationId { get; set; }
}
