using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("UpdateOrganizationTaxDetailsInput")]
public class UpdateOrganizationTaxDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("fieldsToUpdate")] public IEnumerable<OrganizationTaxDetailsPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("taxId")] public string? TaxId { get; set; }
    [GraphQLName("taxRatePercentage")] public decimal? TaxRatePercentage { get; set; }
}
