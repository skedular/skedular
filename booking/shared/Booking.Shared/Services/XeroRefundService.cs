using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroOAuth2Token = Xero.NetStandard.OAuth2.Token.XeroOAuth2Token;
using CurrencyCode = Xero.NetStandard.OAuth2.Model.Accounting.CurrencyCode;
using Enum = System.Enum;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using XeroInvoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;

namespace Booking.Shared.Services;

public interface IXeroRefundService
{
    Task<XeroRefundProcessingAvailability> GetProcessingAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
}

public class XeroRefundService(
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IRepositoryFactory repositoryFactory,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    TimeProvider timeProvider) : IXeroRefundService
{
    public async Task<XeroRefundProcessingAvailability> GetProcessingAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        if (refund.Status != MarketplaceRefundStatusConstants.PendingAccounting)
        {
            return new XeroRefundProcessingAvailability(false, "Refund must be pending accounting before Xero processing is available.");
        }

        if (!refund.RefundAmount.HasValue || refund.RefundAmount <= 0)
        {
            return new XeroRefundProcessingAvailability(false, "Refund amount must be greater than zero before Xero processing is available.");
        }

        var resolution = await ResolveInvoiceTargetAsync(refund, cancellationToken);
        return resolution.InvoiceTarget is null
            ? new XeroRefundProcessingAvailability(false, resolution.ErrorMessage)
            : new XeroRefundProcessingAvailability(true, null);
    }

    public async Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        try
        {
            if (refund.Status != MarketplaceRefundStatusConstants.PendingAccounting)
            {
                throw new InvalidOperationException("Refund must be pending accounting before Xero processing.");
            }

            var resolution = await ResolveInvoiceTargetAsync(refund, cancellationToken);
            if (resolution.InvoiceTarget is null)
            {
                return MarkFailed(refund, resolution.ErrorMessage ?? "The original Xero invoice could not be resolved for this refund.");
            }

            var invoiceTarget = resolution.InvoiceTarget;

            if (!Guid.TryParse(invoiceTarget.ExternalInvoiceId, out var invoiceId))
            {
                return MarkFailed(refund, "The original Xero invoice id is invalid.");
            }

            if (!refund.RefundAmount.HasValue || refund.RefundAmount <= 0)
            {
                return MarkFailed(refund, "Refund amount must be greater than zero before Xero processing.");
            }

            var xeroConnection = await GetOrganizationXeroConnectionAsync(refund.OrganizationId, cancellationToken);
            if (!IsXeroConnectionReady(xeroConnection))
            {
                return MarkFailed(refund, xeroConnection?.LastError ?? "Xero connection is not active.");
            }

            var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(refund.OrganizationId, xeroConnection!, cancellationToken);
            var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
            var invoiceResponse = await GetInvoiceAsync(accountingApi, accessToken, refreshedConnection.TenantId, invoiceId, cancellationToken);
            var originalInvoice = invoiceResponse._Invoices?.FirstOrDefault();
            if (originalInvoice?.Contact?.ContactID is null)
            {
                return MarkFailed(refund, "The original Xero invoice could not be loaded with a contact.");
            }

            var accountCode = ResolveRefundAccountCode(originalInvoice, refreshedConnection);
            if (string.IsNullOrWhiteSpace(accountCode))
            {
                return MarkFailed(
                    refund,
                    "The original Xero invoice does not provide an account code and no default sales account code is configured.");
            }

            var taxType = ResolveRefundTaxType(originalInvoice);
            if (string.IsNullOrWhiteSpace(taxType))
            {
                return MarkFailed(refund, "The original Xero invoice does not provide a tax type for refund credit-note creation.");
            }

            var creditNotes = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        Type = CreditNote.TypeEnum.ACCRECCREDIT,
                        Status = CreditNote.StatusEnum.AUTHORISED,
                        Contact = new Contact { ContactID = originalInvoice.Contact.ContactID },
                        Date = refund.RequestedAt.UtcDateTime.Date,
                        Reference = BuildRefundReference(invoiceTarget),
                        CurrencyCode = ResolveCurrencyCode(refund, originalInvoice),
                        LineAmountTypes = originalInvoice.LineAmountTypes,
                        LineItems =
                        [
                            new LineItem
                            {
                                Description = BuildRefundDescription(invoiceTarget, refund),
                                Quantity = 1,
                                UnitAmount = refund.RefundAmount.Value,
                                LineAmount = refund.RefundAmount.Value,
                                AccountCode = accountCode,
                                TaxType = taxType
                            }
                        ]
                    }
                ]
            };

            var creditNoteResponse = await CreateCreditNotesAsync(
                accountingApi,
                accessToken,
                refreshedConnection.TenantId,
                creditNotes,
                BuildIdempotencyKey(refund.Id),
                cancellationToken);
            var creditNote = creditNoteResponse._CreditNotes?.FirstOrDefault();
            if (creditNote?.CreditNoteID is null)
            {
                return MarkFailed(refund, "Xero credit note creation returned no credit note id.");
            }

            var outstandingAmount = originalInvoice.AmountDue ?? 0m;
            if (outstandingAmount >= refund.RefundAmount.Value)
            {
                await CreateCreditNoteAllocationAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    creditNote.CreditNoteID.Value,
                    new Allocations
                    {
                        _Allocations =
                        [
                            new Allocation
                            {
                                Invoice = new XeroInvoice { InvoiceID = invoiceId },
                                Amount = refund.RefundAmount.Value,
                                Date = refund.RequestedAt.UtcDateTime.Date
                            }
                        ]
                    },
                    BuildAllocationIdempotencyKey(refund.Id),
                    cancellationToken);
            }
            else if (outstandingAmount <= 0m)
            {
                var bankAccountCode = ResolveRefundBankAccountCode(originalInvoice);
                if (string.IsNullOrWhiteSpace(bankAccountCode))
                {
                    return MarkFailed(
                        refund,
                        "The original Xero invoice does not expose a bank account code on its payment history, so the refund credit note cannot be settled automatically.");
                }

                await CreatePaymentAsync(
                    accountingApi,
                    accessToken,
                    refreshedConnection.TenantId,
                    new Payment
                    {
                        CreditNote = new CreditNote { CreditNoteID = creditNote.CreditNoteID },
                        Account = new Account { Code = bankAccountCode },
                        Code = bankAccountCode,
                        Amount = refund.RefundAmount.Value,
                        Date = refund.RequestedAt.UtcDateTime.Date,
                        Reference = BuildRefundReference(invoiceTarget)
                    },
                    BuildPaymentIdempotencyKey(refund.Id),
                    cancellationToken);
            }
            else
            {
                return MarkFailed(
                    refund,
                    "The original Xero invoice has only a partial outstanding balance. Automatic refund settlement is not supported for partially paid invoices yet.");
            }

            refund.Status = MarketplaceRefundStatusConstants.Completed;
            refund.AccountingProvider = AccountingProviderConstants.Xero;
            refund.ExternalRefundId = creditNote.CreditNoteID.Value.ToString();
            refund.ExternalRefundNumber = creditNote.CreditNoteNumber;
            refund.LastProcessedAt = timeProvider.GetUtcNow();
            refund.LastError = null;

            return repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }
        catch (Exception exception)
        {
            return MarkFailed(refund, exception.Message);
        }
    }

    protected virtual Task<Invoices> GetInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid invoiceId,
        CancellationToken cancellationToken) =>
        accountingApi.GetInvoiceAsync(accessToken, tenantId, invoiceId, null, cancellationToken);

    protected virtual Task<CreditNotes> CreateCreditNotesAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        CreditNotes creditNotes,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        accountingApi.CreateCreditNotesAsync(accessToken, tenantId, creditNotes, null, null, idempotencyKey, cancellationToken);

    protected virtual Task<Allocations> CreateCreditNoteAllocationAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid creditNoteId,
        Allocations allocations,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        accountingApi.CreateCreditNoteAllocationAsync(accessToken, tenantId, creditNoteId, allocations, null, idempotencyKey, cancellationToken);

    protected virtual Task<Payments> CreatePaymentAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Payment payment,
        string idempotencyKey,
        CancellationToken cancellationToken) =>
        accountingApi.CreatePaymentAsync(accessToken, tenantId, payment, idempotencyKey, cancellationToken);

    private async Task<XeroRefundInvoiceTargetResolution> ResolveInvoiceTargetAsync(MarketplaceRefund refund, CancellationToken cancellationToken) =>
        refund.LocalEntityType switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking => await ResolveOneTimeBookingInvoiceTargetAsync(refund, cancellationToken),
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription => await ResolveSubscriptionInvoiceTargetAsync(refund,
                cancellationToken),
            _ => new XeroRefundInvoiceTargetResolution(
                null,
                "Xero refund processing currently supports only marketplace bookings and subscription billing windows.")
        };

    private async Task<XeroRefundInvoiceTargetResolution> ResolveOneTimeBookingInvoiceTargetAsync(MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.MarketplaceBooking,
            refund.LocalEntityId,
            cancellationToken);
        if (accountingInvoiceExportLink is null || string.IsNullOrWhiteSpace(accountingInvoiceExportLink.ExternalInvoiceId))
        {
            return new XeroRefundInvoiceTargetResolution(null, "The original Xero invoice link could not be found for this refund.");
        }

        return new XeroRefundInvoiceTargetResolution(
            new XeroRefundInvoiceTarget(
                accountingInvoiceExportLink.LocalEntityId,
                accountingInvoiceExportLink.ExternalInvoiceId,
                accountingInvoiceExportLink.ExternalInvoiceNumber),
            null);
    }

    private async Task<XeroRefundInvoiceTargetResolution> ResolveSubscriptionInvoiceTargetAsync(MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken);
        if (subscription is null)
        {
            return new XeroRefundInvoiceTargetResolution(null, "The subscription refund could not be matched to a subscription.");
        }

        var recurringBooking = ResolveCurrentBillingWindowRecurringBooking(subscription, refund.RequestedAt);
        if (recurringBooking?.MarketplaceBooking is null)
        {
            return new XeroRefundInvoiceTargetResolution(null,
                "The current subscription billing window could not be matched to a billed recurring booking.");
        }

        var accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            cancellationToken);
        if (accountingInvoiceExportLink is null || string.IsNullOrWhiteSpace(accountingInvoiceExportLink.ExternalInvoiceId))
        {
            return new XeroRefundInvoiceTargetResolution(null, "The subscription recurring booking is not linked to a Xero invoice export.");
        }

        if (accountingInvoiceExportLink.ExternalInvoiceMode == AccountingInvoiceExportModeConstants.RepeatingInvoice)
        {
            var accountingInvoiceInstances = await repositoryFactory.AccountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceExportLink.Id,
                cancellationToken);
            var matchingInvoiceInstance = ResolveMatchingInvoiceInstance(recurringBooking, accountingInvoiceInstances);
            if (matchingInvoiceInstance is null || string.IsNullOrWhiteSpace(matchingInvoiceInstance.ExternalInvoiceId))
            {
                return new XeroRefundInvoiceTargetResolution(
                    null,
                    "The current subscription billing window has not been correlated to a concrete Xero invoice instance yet.");
            }

            return new XeroRefundInvoiceTargetResolution(
                new XeroRefundInvoiceTarget(
                    recurringBooking.Id,
                    matchingInvoiceInstance.ExternalInvoiceId,
                    matchingInvoiceInstance.ExternalInvoiceNumber ?? accountingInvoiceExportLink.ExternalInvoiceNumber),
                null);
        }

        var latestInvoiceInstance = await repositoryFactory.AccountingInvoiceInstanceRepository.GetLatestByAccountingInvoiceExportLinkIdAsync(
            accountingInvoiceExportLink.Id,
            cancellationToken);

        return new XeroRefundInvoiceTargetResolution(
            new XeroRefundInvoiceTarget(
                recurringBooking.Id,
                latestInvoiceInstance?.ExternalInvoiceId ?? accountingInvoiceExportLink.ExternalInvoiceId,
                latestInvoiceInstance?.ExternalInvoiceNumber ?? accountingInvoiceExportLink.ExternalInvoiceNumber),
            null);
    }

    private MarketplaceRefund MarkFailed(MarketplaceRefund refund, string message)
    {
        refund.Status = MarketplaceRefundStatusConstants.Failed;
        refund.LastProcessedAt = timeProvider.GetUtcNow();
        refund.LastError = message;
        return repositoryFactory.MarketplaceRefundRepository.Update(refund);
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationBillingServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput { OrganizationId = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return response is { Id: not null and not "" } ? response : null;
    }

    private static bool IsXeroConnectionReady(XeroConnection? xeroConnection) =>
        xeroConnection is { IsActive: true, TenantId: not null and not "", AccessTokenEncrypted: not null and not "" };

    private async Task<(string AccessToken, XeroConnection Connection)> EnsureValidAccessTokenAsync(
        string organizationId,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        var expiresAt = xeroConnection.AccessTokenExpiresAt?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;
        if (expiresAt > timeProvider.GetUtcNow().AddMinutes(1))
        {
            return (xeroTokenEncryptionService.Decrypt(xeroConnection.AccessTokenEncrypted), xeroConnection);
        }

        if (string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted))
        {
            throw new InvalidOperationException("Missing Xero refresh token.");
        }

        var refreshedToken = (XeroOAuth2Token)await xeroSdkClientFactory.CreateClient().RefreshAccessTokenAsync(
            new XeroOAuth2Token { RefreshToken = xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted) });

        var now = timeProvider.GetUtcNow();
        var refreshedConnection = await organizationBillingServiceClient.Admin_RefreshXeroConnectionTokensAsync(
            new Admin_RefreshXeroConnectionTokensInput
            {
                OrganizationId = organizationId,
                AccessTokenEncrypted = xeroTokenEncryptionService.Encrypt(refreshedToken.AccessToken),
                RefreshTokenEncrypted = xeroTokenEncryptionService.Encrypt(
                    string.IsNullOrWhiteSpace(refreshedToken.RefreshToken)
                        ? xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted)
                        : refreshedToken.RefreshToken),
                AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(now.AddMinutes(30)),
                RefreshTokenExpiresAt = Timestamp.FromDateTimeOffset(now.AddDays(60))
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (refreshedToken.AccessToken, refreshedConnection);
    }

    private static string BuildIdempotencyKey(string refundId) => $"refund-credit-note-{refundId}";

    private static string BuildAllocationIdempotencyKey(string refundId) => $"refund-credit-note-allocation-{refundId}";

    private static string BuildPaymentIdempotencyKey(string refundId) => $"refund-credit-note-payment-{refundId}";

    private static string BuildRefundReference(XeroRefundInvoiceTarget invoiceTarget) =>
        string.IsNullOrWhiteSpace(invoiceTarget.ExternalInvoiceNumber)
            ? $"Refund for {invoiceTarget.LocalEntityId}"
            : $"Refund for invoice {invoiceTarget.ExternalInvoiceNumber}";

    private static string BuildRefundDescription(XeroRefundInvoiceTarget invoiceTarget, MarketplaceRefund refund) =>
        string.IsNullOrWhiteSpace(invoiceTarget.ExternalInvoiceNumber)
            ? $"{ResolveRefundSubjectLabel(refund)} refund {invoiceTarget.LocalEntityId}"
            : $"{ResolveRefundSubjectLabel(refund)} refund for invoice {invoiceTarget.ExternalInvoiceNumber}";

    private static string ResolveRefundSubjectLabel(MarketplaceRefund refund) =>
        refund.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription
            ? "Marketplace subscription"
            : "Marketplace booking";

    private static CurrencyCode ResolveCurrencyCode(MarketplaceRefund refund, XeroInvoice originalInvoice)
    {
        if (!string.IsNullOrWhiteSpace(refund.Currency) &&
            Enum.TryParse<CurrencyCode>(refund.Currency, true, out var refundCurrency))
        {
            return refundCurrency;
        }

        return originalInvoice.CurrencyCode;
    }

    private static string? ResolveRefundAccountCode(XeroInvoice originalInvoice, XeroConnection xeroConnection) =>
        originalInvoice.LineItems?
            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.AccountCode))?
            .AccountCode ??
        xeroConnection.DefaultSalesAccountCode;

    private static string? ResolveRefundTaxType(XeroInvoice originalInvoice) =>
        originalInvoice.LineItems?
            .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.TaxType))?
            .TaxType;

    private static string? ResolveRefundBankAccountCode(XeroInvoice originalInvoice) =>
        originalInvoice.Payments?
            .FirstOrDefault(payment =>
                !string.IsNullOrWhiteSpace(payment.Account?.Code) || !string.IsNullOrWhiteSpace(payment.Code))?
            .Account?.Code
        ?? originalInvoice.Payments?
            .FirstOrDefault(payment =>
                !string.IsNullOrWhiteSpace(payment.Account?.Code) || !string.IsNullOrWhiteSpace(payment.Code))?
            .Code;

    private static RecurringBookingEntity? ResolveCurrentBillingWindowRecurringBooking(
        MarketplaceBookingSubscriptionEntity subscription,
        DateTimeOffset now)
    {
        var recurringBookingsInWindow = subscription.RecurringBookings
            .Where(item => !item.DeletedAt.HasValue && item.MarketplaceBooking is not null)
            .Where(item => IntersectsBillingWindow(subscription, item, now))
            .OrderBy(item => item.StartDate)
            .ToList();

        return recurringBookingsInWindow.LastOrDefault();
    }

    private static bool IntersectsBillingWindow(
        MarketplaceBookingSubscriptionEntity subscription,
        RecurringBookingEntity recurringBooking,
        DateTimeOffset now)
    {
        var (windowStartInclusive, windowEndExclusive) = ResolveCurrentBillingWindow(
            subscription.StartedAt,
            now,
            subscription.MarketplaceBooking.ProductVersion.Product.Organization.BillingCycle);
        var recurringBookingEndExclusive = recurringBooking.EndDate?.AddDays(1) ??
                                           ResolveRecurringBookingCycleEndExclusive(recurringBooking);

        return recurringBooking.StartDate < windowEndExclusive && recurringBookingEndExclusive > windowStartInclusive;
    }

    private static (DateTimeOffset StartInclusive, DateTimeOffset EndExclusive) ResolveCurrentBillingWindow(
        DateTimeOffset startedAt,
        DateTimeOffset now,
        string organizationBillingCycle)
    {
        var startInclusive = startedAt;
        var endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);

        while (now >= endExclusive)
        {
            startInclusive = endExclusive;
            endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);
        }

        return (startInclusive, endExclusive);
    }

    private static DateTimeOffset AdvanceBillingWindow(DateTimeOffset startInclusive, string organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycleConstants.Weekly => startInclusive.AddDays(7),
            OrganizationBillingCycleConstants.Fortnightly => startInclusive.AddDays(14),
            OrganizationBillingCycleConstants.Monthly => startInclusive.AddMonths(1),
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle))
        };

    private static DateTimeOffset ResolveRecurringBookingCycleEndExclusive(RecurringBookingEntity recurringBooking)
    {
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);

        return recurringBooking.MarketplaceBooking.ProductPricing.PurchaseCadence switch
        {
            ProductPricingCadence.Weekly => recurringBooking.StartDate.AddDays(7),
            ProductPricingCadence.Fortnightly => recurringBooking.StartDate.AddDays(14),
            ProductPricingCadence.Monthly => recurringBooking.StartDate.AddMonths(1),
            ProductPricingCadence.TwoMonths => recurringBooking.StartDate.AddMonths(2),
            ProductPricingCadence.Quarterly => recurringBooking.StartDate.AddMonths(3),
            ProductPricingCadence.FourMonths => recurringBooking.StartDate.AddMonths(4),
            ProductPricingCadence.FiveMonths => recurringBooking.StartDate.AddMonths(5),
            ProductPricingCadence.SixMonths => recurringBooking.StartDate.AddMonths(6),
            ProductPricingCadence.Yearly => recurringBooking.StartDate.AddYears(1),
            _ => recurringBooking.StartDate.AddDays(1)
        };
    }

    private static AccountingInvoiceInstance? ResolveMatchingInvoiceInstance(
        RecurringBookingEntity recurringBooking,
        IReadOnlyList<AccountingInvoiceInstance> accountingInvoiceInstances)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        if (marketplaceBooking is null || accountingInvoiceInstances.Count == 0)
        {
            return null;
        }

        var matchingByNumber = !string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceNumber)
            ? accountingInvoiceInstances.FirstOrDefault(item =>
                string.Equals(item.ExternalInvoiceNumber, marketplaceBooking.InvoiceNumber, StringComparison.OrdinalIgnoreCase))
            : null;
        if (matchingByNumber is not null)
        {
            return matchingByNumber;
        }

        var matchingByUrl = !string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceUrl)
            ? accountingInvoiceInstances.FirstOrDefault(item =>
                string.Equals(item.ExternalInvoiceUrl, marketplaceBooking.InvoiceUrl, StringComparison.OrdinalIgnoreCase))
            : null;
        if (matchingByUrl is not null)
        {
            return matchingByUrl;
        }

        return accountingInvoiceInstances
                   .Where(item =>
                       !string.Equals(item.ExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal) &&
                       !string.Equals(item.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal))
                   .OrderByDescending(item => item.CreatedAt)
                   .FirstOrDefault() ??
               accountingInvoiceInstances.OrderByDescending(item => item.CreatedAt).FirstOrDefault();
    }

    private sealed record XeroRefundInvoiceTarget(
        string LocalEntityId,
        string ExternalInvoiceId,
        string? ExternalInvoiceNumber);

    private sealed record XeroRefundInvoiceTargetResolution(
        XeroRefundInvoiceTarget? InvoiceTarget,
        string? ErrorMessage);
}
