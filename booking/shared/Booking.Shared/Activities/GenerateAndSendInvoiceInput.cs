namespace Booking.Shared.Activities;

public record GenerateAndSendInvoiceInput(string BookingId, bool FullyPaid, ICollection<string> InvoiceEmailList);
