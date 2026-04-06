using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using FakeItEasy;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Storage;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using AccountingInvoiceExportModeConstants = Booking.Shared.Models.AccountingInvoiceExportModeConstants;
using Constants = Enterprise.Shared.Grpc.Constants;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

namespace Booking.Shared.UnitTests.Services.XeroInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SyncAccountingInvoiceStateAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Sync_Repeating_Invoice_Using_Concrete_Invoice_Hint_Without_Replacing_Template_Id(
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IAccountingPaymentEventRepository accountingPaymentEventRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IUnitOfWork unitOfWork,
        CallInvoker callInvoker,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        IDbTransactionBuilder transactionBuilder,
        IGraphQlTopicEventSender graphQlTopicEventSender,
        ITemporalService temporalService,
        ITemporalOutboxService temporalOutboxService,
        IBookingOutboxPublisher bookingOutboxPublisher,
        IMapper mapper,
        IRandomHelper randomHelper,
        IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
        IXeroRepeatingInvoiceScheduleService xeroRepeatingInvoiceScheduleService,
        IXeroRecurringInvoiceTransitionService xeroRecurringInvoiceTransitionService,
        IInvoicePaymentTermsService invoicePaymentTermsService,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        const string organizationId = "org-1";
        const string recurringBookingId = "recurring-booking-1";
        const string repeatingTemplateId = "b6c885cc-4e18-46e9-aa53-2d901c77e5ff";
        const string generatedInvoiceId = "f6a7c629-af16-4f01-a172-35ce3735f343";
        const string tenantId = "d2576a27-6a2b-4575-8720-0c11eee06fe5";
        var invoice = new Invoice
        {
            InvoiceID = Guid.Parse(generatedInvoiceId), InvoiceNumber = "SKD-000202", Status = Invoice.StatusEnum.AUTHORISED
        };
        var sut = new TestableXeroInvoiceService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            transactionBuilder,
            graphQlTopicEventSender,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            temporalService,
            temporalOutboxService,
            bookingOutboxPublisher,
            mapper,
            randomHelper,
            recurringInvoiceBillingScheduleService,
            xeroRepeatingInvoiceScheduleService,
            xeroRecurringInvoiceTransitionService,
            invoicePaymentTermsService,
            TimeProvider.System) { InvoiceResponse = new Invoices { _Invoices = [invoice] } };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = organizationId,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = repeatingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = tenantId,
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.AddMinutes(30))
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => repositoryFactory.AccountingPaymentEventRepository).Returns(accountingPaymentEventRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(A.Fake<AccountingApi>());
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByProviderAndExternalInvoiceIdAsync(
                AccountingProviderConstants.Xero,
                generatedInvoiceId,
                cancellationToken))
            .Returns((AccountingInvoiceInstance?)null);
        A.CallTo(() => accountingPaymentEventRepository.GetUnprocessedByProviderAndExternalInvoiceIdAsync(
                organizationId,
                AccountingProviderConstants.Xero,
                repeatingTemplateId,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, cancellationToken)).Returns((RecurringBooking?)null);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.SyncAccountingInvoiceStateAsync(
            new SyncAccountingInvoiceStateInput(
                organizationId,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                generatedInvoiceId),
            cancellationToken);

        result.IsTerminal.ShouldBeFalse();
        accountingInvoiceLink.ExternalInvoiceId.ShouldBe(repeatingTemplateId);
        accountingInvoiceLink.ExternalInvoiceNumber.ShouldBeNull();
        accountingInvoiceLink.ExternalInvoiceMode.ShouldBe(AccountingInvoiceExportModeConstants.RepeatingInvoice);
        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Exported);
        A.CallTo(() => accountingInvoiceInstanceRepository.Add(
                A<AccountingInvoiceInstance>.That.Matches(instance =>
                    instance.AccountingInvoiceExportLinkId == accountingInvoiceLink.Id &&
                    instance.Provider == AccountingProviderConstants.Xero &&
                    instance.ExternalInvoiceId == generatedInvoiceId &&
                    instance.ExternalInvoiceNumber == "SKD-000202" &&
                    instance.ExternalStatus == AccountingStatusConstants.Sent)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Confirm_Recurring_Booking_When_Concrete_Repeating_Invoice_Is_Paid(
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IAccountingPaymentEventRepository accountingPaymentEventRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        CallInvoker callInvoker,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        ITemporalService temporalService,
        IBookingOutboxPublisher bookingOutboxPublisher,
        IMapper mapper,
        IRandomHelper randomHelper,
        IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
        IXeroRepeatingInvoiceScheduleService xeroRepeatingInvoiceScheduleService,
        IXeroRecurringInvoiceTransitionService xeroRecurringInvoiceTransitionService,
        IInvoicePaymentTermsService invoicePaymentTermsService,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        const string organizationId = "org-1";
        const string recurringBookingId = "recurring-booking-1";
        const string repeatingTemplateId = "b6c885cc-4e18-46e9-aa53-2d901c77e5ff";
        const string generatedInvoiceId = "f6a7c629-af16-4f01-a172-35ce3735f343";
        const string tenantId = "d2576a27-6a2b-4575-8720-0c11eee06fe5";
        var invoice = new Invoice { InvoiceID = Guid.Parse(generatedInvoiceId), InvoiceNumber = "SKD-000202", Status = Invoice.StatusEnum.PAID };
        var transaction = A.Fake<IDbContextTransaction>();
        var marketplaceBookingSubscription = new MarketplaceBookingSubscription { Id = "subscription-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = marketplaceBookingSubscription,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
                ProductVersion = new ProductVersion { Id = "pv-1" }
            }
        };
        var sut = new TestableXeroInvoiceService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            transactionBuilder,
            graphQlTopicEventSender,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            temporalService,
            temporalOutboxService,
            bookingOutboxPublisher,
            mapper,
            randomHelper,
            recurringInvoiceBillingScheduleService,
            xeroRepeatingInvoiceScheduleService,
            xeroRecurringInvoiceTransitionService,
            invoicePaymentTermsService,
            TimeProvider.System) { InvoiceResponse = new Invoices { _Invoices = [invoice] } };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = organizationId,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = repeatingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = tenantId,
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.AddMinutes(30))
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => repositoryFactory.AccountingPaymentEventRepository).Returns(accountingPaymentEventRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(A.Fake<AccountingApi>());
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByProviderAndExternalInvoiceIdAsync(
                AccountingProviderConstants.Xero,
                generatedInvoiceId,
                cancellationToken))
            .Returns((AccountingInvoiceInstance?)null);
        A.CallTo(() => accountingPaymentEventRepository.GetUnprocessedByProviderAndExternalInvoiceIdAsync(
                organizationId,
                AccountingProviderConstants.Xero,
                generatedInvoiceId,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, cancellationToken)).Returns(recurringBooking);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                recurringBooking.StartDate,
                null,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.SyncAccountingInvoiceStateAsync(
            new SyncAccountingInvoiceStateInput(
                organizationId,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                generatedInvoiceId),
            cancellationToken);

        result.IsTerminal.ShouldBeTrue();
        recurringBooking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatus.Confirmed.ToPaymentStatus());
        A.CallTo(() => marketplaceBookingRepository.Update(A<MarketplaceBooking>.That.Matches(item =>
                item.Id == recurringBooking.MarketplaceBooking.Id &&
                item.PaymentStatus == PaymentStatus.Confirmed.ToPaymentStatus())))
            .MustHaveHappened();
        A.CallTo(() => temporalOutboxService.SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
                recurringBookingId,
                A<SetPaymentStatusArgs>.That.Matches(args => args.PaymentStatus == PaymentStatus.Confirmed.ToPaymentStatus()),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<XeroConnection> CreateResponse(XeroConnection response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private sealed class TestableXeroInvoiceService(
        OrganizationConfiguration organizationConfiguration,
        OrganizationService.OrganizationServiceClient organizationServiceClient,
        IRepositoryFactory repositoryFactory,
        IDbTransactionBuilder transactionBuilder,
        IGraphQlTopicEventSender graphQlTopicEventSender,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ITemporalService temporalService,
        ITemporalOutboxService temporalOutboxService,
        IBookingOutboxPublisher bookingOutboxPublisher,
        IMapper mapper,
        IRandomHelper randomHelper,
        IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
        IXeroRepeatingInvoiceScheduleService xeroRepeatingInvoiceScheduleService,
        IXeroRecurringInvoiceTransitionService xeroRecurringInvoiceTransitionService,
        IInvoicePaymentTermsService invoicePaymentTermsService,
        TimeProvider timeProvider)
        : XeroInvoiceService(
            organizationConfiguration,
            organizationServiceClient,
            repositoryFactory,
            transactionBuilder,
            graphQlTopicEventSender,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            temporalService,
            temporalOutboxService,
            bookingOutboxPublisher,
            mapper,
            randomHelper,
            recurringInvoiceBillingScheduleService,
            xeroRepeatingInvoiceScheduleService,
            xeroRecurringInvoiceTransitionService,
            invoicePaymentTermsService,
            timeProvider)
    {
        public Invoices? InvoiceResponse { get; init; }

        protected override Task<Invoices> GetInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid invoiceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(InvoiceResponse ?? new Invoices());
    }
}
