using Api.Shared.Services.Offering;
using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationOfferingPatchField")]
public enum OrganizationOfferingPatchField
{
    OfferingCode,
}

public record OrganizationOfferingPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationOfferingPatchField> FieldsToUpdate,
    OfferingCode? OfferingCode);
