using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Team.Api.GraphQL.Member;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public IEnumerable<TeamMemberRole> TeamMemberRoles() => [TeamMemberRole.Owner, TeamMemberRole.Administrator, TeamMemberRole.Member];
}
