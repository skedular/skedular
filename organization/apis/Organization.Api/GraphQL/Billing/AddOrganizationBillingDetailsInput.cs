using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Billing;

[GraphQLName("AddOrganizationBillingDetailsInput")]
public class AddOrganizationBillingDetailsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public required string OrganizationId { get; set; }
    [GraphQLName("companyName")] public string? CompanyName { get; set; }
    [GraphQLName("email")] public string Email { get; set; } = string.Empty;
    [GraphQLName("osmType")] public string? OsmType { get; set; }
    [GraphQLName("osmId")] public string? OsmId { get; set; }
    [GraphQLName("placeId")] public string? PlaceId { get; set; }
    [GraphQLName("longitude")] public double? Longitude { get; set; }
    [GraphQLName("latitude")] public double? Latitude { get; set; }
    [GraphQLName("formattedAddress")] public string? FormattedAddress { get; set; }
    [GraphQLName("addressLine1")] public string AddressLine1 { get; set; } = string.Empty;
    [GraphQLName("addressLine2")] public string? AddressLine2 { get; set; }
    [GraphQLName("suburb")] public string Suburb { get; set; } = string.Empty;
    [GraphQLName("city")] public string City { get; set; } = string.Empty;
    [GraphQLName("province")] public string? Province { get; set; }
    [GraphQLName("zipcode")] public string Zipcode { get; set; } = string.Empty;
    [GraphQLName("country")] public string Country { get; set; } = string.Empty;
}
