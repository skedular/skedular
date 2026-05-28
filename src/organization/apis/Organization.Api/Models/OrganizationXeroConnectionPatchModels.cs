using Api.Shared.Services.Models;
using HotChocolate;

namespace Organization.Api.Models;

[GraphQLName("OrganizationXeroConnectionPatchField")]
public enum OrganizationXeroConnectionPatchField
{
    TenantId,
    TenantName,
    BillingMode,
    Scopes,
    IsActive,
    SendInvoicesViaXero,
    AutoReconcilePayments,
    DefaultSalesAccountCode,
    DefaultReceivablesAccountCode,
    DefaultTrackingCategory1,
    DefaultTrackingCategory2,
    DefaultBrandingThemeId,
    DefaultReferencePrefix
}

public record OrganizationXeroConnectionPatchRequest(
    string? OrganizationId,
    string? OrganizationCustomDomain,
    IReadOnlySet<OrganizationXeroConnectionPatchField> FieldsToUpdate,
    string? TenantId,
    string? TenantName,
    OrganizationXeroBillingMode? BillingMode,
    string? Scopes,
    bool? IsActive,
    bool? SendInvoicesViaXero,
    bool? AutoReconcilePayments,
    string? DefaultSalesAccountCode,
    string? DefaultReceivablesAccountCode,
    string? DefaultTrackingCategory1,
    string? DefaultTrackingCategory2,
    string? DefaultBrandingThemeId,
    string? DefaultReferencePrefix);
