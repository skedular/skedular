using HotChocolate;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationAddressDetails")]
public class AddressDetails
{
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }
    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string Suburb { get; set; } = string.Empty;
    [GraphQLName("city")] public string City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
}
