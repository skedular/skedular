using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationBillingDetailsPatchField")]
public enum OrganizationBillingDetailsPatchField
{
    CompanyName,
    Email,
    BillingAddress,
}

public record OrganizationBillingDetailsPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationBillingDetailsPatchField> FieldsToUpdate,
    string? CompanyName,
    string? Email,
    string? OsmType,
    string? OsmId,
    string? PlaceId,
    double? Longitude,
    double? Latitude,
    string? FormattedAddress,
    string? AddressLine1,
    string? AddressLine2,
    string? Suburb,
    string? City,
    string? Province,
    string? Zipcode,
    string? Country,
    string? CountryCode);
