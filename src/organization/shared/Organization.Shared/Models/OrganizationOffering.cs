using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Models;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Shared.Models;

public class OrganizationOffering : ModelBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool AutoRenew { get; set; }
    public int? UnitPrice { get; set; }
    public int? FixedPrice { get; set; }
    public Currency Currency { get; set; }
    public int? PurchasedUserCapacity { get; set; }
    public int? PurchasedLocationCapacity { get; set; }
    public int? PurchasedTeamCapacity { get; set; }
    public CatalogVersion? CatalogVersion { get; set; }
    public int DiscountPercentage { get; set; }
    public decimal HostCommissionPercentage { get; set; }
    public DateTimeOffset? SpacesBillingStartsAt { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
    public OrganizationStripePaymentIntent? OrganizationStripePaymentIntent { get; set; }
    public bool IsComplimentaryBridge => SpacesBillingStartsAt.HasValue && SpacesBillingStartsAt.Value > Start;
}
