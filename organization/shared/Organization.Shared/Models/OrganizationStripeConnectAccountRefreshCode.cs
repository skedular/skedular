using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripeConnectAccountRefreshCode : ModelBaseWithDeleted
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; } = new();
}
