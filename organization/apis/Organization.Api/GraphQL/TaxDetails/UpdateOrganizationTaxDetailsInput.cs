using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("UpdateOrganizationTaxDetailsInput")]
public class UpdateOrganizationTaxDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("gstNumber")] public string GstNumber { get; set; } = string.Empty;
    [GraphQLName("gstPercentage")] public string GstPercentage { get; set; } = string.Empty;
}
