namespace Booking.Shared.Models;

public record XeroRefundProcessingAvailability(
    bool CanProcessInXero,
    string? BlockedReason);
