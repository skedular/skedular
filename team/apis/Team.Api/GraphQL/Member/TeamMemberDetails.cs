using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Team.Api.GraphQL.Team;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberDetails")]
public class TeamMemberDetails : Node
{
    [GraphQLName("role")] public TeamMemberRole? Role { get; set; }
    [GraphQLName("status")] public TeamMemberStatus Status { get; set; }
    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
    [GraphQLName("organizationMember")] public TeamOrganizationMemberDetails? OrganizationMember { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
