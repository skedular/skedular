using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("OrganizationTaxDetails")]
public class OrganizationTaxDetails : Node
{
    [GraphQLName("taxId")] public string TaxId { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public decimal TaxRatePercentage { get; set; }
    [GraphQLName("id")] [ID] public required string Id { get; set; }
}
