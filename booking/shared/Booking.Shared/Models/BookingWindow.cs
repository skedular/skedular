namespace Booking.Shared.Models;

/// <summary>
///     One booking time window within a resource's day view.
///     Detail fields (<see cref="BookedByName" />, <see cref="BookedByUserId" />, <see cref="Notes" />)
///     are nulled when the requesting user does not have visibility rights under the organization
///     type and role rules (Marketplace/Individual org + non-admin/non-owner role).
/// </summary>
public sealed record BookingWindow
{
    public required string BookingId { get; init; }
    public required DateTimeOffset From { get; init; }
    public required DateTimeOffset Until { get; init; }
    public required bool IsRecurring { get; init; }
    public required bool IsCheckedIn { get; init; }

    // Detail fields — null when visibility is restricted by org type + role
    public required string? BookedByName { get; init; }
    public required string? BookedByUserId { get; init; }
    public required string? Notes { get; init; }
}
