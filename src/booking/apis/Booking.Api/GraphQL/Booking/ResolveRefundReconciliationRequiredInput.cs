using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("ResolveRefundReconciliationRequiredInput")]
public class ResolveRefundReconciliationRequiredInput
{
    public string Id { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? ProviderReference { get; set; }
    public string? ClientMutationId { get; set; }
}
