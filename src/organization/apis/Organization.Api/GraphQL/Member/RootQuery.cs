using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Organization.Api.GraphQL.Member;

[QueryType]
public class RootQuery
{
    public IEnumerable<OrganizationMemberRoleDetails> OrganizationMemberRoles() =>
    [
        new() { Type = OrganizationMemberRole.Owner, Name = OrganizationMemberRole.Owner.ToOrganizationMemberRoleName() },
        new() { Type = OrganizationMemberRole.Administrator, Name = OrganizationMemberRole.Administrator.ToOrganizationMemberRoleName() },
        new() { Type = OrganizationMemberRole.Member, Name = OrganizationMemberRole.Member.ToOrganizationMemberRoleName() }
    ];

    public IEnumerable<OrganizationMemberStatusDetails> OrganizationMemberStatuses() =>
    [
        new() { Type = OrganizationMemberStatus.Active, Name = OrganizationMemberStatus.Active.ToOrganizationMemberStatusName() },
        new() { Type = OrganizationMemberStatus.Inactive, Name = OrganizationMemberStatus.Inactive.ToOrganizationMemberStatusName() }
    ];
}
