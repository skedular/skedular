using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("LocationTagOrganizationTagWhereInput")]
public class LocationTagOrganizationTagWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
