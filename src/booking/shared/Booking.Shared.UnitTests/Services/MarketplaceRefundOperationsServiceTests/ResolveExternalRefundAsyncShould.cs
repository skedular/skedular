using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundOperationsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveExternalRefundAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_And_Return_The_Resolved_External_Record(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceRefundOperationsService sut,
        CancellationToken cancellationToken)
    {
        var record = new MarketplaceExternalRefundReconciliation
        {
            Provider = "STRIPE",
            ExternalRefundId = "po_1",
            Status = "Open",
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationAsync("STRIPE", "po_1", "org-1", cancellationToken))
            .Returns(record);
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(record)).Returns(record);

        var result = await sut.ResolveExternalRefundAsync(
            "STRIPE", "po_1", "Resolved", "Matched to provider payout", "org-1", "customer-1", "correlation-1", cancellationToken);

        result.ExternalRefundId.ShouldBe(record.ExternalRefundId);
        result.Status.ShouldBe(MarketplaceExternalRefundReconciliationStatus.Resolved);
        record.Status.ShouldBe("Resolved");
        record.ResolutionReason.ShouldBe("Matched to provider payout");
        record.ResolutionActorCustomerId.ShouldBe("customer-1");
        record.ResolutionCorrelationId.ShouldBe("correlation-1");
        A.CallTo(() => marketplaceRefundRepository.UpdateExternalReconciliation(record)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
