using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("Customer_OrganizationDetails")]
public class OrganizationDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("logoUrl")] public string? LogoUrl { get; set; }
}
