using Api.Shared.Services.Models;
using HotChocolate.Types;

namespace Organization.Api.GraphQL.Member;

[QueryType]
public class RootQuery
{
    [UseResolverScope]
    public IEnumerable<OrganizationMemberRole> OrganizationMemberRoles() =>
    [
        OrganizationMemberRole.Owner,
        OrganizationMemberRole.Administrator,
        OrganizationMemberRole.Member
    ];
}
