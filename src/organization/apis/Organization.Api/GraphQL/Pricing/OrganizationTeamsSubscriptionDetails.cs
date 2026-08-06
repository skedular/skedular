using Api.Shared.Services.Models;
using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("OrganizationTeamsSubscriptionDetails")]
public class OrganizationTeamsSubscriptionDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("organizationId")]
    public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("productOfferingCode")]
    public PricingCatalogProductOfferingCode ProductOfferingCode { get; set; }

    [GraphQLName("planCode")]
    public PricingCatalogSubscriptionPlanCode PlanCode { get; set; }

    [GraphQLName("unitPrice")]
    public int? UnitPrice { get; set; }

    [GraphQLName("fixedPrice")]
    public int? FixedPrice { get; set; }

    [GraphQLName("currency")]
    public Currency Currency { get; set; }

    [GraphQLName("purchasedUserCapacity")]
    public int? PurchasedUserCapacity { get; set; }

    [GraphQLName("purchasedLocationCapacity")]
    public int? PurchasedLocationCapacity { get; set; }

    [GraphQLName("purchasedTeamCapacity")]
    public int? PurchasedTeamCapacity { get; set; }

    [GraphQLName("catalogVersionCode")]
    public string CatalogVersionCode { get; set; } = string.Empty;

    [GraphQLName("status")]
    public OrganizationOfferingPlanStatus Status { get; set; }

    [GraphQLName("effectiveFrom")]
    public DateTimeOffset EffectiveFrom { get; set; }

    [GraphQLName("effectiveUntil")]
    public DateTimeOffset? EffectiveUntil { get; set; }

    [GraphQLName("autoRenew")]
    public bool AutoRenew { get; set; }
}
