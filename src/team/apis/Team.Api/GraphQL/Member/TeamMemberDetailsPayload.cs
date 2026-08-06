using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberDetailsPayload")]
public class TeamMemberDetailsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("member")]
    public TeamMemberDetails? Member { get; set; }
}
