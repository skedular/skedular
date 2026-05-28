using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Random;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundEventServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Add_A_Snapshotted_Refund_Event_Record(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen] IRandomHelper randomHelper,
        MarketplaceRefundEventService sut)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Currency = "NZD",
            RefundAmount = 42m,
            Reason = "Approved manually",
            AccountingProvider = "XERO",
            ExternalRefundId = "credit-note-1",
            ExternalRefundNumber = "CN-1001",
            LastError = "needs review"
        };
        var occurredAt = new DateTimeOffset(2026, 4, 7, 12, 0, 0, TimeSpan.Zero);

        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => randomHelper.Generate()).Returns("refund-event-1");
        A.CallTo(() => marketplaceRefundEventRepository.Add(A<MarketplaceRefundEvent>._))
            .ReturnsLazily((MarketplaceRefundEvent marketplaceRefundEvent) => marketplaceRefundEvent);

        var result = sut.Add(refund, MarketplaceRefundEventTypeConstants.Completed, "customer-1", occurredAt);

        result.Id.ShouldBe("refund-event-1");
        result.MarketplaceRefundId.ShouldBe("refund-1");
        result.EventType.ShouldBe(MarketplaceRefundEventTypeConstants.Completed);
        result.OccurredAt.ShouldBe(occurredAt);
        result.RefundAmount.ShouldBe(42m);
        result.Reason.ShouldBe("Approved manually");
        result.AccountingProvider.ShouldBe("XERO");
        result.ExternalRefundId.ShouldBe("credit-note-1");
        result.ExternalRefundNumber.ShouldBe("CN-1001");
        result.LastError.ShouldBe("needs review");
        result.ActorCustomerId.ShouldBe("customer-1");
    }
}
