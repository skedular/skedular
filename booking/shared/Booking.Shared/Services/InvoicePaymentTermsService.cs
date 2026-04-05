namespace Booking.Shared.Services;

public interface IInvoicePaymentTermsService
{
    int GetInvoiceDueInDays(int? invoiceDueInDays);
    DateTimeOffset GetDueDate(DateTimeOffset invoiceDate, int? invoiceDueInDays);
}

public class InvoicePaymentTermsService : IInvoicePaymentTermsService
{
    public const int DefaultInvoiceDueInDays = 7;

    public int GetInvoiceDueInDays(int? invoiceDueInDays) =>
        invoiceDueInDays is > 0 ? invoiceDueInDays.Value : DefaultInvoiceDueInDays;

    public DateTimeOffset GetDueDate(DateTimeOffset invoiceDate, int? invoiceDueInDays) =>
        new(invoiceDate.UtcDateTime.Date.AddDays(GetInvoiceDueInDays(invoiceDueInDays)), TimeSpan.Zero);
}
