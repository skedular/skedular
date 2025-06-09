using Api.Shared.Services.Offering;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationOffering : ModelBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool AutoRenew { get; set; }
    public int UnitPrice { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
    public StripePaymentIntent? StripePaymentIntent { get; set; }
}
