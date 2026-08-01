using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("CreatePartialMarketplaceRefundInput")]
public class CreatePartialMarketplaceRefundInput
{
    public string AllocationId { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = null!;
    public string IdempotencyKey { get; set; } = null!;
    public string? ClientMutationId { get; set; }
}
