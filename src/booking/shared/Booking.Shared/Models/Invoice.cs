namespace Booking.Shared.Models;

public class Invoice
{
    public Booking Booking { get; set; } = new();
    public DateTimeOffset DueDate { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string OrganizationAddress { get; set; } = string.Empty;
    public string OrganizationGstNumber { get; set; } = string.Empty;
    public string OrganizationBankAccountNumber { get; set; } = string.Empty;
}
