using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationTypeDetails")]
public class OrganizationTypeDetails
{
    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
