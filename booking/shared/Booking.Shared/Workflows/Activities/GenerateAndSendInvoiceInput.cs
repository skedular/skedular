namespace Booking.Shared.Workflows.Activities;

public record GenerateAndSendInvoiceInput(string BookingId, bool FullyPaid, ICollection<string> InvoiceEmailList);
