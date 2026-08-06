using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Billing;

[GraphQLName("UpdateOrganizationBillingDetailsInput")]
public class UpdateOrganizationBillingDetailsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("fieldsToUpdate")]
    public HashSet<OrganizationBillingDetailsPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("companyName")]
    public string? CompanyName { get; set; }

    [GraphQLName("email")]
    public string? Email { get; set; }

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

    [GraphQLName("addressLine1")]
    public string? AddressLine1 { get; set; }

    [GraphQLName("addressLine2")]
    public string? AddressLine2 { get; set; }

    [GraphQLName("suburb")]
    public string? Suburb { get; set; }

    [GraphQLName("city")]
    public string? City { get; set; }

    [GraphQLName("province")]
    public string? Province { get; set; }

    [GraphQLName("zipcode")]
    public string? Zipcode { get; set; }

    [GraphQLName("country")]
    public string? Country { get; set; }

    [GraphQLName("countryCode")]
    public string? CountryCode { get; set; }
}
