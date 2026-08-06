using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplacePurchaseHistory;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;

namespace Booking.Api.UnitTests.GraphQL.MarketplacePurchaseHistory.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchasesShould
{
    [Fact]
    public async Task Return_Stable_Cursor_Pages()
    {
        var service = new StubHistoryService([
            Create("one", TimeProvider.System.GetUtcNow().AddMinutes(-2)),
            Create("two", TimeProvider.System.GetUtcNow().AddMinutes(-1)),
        ]);
        var query = new RootQuery();
        var graphQlMapper = new GraphQlMapper(A.Fake<IEntityMapper>());

        var first = await query.MarketplacePurchasesAsync(null, 1, null, null, null, null, null, null, null, null, null, null, null, null, null,
            service, graphQlMapper, CancellationToken.None);
        var second = await query.MarketplacePurchasesAsync(first.PageInfo.EndCursor, 1, null, null, null, null, null, null, null, null, null, null,
            null, null, null, service, graphQlMapper, CancellationToken.None);

        first.Edges.Single().Node.Id.ShouldBe("marketplace-purchase-history:Booking:one");
        first.Edges.Single().Node.SourceId.ShouldBe("one");
        first.PageInfo.HasNextPage.ShouldBeTrue();
        second.Edges.Single().Node.Id.ShouldBe("marketplace-purchase-history:Booking:two");
        second.Edges.Single().Node.SourceId.ShouldBe("two");
    }

    [Fact]
    public async Task Return_Inactive_History_Evidence()
    {
        var service = new StubHistoryService([
            Create("deleted-booking", TimeProvider.System.GetUtcNow()) with
            {
                LifecycleState = MarketplacePurchaseLifecycleState.Deleted,
                IsDeleted = true,
                DeletedByCustomerId = "operator-1",
                CancellationReason = "Customer request",
            },
        ]);
        var query = new RootQuery();
        var graphQlMapper = new GraphQlMapper(A.Fake<IEntityMapper>());

        var result = await query.MarketplacePurchasesAsync(null, 10, null, null, null, null, null, null, null, null, null, null, null, null, null,
            service, graphQlMapper, CancellationToken.None);

        result.Edges.Single().Node.IsDeleted.ShouldBeTrue();
        result.Edges.Single().Node.DeletedByCustomerId.ShouldBe("operator-1");
        result.Edges.Single().Node.CancellationReason.ShouldBe("Customer request");
        result.Edges.Single().Node.LifecycleState.ShouldBe(MarketplacePurchaseLifecycleState.Deleted);
    }

    [Fact]
    public async Task Forward_Filter_And_Order_Inputs_To_Service()
    {
        var service = new StubHistoryService([Create("booking-1", TimeProvider.System.GetUtcNow())])
        {
            CaptureInputs = true,
        };
        var query = new RootQuery();
        var graphQlMapper = new GraphQlMapper(A.Fake<IEntityMapper>());
        var order = new MarketplacePurchaseHistoryOrderInput
        {
            Field = MarketplacePurchaseHistoryOrderField.BookingUntil,
            Direction = OrderDirection.Ascending,
        };

        await query.MarketplacePurchasesAsync(
            null,
            10,
            null,
            null,
            "example.test",
            [MarketplacePurchaseSourceType.Booking],
            [MarketplacePurchaseLifecycleState.Deleted],
            [PaymentStatus.Confirmed],
            "customer-1",
            "product-1",
            null,
            null,
            null,
            null,
            [order],
            service,
            graphQlMapper,
            CancellationToken.None);

        service.Criteria!.OrganizationCustomDomain.ShouldBe("example.test");
        service.Criteria.CustomerId.ShouldBe("customer-1");
        service.Criteria.ProductVersionId.ShouldBe("product-1");
        service.Criteria.SourceTypes.ShouldBe([MarketplacePurchaseSourceType.Booking]);
        service.Criteria.LifecycleStates.ShouldBe([MarketplacePurchaseLifecycleState.Deleted]);
        service.Criteria.PaymentStatuses.ShouldBe([PaymentStatus.Confirmed]);
        service.Orders!.Single().Field.ShouldBe(MarketplacePurchaseHistoryOrderField.BookingUntil);
        service.Orders!.Single().Direction.ShouldBe(OrderDirection.Ascending);
    }

    private static MarketplacePurchaseHistoryEntry Create(string id, DateTimeOffset activityAt) => new(
        id,
        MarketplacePurchaseSourceType.Booking,
        MarketplacePurchaseLifecycleState.Active,
        MarketplacePurchaseRenewalState.NotApplicable,
        activityAt,
        activityAt,
        null,
        null,
        PaymentStatus.Confirmed,
        null,
        null,
        null,
        null,
        null,
        false);

    private sealed class StubHistoryService(IReadOnlyList<MarketplacePurchaseHistoryEntry> entries) : IMarketplacePurchaseHistoryService
    {
        public bool CaptureInputs { get; init; }
        public MarketplacePurchaseHistorySearchCriteria? Criteria { get; private set; }
        public IReadOnlyList<MarketplacePurchaseHistoryOrder>? Orders { get; private set; }

        public Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryEntry>>, int)> GetPaginatedAsync(
            PaginationInputParam paginationInputParam,
            string? organizationCustomDomain,
            MarketplacePurchaseHistorySearchCriteria searchCriteria,
            IReadOnlyList<MarketplacePurchaseHistoryOrder>? orderBy,
            CancellationToken cancellationToken)
        {
            if (CaptureInputs)
            {
                Criteria = searchCriteria;
                Orders = orderBy;
            }

            var start = paginationInputParam.After == "cursor-one" ? 1 : 0;
            var pageSize = paginationInputParam.First ?? entries.Count;
            var page = entries.Skip(start).Take(pageSize).Select(item => new Edge<MarketplacePurchaseHistoryEntry>(item, $"cursor-{item.Id}"))
                .ToList();
            return Task.FromResult<(PaginatedInfo, IReadOnlyList<Edge<MarketplacePurchaseHistoryEntry>>, int)>(
                (new PaginatedInfo(start + page.Count < entries.Count, start > 0, page.FirstOrDefault()?.Cursor, page.LastOrDefault()?.Cursor), page,
                    entries.Count));
        }
    }
}
