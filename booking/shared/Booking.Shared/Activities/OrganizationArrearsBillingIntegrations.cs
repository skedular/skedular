using System.Globalization;
using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Configurations;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using QuestPDF.Fluent;
using Temporalio.Activities;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using XeroOAuth2Token = Xero.NetStandard.OAuth2.Token.XeroOAuth2Token;
using Constants = Booking.Shared.GraphQL.Constants;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationArrearsInvoice = Booking.Shared.Database.Entities.OrganizationArrearsInvoice;
using OrganizationArrearsInvoiceLine = Booking.Shared.Database.Entities.OrganizationArrearsInvoiceLine;
using OrganizationBillingCycle = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using XeroInvoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;

namespace Booking.Shared.Activities;

public record GenerateOrganizationArrearsInvoicesInput(
    string OrganizationId,
    BillingPeriod BillingPeriod,
    OrganizationBillingCycle BillingCycle);

public record GetOrganizationArrearsBillingNextRunAtInput(OrganizationArrearsBillingConfiguration Configuration);

public record GetOrganizationArrearsBillingPeriodInput(
    DateTimeOffset ScheduledRunAt,
    bool RunNowRequested,
    OrganizationArrearsBillingConfiguration Configuration);

public record SyncOrganizationArrearsInvoiceAccountingStateInput(string OrganizationId, string OrganizationArrearsInvoiceId);

public record SyncOrganizationArrearsInvoiceAccountingStateResult(bool IsTerminal, DateTimeOffset? NextSyncAt);

public class OrganizationArrearsBillingIntegrations(
    EmailConfiguration emailConfiguration,
    CoreConfiguration coreConfiguration,
    OrganizationConfiguration organizationConfiguration,
    IXeroSdkClientFactory xeroSdkClientFactory,
    CoreService.CoreServiceClient coreServiceClient,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
    IOrganizationArrearsInvoiceService organizationArrearsInvoiceService,
    ITemporalService temporalService,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationInvoiceCounterService organizationInvoiceCounterService,
    IEmailService emailService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    IRandomHelper randomHelper,
    IInvoicePaymentTermsService invoicePaymentTermsService,
    TimeProvider timeProvider)
{
    [Activity]
    public Task<DateTimeOffset> GetNextRunAtAsync(GetOrganizationArrearsBillingNextRunAtInput args)
    {
        var now = timeProvider.GetUtcNow();

        return Task.FromResult(args.Configuration.BillingCycle switch
        {
            OrganizationBillingCycle.Weekly => GetNextWeeklyBoundary(now),
            OrganizationBillingCycle.Fortnightly => GetNextFortnightlyBoundary(now),
            OrganizationBillingCycle.Monthly => GetNextMonthlyBoundary(now),
            _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
        });
    }

    [Activity]
    public Task<BillingPeriod> GetBillingPeriodForRunAtAsync(GetOrganizationArrearsBillingPeriodInput args)
    {
        var effectiveRunAt = args.RunNowRequested ? timeProvider.GetUtcNow() : args.ScheduledRunAt;

        if (args.RunNowRequested)
        {
            return Task.FromResult(args.Configuration.BillingCycle switch
            {
                OrganizationBillingCycle.Weekly => new BillingPeriod(GetCurrentWeekStart(effectiveRunAt), effectiveRunAt),
                OrganizationBillingCycle.Fortnightly => new BillingPeriod(GetCurrentFortnightStart(effectiveRunAt), effectiveRunAt),
                OrganizationBillingCycle.Monthly => new BillingPeriod(GetCurrentMonthStart(effectiveRunAt), effectiveRunAt),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
            });
        }

        return Task.FromResult(args.Configuration.BillingCycle switch
        {
            OrganizationBillingCycle.Weekly => new BillingPeriod(effectiveRunAt.AddDays(-7), effectiveRunAt),
            OrganizationBillingCycle.Fortnightly => new BillingPeriod(effectiveRunAt.AddDays(-14), effectiveRunAt),
            OrganizationBillingCycle.Monthly => new BillingPeriod(effectiveRunAt.AddMonths(-1), effectiveRunAt),
            _ => throw new ArgumentOutOfRangeException(nameof(args.Configuration.BillingCycle))
        });
    }

    [Activity]
    public async Task GenerateOrganizationArrearsInvoicesAsync(
        GenerateOrganizationArrearsInvoicesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var bookings = await repositoryFactory.BookingRepository.GetInArrearsByOrganizationBeforeAsync(
            args.OrganizationId,
            args.BillingPeriod.StartInclusive,
            args.BillingPeriod.EndExclusive,
            cancellationToken);
        if (bookings.Count == 0)
        {
            return;
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               args.OrganizationId,
                               null,
                               false,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var persistedProcessedSegmentKeys = await repositoryFactory.OrganizationArrearsInvoiceRepository.GetProcessedSegmentKeysAsync(
            args.OrganizationId,
            args.BillingPeriod.StartInclusive,
            args.BillingPeriod.EndExclusive,
            cancellationToken);

        var bookingModels = bookings.Select(mapper.MapTo).ToList();
        var drafts = organizationArrearsBillingPlannerService.BuildInvoiceDrafts(
            args.BillingPeriod,
            args.BillingCycle,
            bookingModels,
            persistedProcessedSegmentKeys);
        if (drafts.Count == 0)
        {
            return;
        }

        foreach (var draft in drafts)
        {
            // Invoice generation is grouped per customer so one billing-cycle email can cover all
            // arrears bookings earned in the billing period for that customer.
            var draftBookingIds = draft.Lines.Select(line => line.BookingId).ToHashSet();
            var draftBookingModels = bookingModels
                .Where(booking => draftBookingIds.Contains(booking.Id))
                .ToList();
            var recipients = draftBookingModels
                .SelectMany(booking => booking.MarketplaceBooking?.InvoiceEmailList ?? [])
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var invoiceNumber = await organizationInvoiceCounterService.GetNextInvoiceNumberIdAsync(args.OrganizationId, cancellationToken);
            var xeroConnection = await GetOrganizationXeroConnectionAsync(args.OrganizationId, cancellationToken);
            var isXeroManaged = IsXeroManagedForArrears(xeroConnection);
            var requiresLocalPdf = !isXeroManaged || !(xeroConnection?.SendInvoicesViaXero ?? false);

            MemoryStream? pdfStream = null;
            var invoiceUrl = string.Empty;
            if (requiresLocalPdf)
            {
                var invoiceDocument = organizationArrearsInvoiceService.GenerateInvoice(organization, draft, invoiceNumber);
                pdfStream = new MemoryStream();
                invoiceDocument.GeneratePdf(pdfStream);
                pdfStream.Seek(0, SeekOrigin.Begin);
                invoiceUrl = await UploadInvoicePdfAsync(pdfStream, cancellationToken);
            }

            var persistedInvoice = await PersistArrearsInvoiceAndAttachToBookingsAsync(
                draft,
                draft.Lines.Select(line => line.BookingId).Distinct().ToList(),
                invoiceNumber,
                invoiceUrl,
                cancellationToken);

            if (isXeroManaged)
            {
                await ExportOrganizationArrearsInvoiceToXeroAsync(
                    args.OrganizationId,
                    xeroConnection!,
                    draft,
                    persistedInvoice,
                    draft.Lines.Select(line => line.BookingId).Distinct().ToList(),
                    cancellationToken);
            }

            if (!isXeroManaged || !xeroConnection!.SendInvoicesViaXero)
            {
                if (recipients.Count != 0)
                {
                    ArgumentNullException.ThrowIfNull(pdfStream);
                    await SendInvoiceEmailAsync(recipients, organization, draft, invoiceNumber, pdfStream, cancellationToken);
                }
            }

            foreach (var line in draft.Lines)
            {
                await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, line.BookingId, cancellationToken);
            }
        }
    }

    [Activity]
    public async Task<SyncOrganizationArrearsInvoiceAccountingStateResult> SyncOrganizationArrearsInvoiceAccountingStateAsync(
        SyncOrganizationArrearsInvoiceAccountingStateInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var accountingInvoiceLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.OrganizationArrearsInvoice,
            input.OrganizationArrearsInvoiceId,
            cancellationToken);
        if (accountingInvoiceLink is null ||
            string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId) ||
            accountingInvoiceLink.ExternalStatus is AccountingStatusConstants.Paid or AccountingStatusConstants.Failed)
        {
            return new SyncOrganizationArrearsInvoiceAccountingStateResult(true, null);
        }

        var xeroConnection = await GetOrganizationXeroConnectionAsync(input.OrganizationId, cancellationToken);
        if (xeroConnection is null || !xeroConnection.IsActive)
        {
            accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Failed;
            accountingInvoiceLink.LastError = xeroConnection?.LastError ?? "Xero connection is not active.";
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return new SyncOrganizationArrearsInvoiceAccountingStateResult(true, null);
        }

        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(input.OrganizationId, xeroConnection, cancellationToken);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        var invoiceResponse = await accountingApi.GetInvoiceAsync(
            accessToken,
            refreshedConnection.TenantId,
            Guid.Parse(accountingInvoiceLink.ExternalInvoiceId),
            null,
            cancellationToken);
        var invoice = invoiceResponse?._Invoices?.FirstOrDefault();
        if (invoice is null)
        {
            accountingInvoiceLink.ExternalStatus = AccountingStatusConstants.Failed;
            accountingInvoiceLink.LastError = "Xero invoice could not be loaded.";
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            return new SyncOrganizationArrearsInvoiceAccountingStateResult(true, null);
        }

        await ApplyXeroInvoiceSyncAsync(input.OrganizationId, accountingInvoiceLink, invoice, refreshedConnection, cancellationToken);
        await PropagateInvoiceReferencesAsync(accountingInvoiceLink, cancellationToken);

        var isPaid = string.Equals(accountingInvoiceLink.ExternalStatus, AccountingStatusConstants.Paid, StringComparison.Ordinal);
        return new SyncOrganizationArrearsInvoiceAccountingStateResult(
            isPaid,
            isPaid ? null : timeProvider.GetUtcNow().AddHours(12));
    }

    private static DateTimeOffset GetNextWeeklyBoundary(DateTimeOffset now)
    {
        var nextBoundary = GetCurrentWeekStart(now);
        if (nextBoundary <= now)
        {
            nextBoundary = nextBoundary.AddDays(7);
        }

        return nextBoundary;
    }

    private static DateTimeOffset GetNextFortnightlyBoundary(DateTimeOffset now)
    {
        var nextBoundary = GetCurrentFortnightStart(now);
        if (nextBoundary <= now)
        {
            nextBoundary = nextBoundary.AddDays(14);
        }

        return nextBoundary;
    }

    private static DateTimeOffset GetNextMonthlyBoundary(DateTimeOffset now)
    {
        var currentMonthStart = GetCurrentMonthStart(now);
        return currentMonthStart <= now ? currentMonthStart.AddMonths(1) : currentMonthStart;
    }

    private static DateTimeOffset GetCurrentMonthStart(DateTimeOffset value) =>
        new(value.Year, value.Month, 1, 0, 0, 0, value.Offset);

    private static DateTimeOffset GetCurrentWeekStart(DateTimeOffset value)
    {
        var daysSinceMonday = ((int)value.DayOfWeek + 6) % 7;
        return new DateTimeOffset(value.Year, value.Month, value.Day, 0, 0, 0, value.Offset).AddDays(-daysSinceMonday);
    }

    private static DateTimeOffset GetCurrentFortnightStart(DateTimeOffset value)
    {
        var weekStart = GetCurrentWeekStart(value);
        // Fortnightly billing uses alternating Mondays from a fixed system baseline so every
        // organization shares the same billing weeks.
        var baseMonday = new DateTimeOffset(1970, 1, 5, 0, 0, 0, value.Offset);
        var weeksSinceBase = (int)((weekStart - baseMonday).TotalDays / 7);
        return weeksSinceBase % 2 == 0 ? weekStart : weekStart.AddDays(-7);
    }

    private async Task<OrganizationArrearsInvoice> PersistArrearsInvoiceAndAttachToBookingsAsync(
        ArrearsInvoiceDraft draft,
        List<string> bookingIds,
        string invoiceNumber,
        string invoiceUrl,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(draft.CustomerId, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var lineBookings = await repositoryFactory.BookingRepository.GetByIdsMinimalAsync(
            draft.Lines.Select(item => item.BookingId).ToList(),
            cancellationToken);

        var organizationArrearsInvoice = repositoryFactory.OrganizationArrearsInvoiceRepository.Add(
            new OrganizationArrearsInvoice
            {
                Id = randomHelper.Generate(),
                OrganizationId = draft.OrganizationId,
                Customer = customer,
                InvoiceNumber = invoiceNumber,
                InvoiceUrl = invoiceUrl,
                BillingPeriodStartInclusive = draft.BillingPeriod.StartInclusive,
                BillingPeriodEndExclusive = draft.BillingPeriod.EndExclusive,
                Currency = draft.Currency.ToCurrency(),
                TotalAmount = draft.TotalAmount,
                Lines = draft.Lines.Select(line => new OrganizationArrearsInvoiceLine
                {
                    Id = randomHelper.Generate(),
                    Booking = lineBookings.First(booking => booking.Id == line.BookingId),
                    SegmentKey = line.SegmentKey,
                    ServicePeriodStartInclusive = line.ServicePeriod.StartInclusive,
                    ServicePeriodEndExclusive = line.ServicePeriod.EndExclusive,
                    EarnedAt = line.EarnedAt,
                    Amount = line.Amount,
                    Description = line.Description
                }).ToList()
            });

        if (bookingIds.Count == 0)
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return organizationArrearsInvoice;
        }

        var bookings = await repositoryFactory.BookingRepository.GetByIdsWithValidMarketplaceAsync(bookingIds, cancellationToken);

        foreach (var booking in bookings)
        {
            // Keep the first invoice reference on the booking for backward compatibility.
            if (string.IsNullOrWhiteSpace(booking.MarketplaceBooking!.InvoiceNumber))
            {
                booking.MarketplaceBooking.InvoiceNumber = invoiceNumber;
            }

            if (string.IsNullOrWhiteSpace(booking.MarketplaceBooking.InvoiceUrl))
            {
                booking.MarketplaceBooking.InvoiceUrl = invoiceUrl;
            }

            repositoryFactory.MarketplaceBookingRepository.Update(booking.MarketplaceBooking);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationArrearsInvoice;
    }

    private async Task<XeroConnection?> GetOrganizationXeroConnectionAsync(string organizationId, CancellationToken cancellationToken)
    {
        var response = await organizationServiceClient.Admin_GetXeroConnectionAsync(
            new Admin_GetXeroConnectionInput { OrganizationId = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        return string.IsNullOrWhiteSpace(response.Id) ? null : response;
    }

    private async Task<Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> GetOrganizationAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

    private static bool IsXeroManagedForArrears(XeroConnection? xeroConnection) =>
        xeroConnection is { IsActive: true, HasRefreshToken: true } &&
        !string.IsNullOrWhiteSpace(xeroConnection.TenantId) &&
        xeroConnection.BillingMode is XeroBillingModeConstants.Enabled or XeroBillingModeConstants.RepeatingInvoices;

    private async Task ExportOrganizationArrearsInvoiceToXeroAsync(
        string organizationId,
        XeroConnection xeroConnection,
        ArrearsInvoiceDraft draft,
        OrganizationArrearsInvoice organizationArrearsInvoice,
        List<string> bookingIds,
        CancellationToken cancellationToken)
    {
        var accountingInvoiceLink =
            await UpsertPendingAccountingInvoiceExportLinkAsync(organizationId, organizationArrearsInvoice, cancellationToken);
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(organizationArrearsInvoice.CustomerId, false, cancellationToken) ??
                       throw new CustomerNotFound();
        var (accessToken, refreshedConnection) = await EnsureValidAccessTokenAsync(organizationId, xeroConnection, cancellationToken);
        var contact = await UpsertXeroContactAsync(organizationId, customer, refreshedConnection, accessToken, cancellationToken);
        var organization = await GetOrganizationAsync(organizationId, cancellationToken);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        if (!string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId))
        {
            var existingInvoiceResponse = await accountingApi.GetInvoiceAsync(
                accessToken,
                refreshedConnection.TenantId,
                Guid.Parse(accountingInvoiceLink.ExternalInvoiceId),
                null,
                cancellationToken);
            var existingInvoice = existingInvoiceResponse?._Invoices?.FirstOrDefault();
            if (existingInvoice is not null)
            {
                await ApplyXeroInvoiceSyncAsync(organizationId, accountingInvoiceLink, existingInvoice, refreshedConnection, cancellationToken);
                await UpdatePersistedInvoiceReferencesAsync(organizationArrearsInvoice, bookingIds, accountingInvoiceLink, cancellationToken);
                return;
            }
        }

        var isTaxInclusive = await ResolveArrearsTaxInclusiveAsync(bookingIds, cancellationToken);
        var invoiceRequest = BuildXeroInvoice(
            organizationArrearsInvoice,
            draft,
            contact,
            refreshedConnection,
            isTaxInclusive,
            accountingInvoiceLink.ExternalInvoiceId,
            invoicePaymentTermsService.GetInvoiceDueInDays(organization.BillingDetails?.InvoiceDueInDays));
        var invoiceResponse = await accountingApi.CreateInvoicesAsync(
            accessToken,
            refreshedConnection.TenantId,
            new Invoices { _Invoices = [invoiceRequest] },
            null,
            null,
            accountingInvoiceLink.Id,
            cancellationToken);
        var exportedInvoice = invoiceResponse?._Invoices?.FirstOrDefault() ??
                              throw new XeroInvoiceExportFailedException();

        await ApplyXeroInvoiceSyncAsync(organizationId, accountingInvoiceLink, exportedInvoice, refreshedConnection, cancellationToken);

        if (refreshedConnection.SendInvoicesViaXero && exportedInvoice.InvoiceID.HasValue)
        {
            await TryEmailInvoiceAsync(
                accountingApi,
                accessToken,
                refreshedConnection,
                exportedInvoice.InvoiceID.Value,
                accountingInvoiceLink,
                cancellationToken);
        }

        await UpdatePersistedInvoiceReferencesAsync(organizationArrearsInvoice, bookingIds, accountingInvoiceLink, cancellationToken);

        if (refreshedConnection.AutoReconcilePayments &&
            !string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceId) &&
            accountingInvoiceLink.ExternalStatus is not AccountingStatusConstants.Paid)
        {
            await temporalService.StartWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(
                new MaintainOrganizationArrearsInvoiceAccountingStateInput(organizationId, organizationArrearsInvoice.Id),
                cancellationToken);
        }
    }

    private async Task<AccountingInvoiceExportLink> UpsertPendingAccountingInvoiceExportLinkAsync(
        string organizationId,
        OrganizationArrearsInvoice organizationArrearsInvoice,
        CancellationToken cancellationToken)
    {
        var existingLink = await repositoryFactory.AccountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.OrganizationArrearsInvoice,
            organizationArrearsInvoice.Id,
            cancellationToken);

        if (existingLink is null)
        {
            existingLink = repositoryFactory.AccountingInvoiceExportLinkRepository.Add(
                new AccountingInvoiceExportLink
                {
                    Id = randomHelper.Generate(),
                    Provider = AccountingProviderConstants.Xero,
                    LocalEntityType = AccountingEntityTypeConstants.OrganizationArrearsInvoice,
                    LocalEntityId = organizationArrearsInvoice.Id,
                    ExternalInvoiceId = null,
                    ExternalInvoiceNumber = null,
                    ExternalInvoiceUrl = null,
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

        var client = xeroSdkClientFactory.CreateClient();
        var refreshedToken = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(
            new XeroOAuth2Token { RefreshToken = xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted) });
        var now = timeProvider.GetUtcNow();
        var accessTokenEncrypted = xeroTokenEncryptionService.Encrypt(refreshedToken.AccessToken);
        var refreshTokenEncrypted = xeroTokenEncryptionService.Encrypt(
            string.IsNullOrWhiteSpace(refreshedToken.RefreshToken)
                ? xeroTokenEncryptionService.Decrypt(xeroConnection.RefreshTokenEncrypted)
                : refreshedToken.RefreshToken);
        var accessTokenExpiresAt = now.AddMinutes(30);
        var refreshTokenExpiresAt = now.AddDays(60);

        var refreshedConnection = await organizationServiceClient.Admin_RefreshXeroConnectionTokensAsync(
            new Admin_RefreshXeroConnectionTokensInput
            {
                OrganizationId = organizationId,
                AccessTokenEncrypted = accessTokenEncrypted,
                RefreshTokenEncrypted = refreshTokenEncrypted,
                AccessTokenExpiresAt = accessTokenExpiresAt.ToTimestamp(),
                RefreshTokenExpiresAt = refreshTokenExpiresAt.ToTimestamp()
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

        var xeroContactName = !string.IsNullOrWhiteSpace(email) ? $"{displayName} <{email}>" : $"{displayName} [{customer.Id}]";

        var existingLink = await repositoryFactory.AccountingContactLinkRepository.GetByProviderAndLocalEntityAsync(
            organizationId,
            AccountingProviderConstants.Xero,
            AccountingEntityTypeConstants.Customer,
            customer.Id,
            cancellationToken);
        var accountingApi = xeroSdkClientFactory.CreateAccountingApi();
        var contact = new Contact
        {
            Name = xeroContactName,
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
                xeroContactName,
                100,
                cancellationToken);
            var existingXeroContact =
                contactsByName._Contacts?.FirstOrDefault(item => string.Equals(item.Name, xeroContactName, StringComparison.OrdinalIgnoreCase));

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
            : await accountingApi.UpdateOrCreateContactsAsync(accessToken, xeroConnection.TenantId, new Contacts { _Contacts = [contact] }, null,
                null, cancellationToken);
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
                    LastSyncedAt = timeProvider.GetUtcNow(),
                    LastError = null,
                    OrganizationId = organizationId
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

    private static XeroInvoice BuildXeroInvoice(
        OrganizationArrearsInvoice organizationArrearsInvoice,
        ArrearsInvoiceDraft draft,
        Contact contact,
        XeroConnection xeroConnection,
        bool isTaxInclusive,
        string? externalInvoiceId,
        int invoiceDueInDays)
    {
        var invoiceDate = draft.BillingPeriod.EndExclusive.UtcDateTime.Date;
        var dueDate = invoiceDate.AddDays(invoiceDueInDays);

        return new XeroInvoice
        {
            InvoiceID = Guid.TryParse(externalInvoiceId, out var invoiceId) ? invoiceId : null,
            Type = XeroInvoice.TypeEnum.ACCREC,
            Status = xeroConnection.SendInvoicesViaXero ? XeroInvoice.StatusEnum.AUTHORISED : XeroInvoice.StatusEnum.DRAFT,
            LineAmountTypes = isTaxInclusive ? LineAmountTypes.Inclusive : LineAmountTypes.Exclusive,
            Contact = contact,
            InvoiceNumber = organizationArrearsInvoice.InvoiceNumber,
            Reference = BuildReference(organizationArrearsInvoice, xeroConnection),
            Date = invoiceDate,
            DueDate = dueDate,
            LineItems = draft.Lines.Select(line => new LineItem
            {
                Description = line.Description, Quantity = 1, UnitAmount = line.Amount, AccountCode = xeroConnection.DefaultSalesAccountCode
            }).ToList()
        };
    }

    private async Task<bool> ResolveArrearsTaxInclusiveAsync(IReadOnlyList<string> bookingIds, CancellationToken cancellationToken)
    {
        var bookings = await repositoryFactory.BookingRepository.GetByIdsWithValidMarketplaceAsync(bookingIds, cancellationToken);
        var taxInclusiveValues = bookings
            .Select(item => item.MarketplaceBooking!.ProductPricing.IsTaxInclusive)
            .Distinct()
            .ToList();

        return taxInclusiveValues.Count switch
        {
            0 => false,
            1 => taxInclusiveValues[0],
            _ => throw new MixedXeroInvoiceTaxInclusivityException()
        };
    }

    private async Task ApplyXeroInvoiceSyncAsync(
        string organizationId,
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        XeroConnection xeroConnection,
        CancellationToken cancellationToken)
    {
        accountingInvoiceLink.ExternalInvoiceId = invoice.InvoiceID?.ToString();
        accountingInvoiceLink.ExternalInvoiceNumber = invoice.InvoiceNumber;
        accountingInvoiceLink.ExternalInvoiceUrl = await GetOnlineInvoiceUrlAsync(organizationId, xeroConnection, invoice, cancellationToken);
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
        await UpsertAccountingPaymentEventsAsync(accountingInvoiceLink, invoice, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
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
            var onlineInvoice = await xeroSdkClientFactory
                .CreateAccountingApi()
                .GetOnlineInvoiceAsync(accessToken, refreshedConnection.TenantId, invoiceId, cancellationToken);

            return onlineInvoice?._OnlineInvoices?.FirstOrDefault()?.OnlineInvoiceUrl ?? invoice.Url;
        }
        catch
        {
            return invoice.Url;
        }
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
        catch (Exception ex)
        {
            accountingInvoiceLink.LastError = $"Xero invoice exported but email delivery failed: {ex.Message}";
            repositoryFactory.AccountingInvoiceExportLinkRepository.Update(accountingInvoiceLink);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task PropagateInvoiceReferencesAsync(AccountingInvoiceExportLink accountingInvoiceLink, CancellationToken cancellationToken)
    {
        if (accountingInvoiceLink.LocalEntityType != AccountingEntityTypeConstants.OrganizationArrearsInvoice)
        {
            return;
        }

        var organizationArrearsInvoice = await repositoryFactory.OrganizationArrearsInvoiceRepository
            .GetByIdWithLinesAsync(accountingInvoiceLink.LocalEntityId, cancellationToken);
        if (organizationArrearsInvoice is null)
        {
            return;
        }

        var bookingIds = organizationArrearsInvoice.Lines.Select(line => line.Booking.Id).Distinct().ToList();
        await UpdatePersistedInvoiceReferencesAsync(
            organizationArrearsInvoice,
            bookingIds,
            accountingInvoiceLink,
            cancellationToken);

        foreach (var bookingId in bookingIds)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, bookingId, cancellationToken);
        }
    }

    private async Task UpsertAccountingPaymentEventsAsync(
        AccountingInvoiceExportLink accountingInvoiceLink,
        XeroInvoice invoice,
        CancellationToken cancellationToken)
    {
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
                existingPaymentEvent.ProcessedAt = null;
                existingPaymentEvent.PayloadJson = $"{{\"amount\":{payment.Amount?.ToString(CultureInfo.InvariantCulture) ?? "0"}}}";
                repositoryFactory.AccountingPaymentEventRepository.Update(existingPaymentEvent);
                continue;
            }

            repositoryFactory.AccountingPaymentEventRepository.Add(
                new AccountingPaymentEvent
                {
                    Id = randomHelper.Generate(),
                    Provider = AccountingProviderConstants.Xero,
                    ExternalInvoiceId = accountingInvoiceLink.ExternalInvoiceId ?? string.Empty,
                    ExternalPaymentId = externalPaymentId,
                    ExternalStatus = payment.Status.ToString(),
                    OccurredAt = payment.Date ?? timeProvider.GetUtcNow(),
                    PayloadJson = $"{{\"amount\":{payment.Amount?.ToString(CultureInfo.InvariantCulture) ?? "0"}}}",
                    ProcessedAt = null,
                    OrganizationId = accountingInvoiceLink.OrganizationId
                });
        }
    }

    private async Task UpdatePersistedInvoiceReferencesAsync(
        OrganizationArrearsInvoice organizationArrearsInvoice,
        List<string> bookingIds,
        AccountingInvoiceExportLink accountingInvoiceLink,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceNumber))
        {
            organizationArrearsInvoice.InvoiceNumber = accountingInvoiceLink.ExternalInvoiceNumber;
        }

        if (!string.IsNullOrWhiteSpace(accountingInvoiceLink.ExternalInvoiceUrl))
        {
            organizationArrearsInvoice.InvoiceUrl = accountingInvoiceLink.ExternalInvoiceUrl;
        }

        if (bookingIds.Count != 0)
        {
            var bookings = await repositoryFactory.BookingRepository.GetByIdsWithValidMarketplaceAsync(bookingIds, cancellationToken);
            foreach (var booking in bookings)
            {
                booking.MarketplaceBooking!.InvoiceNumber = organizationArrearsInvoice.InvoiceNumber;
                booking.MarketplaceBooking.InvoiceUrl = organizationArrearsInvoice.InvoiceUrl;
                repositoryFactory.MarketplaceBookingRepository.Update(booking.MarketplaceBooking);
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

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

    private static string BuildReference(OrganizationArrearsInvoice organizationArrearsInvoice, XeroConnection xeroConnection) =>
        string.IsNullOrWhiteSpace(xeroConnection.DefaultReferencePrefix)
            ? organizationArrearsInvoice.InvoiceNumber
            : $"{xeroConnection.DefaultReferencePrefix}-{organizationArrearsInvoice.InvoiceNumber}";

    private async Task<string> UploadInvoicePdfAsync(MemoryStream pdfStream, CancellationToken cancellationToken)
    {
        pdfStream.Seek(0, SeekOrigin.Begin);

        using var call =
            coreServiceClient.Admin_UploadToPrivateStorage(coreConfiguration.ApiKey.CreateMetadata(), cancellationToken: cancellationToken);

        ArgumentNullException.ThrowIfNull(call);

        int bytesRead;
        var buffer = new byte[64 * 1024];
        while ((bytesRead = await pdfStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
        {
            await call.RequestStream.WriteAsync(
                new UploadFileRequest { Extension = ".pdf", ContentType = "application/pdf", Chunk = ByteString.CopyFrom(buffer, 0, bytesRead) },
                cancellationToken);
        }

        await call.RequestStream.CompleteAsync();
        var fileUploadResponse = await call.ResponseAsync;

        return fileUploadResponse.Original.Url;
    }

    private async Task SendInvoiceEmailAsync(
        List<string> recipients,
        Organization organization,
        ArrearsInvoiceDraft draft,
        string invoiceNumber,
        MemoryStream pdfStream,
        CancellationToken cancellationToken)
    {
        var organizationName = organization.Name ?? "Organization";

        await using var htmlTemplateStream = typeof(OrganizationArrearsBillingIntegrations).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.OrganizationArrearsInvoice.template.html");
        ArgumentNullException.ThrowIfNull(htmlTemplateStream);
        using var htmlReader = new StreamReader(htmlTemplateStream);
        var bodyHtml = await htmlReader.ReadToEndAsync(cancellationToken);

        await using var textTemplateStream = typeof(OrganizationArrearsBillingIntegrations).Assembly.GetManifestResourceStream(
            "Booking.Shared.EmailTemplates.OrganizationArrearsInvoice.template.txt");
        ArgumentNullException.ThrowIfNull(textTemplateStream);
        using var textReader = new StreamReader(textTemplateStream);
        var bodyText = await textReader.ReadToEndAsync(cancellationToken);

        bodyHtml = bodyHtml
            .Replace("{{COMPANY_NAME}}", organizationName)
            .Replace("{{INVOICE_NUMBER}}", invoiceNumber)
            .Replace("{{BILLING_PERIOD_START}}", draft.BillingPeriod.StartInclusive.ToString("yyyy-MM-dd"))
            .Replace("{{BILLING_PERIOD_END}}", draft.BillingPeriod.EndExclusive.ToString("yyyy-MM-dd"));

        bodyText = bodyText
            .Replace("{{COMPANY_NAME}}", organizationName)
            .Replace("{{INVOICE_NUMBER}}", invoiceNumber)
            .Replace("{{BILLING_PERIOD_START}}", draft.BillingPeriod.StartInclusive.ToString("yyyy-MM-dd"))
            .Replace("{{BILLING_PERIOD_END}}", draft.BillingPeriod.EndExclusive.ToString("yyyy-MM-dd"));

        var attachments = new List<EmailAttachment> { new(pdfStream, $"{invoiceNumber}.pdf", "application/pdf") };

        await emailService.SendRawEmailAsync(
            $"Invoice #{invoiceNumber} from {organizationName}",
            bodyText,
            bodyHtml,
            $"{organizationName} {emailConfiguration.BookingInvoiceEmailSender}",
            recipients,
            [],
            [],
            attachments,
            cancellationToken);
    }
}
