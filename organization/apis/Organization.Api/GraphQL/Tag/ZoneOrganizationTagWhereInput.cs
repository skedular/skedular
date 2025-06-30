using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("ZoneOrganizationTagWhereInput")]
public class ZoneOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
