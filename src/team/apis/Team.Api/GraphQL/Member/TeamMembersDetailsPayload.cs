using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMembersDetailsPayload")]
public class TeamMembersDetailsPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("members")] public IEnumerable<TeamMemberDetails> Members { get; set; } = [];
}
