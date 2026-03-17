using HotChocolate;

namespace Organization.Api.GraphQL.Invitation;

[GraphQLName("InviteCustomersToJoinOrganizationInput")]
public class InviteCustomersToJoinOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("emails")] public IEnumerable<string> Emails { get; set; } = [];
}
