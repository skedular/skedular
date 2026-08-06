using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Billing;

[GraphQLName("OrganizationBillingDetails")]
public class OrganizationBillingDetails : Node
{
    [GraphQLName("companyName")]
    public string? CompanyName { get; set; }

    [GraphQLName("email")]
    public string Email { get; set; } = string.Empty;

    [GraphQLName("osmType")]
    public string? OsmType { get; set; }

    [GraphQLName("osmId")]
    public string? OsmId { get; set; }

    [GraphQLName("placeId")]
    public string? PlaceId { get; set; }

    [GraphQLName("longitude")]
    public double? Longitude { get; set; }

    [GraphQLName("latitude")]
    public double? Latitude { get; set; }

    [GraphQLName("formattedAddress")]
    public string? FormattedAddress { get; set; }

    [GraphQLName("multilinesFormattedAddress")]
    public string? MultilinesFormattedAddress { get; set; }

    [GraphQLName("addressLine1")]
    public string AddressLine1 { get; set; } = string.Empty;

    [GraphQLName("addressLine2")]
    public string? AddressLine2 { get; set; }

    [GraphQLName("suburb")]
    public string? Suburb { get; set; }

    [GraphQLName("city")]
    public string? City { get; set; } = string.Empty;

    [GraphQLName("province")]
    public string? Province { get; set; }

    [GraphQLName("zipcode")]
    public string Zipcode { get; set; } = string.Empty;

    [GraphQLName("country")]
    public string Country { get; set; } = string.Empty;

    [GraphQLName("countryCode")]
    public string? CountryCode { get; set; }
}
