using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplacePurchaseHistory;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;

namespace Booking.Api.UnitTests.GraphQL.MarketplacePurchaseHistory.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplacePurchasesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Stable_Cursor_Pages([Frozen] IGraphQlMapper graphQlMapper, RootQuery sut, CancellationToken cancellationToken)
    {
        ConfigureMapper(graphQlMapper);
        var service = new StubHistoryService([
            Create("one", TimeProvider.System.GetUtcNow().AddMinutes(-2)),
            Create("two", TimeProvider.System.GetUtcNow().AddMinutes(-1)),
        ]);

        var first = await sut.MarketplacePurchasesAsync(null, 1, null, null, new MarketplacePurchaseHistoryWhereInput(), null,
            service, cancellationToken);
        var second = await sut.MarketplacePurchasesAsync(first.PageInfo.EndCursor, 1, null, null, new MarketplacePurchaseHistoryWhereInput(), null,
            service, cancellationToken);

        first.Edges.Single().Node.Id.ShouldBe("marketplace-purchase-history:Booking:one");
        first.Edges.Single().Node.SourceId.ShouldBe("one");
        first.PageInfo.HasNextPage.ShouldBeTrue();
        second.Edges.Single().Node.Id.ShouldBe("marketplace-purchase-history:Booking:two");
        second.Edges.Single().Node.SourceId.ShouldBe("two");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Inactive_History_Evidence([Frozen] IGraphQlMapper graphQlMapper, RootQuery sut, CancellationToken cancellationToken)
    {
        ConfigureMapper(graphQlMapper);
        var service = new StubHistoryService([
            Create("deleted-booking", TimeProvider.System.GetUtcNow()) with
            {
                LifecycleState = MarketplacePurchaseLifecycleState.Deleted,
                IsDeleted = true,
                DeletedByCustomerId = "operator-1",
                CancellationReason = "Customer request",
            },
        ]);

        var result = await sut.MarketplacePurchasesAsync(null, 10, null, null, new MarketplacePurchaseHistoryWhereInput(), null,
            service, cancellationToken);

        result.Edges.Single().Node.IsDeleted.ShouldBeTrue();
        result.Edges.Single().Node.DeletedByCustomerId.ShouldBe("operator-1");
        result.Edges.Single().Node.CancellationReason.ShouldBe("Customer request");
        result.Edges.Single().Node.LifecycleState.ShouldBe(MarketplacePurchaseLifecycleState.Deleted);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Forward_Filter_And_Order_Inputs_To_Service([Frozen] IGraphQlMapper graphQlMapper, RootQuery sut,
        CancellationToken cancellationToken)
    {
        ConfigureMapper(graphQlMapper);
        var service = new StubHistoryService([Create("booking-1", TimeProvider.System.GetUtcNow())])
        {
            CaptureInputs = true,
        };
        var order = new MarketplacePurchaseHistoryOrderInput
        {
            Field = MarketplacePurchaseHistoryOrderField.BookingUntil,
            Direction = OrderDirection.Ascending,
        };

        await sut.MarketplacePurchasesAsync(
            null,
            10,
            null,
            null,
            new MarketplacePurchaseHistoryWhereInput
            {
                OrganizationCustomDomain = "example.test",
                SourceTypes = [MarketplacePurchaseSourceType.Booking],
                LifecycleStates = [MarketplacePurchaseLifecycleState.Deleted],
                PaymentStatuses = [PaymentStatus.Confirmed],
                CustomerId = "customer-1",
                ProductVersionId = "product-1",
            },
            [order],
            service,
            cancellationToken);

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

    private static void ConfigureMapper(IGraphQlMapper graphQlMapper) =>
        A.CallTo(() => graphQlMapper.MapTo(A<MarketplacePurchaseHistoryEntry>._))
            .ReturnsLazily((MarketplacePurchaseHistoryEntry entry) => new MarketplacePurchaseHistoryDetails
            {
                Id = $"marketplace-purchase-history:{entry.SourceType}:{entry.Id}",
                SourceId = entry.Id,
                SourceType = entry.SourceType,
                SourceTypeName = entry.SourceTypeName,
                LifecycleState = entry.LifecycleState,
                LifecycleStateName = entry.LifecycleStateName,
                RenewalState = entry.RenewalState,
                RenewalStateName = entry.RenewalStateName,
                PurchasedAt = entry.PurchasedAt,
                ActivityAt = entry.ActivityAt,
                PaymentStatus = entry.PaymentStatus,
                IsDeleted = entry.IsDeleted,
                DeletedByCustomerId = entry.DeletedByCustomerId,
                CancellationReason = entry.CancellationReason,
            });

    private sealed class StubHistoryService(IReadOnlyList<MarketplacePurchaseHistoryEntry> entries) : IMarketplacePurchaseHistoryService
    {
        public bool CaptureInputs { get; set; }
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
