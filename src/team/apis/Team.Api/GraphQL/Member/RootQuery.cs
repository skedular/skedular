using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Team.Api.GraphQL.Member;

[QueryType]
public class RootQuery
{
    public IEnumerable<TeamMemberRoleDetails> TeamMemberRoles() =>
    [
        new()
        {
            Type = TeamMemberRole.Owner,
            Name = TeamMemberRole.Owner.ToTeamMemberRoleName(),
        },
        new()
        {
            Type = TeamMemberRole.Administrator,
            Name = TeamMemberRole.Administrator.ToTeamMemberRoleName(),
        },
        new()
        {
            Type = TeamMemberRole.Member,
            Name = TeamMemberRole.Member.ToTeamMemberRoleName(),
        },
    ];

    public IEnumerable<TeamMemberStatusDetails> TeamMemberStatuses() =>
    [
        new()
        {
            Type = TeamMemberStatus.Active,
            Name = TeamMemberStatus.Active.ToTeamMemberStatusName(),
        },
        new()
        {
            Type = TeamMemberStatus.Inactive,
            Name = TeamMemberStatus.Inactive.ToTeamMemberStatusName(),
        },
    ];
}
