using HotChocolate;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("OrganizationTaxDetails")]
public class OrganizationTaxDetails
{
    [GraphQLName("gstNumber")] public string GstNumber { get; set; } = string.Empty;
    [GraphQLName("gstPercentage")] public string GstPercentage { get; set; } = string.Empty;
}
