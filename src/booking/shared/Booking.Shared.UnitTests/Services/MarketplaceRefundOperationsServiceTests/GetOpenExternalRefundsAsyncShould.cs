using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;

namespace Booking.Shared.UnitTests.Services.MarketplaceRefundOperationsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetOpenExternalRefundsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Open_External_Reconciliation_Records(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        MarketplaceRefundOperationsService sut,
        CancellationToken cancellationToken)
    {
        var records = new[]
        {
            new MarketplaceExternalRefundReconciliation
            {
                Provider = "STRIPE",
                ExternalRefundId = "po_1",
                Status = "Open",
            },
        };
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        var pagination = new PaginationInputParam(null, 25, null, null);
        var edges = records.Select(record => new Edge<MarketplaceExternalRefundReconciliation>(record, "cursor")).ToArray();
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationsAsync(
                "org-1", "STRIPE", "Open", pagination, cancellationToken))
            .Returns((new PaginatedInfo(false, false, "cursor", "cursor"), edges, records.Length));

        var result = await sut.GetExternalRefundsAsync("org-1", "STRIPE", "Open", pagination, cancellationToken);

        result.Item2.Single().Node.ExternalRefundId.ShouldBe("po_1");
        result.Item2.Single().Cursor.ShouldBe("cursor");
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationsAsync(
                "org-1", "STRIPE", "Open", pagination, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Default_Page_Size_When_No_Page_Size_Is_Requested(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        MarketplaceRefundOperationsService sut,
        CancellationToken cancellationToken)
    {
        var pagination = new PaginationInputParam(null, 50, null, null);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationsAsync(
                "org-1", null, "Open",
                A<PaginationInputParam>.That.Matches(value => value.First == pagination.First && value.After == pagination.After),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null),
                Array.Empty<Edge<MarketplaceExternalRefundReconciliation>>(), 0));

        await sut.GetExternalRefundsAsync("org-1", null, "Open", new PaginationInputParam(null, null, null, null), cancellationToken);

        A.CallTo(() => marketplaceRefundRepository.GetExternalReconciliationsAsync(
                "org-1", null, "Open",
                A<PaginationInputParam>.That.Matches(value => value.First == pagination.First && value.After == pagination.After),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
