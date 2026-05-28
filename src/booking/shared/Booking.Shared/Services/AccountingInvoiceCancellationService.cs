using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
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
    Task CancelRecurringBookingFutureBillingAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken);
}

public class AccountingInvoiceCancellationService(
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
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

    public async Task CancelRecurringBookingFutureBillingAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken) =>
        await CancelAsync(
            ResolveOrganizationId(recurringBooking),
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            "Recurring booking future billing was cancelled.",
            "Recurring booking future billing was cancelled locally, but the live Xero repeating invoice template still requires cancellation.",
            recurringBooking.MarketplaceBooking?.InvoiceNumber,
            recurringBooking.MarketplaceBooking?.InvoiceUrl,
            cancellationToken,
            false);

    private async Task CancelAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        string cancelledMessage,
        string transitionRequiredMessage,
        string? localInvoiceNumber,
        string? localInvoiceUrl,
        CancellationToken cancellationToken,
        bool cancelConcreteInvoices = true)
    {
        var accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            localEntityType,
            localEntityId,
            cancellationToken);
        if (accountingInvoiceExportLink is null)
        {
            accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Skedular,
                localEntityType,
                localEntityId,
                cancellationToken);
            if (accountingInvoiceExportLink is null)
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

        var isRepeatingInvoice = IsLiveRepeatingInvoice(accountingInvoiceExportLink);
        var accountingInvoiceInstances = isRepeatingInvoice
            ? await repositoryFactory.AccountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceExportLink.Id,
                cancellationToken)
            : [];
        var accountingInvoiceInstance = isRepeatingInvoice
            ? accountingInvoiceInstances.FirstOrDefault()
            : await repositoryFactory.AccountingInvoiceInstanceRepository.GetLatestByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceExportLink.Id,
                cancellationToken);
        var effectiveExternalStatus = accountingInvoiceInstance?.ExternalStatus ?? accountingInvoiceExportLink.ExternalStatus;
        var repeatingTemplateExternalInvoiceId = accountingInvoiceExportLink.ExternalInvoiceId;
        var effectiveExternalInvoiceId = isRepeatingInvoice
            ? repeatingTemplateExternalInvoiceId
            : accountingInvoiceInstance?.ExternalInvoiceId ?? accountingInvoiceExportLink.ExternalInvoiceId;
        var effectiveInvoiceUrl = accountingInvoiceInstance?.ExternalInvoiceUrl ?? accountingInvoiceExportLink.ExternalInvoiceUrl;
        var effectiveInvoiceNumber = accountingInvoiceInstance?.ExternalInvoiceNumber ?? accountingInvoiceExportLink.ExternalInvoiceNumber;

        if (isRepeatingInvoice &&
            string.Equals(accountingInvoiceExportLink.ExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal) &&
            !accountingInvoiceInstances.Any(ShouldCancelConcreteInvoiceInstance))
        {
            accountingInvoiceExportLink.LastError = null;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceExportLink);
            return;
        }

        if (!isRepeatingInvoice &&
            (string.Equals(effectiveExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal) ||
             string.Equals(effectiveExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal)))
        {
            accountingInvoiceExportLink.LastError = null;

            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceExportLink);

            if (accountingInvoiceInstance is null)
            {
                return;
            }

            accountingInvoiceInstance.LastError = null;
            repositoryFactory.AccountingInvoiceInstanceRepository.Update(accountingInvoiceInstance);

            return;
        }

        if (string.IsNullOrWhiteSpace(effectiveExternalInvoiceId))
        {
            accountingInvoiceExportLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Cancelled;
            accountingInvoiceExportLink.ExportConfigurationMessage = cancelledMessage;
            accountingInvoiceExportLink.ExternalStatus = AccountingStatusConstants.Cancelled;
            accountingInvoiceExportLink.ExternalInvoiceNumber ??= effectiveInvoiceNumber;
            accountingInvoiceExportLink.ExternalInvoiceUrl ??= effectiveInvoiceUrl;
            accountingInvoiceExportLink.LastError = null;

            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceExportLink);

            if (accountingInvoiceInstance is null)
            {
                return;
            }

            accountingInvoiceInstance.ExternalStatus = AccountingStatusConstants.Cancelled;
            accountingInvoiceInstance.LastError = null;
            repositoryFactory.AccountingInvoiceInstanceRepository.Update(accountingInvoiceInstance);

            return;
        }

        try
        {
            var xeroConnection = await GetOrganizationXeroConnectionAsync(organizationId, cancellationToken);
            if (!IsXeroConnectionReady(xeroConnection) || !Guid.TryParse(effectiveExternalInvoiceId, out var externalInvoiceId))
            {
                MarkTransitionRequired(accountingInvoiceExportLink, transitionRequiredMessage, null);

                return;
            }

            var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection!, cancellationToken);
            var accountingApi = xeroSdkClientFactory.CreateAccountingApi();

            if (isRepeatingInvoice)
            {
                foreach (var concreteInvoiceInstance in accountingInvoiceInstances.Where(item =>
                             cancelConcreteInvoices && ShouldCancelConcreteInvoiceInstance(item)))
                {
                    if (!Guid.TryParse(concreteInvoiceInstance.ExternalInvoiceId, out var concreteInvoiceId))
                    {
                        continue;
                    }

                    await CancelLiveStandardInvoiceAsync(
                        accountingApi,
                        accessToken,
                        refreshedConnection.TenantId,
                        concreteInvoiceId,
                        BuildCancellationIdempotencyKey(concreteInvoiceInstance.Id, false),
                        cancellationToken);
                }

                await CancelLiveRepeatingInvoiceAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    externalInvoiceId,
                    BuildCancellationIdempotencyKey(accountingInvoiceExportLink.Id, true),
                    cancellationToken);
            }
            else
            {
                await CancelLiveStandardInvoiceAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    externalInvoiceId,
                    BuildCancellationIdempotencyKey(accountingInvoiceExportLink.Id, false),
                    cancellationToken);
            }

            accountingInvoiceExportLink.ExternalStatus = AccountingStatusConstants.Cancelled;
            accountingInvoiceExportLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Cancelled;
            accountingInvoiceExportLink.ExportConfigurationMessage = cancelledMessage;
            accountingInvoiceExportLink.LastSyncedAt = timeProvider.GetUtcNow();
            accountingInvoiceExportLink.LastError = null;

            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceExportLink);

            if (!isRepeatingInvoice && accountingInvoiceInstance is not null)
            {
                accountingInvoiceInstance.ExternalStatus = AccountingStatusConstants.Cancelled;
                accountingInvoiceInstance.LastSyncedAt = accountingInvoiceExportLink.LastSyncedAt;
                accountingInvoiceInstance.LastError = null;
                repositoryFactory.AccountingInvoiceInstanceRepository.Update(accountingInvoiceInstance);
            }

            if (isRepeatingInvoice && cancelConcreteInvoices)
            {
                foreach (var concreteInvoiceInstance in accountingInvoiceInstances)
                {
                    concreteInvoiceInstance.ExternalStatus = AccountingStatusConstants.Cancelled;
                    concreteInvoiceInstance.LastSyncedAt = accountingInvoiceExportLink.LastSyncedAt;
                    concreteInvoiceInstance.LastError = null;
                    repositoryFactory.AccountingInvoiceInstanceRepository.Update(concreteInvoiceInstance);
                }
            }
        }
        catch (Exception exception)
        {
            MarkTransitionRequired(accountingInvoiceExportLink, transitionRequiredMessage, exception.Message);

            if (!isRepeatingInvoice && accountingInvoiceInstance is not null)
            {
                accountingInvoiceInstance.LastError = exception.Message;
                repositoryFactory.AccountingInvoiceInstanceRepository.Update(accountingInvoiceInstance);
            }

            if (isRepeatingInvoice)
            {
                foreach (var concreteInvoiceInstance in accountingInvoiceInstances)
                {
                    concreteInvoiceInstance.LastError = exception.Message;
                    repositoryFactory.AccountingInvoiceInstanceRepository.Update(concreteInvoiceInstance);
                }
            }
        }
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationBillingServiceClient.Admin_GetXeroConnectionAsync(
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

    private static bool IsXeroConnectionReady(XeroConnection? xeroConnection) => xeroConnection is { IsActive: true, HasRefreshToken: true } &&
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
        var refreshedConnection = await organizationBillingServiceClient.Admin_RefreshXeroConnectionTokensAsync(
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

    private void MarkTransitionRequired(AccountingInvoiceExportLink accountingInvoiceLink, string message, string? lastError)
    {
        accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.TransitionRequired;
        accountingInvoiceLink.ExportConfigurationMessage = message;
        accountingInvoiceLink.LastError = lastError;
        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
    }

    private static bool IsLiveRepeatingInvoice(AccountingInvoiceExportLink accountingInvoiceLink) =>
        accountingInvoiceLink.ExternalInvoiceMode == AccountingInvoiceExportModeConstants.RepeatingInvoice &&
        !string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId);

    private static bool ShouldCancelConcreteInvoiceInstance(AccountingInvoiceInstance? accountingInvoiceInstance) =>
        accountingInvoiceInstance is not null &&
        !string.IsNullOrWhiteSpace(accountingInvoiceInstance.ExternalInvoiceId) &&
        !string.Equals(accountingInvoiceInstance.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal) &&
        !string.Equals(accountingInvoiceInstance.ExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal);

    private static string BuildCancellationIdempotencyKey(string accountingInvoiceExportLinkId, bool isRepeatingInvoice) =>
        isRepeatingInvoice
            ? $"{accountingInvoiceExportLinkId}:cancel-repeating"
            : $"{accountingInvoiceExportLinkId}:cancel-standard";
}
