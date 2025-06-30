using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberPayload")]
public class OrganizationMemberPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
