using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Activities;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using FakeItEasy;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Temporalio.Testing;
using Xero.NetStandard.OAuth2.Model.Accounting;
using AccountingInvoiceExportConfigurationStateConstants = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants;
using AccountingInvoiceExportModeConstants = Booking.Shared.Models.AccountingInvoiceExportModeConstants;
using AccountingInvoiceLinkEntity = Booking.Shared.Database.Entities.AccountingInvoiceLink;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using Constants = Booking.Shared.GraphQL.Constants;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationBillingCycleModel = Api.Shared.Services.Models.OrganizationBillingCycle;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using XeroRepeatingInvoiceScheduleSourceConstants = Booking.Shared.Models.XeroRepeatingInvoiceScheduleSourceConstants;
using CoreConfiguration = Api.Shared.Clients.Configurations.Grpc.CoreConfiguration;

namespace Booking.Shared.UnitTests.Activities.InvoiceIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GenerateAndSendRecurringInvoiceAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Freeze_Existing_Live_Repeating_Invoice_And_Publish_Changes_When_Current_Cadence_Differs(
        [Frozen] CoreConfiguration coreConfiguration,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IBookingInvoiceService bookingInvoiceService,
        EmailConfiguration emailConfiguration,
        CoreService.CoreServiceClient coreServiceClient,
        IDbTransactionBuilder transactionBuilder,
        IOrganizationInvoiceCounterService organizationInvoiceCounterService,
        IEmailService emailService,
        IHostEnvironment hostEnvironment,
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
        string relatedBookingId,
        string subscriptionId,
        string existingTemplateId)
    {
        var environment = new ActivityEnvironment();
        coreConfiguration.ApiKey = "api-key";
        organizationConfiguration.ApiKey = "api-key";
        var sut = new InvoiceIntegrations(
            emailConfiguration,
            coreConfiguration,
            organizationConfiguration,
            coreServiceClient,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            bookingInvoiceService,
            transactionBuilder,
            organizationInvoiceCounterService,
            emailService,
            hostEnvironment,
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

        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = new MarketplaceBookingSubscriptionEntity { Id = subscriptionId },
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                InvoiceNumber = "INV-001",
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersionEntity { Id = productVersionId }
            }
        };
        var productVersion = new ProductVersionEntity
        {
            Id = productVersionId,
            Product = new ProductEntity
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId, BillingCycle = OrganizationBillingCycleModel.Weekly.ToOrganizationBillingCycle()
                }
            }
        };
        var existingLink = new AccountingInvoiceLinkEntity
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            OrganizationId = organizationId,
            ExternalInvoiceId = existingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
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
            XeroRepeatingInvoiceScheduleSourceConstants.PurchaseCadence,
            Schedule.UnitEnum.MONTHLY,
            1,
            100m);
        var transitionDecision = new XeroRecurringInvoiceTransitionDecision(
            XeroRecurringInvoiceExportPath.FreezeExistingRepeatingInvoice,
            AccountingInvoiceExportConfigurationStateConstants.TransitionRequired,
            "Existing recurring Xero repeating invoice schedule differs from the current settings and requires manual migration.");
        var relatedBookings = new List<BookingEntity> { new() { Id = relatedBookingId } };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, environment.CancellationTokenSource.Token))
            .Returns(productVersion);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                environment.CancellationTokenSource.Token))
            .Returns(existingLink);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                recurringBooking.StartDate,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(relatedBookings);
        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                OrganizationBillingCycleModel.Weekly))
            .Returns(desiredSchedule);
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(
                existingLink,
                true,
                desiredSchedule))
            .Returns(transitionDecision);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == environment.CancellationTokenSource.Token &&
                    options.Headers != null &&
                    options.Headers.Any(item =>
                        item.Key == Enterprise.Shared.Grpc.Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).Returns(1);

        await environment.RunAsync(() =>
            sut.GenerateAndSendRecurringInvoiceAsync(new GenerateAndSendRecurringInvoiceInput(recurringBookingId, [])));

        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(
                recurringBooking,
                recurringBooking.MarketplaceBooking,
                OrganizationBillingCycleModel.Weekly))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(
                existingLink,
                true,
                desiredSchedule))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceLinkRepository.Update(A<AccountingInvoiceLinkEntity>.That.Matches(link =>
                link.Id == existingLink.Id &&
                link.ExportConfigurationState == AccountingInvoiceExportConfigurationStateConstants.TransitionRequired &&
                !string.IsNullOrWhiteSpace(link.ExportConfigurationMessage))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingInvoiceService.GenerateRecurringInvoiceAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .MustNotHaveHappened();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                subscriptionId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName,
                relatedBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Freeze_Existing_Live_Repeating_Invoice_And_Publish_Changes_When_Repeating_Mode_Is_Turned_Off(
        [Frozen] CoreConfiguration coreConfiguration,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IProductVersionRepository productVersionRepository,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IBookingInvoiceService bookingInvoiceService,
        EmailConfiguration emailConfiguration,
        CoreService.CoreServiceClient coreServiceClient,
        IDbTransactionBuilder transactionBuilder,
        IOrganizationInvoiceCounterService organizationInvoiceCounterService,
        IEmailService emailService,
        IHostEnvironment hostEnvironment,
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
        string relatedBookingId,
        string subscriptionId,
        string existingTemplateId)
    {
        var environment = new ActivityEnvironment();
        coreConfiguration.ApiKey = "api-key";
        organizationConfiguration.ApiKey = "api-key";
        var sut = new InvoiceIntegrations(
            emailConfiguration,
            coreConfiguration,
            organizationConfiguration,
            coreServiceClient,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            bookingInvoiceService,
            transactionBuilder,
            organizationInvoiceCounterService,
            emailService,
            hostEnvironment,
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

        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = new MarketplaceBookingSubscriptionEntity { Id = subscriptionId },
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                InvoiceNumber = "INV-001",
                BillingMode = ProductPricingBillingMode.InArrears.ToProductPricingBillingMode(),
                ProductPricing = ProductPricing.Empty(pricingId) with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersionEntity { Id = productVersionId }
            }
        };
        var productVersion = new ProductVersionEntity
        {
            Id = productVersionId,
            Product = new ProductEntity
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId, BillingCycle = OrganizationBillingCycleModel.Monthly.ToOrganizationBillingCycle()
                }
            }
        };
        var existingLink = new AccountingInvoiceLinkEntity
        {
            Id = "link-2",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            OrganizationId = organizationId,
            ExternalInvoiceId = existingTemplateId,
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            RepeatingScheduleSource = XeroRepeatingInvoiceScheduleSourceConstants.OrganizationBillingCycle,
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
        var relatedBookings = new List<BookingEntity> { new() { Id = relatedBookingId } };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersionId, environment.CancellationTokenSource.Token))
            .Returns(productVersion);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                environment.CancellationTokenSource.Token))
            .Returns(existingLink);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                recurringBooking.StartDate,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(relatedBookings);
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(
                existingLink,
                false,
                null))
            .Returns(transitionDecision);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == environment.CancellationTokenSource.Token &&
                    options.Headers != null &&
                    options.Headers.Any(item =>
                        item.Key == Enterprise.Shared.Grpc.Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).Returns(1);

        await environment.RunAsync(() =>
            sut.GenerateAndSendRecurringInvoiceAsync(new GenerateAndSendRecurringInvoiceInput(recurringBookingId, [])));

        A.CallTo(() => xeroRepeatingInvoiceScheduleService.GetSchedule(A<RecurringBookingEntity>._, A<MarketplaceBookingEntity>._,
                A<OrganizationBillingCycleModel>._))
            .MustNotHaveHappened();
        A.CallTo(() => xeroRecurringInvoiceTransitionService.Decide(
                existingLink,
                false,
                null))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceLinkRepository.Update(A<AccountingInvoiceLinkEntity>.That.Matches(link =>
                link.Id == existingLink.Id &&
                link.ExportConfigurationState == AccountingInvoiceExportConfigurationStateConstants.TransitionRequired &&
                !string.IsNullOrWhiteSpace(link.ExportConfigurationMessage))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingInvoiceService.GenerateRecurringInvoiceAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .MustNotHaveHappened();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                subscriptionId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName,
                relatedBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }


    private static AsyncUnaryCall<XeroConnection> CreateResponse(XeroConnection response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });
}
