namespace Booking.Shared.Workflows.Payment.PayViaCard;

public record PayBookingViaCardState(string? PaymentStatus, bool BookingDeleted);
