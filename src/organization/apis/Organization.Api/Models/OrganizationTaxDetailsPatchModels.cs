using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationTaxDetailsPatchField")]
public enum OrganizationTaxDetailsPatchField
{
    IsRegistered,
    TaxId,
    TaxRatePercentage,
}

public record OrganizationTaxDetailsPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationTaxDetailsPatchField> FieldsToUpdate,
    bool? IsRegistered,
    string? TaxId,
    decimal? TaxRatePercentage);
