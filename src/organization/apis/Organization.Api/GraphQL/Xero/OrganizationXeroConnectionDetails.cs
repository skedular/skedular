using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Xero;

[GraphQLName("OrganizationXeroConnection")]
public class OrganizationXeroConnectionDetails : Node
{
    [GraphQLName("tenantId")]
    public string TenantId { get; set; } = string.Empty;

    [GraphQLName("tenantName")]
    public string TenantName { get; set; } = string.Empty;

    [GraphQLName("billingMode")]
    public OrganizationXeroBillingMode BillingMode { get; set; }

    [GraphQLName("scopes")]
    public string? Scopes { get; set; }

    [GraphQLName("isActive")]
    public bool IsActive { get; set; }

    [GraphQLName("sendInvoicesViaXero")]
    public bool SendInvoicesViaXero { get; set; }

    [GraphQLName("autoReconcilePayments")]
    public bool AutoReconcilePayments { get; set; }

    [GraphQLName("defaultSalesAccountCode")]
    public string? DefaultSalesAccountCode { get; set; }

    [GraphQLName("defaultReceivablesAccountCode")]
    public string? DefaultReceivablesAccountCode { get; set; }

    [GraphQLName("defaultTrackingCategory1")]
    public string? DefaultTrackingCategory1 { get; set; }

    [GraphQLName("defaultTrackingCategory2")]
    public string? DefaultTrackingCategory2 { get; set; }

    [GraphQLName("defaultBrandingThemeId")]
    public string? DefaultBrandingThemeId { get; set; }

    [GraphQLName("defaultReferencePrefix")]
    public string? DefaultReferencePrefix { get; set; }

    [GraphQLName("accessTokenExpiresAt")]
    public DateTimeOffset? AccessTokenExpiresAt { get; set; }

    [GraphQLName("refreshTokenExpiresAt")]
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }

    [GraphQLName("lastSuccessfulSyncAt")]
    public DateTimeOffset? LastSuccessfulSyncAt { get; set; }

    [GraphQLName("lastError")]
    public string? LastError { get; set; }

    [GraphQLName("hasAccessToken")]
    public bool HasAccessToken { get; set; }

    [GraphQLName("hasRefreshToken")]
    public bool HasRefreshToken { get; set; }
}
