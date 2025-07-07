namespace Booking.Shared.Workflows;

public record PayBookingByCardState(string? PaymentStatus, bool BookingDeleted);
