namespace Booking.Shared.Models;

public record MarketplaceBookingCleanupIdentity(
    string FailureId,
    string? BookingId,
    string? RecurringBookingId,
    string FailureCategory);

public record MarketplaceBookingCleanupLease(
    string WorkerId,
    DateTimeOffset AcquiredAt,
    DateTimeOffset ExpiresAt,
    int AttemptCount);

public record MarketplaceBookingCleanupTransition(
    MarketplaceBookingFailureResourceReleaseStatus ResourceReleaseStatus,
    MarketplaceBookingFailureAccountingCleanupStatus AccountingCleanupStatus);
