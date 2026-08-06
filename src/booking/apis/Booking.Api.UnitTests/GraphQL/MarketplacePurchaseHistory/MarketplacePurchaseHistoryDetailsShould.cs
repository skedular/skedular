using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.MarketplacePurchaseHistory;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;

namespace Booking.Api.UnitTests.GraphQL.MarketplacePurchaseHistory;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchaseHistoryDetailsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Full_Refund_Timeline(
        [Frozen]
        IMarketplaceRefundReadService refundReadService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var model = new MarketplaceRefundReadModel
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatus.Completed,
            Events =
            [
                new MarketplaceRefundEventModel
                {
                    Id = "event-1",
                    OccurredAt = TimeProvider.System.GetUtcNow(),
                },
            ],
        };
        var details = new MarketplaceRefundDetails
        {
            Id = model.Id,
        };
        A.CallTo(() => refundReadService.GetByIdAsync(model.Id, cancellationToken)).Returns(model);
        A.CallTo(() => graphQlMapper.MapTo(model)).Returns(details);

        var sut = CreateDetails(model.Id);

        var result = await sut.GetRefund(refundReadService, graphQlMapper, cancellationToken);

        result.ShouldBeSameAs(details);
        A.CallTo(() => refundReadService.GetByIdAsync(model.Id, cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Refund_Is_Partially_Unavailable(
        [Frozen]
        IMarketplaceRefundReadService refundReadService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => refundReadService.GetByIdAsync("refund-1", cancellationToken))
            .Throws<UnauthorizedAccessException>();
        var sut = CreateDetails("refund-1");

        var result = await sut.GetRefund(refundReadService, graphQlMapper, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => graphQlMapper.MapTo(A<MarketplaceRefundReadModel>._)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_Refund_Read_When_History_Has_No_Refund(
        [Frozen]
        IMarketplaceRefundReadService refundReadService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var sut = CreateDetails(null);

        var result = await sut.GetRefund(refundReadService, graphQlMapper, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => refundReadService.GetByIdAsync(A<string>._, A<CancellationToken>._)).MustNotHaveHappened();
    }

    private static MarketplacePurchaseHistoryDetails CreateDetails(string? refundId) => new()
    {
        Id = "purchase-1",
        SourceId = "purchase-1",
        SourceType = default,
        SourceTypeName = string.Empty,
        LifecycleState = default,
        LifecycleStateName = string.Empty,
        RenewalState = default,
        RenewalStateName = string.Empty,
        PurchasedAt = TimeProvider.System.GetUtcNow(),
        ActivityAt = TimeProvider.System.GetUtcNow(),
        PaymentStatus = default,
        IsDeleted = false,
        RefundId = refundId,
    };
}
