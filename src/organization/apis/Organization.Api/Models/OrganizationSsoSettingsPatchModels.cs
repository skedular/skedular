using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.Models;

[GraphQLName("OrganizationSsoSettingsPatchField")]
public enum OrganizationSsoSettingsPatchField
{
    SsoSettings
}

public record OrganizationSsoSettingsPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationSsoSettingsPatchField> FieldsToUpdate,
    OrganizationSsoSettings SsoSettings);
