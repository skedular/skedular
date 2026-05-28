using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductEdge")]
public class ProductEdge(ProductDetails node, string cursor) : Edge<ProductDetails>(node, cursor);
