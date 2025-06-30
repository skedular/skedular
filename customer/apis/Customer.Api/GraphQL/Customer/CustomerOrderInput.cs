using Customer.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerOrderInput")]
public class CustomerOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public CustomerOrderField Field { get; set; }
}
