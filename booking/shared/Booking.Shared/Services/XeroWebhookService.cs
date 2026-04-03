using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Grpc;

namespace Booking.Shared.Services;

public interface IXeroWebhookService
{
    bool IsSignatureValid(string payloadJson, string? xeroSignature);
    Task ProcessAsync(string payloadJson, CancellationToken cancellationToken);
}

public class XeroWebhookService(
    XeroConfiguration xeroConfiguration,
    IRepositoryFactory repositoryFactory,
    ITemporalService temporalService,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    OrganizationConfiguration organizationConfiguration) : IXeroWebhookService
{
    public bool IsSignatureValid(string payloadJson, string? xeroSignature)
    {
        if (string.IsNullOrWhiteSpace(xeroSignature) || string.IsNullOrWhiteSpace(xeroConfiguration.WebhookKey))
        {
            return false;
        }

        byte[] actualSignature;
        try
        {
            actualSignature = Convert.FromBase64String(xeroSignature.Trim());
        }
        catch (FormatException)
        {
            return false;
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(xeroConfiguration.WebhookKey));
        var expectedSignature = hmac.ComputeHash(Encoding.UTF8.GetBytes(payloadJson));

        return CryptographicOperations.FixedTimeEquals(expectedSignature, actualSignature);
    }

    public async Task ProcessAsync(string payloadJson, CancellationToken cancellationToken)
    {
        var syncTargets = await GetSyncTargetsAsync(payloadJson, cancellationToken);
        if (syncTargets.Count == 0)
        {
            return;
        }

        foreach (var syncTarget in syncTargets)
        {
            switch (syncTarget.LocalEntityType)
            {
                case AccountingEntityTypeConstants.OrganizationArrearsInvoice:
                    await temporalService.SignalWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(
                        new MaintainOrganizationArrearsInvoiceAccountingStateInput(
                            syncTarget.OrganizationId,
                            syncTarget.LocalEntityId),
                        cancellationToken);
                    break;

                default:
                    await temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                        new MaintainAccountingInvoiceStateInput(
                            syncTarget.OrganizationId,
                            syncTarget.LocalEntityType,
                            syncTarget.LocalEntityId),
                        cancellationToken);
                    break;
            }
        }
    }

    private async Task<ICollection<XeroWebhookSyncTarget>> GetSyncTargetsAsync(string payloadJson, CancellationToken cancellationToken)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (!document.RootElement.TryGetProperty("events", out var eventsElement) || eventsElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var eventElements = eventsElement.EnumerateArray().ToList();
        var externalInvoiceIds = eventElements
            .Select(eventElement => IsInvoiceEvent(eventElement) ? GetOptionalString(eventElement, "resourceId") : null)
            .Where(externalInvoiceId => !string.IsNullOrWhiteSpace(externalInvoiceId))
            .Select(externalInvoiceId => externalInvoiceId!)
            .Distinct(StringComparer.Ordinal)
            .ToList();
        var invoiceLinks = await repositoryFactory.AccountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
            AccountingProviderConstants.Xero,
            externalInvoiceIds,
            cancellationToken);
        var invoiceLinkLookup = invoiceLinks
            .Where(invoiceLink => !string.IsNullOrWhiteSpace(invoiceLink.ExternalInvoiceId))
            .ToDictionary(invoiceLink => invoiceLink.ExternalInvoiceId!, StringComparer.Ordinal);
        var tenantCache = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var syncTargets = new Dictionary<string, XeroWebhookSyncTarget>(StringComparer.Ordinal);

        foreach (var eventElement in eventElements)
        {
            if (!IsInvoiceEvent(eventElement))
            {
                continue;
            }

            var externalInvoiceId = GetOptionalString(eventElement, "resourceId");
            if (string.IsNullOrWhiteSpace(externalInvoiceId))
            {
                continue;
            }

            if (!invoiceLinkLookup.TryGetValue(externalInvoiceId, out var accountingInvoiceLink))
            {
                continue;
            }

            var tenantId = GetOptionalString(eventElement, "tenantId");
            if (!await MatchesTenantAsync(accountingInvoiceLink.OrganizationId, tenantId, tenantCache, cancellationToken))
            {
                continue;
            }

            var key = $"{accountingInvoiceLink.OrganizationId}:{accountingInvoiceLink.LocalEntityType}:{accountingInvoiceLink.LocalEntityId}";
            syncTargets[key] = new XeroWebhookSyncTarget(
                accountingInvoiceLink.OrganizationId,
                accountingInvoiceLink.LocalEntityType,
                accountingInvoiceLink.LocalEntityId);
        }

        return syncTargets.Values.ToList();
    }

    private async Task<bool> MatchesTenantAsync(
        string organizationId,
        string? tenantId,
        Dictionary<string, string?> tenantCache,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            return true;
        }

        if (!tenantCache.TryGetValue(organizationId, out var expectedTenantId))
        {
            var xeroConnection = await organizationServiceClient.Admin_GetXeroConnectionAsync(
                new Admin_GetXeroConnectionInput { OrganizationId = organizationId },
                organizationConfiguration.ApiKey.CreateMetadata(),
                cancellationToken: cancellationToken);
            expectedTenantId = string.IsNullOrWhiteSpace(xeroConnection.TenantId) ? null : xeroConnection.TenantId;
            tenantCache[organizationId] = expectedTenantId;
        }

        return string.IsNullOrWhiteSpace(expectedTenantId) ||
               string.Equals(expectedTenantId, tenantId, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInvoiceEvent(JsonElement eventElement)
    {
        var eventCategory = GetOptionalString(eventElement, "eventCategory");
        if (string.Equals(eventCategory, "INVOICE", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var resourceType = GetOptionalString(eventElement, "resourceType");
        return string.Equals(resourceType, "INVOICE", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetOptionalString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private sealed record XeroWebhookSyncTarget(string OrganizationId, string LocalEntityType, string LocalEntityId);
}
