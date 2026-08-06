using HotChocolate;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Pricing;

[GraphQLName("PricingCatalogVersionDetails")]
public class PricingCatalogVersionDetails
{
    [GraphQLName("code")]
    public string Code { get; set; } = string.Empty;

    [GraphQLName("status")]
    public PricingCatalogVersionStatus Status { get; set; }

    [GraphQLName("effectiveFrom")]
    public DateTimeOffset EffectiveFrom { get; set; }

    [GraphQLName("effectiveUntil")]
    public DateTimeOffset? EffectiveUntil { get; set; }

    [GraphQLName("compatibilityNotes")]
    public string? CompatibilityNotes { get; set; }
}
