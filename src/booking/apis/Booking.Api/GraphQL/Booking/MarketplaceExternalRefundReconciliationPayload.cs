using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceExternalRefundReconciliationPayload")]
public class MarketplaceExternalRefundReconciliationPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("reconciliation")]
    public MarketplaceExternalRefundReconciliationDetails Reconciliation { get; set; } = new();
}
