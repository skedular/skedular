using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("OrganizationTaxDetails")]
public class OrganizationTaxDetails : Node
{
    [GraphQLName("isRegistered")] public bool IsRegistered { get; set; }
    [GraphQLName("taxId")] public string TaxId { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public decimal TaxRatePercentage { get; set; }
}
