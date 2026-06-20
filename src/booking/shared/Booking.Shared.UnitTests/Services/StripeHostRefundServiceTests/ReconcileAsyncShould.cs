using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Stripe;

namespace Booking.Shared.UnitTests.Services.StripeHostRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReconcileAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Correlate_By_Metadata_And_Persist_A_Status_Change(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IUnitOfWork unitOfWork,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund { Id = "refund-1", Status = MarketplaceRefundStatusConstants.PendingAccounting };
        var stripeRefund = new Refund
        {
            Id = "re_1", Status = "succeeded", Metadata = new Dictionary<string, string> { ["marketplace_refund_id"] = refund.Id }
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken))
            .Returns((MarketplaceRefund?)null);
        A.CallTo(() => marketplaceRefundRepository.GetByIdAsync(refund.Id, cancellationToken)).Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBe(refund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.Completed, null, A<DateTimeOffset?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Ignore_A_Duplicate_Status_Event(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IUnitOfWork unitOfWork,
        StripeHostRefundService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.ManualRequired,
            PaymentRefundStatus = MarketplaceRefundStatusConstants.Completed
        };
        var stripeRefund = new Refund { Id = "re_1", Status = "succeeded", Metadata = [] };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetByExternalPaymentRefundIdAsync(stripeRefund.Id, cancellationToken)).Returns(refund);

        var result = await sut.ReconcileAsync(stripeRefund, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => marketplaceRefundEventService.Add(A<MarketplaceRefund>._, A<string>._, A<string?>._, A<DateTimeOffset?>._))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustNotHaveHappened();
    }
}
