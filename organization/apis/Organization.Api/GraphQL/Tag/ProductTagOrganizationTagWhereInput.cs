using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("ProductTagOrganizationTagWhereInput")]
public class ProductTagOrganizationTagWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
