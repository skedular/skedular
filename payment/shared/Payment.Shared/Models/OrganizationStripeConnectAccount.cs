using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class OrganizationStripeConnectAccount : ModelBaseWithDeleted
{
    public string Name { get; set; }
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public string Type { get; set; }

    public Organization Organization { get; set; }
}
