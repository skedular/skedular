using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripeConnectAccountAuthorization : ModelBase
{
    public bool IsAuthorized { get; set; }
    public OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; } = new();
}
