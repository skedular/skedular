using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagTypeDetails")]
public class OrganizationTagTypeDetails
{
    [GraphQLName("tagType")] public OrganizationTagType TagType { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
