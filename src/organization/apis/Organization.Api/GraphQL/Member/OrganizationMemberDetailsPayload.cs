using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberDetailsPayload")]
public class OrganizationMemberDetailsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("member")]
    public OrganizationMemberDetails? Member { get; set; }
}
