using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripeCustomer : ModelBaseWithDeleted
{
    public string StripeCustomerId { get; set; } = string.Empty;
    public Organization Organization { get; set; } = new();
}
