using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationMemberVisibilityPolicyDetails")]
public class OrganizationMemberVisibilityPolicyDetails
{
    [GraphQLName("type")] public OrganizationMemberVisibilityPolicy Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
