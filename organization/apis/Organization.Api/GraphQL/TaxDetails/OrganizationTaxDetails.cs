using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("OrganizationTaxDetails")]
public class OrganizationTaxDetails : Node
{
    [GraphQLName("taxId")] public string TaxId { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public string TaxRatePercentage { get; set; } = string.Empty;
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}
