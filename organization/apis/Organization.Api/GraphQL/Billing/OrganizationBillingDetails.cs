using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Billing;

[GraphQLName("OrganizationBillingDetails")]
public class OrganizationBillingDetails : Node
{
    [GraphQLName("companyName")] public string? CompanyName { get; set; }
    [GraphQLName("email")] public string Email { get; set; } = string.Empty;
    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string Suburb { get; set; } = string.Empty;
    [GraphQLName("city")] public string City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
