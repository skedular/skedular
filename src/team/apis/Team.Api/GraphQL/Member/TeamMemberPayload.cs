using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberPayload")]
public class TeamMemberPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("teamMember")]
    public TeamMemberDetails TeamMember { get; set; } = new();
}
