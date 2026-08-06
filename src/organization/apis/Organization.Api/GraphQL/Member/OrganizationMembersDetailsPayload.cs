using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMembersDetailsPayload")]
public class OrganizationMembersDetailsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("members")]
    public IEnumerable<OrganizationMemberDetails> Members { get; set; } = [];
}
