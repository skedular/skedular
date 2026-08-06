using Enterprise.Shared.Pagination;
using HotChocolate;
using Marketplace.Shared.Models;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductOrderInput")]
public class ProductOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public ProductOrderField Field { get; set; }
}
