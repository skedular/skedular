using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Random;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CreateBookingCancellationRefundAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_A_New_Refund_Record_When_Booking_Is_Refundable(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IRandomHelper randomHelper,
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
        var booking = CreateBooking(
            "marketplace-booking-1",
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(180, 50)]);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                "cancellation:MarketplaceBooking:marketplace-booking-1", cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily((MarketplaceRefund marketplaceRefund) => marketplaceRefund);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-1");

        var result = await sut.CreateBookingCancellationRefundAsync(booking, requestedByCustomer, cancellationToken);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("refund-1");
        result.OrganizationId.ShouldBe("org-1");
        result.LocalEntityType.ShouldBe(MarketplaceRefundEntityTypeConstants.MarketplaceBooking);
        result.LocalEntityId.ShouldBe("marketplace-booking-1");
        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Requested);
        result.RequestedAt.ShouldBe(requestedAt);
        result.ReferenceTime.ShouldBe(booking.From);
        result.RefundPercentage.ShouldBe(50);
        result.AppliedRuleMinutesBefore.ShouldBe(180);
        result.BaseAmount.ShouldBe(120m);
        result.RefundAmount.ShouldBe(60m);
        result.Currency.ShouldBe("NZD");
        result.RequestedByCustomerId.ShouldBe(requestedByCustomer.Id);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>.That.Matches(item =>
                item.Id == "refund-1" &&
                item.OrganizationId == "org-1" &&
                item.LocalEntityType == MarketplaceRefundEntityTypeConstants.MarketplaceBooking &&
                item.LocalEntityId == "marketplace-booking-1" &&
                item.RefundAmount == 60m)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundEventService.Add(
                result,
                MarketplaceRefundEventTypeConstants.Requested,
                "customer-1",
                requestedAt))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                A<ProcessMarketplaceRefundInput>.That.Matches(input =>
                    input.RefundId == "refund-1" && input.ActorCustomerId == "customer-1"),
                repositoryFactory.UnitOfWork))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Booking_Is_Cancellable_But_Not_Refundable(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking(
            "marketplace-booking-1",
            requestedAt.AddHours(1),
            ProductPricingCancellationPolicyType.TieredRefund,
            [new ProductPricingCancellationRefundRule(0, 0)]);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._)).MustNotHaveHappened();
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Payment_Is_Not_Confirmed(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking(
            "marketplace-booking-1",
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            []);
        booking.MarketplaceBooking!.PaymentStatus = PaymentStatus.Pending.ToPaymentStatus();

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundRepository.GetByLocalEntityAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Product_Version_Currency_When_Marketplace_Booking_Currency_Is_Missing(
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
        var booking = CreateBooking(
            "marketplace-booking-1",
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(180, 50)]);
        booking.MarketplaceBooking!.Currency = null;
        booking.MarketplaceBooking.ProductVersion!.Currency = "NZD";

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                "cancellation:MarketplaceBooking:marketplace-booking-1", cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily((MarketplaceRefund marketplaceRefund) => marketplaceRefund);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-1");

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldNotBeNull();
        result.Currency.ShouldBe("NZD");
        A.CallTo(() => marketplaceRefundEventService.Add(
                result,
                MarketplaceRefundEventTypeConstants.Requested,
                null,
                requestedAt))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Terminal_Refund_Without_Resetting_It(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking(
            "marketplace-booking-1",
            requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            []);
        var completedRefund = new MarketplaceRefund
        {
            Id = "refund-1",
            IdempotencyKey = "cancellation:MarketplaceBooking:marketplace-booking-1",
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            Status = MarketplaceRefundStatusConstants.Completed,
            RefundAmount = 120m,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                "cancellation:MarketplaceBooking:marketplace-booking-1", cancellationToken))
            .Returns(completedRefund);

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldBe(completedRefund);
        completedRefund.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).MustNotHaveHappened();
    }

    [Theory]
    [InlineAutoFakeItEasyData(new Type[] { }, MarketplaceRefundStatusConstants.Rejected)]
    [InlineAutoFakeItEasyData(new Type[] { }, MarketplaceRefundStatusConstants.Cancelled)]
    [InlineAutoFakeItEasyData(new Type[] { }, MarketplaceRefundStatusConstants.Completed)]
    public async Task Return_Terminal_Refund_Without_Resetting_It_For_All_Terminal_Statuses(
        string terminalStatus,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking("marketplace-booking-1", requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff, []);
        var terminalRefund = new MarketplaceRefund
        {
            Id = "refund-1",
            IdempotencyKey = "cancellation:MarketplaceBooking:marketplace-booking-1",
            OrganizationId = "org-1",
            Status = terminalStatus,
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            RefundAmount = 120m,
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(
                "cancellation:MarketplaceBooking:marketplace-booking-1", cancellationToken))
            .Returns(terminalRefund);

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldBe(terminalRefund);
        terminalRefund.Status.ShouldBe(terminalStatus);
        A.CallTo(() => marketplaceRefundRepository.Update(A<MarketplaceRefund>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Idempotency_Key_On_New_Refund(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking("marketplace-booking-1", requestedAt.AddHours(6),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff, []);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(A<string>._, cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily((MarketplaceRefund r) => r);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-idempotency-1");

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken);

        result.ShouldNotBeNull();
        result.IdempotencyKey.ShouldBe($"cancellation:{MarketplaceRefundEntityTypeConstants.MarketplaceBooking}:marketplace-booking-1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Force_Full_Refund_Regardless_Of_Policy_When_Operator_Cancels(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundService sut,
        CancellationToken cancellationToken)
    {
        // Policy would normally give 0% (outside window), but operator cancellation forces 100%
        var requestedAt = new DateTimeOffset(2026, 4, 7, 9, 0, 0, TimeSpan.Zero);
        var booking = CreateBooking("marketplace-booking-1", requestedAt.AddHours(-1),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(120, 100)]);

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(requestedAt);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByIdempotencyKeyAsync(A<string>._, cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.Add(A<MarketplaceRefund>._))
            .ReturnsLazily((MarketplaceRefund r) => r);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-1");

        var result = await sut.CreateBookingCancellationRefundAsync(booking, null, cancellationToken, true);

        result.ShouldNotBeNull();
        result.RefundPercentage.ShouldBe(100);
        result.RefundAmount.ShouldBe(120m);
    }

    private static Database.Entities.Booking CreateBooking(
        string marketplaceBookingId,
        DateTimeOffset from,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "booking-1",
            From = from,
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = marketplaceBookingId,
                Quantity = 1,
                TotalAmount = 120m,
                Currency = "NZD",
                PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
                ProductPricing = CreatePricing(cancellationPolicyType, cancellationRefundRules),
                ProductVersion = new ProductVersionEntity
                {
                    Product = new ProductEntity
                    {
                        Organization = new OrganizationEntity
                        {
                            Id = "org-1",
                        },
                    },
                },
            },
        };

    private static ProductPricing CreatePricing(
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new(
            "pricing-1",
            0,
            ListingMetadata.Empty,
            ProductPricingCadence.OneTime,
            ProductPricingCadence.OneTime,
            120m,
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
