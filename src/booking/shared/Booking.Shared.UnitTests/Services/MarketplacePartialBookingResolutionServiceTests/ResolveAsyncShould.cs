using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Random;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;

namespace Booking.Shared.UnitTests.Services.MarketplacePartialBookingResolutionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Existing_Resolution_Without_Replaying_Refund_On_Timeout(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingFailureRepository failureRepository,
        [Frozen] IMarketplaceRefundService refundService,
        MarketplacePartialBookingResolutionService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure",
            ResolutionDecision = MarketplaceBookingFailureResolutionDecisionConstants.Accepted,
            ResolutionDecidedAt = TimeProvider.System.GetUtcNow().AddMinutes(-1)
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => failureRepository.GetByIdAsync(failure.Id, cancellationToken)).Returns(failure);

        var result = await sut.ResolveAsync(failure.Id, MarketplaceBookingFailureResolutionDecisionConstants.Expired, null, cancellationToken);

        result.ShouldBe(failure);
        A.CallTo(() => refundService.CreateBookingCancellationRefundAsync(A<BookingEntity>._, A<CustomerEntity?>._, A<CancellationToken>._,
                A<bool>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Created_Occurrences_And_Automate_Full_Refund_On_Expiry(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingFailureRepository failureRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceRefundService refundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IMarketplaceRefundOwnershipService ownershipService,
        [Frozen] IRandomHelper randomHelper,
        MarketplacePartialBookingResolutionService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure",
            BookingId = "booking",
            ResolutionDeadlineAt = TimeProvider.System.GetUtcNow().AddHours(-1),
            CreatedOccurrenceIds = ["occurrence"]
        };
        var occurrence = new BookingEntity { Id = "occurrence" };
        var booking = new BookingEntity { Id = "booking", MarketplaceBooking = new MarketplaceBookingEntity { Id = "marketplace" } };
        var refund = new MarketplaceRefund { Id = "refund" };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => ownershipService.Resolve(failure)).Returns(new MarketplaceRefundOwnership(MarketplaceRefundOwnershipScope.OneTimeBooking,
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking, "booking", "booking", null, null));
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => failureRepository.GetByIdAsync("failure", cancellationToken)).Returns(failure);
        A.CallTo(() => bookingRepository.GetByIdsMinimalAsync(A<IReadOnlyList<string>>._, cancellationToken))
            .Returns(Task.FromResult<IReadOnlyList<BookingEntity>>([occurrence]));
        A.CallTo(() => bookingRepository.GetByIdAsync("booking", cancellationToken)).Returns(booking);
        A.CallTo(() => refundService.CreateBookingCancellationRefundAsync(booking, null, cancellationToken, true)).Returns(refund);
        A.CallTo(() => randomHelper.Generate()).Returns("event");

        await sut.ResolveAsync("failure", MarketplaceBookingFailureResolutionDecisionConstants.Expired, null, cancellationToken);

        A.CallTo(() => bookingRepository.Remove(occurrence)).MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refund.Id, null), repositoryFactory.UnitOfWork))
            .MustHaveHappenedOnceExactly();
        failure.ResolutionDecision.ShouldBe(MarketplaceBookingFailureResolutionDecisionConstants.Expired);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Automate_Allocated_Refund_On_Acceptance(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingFailureRepository failureRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceRefundService refundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IMarketplaceRefundOwnershipService ownershipService,
        [Frozen] IRandomHelper randomHelper,
        MarketplacePartialBookingResolutionService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure { Id = "failure", BookingId = "booking", AllocatedRefundAmount = 25m };
        var booking = new BookingEntity
        {
            Id = "booking", MarketplaceBooking = new MarketplaceBookingEntity { Id = "marketplace", TotalAmount = 100m }
        };
        var refund = new MarketplaceRefund { Id = "refund" };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => ownershipService.Resolve(failure)).Returns(new MarketplaceRefundOwnership(MarketplaceRefundOwnershipScope.OneTimeBooking,
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking, "booking", "booking", null, null));
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => failureRepository.GetByIdAsync("failure", cancellationToken)).Returns(failure);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking", cancellationToken)).Returns(booking);
        A.CallTo(() => refundService.CreateModificationRefundAsync(booking, 100m, 75m, null, cancellationToken)).Returns(refund);
        A.CallTo(() => randomHelper.Generate()).Returns("event");

        await sut.ResolveAsync("failure", MarketplaceBookingFailureResolutionDecisionConstants.Accepted, null, cancellationToken);

        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refund.Id, null), repositoryFactory.UnitOfWork))
            .MustHaveHappenedOnceExactly();
        failure.ResolutionDecision.ShouldBe(MarketplaceBookingFailureResolutionDecisionConstants.Accepted);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Automate_A_Full_Subscription_Refund_When_The_Partial_Offer_Expires(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingFailureRepository failureRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceBookingSubscriptionRepository subscriptionRepository,
        [Frozen] IMarketplaceRefundService refundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IMarketplaceRefundOwnershipService ownershipService,
        [Frozen] IRandomHelper randomHelper,
        MarketplacePartialBookingResolutionService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure", MarketplaceBookingSubscriptionId = "subscription", CreatedOccurrenceIds = ["occurrence"]
        };
        var subscription =
            new MarketplaceBookingSubscriptionEntity
            {
                Id = "subscription", MarketplaceBooking = new MarketplaceBookingEntity { Id = "marketplace" }
            };
        var refund = new MarketplaceRefund { Id = "refund" };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(subscriptionRepository);
        A.CallTo(() => ownershipService.Resolve(failure)).Returns(new MarketplaceRefundOwnership(
            MarketplaceRefundOwnershipScope.SubscriptionBillingWindow,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            "subscription", null, null, "subscription"));
        A.CallTo(() => failureRepository.GetByIdAsync("failure", cancellationToken)).Returns(failure);
        A.CallTo(() => bookingRepository.GetByIdsMinimalAsync(A<IReadOnlyList<string>>._, cancellationToken))
            .Returns(Task.FromResult<IReadOnlyList<BookingEntity>>([]));
        A.CallTo(() => subscriptionRepository.GetByIdAsync("subscription", cancellationToken)).Returns(subscription);
        A.CallTo(() => refundService.CreateImmediateSubscriptionCancellationRefundAsync(subscription, null, cancellationToken, true)).Returns(refund);
        A.CallTo(() => randomHelper.Generate()).Returns("event");

        await sut.ResolveAsync("failure", MarketplaceBookingFailureResolutionDecisionConstants.Expired, null, cancellationToken);

        A.CallTo(() => refundService.CreateImmediateSubscriptionCancellationRefundAsync(subscription, null, cancellationToken, true))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refund.Id, null), repositoryFactory.UnitOfWork))
            .MustHaveHappenedOnceExactly();
    }
}
