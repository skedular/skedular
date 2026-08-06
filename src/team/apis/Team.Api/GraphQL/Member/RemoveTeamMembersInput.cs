using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("RemoveTeamMembersInput")]
public class RemoveTeamMembersInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("ids")]
    public IEnumerable<string> Ids { get; set; } = [];
}
