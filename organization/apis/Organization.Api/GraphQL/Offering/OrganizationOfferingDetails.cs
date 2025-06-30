using HotChocolate;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("OrganizationOfferingDetails")]
public class OrganizationOfferingDetails
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;
    [GraphQLName("isEnterprise")] public bool IsEnterprise { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }
    [GraphQLName("underPriceLines")] public IEnumerable<string> UnderPriceLines { get; set; } = [];
    [GraphQLName("featureSet")] public IEnumerable<string> FeatureSet { get; set; } = [];
    [GraphQLName("free")] public bool Free { get; set; }
    [GraphQLName("earlyBird")] public bool EarlyBird { get; set; }
}
