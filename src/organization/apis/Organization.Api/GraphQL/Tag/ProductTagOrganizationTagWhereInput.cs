using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("ProductTagOrganizationTagWhereInput")]
public class ProductTagOrganizationTagWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
}
