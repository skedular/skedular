using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("OrganizationActiveOfferingDetails")]
public class OrganizationActiveOfferingDetails : Node
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;
    [GraphQLName("isEnterprise")] public bool IsEnterprise { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("start")] public DateTimeOffset Start { get; set; }
    [GraphQLName("end")] public DateTimeOffset End { get; set; }
    [GraphQLName("unitPrice")] public int? UnitPrice { get; set; }
    [GraphQLName("fixedPrice")] public int? FixedPrice { get; set; }
    [GraphQLName("discountPercentage")] public int DiscountPercentage { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("catalogVersion")] public CatalogVersionDetails? CatalogVersion { get; set; }
    [GraphQLName("purchasedUserCapacity")] public int? PurchasedUserCapacity { get; set; }

    [GraphQLName("purchasedLocationCapacity")]
    public int? PurchasedLocationCapacity { get; set; }

    [GraphQLName("purchasedTeamCapacity")] public int? PurchasedTeamCapacity { get; set; }
    [GraphQLName("underPriceLines")] public IEnumerable<string> UnderPriceLines { get; set; } = [];
    [GraphQLName("featureSet")] public IEnumerable<string> FeatureSet { get; set; } = [];
    [GraphQLName("free")] public bool Free { get; set; }
    [GraphQLName("earlyBird")] public bool EarlyBird { get; set; }
}
