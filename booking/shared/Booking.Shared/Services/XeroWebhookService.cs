using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Token;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

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
    OrganizationConfiguration organizationConfiguration,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    TimeProvider timeProvider,
    ILogger<XeroWebhookService> logger) : IXeroWebhookService
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
                            syncTarget.LocalEntityId,
                            syncTarget.ExternalInvoiceIdHint),
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
        var invoiceLinks = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
            AccountingProviderConstants.Xero,
            externalInvoiceIds,
            cancellationToken);
        var invoiceInstances = await repositoryFactory.AccountingInvoiceInstanceRepository.GetByProviderAndExternalInvoiceIdsAsync(
            AccountingProviderConstants.Xero,
            externalInvoiceIds,
            cancellationToken);
        var invoiceLinkLookup = invoiceLinks
            .Where(invoiceLink => !string.IsNullOrWhiteSpace(invoiceLink.ExternalInvoiceId))
            .ToDictionary(invoiceLink => invoiceLink.ExternalInvoiceId!, StringComparer.Ordinal);
        var invoiceInstanceLookup = invoiceInstances
            .Where(invoiceInstance => !string.IsNullOrWhiteSpace(invoiceInstance.ExternalInvoiceId))
            .ToDictionary(invoiceInstance => invoiceInstance.ExternalInvoiceId, StringComparer.Ordinal);
        var tenantCache = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var syncTargets = new Dictionary<string, XeroWebhookSyncTarget>(StringComparer.Ordinal);

        foreach (var eventElement in eventElements)
        {
            if (!IsInvoiceEvent(eventElement))
            {
                continue;
            }

            var externalInvoiceId = GetOptionalString(eventElement, "resourceId");
            var tenantId = GetOptionalString(eventElement, "tenantId");
            if (string.IsNullOrWhiteSpace(externalInvoiceId))
            {
                continue;
            }

            if (!invoiceLinkLookup.TryGetValue(externalInvoiceId, out var accountingInvoiceLink))
            {
                if (invoiceInstanceLookup.TryGetValue(externalInvoiceId, out var accountingInvoiceInstance))
                {
                    accountingInvoiceLink = accountingInvoiceInstance.AccountingInvoiceExportLink;
                }
                else
                {
                    accountingInvoiceLink = await TryResolveRepeatingInvoiceLinkAsync(externalInvoiceId, tenantId, cancellationToken);
                    if (accountingInvoiceLink is null)
                    {
                        continue;
                    }
                }
            }

            if (!await MatchesTenantAsync(accountingInvoiceLink.OrganizationId, tenantId, tenantCache, cancellationToken))
            {
                continue;
            }

            var externalInvoiceIdHint = ResolveExternalInvoiceIdHint(accountingInvoiceLink, externalInvoiceId);
            if (string.Equals(accountingInvoiceLink.ExternalInvoiceMode, AccountingInvoiceExportModeConstants.RepeatingInvoice,
                    StringComparison.Ordinal) &&
                string.IsNullOrWhiteSpace(externalInvoiceIdHint))
            {
                continue;
            }

            var key = $"{accountingInvoiceLink.OrganizationId}:{accountingInvoiceLink.LocalEntityType}:{accountingInvoiceLink.LocalEntityId}";
            var candidateTarget = new XeroWebhookSyncTarget(
                accountingInvoiceLink.OrganizationId,
                accountingInvoiceLink.LocalEntityType,
                accountingInvoiceLink.LocalEntityId,
                externalInvoiceIdHint);

            if (syncTargets.TryGetValue(key, out var existingTarget) &&
                !string.IsNullOrWhiteSpace(existingTarget.ExternalInvoiceIdHint) &&
                string.IsNullOrWhiteSpace(candidateTarget.ExternalInvoiceIdHint))
            {
                continue;
            }

            syncTargets[key] = candidateTarget;
        }

        return syncTargets.Values.ToList();
    }

    private static string? ResolveExternalInvoiceIdHint(AccountingInvoiceExportLink accountingInvoiceLink, string externalInvoiceId)
    {
        if (!string.Equals(
                accountingInvoiceLink.ExternalInvoiceMode,
                AccountingInvoiceExportModeConstants.RepeatingInvoice,
                StringComparison.Ordinal))
        {
            return externalInvoiceId;
        }

        return string.Equals(accountingInvoiceLink.ExternalInvoiceId, externalInvoiceId, StringComparison.OrdinalIgnoreCase)
            ? null
            : externalInvoiceId;
    }

    private async Task<AccountingInvoiceExportLink?> ResolveRepeatingInvoiceLinkAsync(
        string externalInvoiceId,
        string? tenantId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(externalInvoiceId, out var invoiceId) || string.IsNullOrWhiteSpace(tenantId))
        {
            return null;
        }

        var organization = await organizationServiceClient.Admin_GetByXeroTenantIdAsync(
            new Admin_GetByXeroTenantIdInput { TenantId = tenantId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(organization.Id))
        {
            return null;
        }

        var xeroConnection = await organizationServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput { OrganizationId = organization.Id },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        if (string.IsNullOrWhiteSpace(xeroConnection.Id) || string.IsNullOrWhiteSpace(xeroConnection.TenantId))
        {
            return null;
        }

        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organization.Id, xeroConnection, cancellationToken);
        var invoiceResponse = await GetInvoiceAsync(
            xeroSdkClientFactory.CreateAccountingApi(),
            accessToken,
            refreshedConnection.TenantId,
            invoiceId,
            cancellationToken);
        var repeatingTemplateId = invoiceResponse._Invoices?.FirstOrDefault()?.RepeatingInvoiceID?.ToString();
        if (string.IsNullOrWhiteSpace(repeatingTemplateId))
        {
            return null;
        }

        var matchingLinks = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
            AccountingProviderConstants.Xero,
            [repeatingTemplateId],
            cancellationToken);
        return matchingLinks.FirstOrDefault();
    }

    private async Task<AccountingInvoiceExportLink?> TryResolveRepeatingInvoiceLinkAsync(
        string externalInvoiceId,
        string? tenantId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await ResolveRepeatingInvoiceLinkAsync(externalInvoiceId, tenantId, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Failed to resolve repeating Xero invoice link for invoice {ExternalInvoiceId} and tenant {TenantId}",
                externalInvoiceId,
                tenantId);
            return null;
        }
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

    private async Task<(string AccessToken, XeroConnection Connection)> EnsureValidAccessTokenAsync(
        string organizationId,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(xeroConnection.AccessTokenEncrypted) &&
            xeroConnection.AccessTokenExpiresAt.ToDateTimeOffset() > timeProvider.GetUtcNow().AddMinutes(1))
        {
            return (xeroTokenEncryptionService.Decrypt(xeroConnection.AccessTokenEncrypted), xeroConnection);
        }

        if (string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted))
        {
            throw new MissingXeroRefreshTokenException();
        }

        var refreshedToken = (XeroOAuth2Token)await xeroSdkClientFactory.CreateClient().RefreshAccessTokenAsync(
            new XeroOAuth2Token { RefreshToken = xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted) });
        var now = timeProvider.GetUtcNow();
        var refreshedConnection = await organizationServiceClient.Admin_RefreshXeroConnectionTokensAsync(
            new Admin_RefreshXeroConnectionTokensInput
            {
                OrganizationId = organizationId,
                AccessTokenEncrypted = xeroTokenEncryptionService.Encrypt(refreshedToken.AccessToken),
                RefreshTokenEncrypted = xeroTokenEncryptionService.Encrypt(
                    string.IsNullOrWhiteSpace(refreshedToken.RefreshToken)
                        ? xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted)
                        : refreshedToken.RefreshToken),
                AccessTokenExpiresAt = now.AddMinutes(30).ToTimestamp(),
                RefreshTokenExpiresAt = now.AddDays(60).ToTimestamp()
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (refreshedToken.AccessToken, refreshedConnection);
    }

    protected virtual Task<Invoices> GetInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid invoiceId,
        CancellationToken cancellationToken) =>
        accountingApi.GetInvoiceAsync(accessToken, tenantId, invoiceId, null, cancellationToken);

    private sealed record XeroWebhookSyncTarget(string OrganizationId, string LocalEntityType, string LocalEntityId, string? ExternalInvoiceIdHint);
}
