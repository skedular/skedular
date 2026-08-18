using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("AdjustEntitlementCreditInput")]
public sealed class AdjustEntitlementCreditInput
{
    public string? ClientMutationId { get; set; }
    public string EntitlementId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
}

[GraphQLName("AdjustEntitlementCreditPayload")]
public sealed class AdjustEntitlementCreditPayload
{
    public string? ClientMutationId { get; set; }
    public CreditLedgerEntryDetails? LedgerEntry { get; set; }
    public string? Error { get; set; }
}
