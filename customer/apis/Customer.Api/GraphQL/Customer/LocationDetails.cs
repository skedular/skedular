using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("Customer_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("organization")] public OrganizationDetails? Organization { get; set; }
}
