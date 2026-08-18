namespace Booking.Shared.Models;

public sealed class CreditLedgerEntryMetadata
{
    public int? UnusedCredits { get; set; }
    public string? RefundId { get; set; }
    public string? RefundError { get; set; }
    public string? Reason { get; set; }
    public string? ActorCustomerId { get; set; }
}
