using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("CustomerResourceDetails")]
public class CustomerResourceDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
}
