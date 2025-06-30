using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("OrganizationActiveOfferingDetails")]
public class OrganizationActiveOfferingDetails : Node
{
    [GraphQLName("code")] public string Code { get; set; } = string.Empty;
    [GraphQLName("isEnterprise")] public bool IsEnterprise { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("start")] public DateTimeOffset Start { get; set; }
    [GraphQLName("end")] public DateTimeOffset End { get; set; }
    [GraphQLName("unitPrice")] public int UnitPrice { get; set; }
    [GraphQLName("underPriceLines")] public IEnumerable<string> UnderPriceLines { get; set; } = [];
    [GraphQLName("featureSet")] public IEnumerable<string> FeatureSet { get; set; } = [];
    [GraphQLName("free")] public bool Free { get; set; }
    [GraphQLName("earlyBird")] public bool EarlyBird { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
