using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("ZoneOrganizationTagWhereInput")]
public class ZoneOrganizationTagWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
