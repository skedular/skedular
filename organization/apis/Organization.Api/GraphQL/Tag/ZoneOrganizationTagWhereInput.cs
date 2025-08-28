using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("ZoneOrganizationTagWhereInput")]
public class ZoneOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
