using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationInvoiceCounter : ModelBase
{
    public int InvoiceNumber { get; set; }
    public Organization Organization { get; set; } = new();
}
