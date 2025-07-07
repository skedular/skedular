namespace Booking.Shared.Workflows;

public record PayBookingViaCardState(string? PaymentStatus, bool BookingDeleted);
