using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundTransitionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TransitionAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Publish_GraphQl_Update_For_Payout_Failure_Transition(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository refundRepository,
        [Frozen]
        IMarketplaceRefundEventService refundEventService,
        [Frozen]
        IMarketplaceRefundNotificationService notificationService,
        [Frozen]
        IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceRefundTransitionService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1",
            Status = MarketplaceRefundStatusConstants.Processing,
        };
        var now = new DateTimeOffset(2026, 7, 29, 10, 0, 0, TimeSpan.Zero);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => refundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => notificationService.NotifyStatusChangedAsync(refund, cancellationToken)).Returns(Task.CompletedTask);
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName, "booking-1", cancellationToken))
            .Returns(Task.CompletedTask);

        await sut.TransitionAsync(
            refund,
            MarketplaceRefundStatusConstants.ReconciliationRequired,
            "Payout failed",
            null,
            "evt_payout_failed",
            cancellationToken);

        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.ReconciliationRequired);
        refund.LastError.ShouldBe("Payout failed");
        refund.LastProcessedAt.ShouldBe(now);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository.Update(refund))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => refundEventService.Add(
                refund,
                MarketplaceRefundEventTypeConstants.ReconciliationRequired,
                null,
                now,
                MarketplaceRefundStatusConstants.Processing,
                "evt_payout_failed"))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => notificationService.NotifyStatusChangedAsync(refund, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.BookingTopicName, "booking-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
