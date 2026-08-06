using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Random;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreateImmediateSubscriptionCancellationRefundAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Existing_Refund_Unchanged_When_Subscription_Refund_Already_Exists(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var requestedByCustomer = new CustomerEntity
        {
            Id = "customer-1",
        };
        var subscription = CreateSubscription(
            requestedAt.AddDays(-5),
            requestedAt.AddDays(2),
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(1440, 75), new ProductPricingCancellationRefundRule(60, 25)]);
        var existingRefund = new MarketplaceRefund
        {
            Id = "refund-1",
            IdempotencyKey = $"cancellation:{MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription}:{subscription.Id}",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = subscription.Id,
            Status = "OldStatus",
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                $"cancellation:{MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription}:{subscription.Id}", cancellationToken))
            .Returns(existingRefund);
        var result = await sut.CreateImmediateSubscriptionCancellationRefundAsync(subscription, requestedByCustomer, cancellationToken);

        result.ShouldBe(existingRefund);
        existingRefund.Status.ShouldBe("OldStatus");
        A.CallTo(() => marketplaceRefundRepository.Update(existingRefund)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Current_Billing_Window_Payment_Is_Not_Confirmed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var subscription = CreateSubscription(
            requestedAt.AddDays(-5),
            requestedAt.AddDays(2),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            []);
        subscription.RecurringBookings =
        [
            new RecurringBookingEntity
            {
                Id = "recurring-1",
                StartDate = requestedAt.AddDays(-5),
                MarketplaceBooking = new MarketplaceBookingEntity
                {
                    PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                    ProductPricing = subscription.MarketplaceBooking.ProductPricing,
                },
            },
        ];

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);

        var result = await sut.CreateImmediateSubscriptionCancellationRefundAsync(subscription, null, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allocate_A_Subscription_Refund_Against_The_Confirmed_Current_Bank_Transfer_Payment(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var subscription = CreateSubscription(
            requestedAt.AddDays(-6),
            requestedAt.AddDays(24),
            ProductPricingCancellationPolicyType.NotSet,
            []);
        subscription.MarketplaceBooking.TotalAmount = 0m;
        var currentCycleMarketplaceBooking = subscription.RecurringBookings.Single().MarketplaceBooking!;
        currentCycleMarketplaceBooking.Id = "current-cycle-booking-1";
        currentCycleMarketplaceBooking.TotalAmount = 102.20m;
        currentCycleMarketplaceBooking.TotalAmountExcludeTax = 100m;
        currentCycleMarketplaceBooking.TaxAmount = 2.20m;
        currentCycleMarketplaceBooking.ProductPricing = currentCycleMarketplaceBooking.ProductPricing with
        {
            Price = 100m,
        };
        currentCycleMarketplaceBooking.PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod();

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                $"cancellation:{MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription}:{subscription.Id}", cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily((MarketplaceRefund refund) => refund);
        A.CallTo(() => marketplaceRefundRepository.GetSourceAllocationAsync("BANK_TRANSFER", currentCycleMarketplaceBooking.Id, cancellationToken))
            .Returns((MarketplaceRefundPaymentAllocation?)null);
        A.CallTo(() => marketplaceRefundRepository.AddAllocation(A<MarketplaceRefundPaymentAllocation>._))
            .ReturnsLazily((MarketplaceRefundPaymentAllocation allocation) => allocation);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-1");

        await sut.CreateImmediateSubscriptionCancellationRefundAsync(subscription, null, cancellationToken);

        A.CallTo(() => marketplaceRefundRepository.AddAllocation(A<MarketplaceRefundPaymentAllocation>.That.Matches(allocation =>
                allocation.IsSourcePayment &&
                allocation.SourcePaymentProvider == "BANK_TRANSFER" &&
                allocation.SourcePaymentReference == currentCycleMarketplaceBooking.Id &&
                allocation.SourceCapturedAmount == 102.20m)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundRepository.ReserveAllocationAsync(A<string>._, A<string>._, A<decimal>._, cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => marketplaceRefundEventService.Add(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<DateTimeOffset>._))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Refund_Only_Undelivered_Recurring_Occurrences(
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var subscription = CreateSubscription(
            new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 1, 9, 0, 0, TimeSpan.Zero),
            ProductPricingCancellationPolicyType.NotSet,
            []);
        var recurringBooking = subscription.RecurringBookings.Single();
        recurringBooking.Bookings =
        [
            new BookingEntity
            {
                From = requestedAt.AddDays(-5),
                Until = requestedAt.AddDays(-5).AddHours(1),
            },
            new BookingEntity
            {
                From = requestedAt.AddDays(-4),
                Until = requestedAt.AddDays(-4).AddHours(1),
            },
            new BookingEntity
            {
                From = requestedAt.AddDays(1),
                Until = requestedAt.AddDays(1).AddHours(1),
            },
            new BookingEntity
            {
                From = requestedAt.AddDays(2),
                Until = requestedAt.AddDays(2).AddHours(1),
            },
            new BookingEntity
            {
                From = requestedAt.AddDays(3),
                Until = requestedAt.AddDays(3).AddHours(1),
            },
        ];

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);

        var preview = await sut.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken);

        preview.RefundPercentage.ShouldBe(60);
        preview.RefundAmount.ShouldBe(48m);
    }

    private static MarketplaceBookingSubscriptionEntity CreateSubscription(
        DateTimeOffset startedAt,
        DateTimeOffset? nextRenewalAt,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "subscription-1",
            StartedAt = startedAt,
            NextRenewalAt = nextRenewalAt,
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = "marketplace-booking-1",
                Quantity = 1,
                TotalAmount = 80m,
                Currency = "NZD",
                PaymentStatus = PaymentStatus.NotSet.ToPaymentStatus(),
                ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules),
                ProductVersion = new ProductVersionEntity
                {
                    Product = new ProductEntity
                    {
                        Organization = new OrganizationEntity
                        {
                            Id = "org-1",
                            BillingCycle = OrganizationBillingCycleConstants.Monthly,
                        },
                    },
                },
            },
            RecurringBookings =
            [
                new RecurringBookingEntity
                {
                    Id = "recurring-1",
                    StartDate = startedAt,
                    MarketplaceBooking = new MarketplaceBookingEntity
                    {
                        PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
                        ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules),
                    },
                },
            ],
        };

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new(
            "pricing-1",
            0,
            ListingMetadata.Empty,
            ProductPricingCadence.Monthly,
            ProductPricingCadence.Monthly,
            80m,
            true,
            false,
            [],
            ProductPricingBillingMode.Upfront,
            null,
            null,
            10,
            10,
            1,
            cancellationPolicyType,
            cancellationRefundRules);
}
