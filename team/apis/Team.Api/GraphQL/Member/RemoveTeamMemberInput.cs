using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("RemoveTeamMemberInput")]
public class RemoveTeamMemberInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
