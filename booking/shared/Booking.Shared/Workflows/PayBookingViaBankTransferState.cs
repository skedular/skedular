namespace Booking.Shared.Workflows;

public record PayBookingViaBankTransferState(string? PaymentStatus, bool BookingDeleted);
