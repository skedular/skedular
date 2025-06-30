using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("Customer_OrganizationTagDetails")]
public class OrganizationTagDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
