using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Activities.MarketplaceRefundIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarkProcessingExhaustedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Failed_State_And_Notify_When_Automatic_Attempts_Are_Exhausted(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository refundRepository,
        [Frozen] IMarketplaceRefundTransitionService transitionService,
        MarketplaceRefundExhaustionService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            RetryCount = 1,
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "booking-1"
        };
        const string Error = "provider unavailable";
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(refundRepository);
        A.CallTo(() => refundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);
        A.CallTo(() => refundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => transitionService.TransitionAsync(
                refund,
                MarketplaceRefundStatusConstants.Failed,
                A<string>._,
                null,
                null,
                cancellationToken))
            .ReturnsLazily(call =>
            {
                refund.Status = MarketplaceRefundStatusConstants.Failed;
                refund.LastError = call.GetArgument<string>(2);
                return Task.FromResult(refund);
            });

        await sut.FinalizeAsync(refund.Id, Error, cancellationToken);

        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        refund.RetryCount.ShouldBe(3);
        refund.LastError.ShouldNotBeNull();
        refund.LastError.ShouldContain(Error);
        A.CallTo(() => transitionService.TransitionAsync(
            refund,
            MarketplaceRefundStatusConstants.Failed,
            A<string>._,
            null,
            null,
            cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
