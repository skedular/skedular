using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ResolveRefundReconciliationRequiredInput")]
public class ResolveRefundReconciliationRequiredInput
{
    public string Id { get; set; } = null!;
    public bool Completed { get; set; }
    public string Reason { get; set; } = null!;
    public string? ProviderReference { get; set; }
    public string? ClientMutationId { get; set; }
}
