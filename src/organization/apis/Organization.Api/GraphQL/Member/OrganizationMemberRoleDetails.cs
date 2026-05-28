using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberRoleDetails")]
public class OrganizationMemberRoleDetails
{
    [GraphQLName("type")] public OrganizationMemberRole Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
