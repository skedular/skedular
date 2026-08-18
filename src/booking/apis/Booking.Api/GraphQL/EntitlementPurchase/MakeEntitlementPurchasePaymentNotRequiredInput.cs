using HotChocolate;

namespace Booking.Api.GraphQL.EntitlementPurchase;

public sealed class MakeEntitlementPurchasePaymentNotRequiredInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("purchaseId")]
    public string PurchaseId { get; set; } = string.Empty;
}
