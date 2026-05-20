namespace Booking.Shared.Models;

public sealed record ResourceBookingWindowRow
{
    public required string ResourceId { get; init; }
    public required string BookingId { get; init; }
    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset Until { get; init; }
    public required bool IsRecurring { get; init; }
    public required string? CustomerId { get; init; }
    public required string? CustomerName { get; init; }
    public required string? CustomerGivenName { get; init; }
    public required string? CustomerFamilyName { get; init; }
    public required string? Notes { get; init; }
}
