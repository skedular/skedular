using Api.Shared.Services.Models;
using HotChocolate;
using Organization.Api.Models;

namespace Organization.Api.GraphQL.Xero;

[GraphQLName("UpdateOrganizationXeroConnectionInput")]
public class UpdateOrganizationXeroConnectionInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("fieldsToUpdate")] public HashSet<OrganizationXeroConnectionPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("tenantId")] public string? TenantId { get; set; }
    [GraphQLName("tenantName")] public string? TenantName { get; set; }
    [GraphQLName("billingMode")] public OrganizationXeroBillingMode? BillingMode { get; set; }
    [GraphQLName("scopes")] public string? Scopes { get; set; }
    [GraphQLName("isActive")] public bool? IsActive { get; set; }
    [GraphQLName("sendInvoicesViaXero")] public bool? SendInvoicesViaXero { get; set; }
    [GraphQLName("autoReconcilePayments")] public bool? AutoReconcilePayments { get; set; }

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
}
