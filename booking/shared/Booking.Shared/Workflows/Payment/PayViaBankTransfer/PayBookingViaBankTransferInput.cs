namespace Booking.Shared.Workflows.Payment.PayViaBankTransfer;

public record PayBookingViaBankTransferInput(string BookingId, DateTimeOffset ExpiryDate, bool SendInvoice, ICollection<string> InvoiceEmailList);
