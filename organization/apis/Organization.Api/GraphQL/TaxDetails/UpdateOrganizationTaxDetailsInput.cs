using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("UpdateOrganizationTaxDetailsInput")]
public class UpdateOrganizationTaxDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("taxId")] public string TaxId { get; set; } = string.Empty;
    [GraphQLName("taxRatePercentage")] public string TaxRatePercentage { get; set; } = string.Empty;
}
