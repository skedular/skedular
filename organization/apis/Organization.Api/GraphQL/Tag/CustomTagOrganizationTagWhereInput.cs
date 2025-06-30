using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("CustomTagOrganizationTagWhereInput")]
public class CustomTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
