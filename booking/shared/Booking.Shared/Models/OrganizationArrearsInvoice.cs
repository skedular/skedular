using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationArrearsInvoice : ModelBase
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string InvoiceUrl { get; set; } = string.Empty;
    public DateTimeOffset BillingPeriodStartInclusive { get; set; }
    public DateTimeOffset BillingPeriodEndExclusive { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Organization Organization { get; set; } = new();
    public Customer Customer { get; set; } = new();
}
