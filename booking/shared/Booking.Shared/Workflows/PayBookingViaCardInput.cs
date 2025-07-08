namespace Booking.Shared.Workflows;

public record PayBookingViaCardInput(string BookingId, DateTimeOffset ExpiryDate, bool SendInvoice, ICollection<string> InvoiceEmailList);
