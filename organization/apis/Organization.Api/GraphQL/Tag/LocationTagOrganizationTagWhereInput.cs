using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("LocationTagOrganizationTagWhereInput")]
public class LocationTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
