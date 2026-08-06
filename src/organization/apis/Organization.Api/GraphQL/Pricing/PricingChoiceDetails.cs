using Api.Shared.Services.Offering;
using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("PricingCatalogProductOfferingDetails")]
public class PricingCatalogProductOfferingDetails
{
    [GraphQLName("type")]
    public PricingCatalogProductOfferingCode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("PricingCatalogSubscriptionPlanDetails")]
public class PricingCatalogSubscriptionPlanDetails
{
    [GraphQLName("type")]
    public PricingCatalogSubscriptionPlanCode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("PricingCatalogPlanAvailabilityDetails")]
public class PricingCatalogPlanAvailabilityDetails
{
    [GraphQLName("type")]
    public PricingCatalogPlanAvailability Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("OrganizationOfferingPlanStatusDetails")]
public class OrganizationOfferingPlanStatusDetails
{
    [GraphQLName("type")]
    public OrganizationOfferingPlanStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("PricingEntitlementReasonCodeDetails")]
public class PricingEntitlementReasonCodeDetails
{
    [GraphQLName("type")]
    public EntitlementReasonCode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
