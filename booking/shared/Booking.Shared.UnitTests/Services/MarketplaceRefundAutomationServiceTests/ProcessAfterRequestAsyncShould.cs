using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundAutomationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessAfterRequestAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Process_In_Xero_When_Availability_Is_Confirmed(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Requested,
            RefundAmount = 50m
        };
        var completedRefund = new MarketplaceRefund
        {
            Id = refund.Id,
            Status = MarketplaceRefundStatusConstants.Completed,
            LastProcessedAt = new DateTimeOffset(2026, 4, 8, 9, 0, 0, TimeSpan.Zero)
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(true, null));
        A.CallTo(() => xeroRefundService.ProcessAsync(refund, cancellationToken)).Returns(completedRefund);

        var result = await sut.ProcessAfterRequestAsync(refund, "customer-1", cancellationToken);

        result.ShouldBe(completedRefund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.PendingAccounting);
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.PendingAccounting, "customer-1", A<DateTimeOffset?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.SentToXero, "customer-1", A<DateTimeOffset?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundEventService.Add(completedRefund, MarketplaceRefundEventTypeConstants.Completed, "customer-1", A<DateTimeOffset?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Manual_Required_When_Xero_Is_Not_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventService marketplaceRefundEventService,
        [Frozen] IXeroRefundService xeroRefundService,
        [Frozen] IUnitOfWork unitOfWork,
        MarketplaceRefundAutomationService sut,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Requested,
            RefundAmount = 50m
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "Invoice correlation is missing."));

        var result = await sut.ProcessAfterRequestAsync(refund, "customer-1", cancellationToken);

        result.ShouldBe(refund);
        refund.Status.ShouldBe(MarketplaceRefundStatusConstants.ManualRequired);
        refund.LastError.ShouldBe("Invoice correlation is missing.");
        A.CallTo(() => marketplaceRefundEventService.Add(refund, MarketplaceRefundEventTypeConstants.ManualRequired, "customer-1", A<DateTimeOffset?>._))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => xeroRefundService.ProcessAsync(A<MarketplaceRefund>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
