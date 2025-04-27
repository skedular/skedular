using Api.Shared.Services.Offering;
using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class OrganizationOffering : ReplicatedModelBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public int UnitPrice { get; set; }
    public int TotalNumberOfActiveCustomers { get; set; }
    public long TotalCost { get; set; }
    public DateTimeOffset? InvoiceDate { get; set; }
    public Organization Organization { get; set; } = new();
}
