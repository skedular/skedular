using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberWhereInput")]
public class TeamMemberWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
