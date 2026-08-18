namespace Booking.Api.GraphQL.EntitlementPurchase;

public sealed class EntitlementPurchasePayload
{
    public string? ClientMutationId { get; set; }
    public EntitlementPurchaseDetails? Purchase { get; set; }
    public string? Error { get; set; }
}
