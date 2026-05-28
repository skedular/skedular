using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationXeroConnection : ModelBase
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public OrganizationXeroBillingMode BillingMode { get; set; }
    public string? Scopes { get; set; }
    public bool IsActive { get; set; }
    public bool SendInvoicesViaXero { get; set; }
    public bool AutoReconcilePayments { get; set; }
    public string? DefaultSalesAccountCode { get; set; }
    public string? DefaultReceivablesAccountCode { get; set; }
    public string? DefaultTrackingCategory1 { get; set; }
    public string? DefaultTrackingCategory2 { get; set; }
    public string? DefaultBrandingThemeId { get; set; }
    public string? DefaultReferencePrefix { get; set; }
    public DateTimeOffset? AccessTokenExpiresAt { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
    public DateTimeOffset? LastSuccessfulSyncAt { get; set; }
    public string? LastError { get; set; }
    public string? AccessTokenEncrypted { get; set; }
    public string? RefreshTokenEncrypted { get; set; }
    public bool HasAccessToken { get; set; }
    public bool HasRefreshToken { get; set; }
    public Organization? Organization { get; set; }
}
