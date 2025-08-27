using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;
using Organization.Api.GraphQL.Organization;

namespace Organization.Api.GraphQL.PhysicalAddress;

[GraphQLName("OrganizationPhysicalAddressDetails")]
public class OrganizationPhysicalAddressDetails : Node
{
    [GraphQLName("osmType")] public string? OsmType { get; set; }
    [GraphQLName("osmId")] public string? OsmId { get; set; }
    [GraphQLName("placeId")] public string? PlaceId { get; set; }
    [GraphQLName("longitude")] public double? Longitude { get; set; }
    [GraphQLName("latitude")] public double? Latitude { get; set; }
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }

    [GraphQLName("multilinesFormattedAddress")]
    public string? MultilinesFormattedAddress { get; set; }

    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string? Suburb { get; set; }
    [GraphQLName("city")] public string? City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
    [GraphQLName("countryCode")] public string? CountryCode { get; set; }
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
