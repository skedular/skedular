using HotChocolate;

namespace Booking.Api.GraphQL.EntitlementPurchase;

[GraphQLName("CreateEntitlementAutomaticPaymentRecoveryInput")]
public sealed class CreateAutomaticPaymentRecoveryInput
{
    public string ClientMutationId { get; set; } = string.Empty;
    public string PurchaseId { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}
