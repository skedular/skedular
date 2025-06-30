using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerEdge")]
public class CustomerEdge(CustomerDetails node, string cursor) : Edge<CustomerDetails>(node, cursor);
