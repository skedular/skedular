using HotChocolate;

namespace Booking.Api.GraphQL.Entitlement;

[GraphQLName("ConsumeEntitlementCreditInput")]
public sealed class ConsumeEntitlementCreditInput
{
    public string? ClientMutationId { get; set; }
    public string BookingId { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTimeOffset BookingAt { get; set; }
}

[GraphQLName("ConsumeEntitlementCreditPayload")]
public sealed class ConsumeEntitlementCreditPayload
{
    public string? ClientMutationId { get; set; }
    public CreditLedgerEntryDetails? LedgerEntry { get; set; }
    public string? Error { get; set; }
}
