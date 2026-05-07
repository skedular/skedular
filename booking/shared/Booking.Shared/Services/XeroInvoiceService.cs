using System.Globalization;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroOAuth2Token = Xero.NetStandard.OAuth2.Token.XeroOAuth2Token;
using AccountingInvoiceExportConfigurationStateConstants = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants;
using AccountingInvoiceExportModeConstants = Booking.Shared.Models.AccountingInvoiceExportModeConstants;
using Constants = Booking.Shared.GraphQL.Constants;
using AccountingInvoiceExportLink = Booking.Shared.Database.Entities.AccountingInvoiceExportLink;
using AccountingContactLink = Booking.Shared.Database.Entities.AccountingContactLink;
using AccountingPaymentEvent = Booking.Shared.Database.Entities.AccountingPaymentEvent;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using Organization = Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;
using XeroInvoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;

namespace Booking.Shared.Services;

public enum RecurringInvoiceHandlingDisposition
{
    ContinueToSkedular,
    StopAndPublish
}

public interface IXeroInvoiceService
{
    Task<bool> TryHandleMarketplaceBookingInvoiceAsync(
        string organizationId,
        Database.Entities.Booking booking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        CancellationToken cancellationToken);

    Task<RecurringInvoiceHandlingDisposition> HandleRecurringBookingInvoiceAsync(
        string organizationId,
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        CancellationToken cancellationToken);

    Task<SyncAccountingInvoiceStateResult> SyncAccountingInvoiceStateAsync(
        SyncAccountingInvoiceStateInput input,
        CancellationToken cancellationToken);
}

public class XeroInvoiceService(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    ITemporalService temporalService,
    ITemporalOutboxService temporalOutboxService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
    IXeroRepeatingInvoiceScheduleService xeroRepeatingInvoiceScheduleService,
    IXeroRecurringInvoiceTransitionService xeroRecurringInvoiceTransitionService,
    IInvoicePaymentTermsService invoicePaymentTermsService,
    TimeProvider timeProvider) : IXeroInvoiceService
{
    public async Task<bool> TryHandleMarketplaceBookingInvoiceAsync(
        string organizationId,
        Database.Entities.Booking booking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        CancellationToken cancellationToken)
    {
        var xeroConnection = await GetOrganizationXeroConnectionAsync(organizationId, cancellationToken);
        if (!IsXeroManagedForStandardInvoicing(xeroConnection))
        {
            return false;
        }

        await ExportMarketplaceBookingInvoiceToXeroAsync(
            organizationId,
            booking,
            null,
            marketplaceBooking,
            productVersion,
            xeroConnection!,
            cancellationToken);

        return xeroConnection!.SendInvoicesViaXero;
    }

    public async Task<RecurringInvoiceHandlingDisposition> HandleRecurringBookingInvoiceAsync(
        string organizationId,
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(productVersion.Product);
        ArgumentNullException.ThrowIfNull(productVersion.Product.Organization);

        var organizationBillingCycle = productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle();
        var existingAccountingInvoiceExportLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            cancellationToken);
        var xeroConnection = await GetOrganizationXeroConnectionAsync(organizationId, cancellationToken);
        var baseRepeatingSchedule = ShouldUseXeroRepeatingInvoices(xeroConnection)
            ? xeroRepeatingInvoiceScheduleService.GetSchedule(recurringBooking, marketplaceBooking, organizationBillingCycle)
            : null;
        var desiredRepeatingSchedule = ShouldUseRepeatingInvoiceTemplate(recurringBooking, baseRepeatingSchedule)
            ? baseRepeatingSchedule
            : null;
        var transitionDecision = xeroRecurringInvoiceTransitionService.Decide(
            existingAccountingInvoiceExportLink,
            ShouldUseXeroRepeatingInvoices(xeroConnection),
            desiredRepeatingSchedule);

        await ApplyRecurringInvoiceTransitionStateAsync(existingAccountingInvoiceExportLink, transitionDecision, cancellationToken);

        if (transitionDecision.Path == XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice)
        {
            return RecurringInvoiceHandlingDisposition.StopAndPublish;
        }

        if (!IsXeroManagedForRecurringInvoicing(xeroConnection))
        {
            return RecurringInvoiceHandlingDisposition.ContinueToSkedular;
        }

        if (transitionDecision.Path != XeroRecurringInvoiceExportPath.RepeatingInvoice ||
            !await TryExportRecurringInvoiceTemplateToXeroAsync(
                organizationId,
                recurringBooking,
                marketplaceBooking,
                productVersion,
                xeroConnection!,
                desiredRepeatingSchedule,
                cancellationToken))
        {
            await ExportMarketplaceBookingInvoiceToXeroAsync(
                organizationId,
                null,
                recurringBooking,
                marketplaceBooking,
                productVersion,
                xeroConnection!,
                cancellationToken);
        }

        return xeroConnection!.SendInvoicesViaXero
            ? RecurringInvoiceHandlingDisposition.StopAndPublish
            : RecurringInvoiceHandlingDisposition.ContinueToSkedular;
    }

    public async Task<SyncAccountingInvoiceStateResult> SyncAccountingInvoiceStateAsync(
        SyncAccountingInvoiceStateInput input,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            input.LocalEntityType,
            input.LocalEntityId,
            cancellationToken);
        if (accountingInvoiceLink is null ||
            string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId) ||
            accountingInvoiceLink.ExternalStatus is
                AccountingStatusConstants.Paid
                or AccountingStatusConstants.Failed
                or AccountingStatusConstants.Cancelled)
        {
            return new SyncAccountingInvoiceStateResult(true, null);
        }

        var xeroConnection = await GetOrganizationXeroConnectionAsync(input.OrganizationId, cancellationToken);
        if (xeroConnection is null || !xeroConnection.IsActive)
        {
            accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Failed;
            accountingInvoiceLink.LastError = xeroConnection?.LastError ?? "Xero connection is not active.";
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return new SyncAccountingInvoiceStateResult(true, null);
        }

        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(input.OrganizationId, xeroConnection, cancellationToken);
        var isRepeatingInvoice = string.Equals(
            accountingInvoiceLink.ExternalInvoiceMode,
            AccountingInvoiceExportModeConstants.RepeatingInvoice,
            StringComparison.Ordinal);
        if (isRepeatingInvoice && string.IsNullOrWhiteSpace(input.ExternalInvoiceIdHint))
        {
            return new SyncAccountingInvoiceStateResult(true, null);
        }

        var invoiceIdToLoad = isRepeatingInvoice ? input.ExternalInvoiceIdHint! : accountingInvoiceLink.ExternalInvoiceId;
        var invoiceResponse = await GetInvoiceAsync(
            xeroSdkClientFactory.CreateAccountingApi(),
            accessToken,
            refreshedConnection.TenantId,
            Guid.Parse(invoiceIdToLoad),
            cancellationToken);
        var invoice = invoiceResponse?._Invoices?.FirstOrDefault();
        if (invoice is null)
        {
            accountingInvoiceLink.LastError = "Xero invoice could not be loaded.";
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return new SyncAccountingInvoiceStateResult(
                false,
                isRepeatingInvoice ? timeProvider.GetUtcNow().AddHours(1) : null);
        }

        if (isRepeatingInvoice)
        {
            await ApplyXeroGeneratedRepeatingInvoiceSyncAsync(
                input.OrganizationId,
                accountingInvoiceLink,
                invoice,
                refreshedConnection,
                cancellationToken);
        }
        else
        {
            await ApplyXeroInvoiceSyncAsync(input.OrganizationId, accountingInvoiceLink, invoice, refreshedConnection, cancellationToken);
        }

        var isConcreteInvoicePaid = string.Equals(GetAccountingStatus(invoice), AccountingStatusConstants.Paid, StringComparison.Ordinal);

        await ProcessAccountingPaymentEventsAsync(
            accountingInvoiceLink,
            invoiceIdToLoad,
            isConcreteInvoicePaid,
            cancellationToken);
        await PropagateInvoiceReferencesAsync(
            accountingInvoiceLink.LocalEntityType,
            accountingInvoiceLink.LocalEntityId,
            accountingInvoiceLink,
            cancellationToken);

        var isPaid = isConcreteInvoicePaid;
        return new SyncAccountingInvoiceStateResult(isPaid, isPaid ? null : timeProvider.GetUtcNow().AddHours(12));
    }

    private static bool IsXeroManagedForStandardInvoicing(XeroConnection? xeroConnection) =>
        IsXeroConnectionReady(xeroConnection) &&
        xeroConnection!.BillingMode is XeroBillingModeConstants.Enabled or XeroBillingModeConstants.RepeatingInvoices;

    private static DateTime ResolveRepeatingInvoiceStartDate(RecurringBooking recurringBooking) =>
        recurringBooking.StartDate.UtcDateTime.Date;

    private static DateTime ResolveRepeatingInvoiceStartDate(
        RecurringBooking recurringBooking,
        XeroRepeatingInvoiceScheduleDefinition scheduleDefinition) =>
        scheduleDefinition.Unit switch
        {
            Schedule.UnitEnum.WEEKLY => ResolveRepeatingInvoiceStartDate(recurringBooking).AddDays(7 * scheduleDefinition.Period),
            Schedule.UnitEnum.MONTHLY => ResolveRepeatingInvoiceStartDate(recurringBooking).AddMonths(scheduleDefinition.Period),
            _ => throw new ArgumentOutOfRangeException(nameof(scheduleDefinition))
        };

    private static bool ShouldUseRepeatingInvoiceTemplate(
        RecurringBooking recurringBooking,
        XeroRepeatingInvoiceScheduleDefinition? scheduleDefinition) =>
        scheduleDefinition is not null &&
        (recurringBooking.MarketplaceBookingSubscription?.AutoRenew == true ||
         string.Equals(
             scheduleDefinition.Source,
             XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
             StringComparison.Ordinal));

    private static bool ShouldPreserveStandardInvoiceTransitionState(AccountingInvoiceExportLink accountingInvoiceLink) =>
        string.Equals(accountingInvoiceLink.ExternalInvoiceMode, AccountingInvoiceExportModeConstants.StandardInvoice, StringComparison.Ordinal) &&
        string.Equals(accountingInvoiceLink.ExportConfigurationState, AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
            StringComparison.Ordinal);

    private async Task ExportMarketplaceBookingInvoiceToXeroAsync(
        string organizationId,
        Database.Entities.Booking? booking,
        RecurringBooking? recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        var localEntityType = booking is null ? AccountingEntityTypeConstants.RecurringBooking : AccountingEntityTypeConstants.MarketplaceBooking;
        var localEntityId = booking is null ? recurringBooking!.Id : marketplaceBooking.Id;
        var accountingInvoiceLink =
            await UpsertPendingAccountingInvoiceExportLinkAsync(organizationId, localEntityType, localEntityId, cancellationToken);
        var customer = GetInvoiceCustomer(booking, recurringBooking, marketplaceBooking) ?? throw new CustomerNotFound();
        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection, cancellationToken);
        var contact = await UpsertXeroContactAsync(organizationId, customer, refreshedConnection, accessToken, cancellationToken);
        _ = await ExportInvoiceAsync(
            booking,
            recurringBooking,
            marketplaceBooking,
            organizationId,
            productVersion,
            accountingInvoiceLink,
            contact,
            refreshedConnection,
            accessToken,
            cancellationToken);

        await ProcessAccountingPaymentEventsAsync(
            accountingInvoiceLink,
            accountingInvoiceLink.ExternalInvoiceId,
            string.Equals(accountingInvoiceLink.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal),
            cancellationToken);
        await UpdateMarketplaceInvoiceReferencesAsync(marketplaceBooking, accountingInvoiceLink, cancellationToken);

        if (refreshedConnection.AutoReconcilePayments &&
            !string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId) &&
            accountingInvoiceLink.ExternalStatus is not AccountingStatusConstants.Paid)
        {
            await temporalService.StartWorkflowMaintainAccountingInvoiceStateAsync(
                new MaintainAccountingInvoiceStateInput(organizationId, localEntityType, localEntityId),
                cancellationToken);
        }
    }

    private async Task<bool> TryExportRecurringInvoiceTemplateToXeroAsync(
        string organizationId,
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        XeroConnection xeroConnection,
        XeroRepeatingInvoiceScheduleDefinition? scheduleDefinition,
        CancellationToken cancellationToken)
    {
        if (scheduleDefinition is null)
        {
            return false;
        }

        var accountingInvoiceLink = await UpsertPendingAccountingInvoiceExportLinkAsync(
            organizationId,
            AccountingEntityTypeConstants.RecurringBooking,
            recurringBooking.Id,
            cancellationToken);
        var customer = GetInvoiceCustomer(null, recurringBooking, marketplaceBooking) ?? throw new CustomerNotFound();
        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection, cancellationToken);
        var contact = await UpsertXeroContactAsync(organizationId, customer, refreshedConnection, accessToken, cancellationToken);
        var existingInvoiceInstances = await repositoryFactory.AccountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
            accountingInvoiceLink.Id,
            cancellationToken);
        XeroInvoice? initialInvoice = null;
        if (existingInvoiceInstances.Count == 0)
        {
            initialInvoice = await ExportInitialRecurringInvoiceAsync(
                organizationId,
                recurringBooking,
                marketplaceBooking,
                productVersion,
                accountingInvoiceLink,
                contact,
                refreshedConnection,
                accessToken,
                scheduleDefinition,
                cancellationToken);
        }

        var repeatingInvoice = await ExportRepeatingInvoiceAsync(
            recurringBooking,
            marketplaceBooking,
            productVersion,
            accountingInvoiceLink,
            contact,
            refreshedConnection,
            accessToken,
            scheduleDefinition,
            cancellationToken);

        await ApplyXeroRepeatingInvoiceSyncAsync(accountingInvoiceLink, repeatingInvoice, scheduleDefinition, cancellationToken);
        await UpdateMarketplaceInvoiceReferencesAsync(marketplaceBooking, accountingInvoiceLink, cancellationToken);

        if (initialInvoice?.InvoiceID.HasValue == true &&
            refreshedConnection.AutoReconcilePayments &&
            GetAccountingStatus(initialInvoice) is not AccountingStatusConstants.Paid)
        {
            await temporalService.StartWorkflowMaintainAccountingInvoiceStateAsync(
                new MaintainAccountingInvoiceStateInput(
                    organizationId,
                    AccountingEntityTypeConstants.RecurringBooking,
                    recurringBooking.Id,
                    initialInvoice.InvoiceID.Value.ToString()),
                cancellationToken);
        }

        return true;
    }

    private async Task<XeroInvoice> ExportInitialRecurringInvoiceAsync(
        string organizationId,
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        AccountingInvoiceExportLink accountingInvoiceLink,
        Contact contact,
        XeroConnection xeroConnection,
        string accessToken,
        XeroRepeatingInvoiceScheduleDefinition scheduleDefinition,
        CancellationToken cancellationToken)
    {
        var organization = await GetOrganizationAsync(organizationId, cancellationToken);
        var invoiceDate = timeProvider.GetUtcNow();
        var dueDate = invoicePaymentTermsService.GetDueDate(invoiceDate, organization.BillingDetails?.InvoiceDueInDays);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        var invoiceRequest = new XeroInvoice
        {
            Type = XeroInvoice.TypeEnum.ACCREC,
            Status = xeroConnection.SendInvoicesViaXero ? XeroInvoice.StatusEnum.AUTHORISED : XeroInvoice.StatusEnum.DRAFT,
            LineAmountTypes = marketplaceBooking.ProductPricing.IsTaxInclusive ? LineAmountTypes.Inclusive : LineAmountTypes.Exclusive,
            Contact = contact,
            InvoiceNumber = marketplaceBooking.InvoiceNumber,
            Reference = BuildReference(marketplaceBooking.InvoiceNumber ?? string.Empty, xeroConnection),
            Date = invoiceDate.UtcDateTime.Date,
            DueDate = dueDate.UtcDateTime.Date,
            LineItems =
            [
                new LineItem
                {
                    Description = BuildInvoiceLineDescription(null, recurringBooking, marketplaceBooking, productVersion),
                    Quantity = marketplaceBooking.Quantity <= 0 ? 1 : marketplaceBooking.Quantity,
                    UnitAmount = CalculateUnitAmount(scheduleDefinition.InvoiceAmount, marketplaceBooking.Quantity),
                    AccountCode = xeroConnection.DefaultSalesAccountCode
                }
            ]
        };

        var invoiceResponse = await accountingApi.CreateInvoicesAsync(
            accessToken,
            xeroConnection.TenantId,
            new Invoices { _Invoices = [invoiceRequest] },
            null,
            null,
            $"{accountingInvoiceLink.Id}:initial-standard",
            cancellationToken);
        var exportedInvoice = invoiceResponse?._Invoices?.FirstOrDefault() ?? throw new XeroInvoiceExportFailedException();
        var externalInvoiceUrl = await GetOnlineInvoiceUrlAsync(organizationId, xeroConnection, exportedInvoice, cancellationToken);

        await UpsertAccountingInvoiceInstanceAsync(accountingInvoiceLink, exportedInvoice, externalInvoiceUrl, cancellationToken);
        await UpsertAccountingPaymentEventsAsync(accountingInvoiceLink, exportedInvoice, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (xeroConnection.SendInvoicesViaXero && exportedInvoice.InvoiceID.HasValue)
        {
            await TryEmailInvoiceAsync(
                accountingApi,
                accessToken,
                xeroConnection,
                exportedInvoice.InvoiceID.Value,
                accountingInvoiceLink,
                cancellationToken);
        }

        return exportedInvoice;
    }

    private static CustomerEntity? GetInvoiceCustomer(
        Database.Entities.Booking? booking,
        RecurringBooking? recurringBooking,
        MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.PaidByCustomer ??
        booking?.CreatedByCustomer ??
        recurringBooking?.CreatedByCustomer ??
        booking?.InvolvedCustomers.FirstOrDefault() ??
        recurringBooking?.InvolvedCustomers.FirstOrDefault();

    private async Task<AccountingInvoiceExportLink> UpsertPendingAccountingInvoiceExportLinkAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        CancellationToken cancellationToken)
    {
        var existingLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            localEntityType,
            localEntityId,
            cancellationToken);

        if (existingLink is null)
        {
            existingLink = repositoryFactory.AccountingInvoiceExportLinkRepository.Add(
                new AccountingInvoiceExportLink
                {
                    Id = randomHelper.Generate(),
                    Provider = AccountingProviderConstants.Xero,
                    LocalEntityType = localEntityType,
                    LocalEntityId = localEntityId,
                    ExternalStatus = AccountingStatusConstants.PendingExport,
                    OrganizationId = organizationId
                });
        }
        else
        {
            existingLink.ExternalStatus = AccountingStatusConstants.PendingExport;
            existingLink.LastError = null;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(existingLink);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return existingLink;
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationBillingServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput { OrganizationId = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return string.IsNullOrWhiteSpace(response.Id) ? null : response;
    }

    private async Task<Organization> GetOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
        await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

    private static bool IsXeroConnectionReady(XeroConnection? xeroConnection) =>
        xeroConnection is { IsActive: true, HasRefreshToken: true } && !string.IsNullOrWhiteSpace(xeroConnection.TenantId);

    private static bool IsXeroManagedForRecurringInvoicing(XeroConnection? xeroConnection) =>
        IsXeroConnectionReady(xeroConnection) &&
        xeroConnection!.BillingMode is XeroBillingModeConstants.Enabled or XeroBillingModeConstants.RepeatingInvoices;

    private static bool ShouldUseXeroRepeatingInvoices(XeroConnection? xeroConnection) =>
        IsXeroConnectionReady(xeroConnection) &&
        xeroConnection!.BillingMode == XeroBillingModeConstants.RepeatingInvoices;

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

    private async Task<Contact> UpsertXeroContactAsync(
        string organizationId,
        CustomerEntity customer,
        XeroConnection xeroConnection,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var email = customer.Identities
                        .Where(item => item.EmailVerified == true && !string.IsNullOrWhiteSpace(item.Email))
                        .Select(item => item.Email)
                        .FirstOrDefault() ??
                    customer.Identities.Select(item => item.Email).FirstOrDefault(item => !string.IsNullOrWhiteSpace(item)) ??
                    string.Empty;
        var displayName = customer.Name ??
                          string.Join(" ", new[] { customer.GivenName, customer.FamilyName }.Where(item => !string.IsNullOrWhiteSpace(item))).Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = customer.Id;
        }

        var existingLink = await repositoryFactory.AccountingContactLinkRepository.GetByProviderAndLocalEntityAsync(
            organizationId,
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.Customer,
            customer.Id,
            cancellationToken);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        var contact = new Contact
        {
            Name = displayName,
            EmailAddress = email,
            ContactNumber = customer.Id,
            ContactID = Guid.TryParse(existingLink?.ExternalContactId, out var contactId) ? contactId : null
        };

        if (contact.ContactID is null)
        {
            var contactsByName = await accountingApi.GetContactsAsync(
                accessToken,
                xeroConnection.TenantId,
                null,
                null,
                null,
                null,
                1,
                false,
                true,
                displayName,
                100,
                cancellationToken);
            var existingXeroContact =
                contactsByName._Contacts?.FirstOrDefault(item => string.Equals(item.Name, displayName, StringComparison.OrdinalIgnoreCase));

            if (existingXeroContact is null && !string.IsNullOrWhiteSpace(email))
            {
                var contactsByEmail = await accountingApi.GetContactsAsync(
                    accessToken,
                    xeroConnection.TenantId,
                    null,
                    null,
                    null,
                    null,
                    1,
                    false,
                    true,
                    email,
                    100,
                    cancellationToken);
                existingXeroContact =
                    contactsByEmail._Contacts?.FirstOrDefault(item => string.Equals(item.EmailAddress, email, StringComparison.OrdinalIgnoreCase));
            }

            if (existingXeroContact?.ContactID is not null)
            {
                contact.ContactID = existingXeroContact.ContactID;
            }
        }

        var contactsResponse = contact.ContactID is null
            ? await accountingApi.CreateContactsAsync(accessToken, xeroConnection.TenantId, new Contacts { _Contacts = [contact] }, null, null,
                cancellationToken)
            : await accountingApi.UpdateOrCreateContactsAsync(
                accessToken,
                xeroConnection.TenantId,
                new Contacts { _Contacts = [contact] },
                null,
                null,
                cancellationToken);
        var exportedContact = contactsResponse?._Contacts?.FirstOrDefault() ??
                              throw new XeroContactExportFailedException();

        if (existingLink is null)
        {
            repositoryFactory.AccountingContactLinkRepository.Add(
                new AccountingContactLink
                {
                    Id = randomHelper.Generate(),
                    Provider = AccountingProviderConstants.Xero,
                    LocalEntityType = AccountingEntityTypeConstants.Customer,
                    LocalEntityId = customer.Id,
                    ExternalContactId = exportedContact.ContactID?.ToString(),
                    ExternalContactName = exportedContact.Name,
                    OrganizationId = organizationId,
                    LastSyncedAt = timeProvider.GetUtcNow()
                });
        }
        else
        {
            existingLink.ExternalContactId = exportedContact.ContactID?.ToString();
            existingLink.ExternalContactName = exportedContact.Name;
            existingLink.LastSyncedAt = timeProvider.GetUtcNow();
            existingLink.LastError = null;
            repositoryFactory.AccountingContactLinkRepository.Update(existingLink);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return exportedContact;
    }

    private async Task<XeroInvoice> ExportInvoiceAsync(
        Database.Entities.Booking? booking,
        RecurringBooking? recurringBooking,
        MarketplaceBooking marketplaceBooking,
        string organizationId,
        ProductVersion productVersion,
        AccountingInvoiceExportLink accountingInvoiceLink,
        Contact contact,
        XeroConnection xeroConnection,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var organization = await GetOrganizationAsync(organizationId, cancellationToken);
        var invoiceDate = ResolveInvoiceDate(booking, recurringBooking, marketplaceBooking);
        var dueDate = invoicePaymentTermsService.GetDueDate(invoiceDate, organization.BillingDetails?.InvoiceDueInDays);
        var recurringBillingDefinition = recurringBooking is null
            ? null
            : recurringInvoiceBillingScheduleService.GetSchedule(
                recurringBooking,
                marketplaceBooking,
                productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        if (!string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId))
        {
            var existingInvoiceResponse = await accountingApi.GetInvoiceAsync(
                accessToken,
                xeroConnection.TenantId,
                Guid.Parse(accountingInvoiceLink.ExternalInvoiceId),
                null,
                cancellationToken);
            var existingInvoice = existingInvoiceResponse?._Invoices?.FirstOrDefault();
            if (existingInvoice is not null)
            {
                await ApplyXeroInvoiceSyncAsync(organizationId, accountingInvoiceLink, existingInvoice, xeroConnection, cancellationToken);
                return existingInvoice;
            }
        }

        var invoiceRequest = new XeroInvoice
        {
            InvoiceID = Guid.TryParse(accountingInvoiceLink.ExternalInvoiceId, out var invoiceId) ? invoiceId : null,
            Type = XeroInvoice.TypeEnum.ACCREC,
            Status = xeroConnection.SendInvoicesViaXero ? XeroInvoice.StatusEnum.AUTHORISED : XeroInvoice.StatusEnum.DRAFT,
            LineAmountTypes = marketplaceBooking.ProductPricing.IsTaxInclusive ? LineAmountTypes.Inclusive : LineAmountTypes.Exclusive,
            Contact = contact,
            InvoiceNumber = marketplaceBooking.InvoiceNumber,
            Reference = BuildReference(marketplaceBooking.InvoiceNumber ?? string.Empty, xeroConnection),
            Date = invoiceDate.UtcDateTime.Date,
            DueDate = dueDate.UtcDateTime.Date,
            LineItems =
            [
                new LineItem
                {
                    Description = BuildInvoiceLineDescription(booking, recurringBooking, marketplaceBooking, productVersion),
                    Quantity = marketplaceBooking.Quantity <= 0 ? 1 : marketplaceBooking.Quantity,
                    UnitAmount = CalculateUnitAmount(
                        recurringBillingDefinition?.InvoiceAmount ?? CalculateInvoiceTotalAmount(marketplaceBooking),
                        marketplaceBooking.Quantity),
                    AccountCode = xeroConnection.DefaultSalesAccountCode
                }
            ]
        };

        var invoiceResponse = await accountingApi.CreateInvoicesAsync(
            accessToken,
            xeroConnection.TenantId,
            new Invoices { _Invoices = [invoiceRequest] },
            null,
            null,
            accountingInvoiceLink.Id,
            cancellationToken);
        var exportedInvoice = invoiceResponse?._Invoices?.FirstOrDefault() ?? throw new XeroInvoiceExportFailedException();

        await ApplyXeroInvoiceSyncAsync(organizationId, accountingInvoiceLink, exportedInvoice, xeroConnection, cancellationToken);

        if (xeroConnection.SendInvoicesViaXero && exportedInvoice.InvoiceID.HasValue)
        {
            await TryEmailInvoiceAsync(accountingApi, accessToken, xeroConnection, exportedInvoice.InvoiceID.Value, accountingInvoiceLink,
                cancellationToken);
        }

        return exportedInvoice;
    }

    private async Task<RepeatingInvoice> ExportRepeatingInvoiceAsync(
        RecurringBooking recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion,
        AccountingInvoiceExportLink accountingInvoiceLink,
        Contact contact,
        XeroConnection xeroConnection,
        string accessToken,
        XeroRepeatingInvoiceScheduleDefinition scheduleDefinition,
        CancellationToken cancellationToken)
    {
        var organization = await GetOrganizationAsync(productVersion.Product.Organization.Id, cancellationToken);
        var invoiceDueInDays = invoicePaymentTermsService.GetInvoiceDueInDays(organization.BillingDetails?.InvoiceDueInDays);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        if (Guid.TryParse(accountingInvoiceLink.ExternalInvoiceId, out var repeatingInvoiceId))
        {
            var existingInvoiceResponse = await accountingApi.GetRepeatingInvoiceAsync(
                accessToken,
                xeroConnection.TenantId,
                repeatingInvoiceId,
                cancellationToken);
            var existingInvoice = existingInvoiceResponse?._RepeatingInvoices?.FirstOrDefault();
            if (existingInvoice is not null)
            {
                return existingInvoice;
            }
        }

        var repeatingInvoiceRequest = new RepeatingInvoice
        {
            Type = RepeatingInvoice.TypeEnum.ACCREC,
            Status = xeroConnection.SendInvoicesViaXero ? RepeatingInvoice.StatusEnum.AUTHORISED : RepeatingInvoice.StatusEnum.DRAFT,
            ApprovedForSending = xeroConnection.SendInvoicesViaXero,
            LineAmountTypes = marketplaceBooking.ProductPricing.IsTaxInclusive ? LineAmountTypes.Inclusive : LineAmountTypes.Exclusive,
            Contact = contact,
            Reference = BuildReference(marketplaceBooking.InvoiceNumber ?? string.Empty, xeroConnection),
            BrandingThemeID = Guid.TryParse(xeroConnection.DefaultBrandingThemeId, out var brandingThemeId) ? brandingThemeId : null,
            Schedule = new Schedule
            {
                Unit = scheduleDefinition.Unit,
                Period = scheduleDefinition.Period,
                DueDateType = Schedule.DueDateTypeEnum.DAYSAFTERBILLDATE,
                DueDate = invoiceDueInDays,
                StartDate = ResolveRepeatingInvoiceStartDate(recurringBooking, scheduleDefinition)
            },
            LineItems =
            [
                new LineItem
                {
                    Description = BuildInvoiceLineDescription(null, recurringBooking, marketplaceBooking, productVersion),
                    Quantity = marketplaceBooking.Quantity <= 0 ? 1 : marketplaceBooking.Quantity,
                    UnitAmount = CalculateUnitAmount(scheduleDefinition.InvoiceAmount, marketplaceBooking.Quantity),
                    AccountCode = xeroConnection.DefaultSalesAccountCode
                }
            ]
        };

        var invoiceResponse = await accountingApi.CreateRepeatingInvoicesAsync(
            accessToken,
            xeroConnection.TenantId,
            new RepeatingInvoices { _RepeatingInvoices = [repeatingInvoiceRequest] },
            null,
            accountingInvoiceLink.Id,
            cancellationToken);
        return invoiceResponse?._RepeatingInvoices?.FirstOrDefault() ?? throw new XeroInvoiceExportFailedException();
    }

    private static string BuildInvoiceLineDescription(
        Database.Entities.Booking? booking,
        RecurringBooking? recurringBooking,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion)
    {
        var title = productVersion.ListingMetadata?.Title;
        var fallbackTitle = !string.IsNullOrWhiteSpace(title) ? title : $"Marketplace {productVersion.Type} booking";

        if (booking is not null)
        {
            return $"{fallbackTitle}{Environment.NewLine}{BookingInvoiceService.FormatInvoicePeriod(booking.From, booking.Until)}";
        }

        if (recurringBooking is not null)
        {
            var billingDefinition = new RecurringInvoiceBillingScheduleService().GetSchedule(
                recurringBooking,
                marketplaceBooking,
                productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
            var (displayStart, displayEnd) = BookingInvoiceService.ResolveRecurringInvoiceDisplayPeriod(recurringBooking, billingDefinition);
            return
                $"{fallbackTitle}{Environment.NewLine}" +
                $"{marketplaceBooking.ProductPricing.PurchaseCadence.ToProductPricingCadenceName()} pass{Environment.NewLine}" +
                $"{displayStart.ToShortDate()} - {displayEnd.ToShortDate()}";
        }

        return fallbackTitle;
    }

    private static decimal CalculateInvoiceTotalAmount(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.ProductPricing.IsTaxInclusive
            ? marketplaceBooking.TotalAmount ?? marketplaceBooking.TotalAmountExcludeTax ?? 0m
            : marketplaceBooking.TotalAmountExcludeTax ?? marketplaceBooking.TotalAmount ?? 0m;

    private static decimal CalculateUnitAmount(decimal totalAmount, int quantity)
    {
        var safeQuantity = quantity <= 0 ? 1 : quantity;
        return safeQuantity <= 1 ? totalAmount : decimal.Round(totalAmount / safeQuantity, 4, MidpointRounding.AwayFromZero);
    }

    private static DateTimeOffset ResolveInvoiceDate(
        Database.Entities.Booking? booking,
        RecurringBooking? recurringBooking,
        MarketplaceBooking marketplaceBooking) =>
        booking?.CreatedAt ??
        recurringBooking?.CreatedAt ??
        marketplaceBooking.CreatedAt;

    private async Task ApplyXeroInvoiceSyncAsync(
        string organizationId,
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        var shouldPreserveTransitionState = ShouldPreserveStandardInvoiceTransitionState(accountingInvoiceLink);
        accountingInvoiceLink.ExternalInvoiceId = invoice.InvoiceID?.ToString();
        accountingInvoiceLink.ExternalInvoiceNumber = invoice.InvoiceNumber;
        accountingInvoiceLink.ExternalInvoiceUrl = await GetOnlineInvoiceUrlAsync(organizationId, xeroConnection, invoice, cancellationToken);
        accountingInvoiceLink.ExternalInvoiceMode = AccountingInvoiceExportModeConstants.StandardInvoice;
        accountingInvoiceLink.ExportConfigurationState = shouldPreserveTransitionState
            ? accountingInvoiceLink.ExportConfigurationState
            : AccountingInvoiceExportConfigurationStateConstants.Active;
        accountingInvoiceLink.ExportConfigurationMessage = shouldPreserveTransitionState
            ? accountingInvoiceLink.ExportConfigurationMessage
            : null;
        accountingInvoiceLink.RepeatingScheduleSource = null;
        accountingInvoiceLink.RepeatingScheduleUnit = null;
        accountingInvoiceLink.RepeatingSchedulePeriod = null;
        accountingInvoiceLink.LastSyncedAt = timeProvider.GetUtcNow();
        accountingInvoiceLink.LastError = null;
        accountingInvoiceLink.ExternalStatus = GetAccountingStatus(invoice);
        accountingInvoiceLink.SentAt ??= accountingInvoiceLink.ExternalStatus is AccountingStatusConstants.Sent or AccountingStatusConstants.Paid
            ? timeProvider.GetUtcNow()
            : null;
        accountingInvoiceLink.PaidAt = accountingInvoiceLink.ExternalStatus == AccountingStatusConstants.Paid
            ? timeProvider.GetUtcNow()
            : accountingInvoiceLink.PaidAt;

        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
        await UpsertAccountingInvoiceInstanceAsync(accountingInvoiceLink, invoice, accountingInvoiceLink.ExternalInvoiceUrl, cancellationToken);
        await UpsertAccountingPaymentEventsAsync(accountingInvoiceLink, invoice, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ApplyXeroRepeatingInvoiceSyncAsync(
        AccountingInvoiceExportLink accountingInvoiceLink,
        RepeatingInvoice invoice,
        XeroRepeatingInvoiceScheduleDefinition scheduleDefinition,
        CancellationToken cancellationToken)
    {
        accountingInvoiceLink.ExternalInvoiceId = invoice.RepeatingInvoiceID?.ToString() ?? invoice.ID?.ToString();
        accountingInvoiceLink.ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice;
        accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Active;
        accountingInvoiceLink.ExportConfigurationMessage = null;
        accountingInvoiceLink.RepeatingScheduleSource = scheduleDefinition.Source;
        accountingInvoiceLink.RepeatingScheduleUnit = scheduleDefinition.Unit.ToString();
        accountingInvoiceLink.RepeatingSchedulePeriod = scheduleDefinition.Period;
        accountingInvoiceLink.LastSyncedAt = timeProvider.GetUtcNow();
        accountingInvoiceLink.LastError = null;
        accountingInvoiceLink.ExternalStatus = GetAccountingStatus(invoice);
        accountingInvoiceLink.SentAt ??= accountingInvoiceLink.ExternalStatus == AccountingStatusConstants.Sent
            ? timeProvider.GetUtcNow()
            : null;

        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ApplyXeroGeneratedRepeatingInvoiceSyncAsync(
        string organizationId,
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        var externalInvoiceUrl = await GetOnlineInvoiceUrlAsync(organizationId, xeroConnection, invoice, cancellationToken);
        accountingInvoiceLink.ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice;
        accountingInvoiceLink.ExportConfigurationState = AccountingInvoiceExportConfigurationStateConstants.Active;
        accountingInvoiceLink.ExportConfigurationMessage = null;
        accountingInvoiceLink.LastSyncedAt = timeProvider.GetUtcNow();
        accountingInvoiceLink.LastError = null;
        accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Exported;

        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
        await UpsertAccountingInvoiceInstanceAsync(accountingInvoiceLink, invoice, externalInvoiceUrl, cancellationToken);
        await UpsertAccountingPaymentEventsAsync(accountingInvoiceLink, invoice, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertAccountingInvoiceInstanceAsync(
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        string? externalInvoiceUrl,
        CancellationToken cancellationToken)
    {
        var externalInvoiceId = invoice.InvoiceID?.ToString();
        if (string.IsNullOrWhiteSpace(externalInvoiceId))
        {
            return;
        }

        var existingInvoiceInstance = await repositoryFactory.AccountingInvoiceInstanceRepository.GetByProviderAndExternalInvoiceIdAsync(
            AccountingProviderConstants.Xero,
            externalInvoiceId,
            cancellationToken);
        if (existingInvoiceInstance is null)
        {
            repositoryFactory.AccountingInvoiceInstanceRepository.Add(
                new AccountingInvoiceInstance
                {
                    Id = randomHelper.Generate(),
                    AccountingInvoiceExportLinkId = accountingInvoiceLink.Id,
                    Provider = AccountingProviderConstants.Xero,
                    ExternalInvoiceId = externalInvoiceId,
                    ExternalInvoiceNumber = invoice.InvoiceNumber,
                    ExternalInvoiceUrl = externalInvoiceUrl,
                    ExternalStatus = GetAccountingStatus(invoice),
                    SentAt = GetAccountingStatus(invoice) is AccountingStatusConstants.Sent or AccountingStatusConstants.Paid
                        ? timeProvider.GetUtcNow()
                        : null,
                    PaidAt = GetAccountingStatus(invoice) == AccountingStatusConstants.Paid
                        ? timeProvider.GetUtcNow()
                        : null,
                    LastSyncedAt = timeProvider.GetUtcNow(),
                    OrganizationId = accountingInvoiceLink.OrganizationId
                });
            return;
        }

        existingInvoiceInstance.AccountingInvoiceExportLinkId = accountingInvoiceLink.Id;
        existingInvoiceInstance.ExternalInvoiceNumber = invoice.InvoiceNumber;
        existingInvoiceInstance.ExternalInvoiceUrl = externalInvoiceUrl;
        existingInvoiceInstance.ExternalStatus = GetAccountingStatus(invoice);
        existingInvoiceInstance.SentAt ??= existingInvoiceInstance.ExternalStatus is AccountingStatusConstants.Sent or AccountingStatusConstants.Paid
            ? timeProvider.GetUtcNow()
            : null;
        existingInvoiceInstance.PaidAt = existingInvoiceInstance.ExternalStatus == AccountingStatusConstants.Paid
            ? timeProvider.GetUtcNow()
            : existingInvoiceInstance.PaidAt;
        existingInvoiceInstance.LastSyncedAt = timeProvider.GetUtcNow();
        existingInvoiceInstance.LastError = null;
        repositoryFactory.AccountingInvoiceInstanceRepository.Update(existingInvoiceInstance);
    }

    private async Task ApplyRecurringInvoiceTransitionStateAsync(
        AccountingInvoiceExportLink? accountingInvoiceLink,
        XeroRecurringInvoiceTransitionDecision decision,
        CancellationToken cancellationToken)
    {
        if (accountingInvoiceLink is null)
        {
            return;
        }

        accountingInvoiceLink.ExportConfigurationState = decision.ConfigurationState;
        accountingInvoiceLink.ExportConfigurationMessage = decision.ConfigurationMessage;
        repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task TryEmailInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        XeroConnection xeroConnection,
        Guid invoiceId,
        AccountingInvoiceExportLink accountingInvoiceLink,
        CancellationToken cancellationToken)
    {
        try
        {
            await accountingApi.EmailInvoiceAsync(
                accessToken,
                xeroConnection.TenantId,
                invoiceId,
                new RequestEmpty(),
                null,
                cancellationToken);
        }
        catch (Exception exception)
        {
            accountingInvoiceLink.LastError = exception.Message;
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<string?> GetOnlineInvoiceUrlAsync(
        string organizationId,
        XeroConnection xeroConnection,
        XeroInvoice invoice,
        CancellationToken cancellationToken)
    {
        if (invoice.InvoiceID is not { } invoiceId)
        {
            return invoice.Url;
        }

        try
        {
            var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection, cancellationToken);
            var onlineInvoice = await xeroSdkClientFactory.CreateAccountingApi()
                .GetOnlineInvoiceAsync(accessToken, refreshedConnection.TenantId, invoiceId, cancellationToken);

            return onlineInvoice?._OnlineInvoices?.FirstOrDefault()?.OnlineInvoiceUrl ?? invoice.Url;
        }
        catch
        {
            return invoice.Url;
        }
    }

    private async Task UpsertAccountingPaymentEventsAsync(
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        CancellationToken cancellationToken)
    {
        var externalInvoiceId = invoice.InvoiceID?.ToString();
        if (string.IsNullOrWhiteSpace(externalInvoiceId))
        {
            return;
        }

        foreach (var payment in invoice.Payments ?? [])
        {
            var externalPaymentId = payment.PaymentID?.ToString();
            if (string.IsNullOrWhiteSpace(externalPaymentId))
            {
                continue;
            }

            var existingPaymentEvent = await repositoryFactory.AccountingPaymentEventRepository.GetByProviderAndExternalPaymentIdAsync(
                accountingInvoiceLink.OrganizationId,
                AccountingProviderConstants.Xero,
                externalPaymentId,
                cancellationToken);
            if (existingPaymentEvent is not null)
            {
                existingPaymentEvent.ExternalStatus = payment.Status.ToString();
                existingPaymentEvent.OccurredAt = payment.Date ?? timeProvider.GetUtcNow();
                existingPaymentEvent.PayloadJson = $"{{\"amount\":{payment.Amount?.ToString(CultureInfo.InvariantCulture) ?? "0"}}}";
                existingPaymentEvent.ProcessedAt = null;
                existingPaymentEvent.ExternalInvoiceId = externalInvoiceId;
                repositoryFactory.AccountingPaymentEventRepository.Update(existingPaymentEvent);
                continue;
            }

            repositoryFactory.AccountingPaymentEventRepository.Add(
                new AccountingPaymentEvent
                {
                    Id = randomHelper.Generate(),
                    Provider = AccountingProviderConstants.Xero,
                    ExternalInvoiceId = externalInvoiceId,
                    ExternalPaymentId = externalPaymentId,
                    ExternalStatus = payment.Status.ToString(),
                    OccurredAt = payment.Date ?? timeProvider.GetUtcNow(),
                    PayloadJson = $"{{\"amount\":{payment.Amount?.ToString(CultureInfo.InvariantCulture) ?? "0"}}}",
                    ProcessedAt = null,
                    OrganizationId = accountingInvoiceLink.OrganizationId
                });
        }
    }

    private async Task ProcessAccountingPaymentEventsAsync(
        AccountingInvoiceExportLink accountingInvoiceLink,
        string? externalInvoiceId,
        bool isInvoicePaid,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(externalInvoiceId))
        {
            return;
        }

        var unprocessedEvents = await repositoryFactory.AccountingPaymentEventRepository.GetUnprocessedByProviderAndExternalInvoiceIdAsync(
            accountingInvoiceLink.OrganizationId,
            accountingInvoiceLink.Provider,
            externalInvoiceId,
            cancellationToken);
        if (unprocessedEvents.Count == 0)
        {
            if (isInvoicePaid)
            {
                await ConfirmAccountingInvoicePaymentAsync(accountingInvoiceLink, cancellationToken);
            }

            return;
        }

        if (isInvoicePaid)
        {
            await ConfirmAccountingInvoicePaymentAsync(accountingInvoiceLink, cancellationToken);
        }

        foreach (var paymentEvent in unprocessedEvents)
        {
            paymentEvent.ProcessedAt = timeProvider.GetUtcNow();
            repositoryFactory.AccountingPaymentEventRepository.Update(paymentEvent);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ConfirmAccountingInvoicePaymentAsync(AccountingInvoiceExportLink accountingInvoiceLink, CancellationToken cancellationToken)
    {
        switch (accountingInvoiceLink.LocalEntityType)
        {
            case AccountingEntityTypeConstants.MarketplaceBooking:
                await ConfirmMarketplaceBookingPaymentAsync(accountingInvoiceLink.LocalEntityId, cancellationToken);
                break;
            case AccountingEntityTypeConstants.RecurringBooking:
                await ConfirmRecurringBookingPaymentAsync(accountingInvoiceLink.LocalEntityId, cancellationToken);
                break;
        }
    }

    private async Task ConfirmMarketplaceBookingPaymentAsync(string marketplaceBookingId, CancellationToken cancellationToken)
    {
        var marketplaceBooking = await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(marketplaceBookingId, cancellationToken);
        if (marketplaceBooking is null || marketplaceBooking.BookingId is null)
        {
            return;
        }

        if (marketplaceBooking.PaymentStatus == PaymentStatusConstants.Confirmed)
        {
            return;
        }

        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(marketplaceBooking.BookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        marketplaceBooking.PaymentStatus = PaymentStatusConstants.Confirmed;
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        temporalOutboxService.SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
            marketplaceBooking.BookingId,
            new SetPaymentStatusArgs(PaymentStatusConstants.Confirmed),
            repositoryFactory.UnitOfWork);
        bookingOutboxPublisher.PublishBookings([entityMapper.MapTo(booking)], repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, marketplaceBooking.BookingId, cancellationToken);
    }

    private async Task ConfirmRecurringBookingPaymentAsync(string recurringBookingId, CancellationToken cancellationToken)
    {
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBookingId, cancellationToken);
        if (recurringBooking?.MarketplaceBooking is null)
        {
            return;
        }

        if (recurringBooking.MarketplaceBooking.PaymentStatus != PaymentStatusConstants.Confirmed)
        {
            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
            recurringBooking.MarketplaceBooking.PaymentStatus = PaymentStatusConstants.Confirmed;
            repositoryFactory.MarketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking);
            temporalOutboxService.SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
                recurringBooking.Id,
                new SetPaymentStatusArgs(PaymentStatusConstants.Confirmed),
                repositoryFactory.UnitOfWork);

            var relatedBookingsToPublish = await repositoryFactory.BookingRepository.GetByRecurringBookingIdUntrackedAsync(
                recurringBooking.Id,
                recurringBooking.StartDate,
                null,
                cancellationToken);
            bookingOutboxPublisher.PublishBookings(relatedBookingsToPublish.Select(entityMapper.MapTo).ToList(), repositoryFactory.UnitOfWork);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        else
        {
            await temporalService.SignalPayRecurringBookingViaBankTransferWorkflowAsync(
                recurringBooking.Id,
                new SetPaymentStatusArgs(PaymentStatusConstants.Confirmed),
                cancellationToken);
        }

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        var relatedBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdUntrackedAsync(
            recurringBooking.Id,
            recurringBooking.StartDate,
            null,
            cancellationToken);
        foreach (var booking in relatedBookings)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
        }
    }

    private async Task UpdateMarketplaceInvoiceReferencesAsync(
        MarketplaceBooking marketplaceBooking,
        AccountingInvoiceExportLink accountingInvoiceLink,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceInstance = await repositoryFactory.AccountingInvoiceInstanceRepository.GetLatestByAccountingInvoiceExportLinkIdAsync(
            accountingInvoiceLink.Id,
            cancellationToken);
        var invoiceNumber = accountingInvoiceInstance?.ExternalInvoiceNumber ?? accountingInvoiceLink.ExternalInvoiceNumber;
        var invoiceUrl = accountingInvoiceInstance?.ExternalInvoiceUrl ?? accountingInvoiceLink.ExternalInvoiceUrl;

        if (!string.IsNullOrWhiteSpace(invoiceNumber))
        {
            marketplaceBooking.InvoiceNumber = invoiceNumber;
        }

        if (!string.IsNullOrWhiteSpace(invoiceUrl))
        {
            marketplaceBooking.InvoiceUrl = invoiceUrl;
        }

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task PropagateInvoiceReferencesAsync(
        string localEntityType,
        string localEntityId,
        AccountingInvoiceExportLink accountingInvoiceLink,
        CancellationToken cancellationToken)
    {
        if (localEntityType == AccountingEntityTypeConstants.MarketplaceBooking)
        {
            var marketplaceBooking = await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(localEntityId, cancellationToken);
            if (marketplaceBooking is null || string.IsNullOrWhiteSpace(marketplaceBooking.BookingId))
            {
                return;
            }

            await UpdateMarketplaceInvoiceReferencesAsync(marketplaceBooking, accountingInvoiceLink, cancellationToken);
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, marketplaceBooking.BookingId, cancellationToken);
            return;
        }

        if (localEntityType != AccountingEntityTypeConstants.RecurringBooking)
        {
            return;
        }

        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(localEntityId, cancellationToken);
        if (recurringBooking?.MarketplaceBooking is null)
        {
            return;
        }

        await UpdateMarketplaceInvoiceReferencesAsync(recurringBooking.MarketplaceBooking, accountingInvoiceLink, cancellationToken);

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        var relatedBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdUntrackedAsync(
            recurringBooking.Id,
            recurringBooking.StartDate,
            null,
            cancellationToken);
        foreach (var booking in relatedBookings)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
        }
    }

    private static string BuildReference(string invoiceNumber, XeroConnection xeroConnection) =>
        string.IsNullOrWhiteSpace(xeroConnection.DefaultReferencePrefix)
            ? invoiceNumber
            : $"{xeroConnection.DefaultReferencePrefix}-{invoiceNumber}";

    private static string GetAccountingStatus(XeroInvoice invoice)
    {
        if (invoice.Status == XeroInvoice.StatusEnum.PAID || (invoice.AmountPaid ?? 0) >= (invoice.AmountDue ?? decimal.MaxValue))
        {
            return AccountingStatusConstants.Paid;
        }

        return invoice.Status switch
        {
            XeroInvoice.StatusEnum.AUTHORISED => AccountingStatusConstants.Sent,
            XeroInvoice.StatusEnum.SUBMITTED => AccountingStatusConstants.Sent,
            _ => AccountingStatusConstants.Exported
        };
    }

    private static string GetAccountingStatus(RepeatingInvoice invoice) =>
        invoice.Status switch
        {
            RepeatingInvoice.StatusEnum.AUTHORISED => AccountingStatusConstants.Sent,
            RepeatingInvoice.StatusEnum.DELETED => AccountingStatusConstants.Cancelled,
            _ => AccountingStatusConstants.Exported
        };

    protected virtual Task<Invoices> GetInvoiceAsync(
        AccountingApi accountingApi,
        string accessToken,
        string tenantId,
        Guid invoiceId,
        CancellationToken cancellationToken) =>
        accountingApi.GetInvoiceAsync(accessToken, tenantId, invoiceId, null, cancellationToken);
}
