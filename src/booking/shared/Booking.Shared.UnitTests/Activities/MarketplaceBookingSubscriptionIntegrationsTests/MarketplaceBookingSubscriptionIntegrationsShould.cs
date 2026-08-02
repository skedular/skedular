using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Temporalio.Testing;
using MarketplaceBookingFailureFinalization = Booking.Shared.Models.MarketplaceBookingFailureFinalization;
using MarketplaceBookingFailureScopeConstants = Booking.Shared.Models.MarketplaceBookingFailureScopeConstants;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingSubscriptionIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionIntegrationsShould
{
    [Theory(Skip = "Requires the persisted recurring aggregate fixture used by Booking.Domain integration tests.")]
    [AutoFakeItEasyData]
    public async Task Keep_Workflow_Alive_Without_Renewing_When_Spaces_Access_Is_Expired(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ISpacesBookingQuotaService spacesBookingQuotaService,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        string organizationId)
    {
        var environment = new ActivityEnvironment();
        var subscription = CreateSubscription(
            MarketplaceBookingSubscriptionStatus.Active,
            new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.InvolvedOrganizations =
        [
            new Organization { Id = organizationId, Type = OrganizationTypeConstants.Marketplace }
        ];
        var denied = new SpacesAccessDecision(
            false,
            SpacesSubscriptionStatus.TrialExpired,
            SpacesAccessReasonCode.TrialExpired,
            SpacesAccessAction.CreateOrModify,
            OfferingCode.SpacesFreeTierV1,
            null,
            null,
            0,
            false,
            false,
            true,
            true,
            null,
            false);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => spacesBookingQuotaService.EvaluateAccessAsync(
                organizationId,
                SpacesAccessAction.CreateOrModify,
                environment.CancellationTokenSource.Token))
            .Returns(denied);

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeFalse();
        A.CallTo(() => repositoryFactory.UnitOfWork.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Deleted_Response_When_Subscription_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository)
    {
        var environment = new ActivityEnvironment();
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(Task.FromResult<MarketplaceBookingSubscription?>(null));

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeTrue();
        result.Ended.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Workflow_Alive_When_Subscription_Is_Paused(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository)
    {
        var environment = new ActivityEnvironment();
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Paused, new DateTimeOffset(2026, 3, 17, 0, 0, 0, TimeSpan.Zero));

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Workflow_Alive_When_Current_Cycle_Is_Fully_Booked_But_Today_Is_Not_The_Last_Day(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1
                    },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Recurring_Cycle_Card_Payment_Workflow_When_Current_Cycle_Already_Exists_Without_Checkout_Session(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] ITemporalService temporalService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        IRecurringBookingRepository recurringBookingRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                PaymentExpiry = new DateTimeOffset(2026, 3, 18, 10, 0, 0, TimeSpan.Zero),
                StripeCheckoutSession = null,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly,
                        BookingCadence = ProductPricingCadence.Daily,
                        NumberOfResourcesToBook = 1,
                        BillingMode = ProductPricingBillingMode.Upfront
                    },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.PaymentMethod = PaymentMethod.Card.ToPaymentMethod();
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly,
            BookingCadence = ProductPricingCadence.Daily,
            BillingMode = ProductPricingBillingMode.Upfront
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 19, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("rb-1", environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        _ = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        A.CallTo(() => temporalService.StartWorkflowPayRecurringBookingViaCardAsync(
                A<PayRecurringBookingViaCardInput>.That.Matches(item =>
                    item.RecurringBookingId == recurringBooking.Id &&
                    item.ExpiryDate == recurringBooking.MarketplaceBooking.PaymentExpiry &&
                    item.InvoiceEmailList.Count == 0),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reload_Current_Cycle_Recurring_Booking_When_Subscription_Aggregate_Does_Not_Have_Its_Marketplace_Booking_Loaded(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] ITemporalService temporalService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IRecurringBookingRepository recurringBookingRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBookingFromSubscription = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = null,
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = recurringBookingFromSubscription.StartDate,
            EndDate = recurringBookingFromSubscription.EndDate,
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                PaymentExpiry = new DateTimeOffset(2026, 3, 18, 10, 0, 0, TimeSpan.Zero),
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Monthly,
                    BookingCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 1,
                    BillingMode = ProductPricingBillingMode.Upfront
                },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBookingFromSubscription];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.PaymentMethod = PaymentMethod.Card.ToPaymentMethod();
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly,
            BookingCadence = ProductPricingCadence.Daily,
            BillingMode = ProductPricingBillingMode.Upfront
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 19, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("rb-1", environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBookingFromSubscription,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        _ = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        A.CallTo(() => temporalService.StartWorkflowPayRecurringBookingViaCardAsync(
                A<PayRecurringBookingViaCardInput>.That.Matches(item =>
                    item.RecurringBookingId == recurringBooking.Id &&
                    item.ExpiryDate == recurringBooking.MarketplaceBooking!.PaymentExpiry &&
                    item.InvoiceEmailList.Count == 0),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Recurring_Cycle_Bank_Transfer_Workflow_When_Current_Cycle_Already_Exists_Without_Invoice(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] ITemporalService temporalService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        IRecurringBookingRepository recurringBookingRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                PaymentExpiry = new DateTimeOffset(2026, 3, 18, 10, 0, 0, TimeSpan.Zero),
                InvoiceUrl = null,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly,
                        BookingCadence = ProductPricingCadence.Daily,
                        NumberOfResourcesToBook = 1,
                        BillingMode = ProductPricingBillingMode.Upfront
                    },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod();
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly,
            BookingCadence = ProductPricingCadence.Daily,
            BillingMode = ProductPricingBillingMode.Upfront
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync("rb-1", environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        _ = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        A.CallTo(() => temporalService.StartWorkflowPayRecurringBookingViaBankTransferAsync(
                A<PayRecurringBookingViaBankTransferInput>.That.Matches(item =>
                    item.RecurringBookingId == recurringBooking.Id &&
                    item.ExpiryDate == recurringBooking.MarketplaceBooking.PaymentExpiry &&
                    item.InvoiceEmailList.Count == 0),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Event_Product_Reaches_Subscription_Recurring_Materialization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 31, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1
                    },
                ProductVersion = new ProductVersion { Id = "pv-1", Type = ProductTypeConstants.Event }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [new DateOnly(2026, 3, 20)], [], false));

        await Should.ThrowAsync<MarketplaceEventProductRecurringBookingNotSupported>(async () =>
            await environment.RunAsync(() =>
                sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                    new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1"))));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task End_Workflow_When_Current_Cycle_Is_Fully_Booked_And_Today_Is_The_Last_Day_Without_Auto_Renew(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 2, 19, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 18, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1
                    },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 3, 19, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 2, 19, 0, 0, 0, TimeSpan.Zero);
        subscription.AutoRenew = false;
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Subscription_As_Cancelled_At_Cycle_End_When_Cancel_At_Period_End_Is_Requested(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-1",
            StartDate = new DateTimeOffset(2026, 2, 19, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 18, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductPricing =
                    ProductPricing.Empty("pricing-1") with
                    {
                        PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1
                    },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 3, 19, 0, 0, 0, TimeSpan.Zero));
        subscription.StartedAt = new DateTimeOffset(2026, 2, 19, 0, 0, 0, TimeSpan.Zero);
        subscription.AutoRenew = false;
        subscription.CancelAtPeriodEnd = true;
        subscription.CancelledAt = new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.Zero);
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.ProductPricing = subscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly, BookingCadence = ProductPricingCadence.Daily
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 19, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(subscription)).Returns(subscription);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                recurringBooking,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeTrue();
        subscription.Status.ToMarketplaceBookingSubscriptionStatus().ShouldBe(MarketplaceBookingSubscriptionStatus.Cancelled);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Delete_Future_Bookings_When_Releasing_Subscription_Resources(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingService marketplaceBookingService,
        MarketplaceBookingSubscriptionIntegrations sut,
        IBookingRepository bookingRepository,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository)
    {
        var environment = new ActivityEnvironment();
        var recurringBooking = new RecurringBooking { Id = "rb-1", DeletedAt = null };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, null);
        subscription.RecurringBookings = [recurringBooking];
        var booking1 = new Database.Entities.Booking
        {
            Id = "b-1",
            Channel = BookingChannelConstants.Marketplace,
            Category = BookingCategoryConstants.WorkingFromCoworkingSpace,
            Schedules = []
        };
        var booking2 = new Database.Entities.Booking
        {
            Id = "b-2",
            Channel = BookingChannelConstants.Marketplace,
            Category = BookingCategoryConstants.WorkingFromCoworkingSpace,
            Schedules = []
        };

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync("rb-1", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([booking1, booking2]);
        A.CallTo(() => marketplaceBookingService.DeleteAsync(A<Database.Entities.Booking>._, null, false, null, false,
                environment.CancellationTokenSource.Token))
            .ReturnsLazily((Database.Entities.Booking booking, Customer? _, bool _, string? _, bool _, CancellationToken _) =>
                Task.FromResult(new Shared.Models.Booking { Id = booking.Id }));

        await environment.RunAsync(() =>
            sut.ReleaseMarketplaceBookingSubscriptionResourcesAsync(
                new ReleaseMarketplaceBookingSubscriptionResourcesInput("sub-1")));

        A.CallTo(() => marketplaceBookingService.DeleteAsync(booking1, subscription.DeletedByCustomer, false, null, false,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingService.DeleteAsync(booking2, subscription.DeletedByCustomer, false, null, false,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Prefer_Last_Booking_Resources_From_Previous_Cycle_When_Auto_Renewing(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
        [Frozen] IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen] IMarketplaceBookingService marketplaceBookingService,
        [Frozen] IProductVersionHelperService productVersionHelperService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        MarketplaceBookingSubscriptionIntegrations sut,
        IUnitOfWork unitOfWork,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        IMarketplaceBookingRepository marketplaceBookingRepository,
        IRecurringBookingRepository recurringBookingRepository,
        IBookingRepository bookingRepository,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository)
    {
        var environment = new ActivityEnvironment();
        var customer = new Customer { Id = "customer-1" };
        var previousRecurringBooking = new RecurringBooking
        {
            Id = "rb-prev",
            StartDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
            EndDate = new DateTimeOffset(2026, 3, 16, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    BookingCadence = ProductPricingCadence.Daily,
                    SupportsSubscriptionAutoRenewal = true,
                    NumberOfResourcesToBook = 1
                },
                ProductVersion = new ProductVersion { Id = "pv-1" }
            },
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = []
        };
        var currentPricing = ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = ProductPricingCadence.Daily,
            BookingCadence = ProductPricingCadence.Daily,
            SupportsSubscriptionAutoRenewal = true,
            NumberOfResourcesToBook = 1
        };
        var renewedProductVersion = new ProductVersion { Id = "pv-1", PricingOptions = [currentPricing] };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, new DateTimeOffset(2026, 3, 18, 0, 0, 0, TimeSpan.Zero));
        subscription.AutoRenew = true;
        subscription.ProductVersion = renewedProductVersion;
        subscription.InvolvedCustomers = [customer];
        subscription.RecurringBookings = [previousRecurringBooking];
        subscription.MarketplaceBooking.Quantity = 1;
        subscription.MarketplaceBooking.ProductPricing = currentPricing;
        subscription.MarketplaceBooking.ProductVersion = renewedProductVersion;
        var previousBooking = new Database.Entities.Booking
        {
            Id = "b-prev",
            From = new DateTimeOffset(2026, 3, 16, 8, 0, 0, TimeSpan.Zero),
            Until = new DateTimeOffset(2026, 3, 16, 17, 0, 0, TimeSpan.Zero),
            InvolvedResources = [new Resource { Id = "res-7" }],
            Channel = BookingChannelConstants.Marketplace,
            Category = BookingCategoryConstants.WorkingFromCoworkingSpace,
            Schedules = []
        };
        var generatedBooking = new Shared.Models.Booking
        {
            Id = "booking-new",
            InvolvedCustomers = [new Shared.Models.Customer { Id = customer.Id }],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            Schedules = []
        };
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).Returns(1);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => productVersionRepository.GetByIdAsync("pv-1", environment.CancellationTokenSource.Token)).Returns(renewedProductVersion);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(renewedProductVersion.PricingOptions!.ToList(),
                subscription.MarketplaceBooking.ProductPricing))
            .Returns(subscription.MarketplaceBooking.ProductPricing);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(subscription)).Returns(subscription);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-prev", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([previousBooking]);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync("rb-current", A<DateTimeOffset>._, null,
                environment.CancellationTokenSource.Token))
            .Returns([]);
        A.CallTo(() => marketplaceBookingRepository.Add(A<MarketplaceBooking>._))
            .ReturnsLazily((MarketplaceBooking booking) => booking);
        A.CallTo(() => recurringBookingRepository.Add(A<RecurringBooking>._))
            .ReturnsLazily((RecurringBooking added) => added);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                A<RecurringBooking>.That.Matches(item => item.Id == "rb-prev"),
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [], [], false));
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                A<RecurringBooking>.That.Matches(item => item.Id == "rb-current"),
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [new DateOnly(2026, 3, 17)], [], false));
        A.CallTo(() => marketplaceBookingOpeningHoursService.ShouldUseLocationOpeningHoursWindow(ProductPricingCadence.Daily)).Returns(true);
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(
                A<ProductPricing>._,
                A<DateOnly>._))
            .Returns(true);
        A.CallTo(() => marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
                customer,
                renewedProductVersion,
                A<ProductPricing>._,
                new DateOnly(2026, 3, 17),
                1,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 0),
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "res-7" })),
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new MarketplaceBookingDailyPlan(
                new DateTimeOffset(2026, 3, 17, 8, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 3, 17, 17, 0, 0, TimeSpan.Zero),
                [new Resource { Id = "res-7" }]));
        A.CallTo(() => entityMapper.MapTo(A<RecurringBooking>._, new DateOnly(2026, 3, 17))).Returns(generatedBooking);
        A.CallTo(() => entityMapper.MapTo(A<MarketplaceBooking>._)).Returns(new Shared.Models.MarketplaceBooking
        {
            ProductPricing = subscription.MarketplaceBooking.ProductPricing, ProductVersion = new Shared.Models.ProductVersion { Id = "pv-1" }
        });
        A.CallTo(() => randomHelper.Generate()).ReturnsNextFromSequence("mb-template", "rb-current", "booking-new", "mb-new");
        A.CallTo(() => marketplaceBookingService.AddAsync(
                A<Shared.Models.Booking>._,
                A<Customer>._,
                A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Team>>._,
                A<RecurringBooking>._,
                A<bool>._,
                A<bool>._,
                A<bool>._,
                environment.CancellationTokenSource.Token))
            .Returns(new Shared.Models.Booking { Id = "booking-new" });

        await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        A.CallTo(() => marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
                customer,
                renewedProductVersion,
                A<ProductPricing>._,
                new DateOnly(2026, 3, 17),
                1,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 0),
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "res-7" })),
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Renewal_Failed_When_Current_Price_No_Longer_Accepts_Selected_Weekly_Days(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] TimeProvider timeProvider,
        [Frozen] IProductVersionHelperService productVersionHelperService,
        [Frozen] IMarketplaceBookingWeeklyDaySelectionService marketplaceBookingWeeklyDaySelectionService,
        MarketplaceBookingSubscriptionIntegrations sut,
        IUnitOfWork unitOfWork,
        IProductVersionRepository productVersionRepository,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository)
    {
        var environment = new ActivityEnvironment();
        var originalPricing = ProductPricing.Empty("pricing-original") with
        {
            PurchaseCadence = ProductPricingCadence.Weekly,
            BookingCadence = ProductPricingCadence.Weekly,
            SupportsSubscriptionAutoRenewal = true,
            RequiredDaysPerWeek = 2
        };
        // The renewed price has removed the optional count rule but no longer permits the
        // subscription's saved Tuesday/Wednesday pattern. Renewal must validate the retained
        // pattern rather than clearing it and broadening recurrence to daily.
        var renewedPricing = originalPricing with { Id = "pricing-renewed", RequiredDaysPerWeek = null, AvailableDays = [DayOfWeek.Monday] };
        var renewedProductVersion = new ProductVersion { Id = "pv-1", PricingOptions = [renewedPricing] };
        var subscription = CreateSubscription(
            MarketplaceBookingSubscriptionStatus.Active,
            new DateTimeOffset(2026, 3, 17, 0, 0, 0, TimeSpan.Zero));
        subscription.AutoRenew = true;
        subscription.ProductVersion = renewedProductVersion;
        subscription.MarketplaceBooking.ProductVersion = renewedProductVersion;
        subscription.MarketplaceBooking.ProductPricing = originalPricing;
        subscription.WeeklySelectedDays = [DayOfWeekConstants.Tuesday, DayOfWeekConstants.Wednesday];

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("sub-1", environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => productVersionRepository.GetByIdAsync("pv-1", environment.CancellationTokenSource.Token))
            .Returns(renewedProductVersion);
        A.CallTo(() => productVersionHelperService.FindMatchingPricing(
                renewedProductVersion.PricingOptions!.ToList(),
                originalPricing))
            .Returns(renewedPricing);
        A.CallTo(() => marketplaceBookingWeeklyDaySelectionService.Validate(
                A<ProductPricing>.That.Matches(pricing =>
                    pricing.Id == renewedPricing.Id && pricing.RequiredDaysPerWeek == 2),
                A<IEnumerable<DayOfWeek>>._))
            .Throws(new MarketplaceBookingWeeklyDaySelectionInvalid());
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(subscription)).Returns(subscription);
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).Returns(1);

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput("sub-1")));

        result.Deleted.ShouldBeFalse();
        result.Ended.ShouldBeTrue();
        subscription.Status.ToMarketplaceBookingSubscriptionStatus().ShouldBe(MarketplaceBookingSubscriptionStatus.RenewalFailed);
        A.CallTo(() => marketplaceBookingWeeklyDaySelectionService.Validate(
                A<ProductPricing>.That.Matches(pricing =>
                    pricing.Id == renewedPricing.Id && pricing.RequiredDaysPerWeek == 2),
                A<IEnumerable<DayOfWeek>>.That.Matches(days => days.SequenceEqual(new[] { DayOfWeek.Tuesday, DayOfWeek.Wednesday }))))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(subscription)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Finalize_One_Initial_Series_Failure_Without_Materializing_Any_Occurrence_When_A_Required_Day_Is_Unavailable(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingScheduleService recurringBookingScheduleService,
        [Frozen] IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
        [Frozen] IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen] IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        ICustomerRepository customerRepository,
        IBookingRepository bookingRepository,
        IMarketplaceBookingService marketplaceBookingService,
        IUnitOfWork unitOfWork)
    {
        var environment = new ActivityEnvironment();
        var start = new DateTimeOffset(2026, 3, 16, 0, 0, 0, TimeSpan.Zero);
        var customer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBooking
        {
            Id = "rb-initial",
            StartDate = start,
            EndDate = start,
            CreatedByCustomer = customer,
            InvolvedCustomers = [customer],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = 1,
                ProductVersion = new ProductVersion { Id = "pv-1", Type = ProductTypeConstants.Resource },
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily, BookingCadence = ProductPricingCadence.Daily, NumberOfResourcesToBook = 1
                }
            }
        };
        var subscription = CreateSubscription(MarketplaceBookingSubscriptionStatus.Active, start.AddDays(1));
        subscription.StartedAt = start;
        subscription.RecurringBookings = [recurringBooking];
        subscription.InvolvedCustomers = [customer];
        subscription.MarketplaceBooking = recurringBooking.MarketplaceBooking;

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(start);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdUntrackedAsync(subscription.Id, environment.CancellationTokenSource.Token))
            .Returns((MarketplaceBookingSubscription?)null);
        A.CallTo(() => customerRepository.GetByIdAsync(customer.Id, true, environment.CancellationTokenSource.Token)).Returns(customer);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(recurringBooking.Id, A<DateTimeOffset>._, null,
            environment.CancellationTokenSource.Token)).Returns([]);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync(recurringBooking.Id, A<DateTimeOffset>._, null,
            environment.CancellationTokenSource.Token)).Returns([]);
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                A<RecurringBooking>._,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .Returns(new RecurringBookingReconciliationPlan([], [DateOnly.FromDateTime(start.DateTime)], [], false));
        A.CallTo(() => marketplaceBookingOpeningHoursService.ShouldUseLocationOpeningHoursWindow(ProductPricingCadence.Daily)).Returns(true);
        A.CallTo(() => marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
                A<Customer?>._,
                A<ProductVersion>._,
                A<ProductPricing>._,
                DateOnly.FromDateTime(start.DateTime),
                1,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<string?>._,
                environment.CancellationTokenSource.Token))
            .Returns((MarketplaceBookingDailyPlan?)null);
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(A<ProductPricing>._, DateOnly.FromDateTime(start.DateTime)))
            .Returns(true);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>._,
                environment.CancellationTokenSource.Token))
            .Returns(new MarketplaceBookingFailure { Id = "failure-initial" });

        var result = await environment.RunAsync(() =>
            sut.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput(subscription.Id)));

        result.Deleted.ShouldBeFalse();
        A.CallTo(() => recurringBookingScheduleService.GetReconciliationPlan(
                A<RecurringBooking>._,
                A<DateTimeOffset>._,
                A<DateTimeOffset>._,
                A<IReadOnlyList<Database.Entities.Booking>>._))
            .MustHaveHappenedOnceExactly();

        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.Scope == MarketplaceBookingFailureScopeConstants.InitialSeries &&
                    item.RecurringBookingId == recurringBooking.Id &&
                    item.MarketplaceBookingSubscriptionId == subscription.Id),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingService.AddAsync(
                A<Shared.Models.Booking>._,
                A<Customer>._!,
                A<IReadOnlyList<Organization>>._,
                A<IReadOnlyList<Team>>._,
                A<RecurringBooking>._,
                A<bool>._,
                A<bool>._,
                A<bool>._,
                environment.CancellationTokenSource.Token))
            .MustNotHaveHappened();
    }

    private static MarketplaceBookingSubscription CreateSubscription(MarketplaceBookingSubscriptionStatus status, DateTimeOffset? nextRenewalAt) =>
        new()
        {
            Id = "sub-1",
            StartedAt = new DateTimeOffset(2026, 3, 16, 0, 0, 0, TimeSpan.Zero),
            NextRenewalAt = nextRenewalAt,
            Status = status.ToMarketplaceBookingSubscriptionStatus(),
            AutoRenew = false,
            CancelAtPeriodEnd = false,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    BookingCadence = ProductPricingCadence.Daily,
                    SupportsSubscriptionAutoRenewal = true,
                    NumberOfResourcesToBook = 1
                },
                ProductVersion = new ProductVersion()
            },
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
            RecurringBookings = [],
            CreatedByCustomer = new Customer { Id = "customer-1" },
            DeletedByCustomer = null
        };
}
