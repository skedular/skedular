namespace Booking.Shared.Models;

public sealed record ResourceBookingWindowRow
{
    public required string ResourceId { get; set; }
    public required string BookingId { get; set; }
    public required DateTimeOffset From { get; set; }
    public required DateTimeOffset Until { get; set; }
    public required bool IsRecurring { get; set; }
    public required string? CustomerId { get; set; }
    public required string? CustomerName { get; set; }
    public required string? CustomerGivenName { get; set; }
    public required string? CustomerFamilyName { get; set; }
    public required string? Notes { get; set; }
}
