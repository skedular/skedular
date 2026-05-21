using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationTaxDetailsPatchField")]
public enum OrganizationTaxDetailsPatchField
{
    TaxId,
    TaxRatePercentage
}

public record OrganizationTaxDetailsPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationTaxDetailsPatchField> FieldsToUpdate,
    string? TaxId,
    decimal? TaxRatePercentage);
