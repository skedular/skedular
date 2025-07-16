namespace Booking.Shared.Workflows.Payment.PayViaCard;

public record PayBookingViaCardInput(string BookingId, DateTimeOffset ExpiryDate, ICollection<string> InvoiceEmailList);
