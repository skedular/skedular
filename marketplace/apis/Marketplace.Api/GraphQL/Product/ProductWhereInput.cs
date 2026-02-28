using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductWhereInput")]
public class ProductWhereInput
{
    [GraphQLName("organizationUniqueAlphanumericNames")]
    public IEnumerable<string>? OrganizationUniqueAlphanumericNames { get; set; }

    [GraphQLName("productIds")] public IEnumerable<string>? ProductIds { get; set; } = [];
    [GraphQLName("includeInactive")] public bool IncludeInactive { get; set; }
}
