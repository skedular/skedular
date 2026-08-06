using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberStatusDetails")]
public class TeamMemberStatusDetails
{
    [GraphQLName("type")]
    public TeamMemberStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
