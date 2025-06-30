using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberWhereInput")]
public class TeamMemberWhereInput
{
    [GraphQLName("teamId")] public string TeamId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
