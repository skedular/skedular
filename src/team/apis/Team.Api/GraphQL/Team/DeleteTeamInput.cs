using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("DeleteTeamInput")]
public class DeleteTeamInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;
}
