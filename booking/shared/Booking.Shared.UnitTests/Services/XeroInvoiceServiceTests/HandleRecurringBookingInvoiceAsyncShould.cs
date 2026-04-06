using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using FakeItEasy;
using Grpc.Core;
using Xero.NetStandard.OAuth2.Model.Accounting;
using AccountingInvoiceExportConfigurationStateConstants = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants;
using AccountingInvoiceExportModeConstants = Booking.Shared.Models.AccountingInvoiceExportModeConstants;
using OrganizationBillingCycleModel = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Shared.UnitTests.Services.XeroInvoiceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class HandleRecurringBookingInvoiceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Freeze_Existing_Live_Repeating_Invoice_When_Current_Cadence_Differs(
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IUnitOfWork unitOfWork,
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
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string recurringBookingId,
        string productVersionId,
        string pricingId,
        string organizationId,
        string existingTemplateId)
    {
        organizationConfiguration.ApiKey = "api-key";
        var sut = new XeroInvoiceService(
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
            timeProvider);

        var cancellationToken = TestContext.Current.CancellationToken;
        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                InvoiceNumber = "INV-001",
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersion { Id = productVersionId }
            }
        };
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        var productVersion = new ProductVersion
        {
            Id = productVersionId,
            Product = new Product
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId, BillingCycle = OrganizationBillingCycleModel.Weekly.ToOrganizationBillingCycle()
                }
            }
        };
        var existingLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            OrganizationId = organizationId,
            ExternalInvoiceId = existingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = Models.XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            RepeatingScheduleUnit = nameof(Schedule.UnitEnum.MONTHLY),
            RepeatingSchedulePeriod = 3
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            IsActive = true,
            HasRefreshToken = true,
            TenantId = "tenant-1",
            BillingMode = XeroBillingModeConstants.RepeatingInvoices
        };
        var desiredSchedule = new XeroRepeatingInvoiceScheduleDefinition(
            Models.XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            Schedule.UnitEnum.MONTHLY,
            1,
            100m);
        var transitionDecision = new XeroRecurringInvoiceTransitionDecision(
            XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
            AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
            "Existing recurring Xero repeating invoice schedule differs from the current settings and requires manual migration.");

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(existingLink);
        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(
                recurringBooking,
                marketplaceBooking,
                OrganizationBillingCycleModel.Weekly))
            .Returns(desiredSchedule);
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(existingLink, true, desiredSchedule))
            .Returns(transitionDecision);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item =>
                        item.Key == Enterprise.Shared.Grpc.Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleRecurringBookingInvoiceAsync(
            organizationId,
            recurringBooking,
            marketplaceBooking,
            productVersion,
            cancellationToken);

        result.ShouldBe(RecurringInvoiceHandlingDisposition.StopAndPublish);
        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(recurringBooking, marketplaceBooking, OrganizationBillingCycleModel.Weekly))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(existingLink, true, desiredSchedule)).MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceLinkRepository.Update(A<AccountingInvoiceExportLink>.That.Matches(link =>
                link.Id == existingLink.Id &&
                link.ExportConfigurationState == AccountingInvoiceExportConfigurationStateConstants.TransitionRequired &&
                !string.IsNullOrWhiteSpace(link.ExportConfigurationMessage))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Freeze_Existing_Live_Repeating_Invoice_When_Repeating_Mode_Is_Turned_Off(
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IUnitOfWork unitOfWork,
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
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string recurringBookingId,
        string productVersionId,
        string pricingId,
        string organizationId,
        string existingTemplateId)
    {
        organizationConfiguration.ApiKey = "api-key";
        var sut = new XeroInvoiceService(
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
            timeProvider);

        var cancellationToken = TestContext.Current.CancellationToken;
        var recurringBooking = new RecurringBooking
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                InvoiceNumber = "INV-001",
                BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersion { Id = productVersionId }
            }
        };
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        var productVersion = new ProductVersion
        {
            Id = productVersionId,
            Product = new Product
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId, BillingCycle = OrganizationBillingCycleModel.Monthly.ToOrganizationBillingCycle()
                }
            }
        };
        var existingLink = new AccountingInvoiceExportLink
        {
            Id = "link-2",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            OrganizationId = organizationId,
            ExternalInvoiceId = existingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = Models.XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
            RepeatingScheduleUnit = nameof(Schedule.UnitEnum.MONTHLY),
            RepeatingSchedulePeriod = 1
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-2",
            IsActive = true,
            HasRefreshToken = true,
            TenantId = "tenant-2",
            BillingMode = XeroBillingModeConstants.Enabled
        };
        var transitionDecision = new XeroRecurringInvoiceTransitionDecision(
            XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
            AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
            "Existing recurring Xero repeating invoice remains active until it is migrated manually.");

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(existingLink);
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(existingLink, false, null))
            .Returns(transitionDecision);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item =>
                        item.Key == Enterprise.Shared.Grpc.Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);

        var result = await sut.HandleRecurringBookingInvoiceAsync(
            organizationId,
            recurringBooking,
            marketplaceBooking,
            productVersion,
            cancellationToken);

        result.ShouldBe(RecurringInvoiceHandlingDisposition.StopAndPublish);
        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(
                A<RecurringBooking>._,
                A<MarketplaceBooking>._,
                A<OrganizationBillingCycleModel>._))
            .MustNotHaveHappened();
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(existingLink, false, null)).MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceLinkRepository.Update(A<AccountingInvoiceExportLink>.That.Matches(link =>
                link.Id == existingLink.Id &&
                link.ExportConfigurationState == AccountingInvoiceExportConfigurationStateConstants.TransitionRequired &&
                !string.IsNullOrWhiteSpace(link.ExportConfigurationMessage))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<XeroConnection> CreateResponse(XeroConnection response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
}
