namespace Booking.Shared.Workflows;

public record PayBookingViaBankTransferInput(string BookingId, DateTimeOffset ExpiryDate);
