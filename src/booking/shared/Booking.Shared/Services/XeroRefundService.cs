using System.Diagnostics;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Grpc;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Client;
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
    Task<bool> HasInvoiceTargetAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<XeroRefundProcessingAvailability> GetProcessingAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<MarketplaceRefund> ProcessAsync(MarketplaceRefund refund, CancellationToken cancellationToken);
    Task<bool> ReconcileAsync(MarketplaceRefund refund, DateTimeOffset since, CancellationToken cancellationToken);
}

public class XeroRefundService(
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IRepositoryFactory repositoryFactory,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    TimeProvider timeProvider,
    ILogger<XeroRefundService> logger) : IXeroRefundService
{
    public async Task<bool> HasInvoiceTargetAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        var resolution = await ResolveInvoiceTargetAsync(refund, cancellationToken);
        logger.LogInformation(
            "Resolved Xero invoice target for refund {RefundId}: found={HasInvoiceTarget}; reason={Reason}",
            refund.Id,
            resolution.InvoiceTarget is not null,
            resolution.ErrorMessage);
        return resolution.InvoiceTarget is not null;
    }

    public async Task<bool> ReconcileAsync(MarketplaceRefund refund, DateTimeOffset since, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(refund.ExternalRefundId, out var creditNoteId))
        {
            refund.ReconciliationStatus = "LookupFailed";
            refund.LastReconciledAt = timeProvider.GetUtcNow();
            repositoryFactory.MarketplaceRefundRepository.Update(refund);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return false;
        }

        var connection = await GetOrganizationXeroConnectionAsync(refund.OrganizationId, cancellationToken);
        if (!IsXeroConnectionReady(connection))
        {
            refund.ReconciliationStatus = "LookupFailed";
            refund.LastReconciledAt = timeProvider.GetUtcNow();
            repositoryFactory.MarketplaceRefundRepository.Update(refund);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return false;
        }

        var (accessToken, refreshed) = await EnsureValidAccessTokenAsync(refund.OrganizationId, connection!, cancellationToken);
        var api = xeroSdkClientFactory.CreateAccountingApi();
        var notes = await GetCreditNotesAsync(api, accessToken, refreshed.TenantId, since.UtcDateTime, creditNoteId, cancellationToken);
        var creditNote = notes._CreditNotes?.FirstOrDefault(note => note.CreditNoteID == creditNoteId);
        if (creditNote is null)
        {
            creditNote = await GetCreditNoteByIdAsync(api, accessToken, refreshed.TenantId, creditNoteId, cancellationToken);
        }

        var matched = creditNote is not null && await IsCreditNoteSettledAsync(
            api, accessToken, refreshed.TenantId, creditNote, refund.RefundAmount ?? 0m, since, cancellationToken);
        refund.ReconciliationStatus = creditNote is null ? "NotFound" : matched ? "Matched" : "Unsettled";
        refund.LastReconciledAt = timeProvider.GetUtcNow();
        repositoryFactory.MarketplaceRefundRepository.Update(refund);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return matched;
    }

    public async Task<XeroRefundProcessingAvailability> GetProcessingAvailabilityAsync(MarketplaceRefund refund, CancellationToken cancellationToken)
    {
        if (refund.Status is not (MarketplaceRefundStatusConstants.Processing or MarketplaceRefundStatusConstants.Completed))
        {
            return new XeroRefundProcessingAvailability(false,
                "Refund must be processing or completed in Stripe before Xero processing is available.");
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
        var stopwatch = Stopwatch.StartNew();
        logger.LogInformation(
            "Starting Xero refund processing for refund {RefundId}, amount {RefundAmount}, currency {Currency}, retry count {RetryCount}", refund.Id,
            refund.RefundAmount, refund.Currency, refund.RetryCount);
        try
        {
            if (refund.Status is not (MarketplaceRefundStatusConstants.Processing or MarketplaceRefundStatusConstants.Completed))
            {
                throw new InvalidOperationException("Refund must be processing or completed in Stripe before Xero processing.");
            }

            var resolution = await ResolveInvoiceTargetAsync(refund, cancellationToken);
            if (resolution.InvoiceTarget is null)
            {
                logger.LogWarning("Xero refund processing blocked for refund {RefundId}: {Reason}", refund.Id, resolution.ErrorMessage);
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
                        Contact = new Contact
                        {
                            ContactID = originalInvoice.Contact.ContactID,
                        },
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
                                TaxType = taxType,
                            },
                        ],
                    },
                ],
            };

            var creditNoteResponse = await CreateCreditNotesAsync(
                accountingApi,
                accessToken,
                refreshedConnection.TenantId,
                creditNotes,
                BuildIdempotencyKey(GetIdempotencyKey(refund)),
                cancellationToken);
            var creditNote = creditNoteResponse._CreditNotes?.FirstOrDefault();
            if (creditNote?.CreditNoteID is null)
            {
                return MarkFailed(refund, "Xero credit note creation returned no credit note id.");
            }

            // Persist the provider reference before allocation or cash-settlement work.
            // If a later Xero call fails, reconciliation can now find the created credit note
            // and retry/resolve it without creating a second credit note.
            refund.AccountingProvider = AccountingProviderConstants.Xero;
            refund.ExternalRefundId = creditNote.CreditNoteID.Value.ToString();
            refund.ExternalRefundNumber = creditNote.CreditNoteNumber;
            refund.LastProcessedAt = timeProvider.GetUtcNow();
            refund.LastError = null;
            refund = repositoryFactory.MarketplaceRefundRepository.Update(refund);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            var refundAmount = refund.RefundAmount ?? 0m;
            var outstandingAmount = originalInvoice.AmountDue ?? 0m;
            if (outstandingAmount >= refundAmount)
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
                                Invoice = new XeroInvoice
                                {
                                    InvoiceID = invoiceId,
                                },
                                Amount = refundAmount,
                                Date = refund.RequestedAt.UtcDateTime.Date,
                            },
                        ],
                    },
                    BuildAllocationIdempotencyKey(GetIdempotencyKey(refund)),
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
                        CreditNote = new CreditNote
                        {
                            CreditNoteID = creditNote.CreditNoteID,
                        },
                        Account = new Account
                        {
                            Code = bankAccountCode,
                        },
                        Code = bankAccountCode,
                        Amount = refundAmount,
                        Date = refund.RequestedAt.UtcDateTime.Date,
                        Reference = BuildRefundReference(invoiceTarget),
                    },
                    BuildPaymentIdempotencyKey(GetIdempotencyKey(refund)),
                    cancellationToken);
            }
            else
            {
                return MarkFailed(
                    refund,
                    "The original Xero invoice has only a partial outstanding balance. Automatic refund settlement is not supported for partially paid invoices yet.");
            }

            MarketplaceRefundStateMachine.EnsureAllowed(refund.Status, MarketplaceRefundStatusConstants.Completed);
            refund.Status = MarketplaceRefundStatusConstants.Completed;
            refund.LastProcessedAt = timeProvider.GetUtcNow();
            refund.LastError = null;

            logger.LogInformation(
                "Completed Xero refund processing for refund {RefundId} with status {Status}, external refund {ExternalRefundId}, duration {DurationMs} ms, retry count {RetryCount}",
                refund.Id, refund.Status, refund.ExternalRefundId, stopwatch.ElapsedMilliseconds, refund.RetryCount);
            return repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Xero refund processing failed for refund {RefundId}", refund.Id);
            return MarkFailed(refund, ToUserFacingError(exception));
        }
    }

    private static string ToUserFacingError(Exception exception)
    {
        var message = exception.Message;
        if (message.Contains("CreateCreditNoteAllocation", StringComparison.OrdinalIgnoreCase) &&
            message.Contains("Only AUTHORISED invoices can have allocations applied to them", StringComparison.OrdinalIgnoreCase))
        {
            return
                "Xero created the credit note, but could not apply it to the invoice because the invoice is not authorized. Manual accounting follow-up is required.";
        }

        if (message.StartsWith("Xero API", StringComparison.OrdinalIgnoreCase))
        {
            return "Xero could not complete the accounting step for this refund. Manual accounting follow-up is required.";
        }

        return message;
    }

    protected virtual Task<CreditNotes> GetCreditNotesAsync(AccountingApi api, string accessToken, string tenantId, DateTime modifiedSince,
        Guid creditNoteId, CancellationToken cancellationToken) =>
        api.GetCreditNotesAsync(accessToken, tenantId, modifiedSince, $"CreditNoteID==Guid(\"{creditNoteId}\")", null, null, null, null,
            cancellationToken);

    protected virtual async Task<CreditNote?> GetCreditNoteByIdAsync(
        AccountingApi api,
        string accessToken,
        string tenantId,
        Guid creditNoteId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await api.GetCreditNoteAsync(accessToken, tenantId, creditNoteId, null, cancellationToken);
            return response._CreditNotes?.FirstOrDefault(note => note.CreditNoteID == creditNoteId);
        }
        catch (ApiException exception) when (exception.ErrorCode == 404)
        {
            return null;
        }
    }

    protected virtual Task<Payments> GetPaymentsAsync(AccountingApi api, string accessToken, string tenantId, DateTime? modifiedSince,
        Guid creditNoteId, CancellationToken cancellationToken) =>
        api.GetPaymentsAsync(accessToken, tenantId, modifiedSince, $"CreditNote.CreditNoteID==Guid(\"{creditNoteId}\")", null, null, null,
            cancellationToken);

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
            MarketplaceRefundEntityTypeConstants.EntitlementPurchase => await ResolveEntitlementPurchaseInvoiceTargetAsync(refund,
                cancellationToken),
            _ => new XeroRefundInvoiceTargetResolution(
                null,
                "Xero refund processing does not support this marketplace purchase type."),
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

    private async Task<XeroRefundInvoiceTargetResolution> ResolveEntitlementPurchaseInvoiceTargetAsync(MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.EntitlementPurchase,
            refund.LocalEntityId,
            cancellationToken);
        if (accountingInvoiceExportLink is null || string.IsNullOrWhiteSpace(accountingInvoiceExportLink.ExternalInvoiceId))
        {
            return new XeroRefundInvoiceTargetResolution(null, "The original Xero entitlement invoice link could not be found for this refund.");
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
            // Subscription loading intentionally filters old recurring windows. A canceled
            // subscription can still need a refund for the invoice that was just paid, so
            // recover the persisted recurring bookings before declaring the refund unresolved.
            var persistedRecurringBookings = await repositoryFactory.RecurringBookingRepository
                .GetByMarketplaceBookingSubscriptionIdAsync(refund.LocalEntityId, cancellationToken);
            recurringBooking = persistedRecurringBookings.FirstOrDefault(item =>
                item.MarketplaceBooking is not null);
            if (recurringBooking is null)
            {
                return new XeroRefundInvoiceTargetResolution(null,
                    "The current subscription billing window could not be matched to a billed recurring booking.");
            }

            logger.LogInformation(
                "Recovered persisted recurring booking {RecurringBookingId} for subscription refund {RefundId}",
                recurringBooking.Id,
                refund.Id);
        }

        var accountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            cancellationToken);
        if (accountingInvoiceExportLink is null || string.IsNullOrWhiteSpace(accountingInvoiceExportLink.ExternalInvoiceId))
        {
            var persistedRecurringBookings = await repositoryFactory.RecurringBookingRepository
                .GetByMarketplaceBookingSubscriptionIdAsync(refund.LocalEntityId, cancellationToken);
            foreach (var candidate in persistedRecurringBookings.Where(item =>
                         item.MarketplaceBooking is not null &&
                         item.Id != recurringBooking.Id &&
                         IntersectsBillingWindow(subscription, item, refund.RequestedAt)))
            {
                var candidateLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                    AccountingProviderConstants.Xero,
                    AccountingEntityTypeConstants.RecurringBooking,
                    candidate.Id,
                    cancellationToken);
                if (candidateLink is null || string.IsNullOrWhiteSpace(candidateLink.ExternalInvoiceId))
                {
                    continue;
                }

                recurringBooking = candidate;
                accountingInvoiceExportLink = candidateLink;
                logger.LogInformation(
                    "Recovered Xero invoice link {AccountingInvoiceExportLinkId} for subscription refund {RefundId} through recurring booking {RecurringBookingId}",
                    candidateLink.Id,
                    refund.Id,
                    candidate.Id);
                break;
            }

            if (accountingInvoiceExportLink is null || string.IsNullOrWhiteSpace(accountingInvoiceExportLink.ExternalInvoiceId))
            {
                return new XeroRefundInvoiceTargetResolution(null, "The subscription recurring booking is not linked to a Xero invoice export.");
            }
        }

        if (accountingInvoiceExportLink.ExternalInvoiceMode == AccountingInvoiceExportModeConstants.RepeatingInvoice)
        {
            var accountingInvoiceInstances = await repositoryFactory.AccountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceExportLink.Id,
                cancellationToken);
            var matchingInvoiceInstance = ResolveMatchingInvoiceInstance(recurringBooking, accountingInvoiceInstances, refund.RequestedAt);
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

        var nonRepeatingInvoiceInstances = await repositoryFactory.AccountingInvoiceInstanceRepository
            .GetByAccountingInvoiceExportLinkIdAsync(accountingInvoiceExportLink.Id, cancellationToken);
        var nonRepeatingInvoiceInstance = ResolveMatchingInvoiceInstance(recurringBooking, nonRepeatingInvoiceInstances, refund.RequestedAt);

        return new XeroRefundInvoiceTargetResolution(
            new XeroRefundInvoiceTarget(
                recurringBooking.Id,
                nonRepeatingInvoiceInstance?.ExternalInvoiceId ?? accountingInvoiceExportLink.ExternalInvoiceId,
                nonRepeatingInvoiceInstance?.ExternalInvoiceNumber ?? accountingInvoiceExportLink.ExternalInvoiceNumber),
            null);
    }

    private async Task<bool> IsCreditNoteSettledAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        CreditNote creditNote,
        decimal refundAmount,
        DateTimeOffset since,
        CancellationToken cancellationToken)
    {
        var allocatedAmount = creditNote.Allocations?.Sum(item => item.Amount ?? 0m) ?? 0m;
        if (allocatedAmount >= refundAmount)
        {
            return true;
        }

        var payments = await GetPaymentsAsync(
            accountingApi, accessToken, tenantId, since.UtcDateTime, creditNote.CreditNoteID!.Value, cancellationToken);
        var paidAmount = GetPaidAmount(payments, creditNote.CreditNoteID.Value);
        if (paidAmount < refundAmount)
        {
            // A payment created before the reconciliation window is omitted by
            // the modified-since query. Repeat the filtered lookup without a
            // timestamp so unchanged historical payments are still found.
            var historicalPayments = await GetPaymentsAsync(
                accountingApi, accessToken, tenantId, null, creditNote.CreditNoteID.Value, cancellationToken);
            paidAmount = Math.Max(paidAmount, GetPaidAmount(historicalPayments, creditNote.CreditNoteID.Value));
        }

        return paidAmount >= refundAmount;
    }

    private static decimal GetPaidAmount(Payments payments, Guid creditNoteId) =>
        payments._Payments?
            .Where(payment => payment.CreditNote?.CreditNoteID == creditNoteId)
            .Sum(payment => payment.Amount ?? 0m) ?? 0m;

    private MarketplaceRefund MarkFailed(MarketplaceRefund refund, string message)
    {
        if (refund.Status == MarketplaceRefundStatusConstants.Completed)
        {
            refund.LastProcessedAt = timeProvider.GetUtcNow();
            refund.LastError = message;
            refund.ReconciliationStatus = "AccountingProjectionRequired";
            return repositoryFactory.MarketplaceRefundRepository.Update(refund);
        }

        MarketplaceRefundStateMachine.EnsureAllowed(refund.Status, MarketplaceRefundStatusConstants.Failed);
        refund.Status = MarketplaceRefundStatusConstants.Failed;
        refund.LastProcessedAt = timeProvider.GetUtcNow();
        refund.LastError = message;
        return repositoryFactory.MarketplaceRefundRepository.Update(refund);
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationBillingServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput
            {
                OrganizationId = organizationId,
            },
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
            new XeroOAuth2Token
            {
                RefreshToken = xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted),
            });

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
                RefreshTokenExpiresAt = Timestamp.FromDateTimeOffset(now.AddDays(60)),
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return (refreshedToken.AccessToken, refreshedConnection);
    }

    private static string BuildIdempotencyKey(string refundId) => $"refund-credit-note-{refundId}";

    private static string GetIdempotencyKey(MarketplaceRefund refund) =>
        string.IsNullOrWhiteSpace(refund.IdempotencyKey) ? refund.Id : refund.IdempotencyKey;

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
        refund.LocalEntityType switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription => "Marketplace subscription",
            MarketplaceRefundEntityTypeConstants.EntitlementPurchase => "Entitlement purchase",
            _ => "Marketplace booking",
        };

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
            // A cancellation soft-deletes the recurring booking before the eligible
            // refund is approved. The paid invoice remains the authoritative Xero
            // target, so deleted recurring bookings must stay eligible here.
            .Where(item => item.MarketplaceBooking is not null)
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
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle)),
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
            _ => recurringBooking.StartDate.AddDays(1),
        };
    }

    private static AccountingInvoiceInstance? ResolveMatchingInvoiceInstance(
        RecurringBookingEntity recurringBooking,
        IReadOnlyList<AccountingInvoiceInstance> accountingInvoiceInstances,
        DateTimeOffset requestedAt)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        if (marketplaceBooking is null || accountingInvoiceInstances.Count == 0)
        {
            return null;
        }

        var hasCreatedAtValues = accountingInvoiceInstances.Any(item => item.CreatedAt != default);
        var currentPeriodInstances = accountingInvoiceInstances
            .Where(item => !string.Equals(item.ExternalStatus, AccountingStatusConstants.Cancelled, StringComparison.Ordinal));
        if (hasCreatedAtValues)
        {
            currentPeriodInstances = currentPeriodInstances.Where(item => item.CreatedAt <= requestedAt);
        }

        var matchingByNumber = !string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceNumber)
            ? currentPeriodInstances.FirstOrDefault(item =>
                string.Equals(item.ExternalInvoiceNumber, marketplaceBooking.InvoiceNumber, StringComparison.InvariantCultureIgnoreCase))
            : null;
        if (matchingByNumber is not null)
        {
            return matchingByNumber;
        }

        var matchingByUrl = !string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceUrl)
            ? currentPeriodInstances.FirstOrDefault(item =>
                string.Equals(item.ExternalInvoiceUrl, marketplaceBooking.InvoiceUrl, StringComparison.InvariantCultureIgnoreCase))
            : null;
        if (matchingByUrl is not null)
        {
            return matchingByUrl;
        }

        return currentPeriodInstances
                   .Where(item =>
                       !string.Equals(item.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal))
                   .OrderByDescending(item => item.CreatedAt)
                   .FirstOrDefault() ??
               currentPeriodInstances.OrderByDescending(item => item.CreatedAt).FirstOrDefault();
    }

    private sealed record XeroRefundInvoiceTarget(
        string LocalEntityId,
        string ExternalInvoiceId,
        string? ExternalInvoiceNumber);

    private sealed record XeroRefundInvoiceTargetResolution(
        XeroRefundInvoiceTarget? InvoiceTarget,
        string? ErrorMessage);
}
