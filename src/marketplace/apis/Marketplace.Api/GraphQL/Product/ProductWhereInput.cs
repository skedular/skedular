using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductWhereInput")]
public class ProductWhereInput
{
    [GraphQLName("organizationIds")]
    public IEnumerable<string>? OrganizationIds { get; set; }

    [GraphQLName("organizationCustomDomains")]
    public IEnumerable<string>? OrganizationCustomDomains { get; set; }

    [GraphQLName("productIds")]
    public IEnumerable<string>? ProductIds { get; set; } = [];

    [GraphQLName("includeInactive")]
    public bool IncludeInactive { get; set; }
}
