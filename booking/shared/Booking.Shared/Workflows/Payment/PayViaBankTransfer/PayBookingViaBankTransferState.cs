namespace Booking.Shared.Workflows.Payment.PayViaBankTransfer;

public record PayBookingViaBankTransferState(string? PaymentStatus, bool BookingDeleted);
