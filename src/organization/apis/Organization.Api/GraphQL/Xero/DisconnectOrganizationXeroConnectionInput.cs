using HotChocolate;

namespace Organization.Api.GraphQL.Xero;

[GraphQLName("DisconnectOrganizationXeroConnectionInput")]
public class DisconnectOrganizationXeroConnectionInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }
}
