using HotChocolate;

namespace Organization.Api.GraphQL.TaxDetails;

[GraphQLName("RemoveOrganizationTaxDetailsInput")]
public class RemoveOrganizationTaxDetailsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }
}
