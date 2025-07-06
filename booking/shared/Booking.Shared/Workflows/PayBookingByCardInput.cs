namespace Booking.Shared.Workflows;

public record PayBookingByCardInput(string BookingId, DateTimeOffset ExpiryDate);
