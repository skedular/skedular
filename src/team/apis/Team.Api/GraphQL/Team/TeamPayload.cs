using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("TeamPayload")]
public class TeamPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("team")]
    public TeamDetails Team { get; set; } = new();
}
