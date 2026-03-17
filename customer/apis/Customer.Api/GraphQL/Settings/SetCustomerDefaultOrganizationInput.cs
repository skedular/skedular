using HotChocolate;

namespace Customer.Api.GraphQL.Settings;

[GraphQLName("SetCustomerDefaultOrganizationInput")]
public class SetCustomerDefaultOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }
}
