using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductConnection")]
public class ProductConnection : Connection<ProductEdge>;
