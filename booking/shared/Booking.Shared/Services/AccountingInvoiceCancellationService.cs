using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroInvoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;
using XeroOAuth2Token = Xero.NetStandard.OAuth2.Token.XeroOAuth2Token;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Services;

public interface IAccountingInvoiceCancellationService
{
    Task CancelBookingAsync(BookingEntity booking, CancellationToken cancellationToken);
    Task CancelRecurringBookingAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken);
}

public class AccountingInvoiceCancellationService(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    TimeProvider timeProvider) : IAccountingInvoiceCancellationService
{
    public async Task CancelBookingAsync(BookingEntity booking, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        await CancelAsync(
            ResolveOrganizationId(booking),
            AccountingEntityTypeConstants.MarketplaceBooking,
            booking.MarketplaceBooking.Id,
            "Booking was cancelled.",
            "Booking was cancelled locally, but the live Xero invoice still requires cancellation.",
            booking.MarketplaceBooking.InvoiceNumber,
            booking.MarketplaceBooking.InvoiceUrl,
            cancellationToken);
    }

    public async Task CancelRecurringBookingAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken) =>
        await CancelAsync(
            ResolveOrganizationId(recurringBooking),
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            "Recurring booking was cancelled.",
            "Recurring booking was cancelled locally, but the live Xero repeating invoice template still requires cancellation.",
            recurringBooking.MarketplaceBooking?.InvoiceNumber,
            recurringBooking.MarketplaceBooking?.InvoiceUrl,
            cancellationToken);

    private async Task CancelAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        string cancelledMessage,
        string transitionRequiredMessage,
        string? localInvoiceNumber,
        string? localInvoiceUrl,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            localEntityType,
            localEntityId,
            cancellationToken);
        if (accountingInvoiceLink is null)
        {
            accountingInvoiceLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Skedular,
                localEntityType,
                localEntityId,
                cancellationToken);
            if (accountingInvoiceLink is null)
            {
                if (string.IsNullOrWhiteSpace(localInvoiceNumber) && string.IsNullOrWhiteSpace(localInvoiceUrl))
                {
                    return;
                }

                repositoryFactory.AccountingInvoiceExportLinkRepository.Add(
                    new AccountingInvoiceExportLink
                    {
                        Provider = AccountingProviderConstants.Skedular,
                        LocalEntityType = localEntityType,
                        LocalEntityId = localEntityId,
                        ExternalInvoiceNumber = localInvoiceNumber,
                        ExternalInvoiceUrl = localInvoiceUrl,
                        ExternalStatus = AccountingStatusConstants.Cancelled,
                        ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Cancelled,
                        ExportConfigurationMessage = cancelledMessage,
                        OrganizationId = organizationId
                    });

                return;
            }
        }

        if (string.Equals(accountingInvoiceLink.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal) ||
            string.Equals(accountingInvoiceLink.ExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal))
        {
            accountingInvoiceLink.LastError = null;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            return;
        }

        if (!HasLiveExternalInvoice(accountingInvoiceLink))
        {
            accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Cancelled;
            accountingInvoiceLink.ExportConfigurationMessage = cancelledMessage;
            accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Cancelled;
            accountingInvoiceLink.LastError = null;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            return;
        }

        try
        {
            var xeroConnection = await GetOrganizationXeroConnectionAsync(organizationId, cancellationToken);
            if (!IsXeroConnectionReady(xeroConnection) ||
                !Guid.TryParse(accountingInvoiceLink.ExternalInvoiceId, out var externalInvoiceId))
            {
                MarkTransitionRequired(accountingInvoiceLink, transitionRequiredMessage, null);
                return;
            }

            var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection!, cancellationToken);
            var accountingApi = xeroSdkClientFactory.CreateAccountingApi();

            if (IsLiveRepeatingInvoice(accountingInvoiceLink))
            {
                await CancelLiveRepeatingInvoiceAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    externalInvoiceId,
                    accountingInvoiceLink.Id,
                    cancellationToken);
            }
            else
            {
                await CancelLiveStandardInvoiceAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    externalInvoiceId,
                    accountingInvoiceLink.Id,
                    cancellationToken);
            }

            accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Cancelled;
            accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Cancelled;
            accountingInvoiceLink.ExportConfigurationMessage = cancelledMessage;
            accountingInvoiceLink.LastSyncedAt = timeProvider.GetUtcNow();
            accountingInvoiceLink.LastError = null;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
        }
        catch (Exception exception)
        {
            MarkTransitionRequired(accountingInvoiceLink, transitionRequiredMessage, exception.Message);
        }
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput { OrganizationId = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return string.IsNullOrWhiteSpace(response.Id) ? null : response;
    }

    protected virtual Task CancelLiveRepeatingInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid externalInvoiceId,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        accountingApi.UpdateRepeatingInvoiceAsync(
            accessToken,
            tenantId,
            externalInvoiceId,
            new RepeatingInvoices
            {
                _RepeatingInvoices =
                [
                    new RepeatingInvoice { RepeatingInvoiceID = externalInvoiceId, Status = RepeatingInvoice.StatusEnum.DELETED }
                ]
            },
            idempotencyKey,
            cancellationToken);

    protected virtual Task CancelLiveStandardInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid externalInvoiceId,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        accountingApi.UpdateInvoiceAsync(
            accessToken,
            tenantId,
            externalInvoiceId,
            new Invoices
            {
                _Invoices =
                [
                    new XeroInvoice { InvoiceID = externalInvoiceId, Status = XeroInvoice.StatusEnum.VOIDED }
                ]
            },
            null,
            idempotencyKey,
            cancellationToken);

    private static bool IsXeroConnectionReady(XeroConnection? xeroConnection) =>
        xeroConnection is not null &&
        xeroConnection.IsActive &&
        xeroConnection.HasRefreshToken &&
        !string.IsNullOrWhiteSpace(xeroConnection.TenantId);

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
        var accessTokenEncrypted = xeroTokenEncryptionService.Encrypt(refreshedToken.AccessToken);
        var refreshTokenEncrypted = xeroTokenEncryptionService.Encrypt(
            string.IsNullOrWhiteSpace(refreshedToken.RefreshToken)
                ? xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted)
                : refreshedToken.RefreshToken);
        var refreshedConnection = await organizationServiceClient.Admin_RefreshXeroConnectionTokensAsync(
            new Admin_RefreshXeroConnectionTokensInput
            {
                OrganizationId = organizationId,
                AccessTokenEncrypted = accessTokenEncrypted,
                RefreshTokenEncrypted = refreshTokenEncrypted,
                AccessTokenExpiresAt = now.AddMinutes(30).ToTimestamp(),
                RefreshTokenExpiresAt = now.AddDays(60).ToTimestamp()
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (refreshedToken.AccessToken, refreshedConnection);
    }

    private static string ResolveOrganizationId(RecurringBooking recurringBooking)
    {
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking.ProductVersion.Product.Organization);
        return recurringBooking.MarketplaceBooking.ProductVersion.Product.Organization.Id;
    }

    private static string ResolveOrganizationId(BookingEntity booking)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion.Product.Organization);
        return booking.MarketplaceBooking.ProductVersion.Product.Organization.Id;
    }

    private void MarkTransitionRequired(
        AccountingInvoiceExportLink accountingInvoiceLink,
        string message,
        string? lastError)
    {
        accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.TransitionRequired;
        accountingInvoiceLink.ExportConfigurationMessage = message;
        accountingInvoiceLink.LastError = lastError;
        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
    }

    private static bool HasLiveExternalInvoice(AccountingInvoiceExportLink accountingInvoiceLink) =>
        !string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId);

    private static bool IsLiveRepeatingInvoice(AccountingInvoiceExportLink accountingInvoiceLink) =>
        accountingInvoiceLink.ExternalInvoiceMode == AccountingInvoiceExportModeConstants.RepeatingInvoice &&
        !string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId);
}
