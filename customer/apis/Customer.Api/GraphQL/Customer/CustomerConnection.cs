using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerConnection")]
public class CustomerConnection : Connection<CustomerEdge>;
