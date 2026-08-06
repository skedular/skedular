using Api.Shared.Services.Models;
using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("PricingCatalogDetails")]
public class PricingCatalogDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("activeVersion")]
    public PricingCatalogVersionDetails ActiveVersion { get; set; } = new();

    [GraphQLName("productOfferings")]
    public IEnumerable<PricingProductOfferingDetails> ProductOfferings { get; set; } = [];

    [GraphQLName("generatedAt")]
    public DateTimeOffset GeneratedAt { get; set; }
}

[GraphQLName("PricingProductOfferingDetails")]
public class PricingProductOfferingDetails
{
    [GraphQLName("code")]
    public PricingCatalogProductOfferingCode Code { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("description")]
    public string Description { get; set; } = string.Empty;

    [GraphQLName("visibility")]
    public PricingCatalogVisibility Visibility { get; set; }

    [GraphQLName("plans")]
    public IEnumerable<PricingSubscriptionPlanDetails> Plans { get; set; } = [];
}

[GraphQLName("PricingSubscriptionPlanDetails")]
public class PricingSubscriptionPlanDetails
{
    [GraphQLName("code")]
    public PricingCatalogSubscriptionPlanCode Code { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("description")]
    public string Description { get; set; } = string.Empty;

    [GraphQLName("commercialModel")]
    public PricingCatalogCommercialModel CommercialModel { get; set; }

    [GraphQLName("features")]
    public IEnumerable<PricingPlanFeatureDetails> Features { get; set; } = [];

    [GraphQLName("limits")]
    public IEnumerable<PricingPlanLimitDetails> Limits { get; set; } = [];

    [GraphQLName("prices")]
    public IEnumerable<PricingPlanPriceDetails> Prices { get; set; } = [];

    [GraphQLName("capacityOptions")]
    public IEnumerable<PricingCapacityOptionDetails> CapacityOptions { get; set; } = [];

    [GraphQLName("availability")]
    public PricingCatalogPlanAvailability Availability { get; set; }

    [GraphQLName("recommended")]
    public bool Recommended { get; set; }

    [GraphQLName("displayOrder")]
    public int DisplayOrder { get; set; }
}

[GraphQLName("PricingCapacityOptionDetails")]
public class PricingCapacityOptionDetails
{
    [GraphQLName("code")]
    public string Code { get; set; } = string.Empty;

    [GraphQLName("userCapacity")]
    public int? UserCapacity { get; set; }

    [GraphQLName("label")]
    public string Label { get; set; } = string.Empty;

    [GraphQLName("price")]
    public PricingPlanPriceDetails? Price { get; set; }

    [GraphQLName("availability")]
    public PricingCatalogPlanAvailability Availability { get; set; }

    [GraphQLName("displayOrder")]
    public int DisplayOrder { get; set; }
}

[GraphQLName("PricingPlanFeatureDetails")]
public class PricingPlanFeatureDetails
{
    [GraphQLName("code")]
    public string Code { get; set; } = string.Empty;

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("displayOrder")]
    public int DisplayOrder { get; set; }
}

[GraphQLName("PricingPlanLimitDetails")]
public class PricingPlanLimitDetails
{
    [GraphQLName("code")]
    public string Code { get; set; } = string.Empty;

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("limit")]
    public int? Limit { get; set; }

    [GraphQLName("unlimited")]
    public bool Unlimited { get; set; }
}

[GraphQLName("PricingPlanPriceDetails")]
public class PricingPlanPriceDetails
{
    [GraphQLName("currency")]
    public Currency Currency { get; set; }

    [GraphQLName("amount")]
    public decimal Amount { get; set; }

    [GraphQLName("cadence")]
    public string Cadence { get; set; } = string.Empty;

    [GraphQLName("taxInclusive")]
    public bool TaxInclusive { get; set; }
}
