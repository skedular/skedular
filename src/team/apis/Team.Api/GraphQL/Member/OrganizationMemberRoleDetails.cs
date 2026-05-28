using Api.Shared.Services.Models;
using HotChocolate;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberRoleDetails")]
public class TeamMemberRoleDetails
{
    [GraphQLName("type")] public TeamMemberRole Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
