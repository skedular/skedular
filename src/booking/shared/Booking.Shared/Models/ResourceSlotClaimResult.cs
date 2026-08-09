namespace Booking.Shared.Models;

public sealed record ResourceSlotClaimResult(
    bool Claimed,
    bool RetryExhausted,
    IReadOnlyCollection<string> UnavailableResourceIds)
{
    public static ResourceSlotClaimResult Success() => new(true, false, []);

    public static ResourceSlotClaimResult Conflict(IEnumerable<string> resourceIds, bool retryExhausted = false) =>
        new(false, retryExhausted, [.. resourceIds.Distinct()]);
}
