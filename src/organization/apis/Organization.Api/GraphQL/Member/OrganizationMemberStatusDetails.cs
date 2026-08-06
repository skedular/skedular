using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberStatusDetails")]
public class OrganizationMemberStatusDetails
{
    [GraphQLName("type")]
    public OrganizationMemberStatus Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
