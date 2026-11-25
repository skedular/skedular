using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.MarketplaceBookingSubscriptionRefundDataLoaderTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LoadBatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Batch_Existing_And_Missing_Refunds_In_One_Service_Call(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IMarketplaceRefundReadService marketplaceRefundReadService,
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefundReadModel
        {
            Id = "refund-1",
        };
        var refundDetails = new MarketplaceRefundDetails
        {
            Id = refund.Id,
        };
        string[] keys = ["subscription-1", "subscription-2"];
        A.CallTo(() => marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdsAsync(keys, cancellationToken))
            .Returns(new Dictionary<string, MarketplaceRefundReadModel>
            {
                [keys[0]] = refund,
            });
        A.CallTo(() => graphQlMapper.MapTo(refund)).Returns(refundDetails);
        await using var serviceProvider = new ServiceCollection()
            .AddSingleton(marketplaceRefundReadService)
            .BuildServiceProvider();
        var sut = new TestDataLoader(
            batchScheduler,
            options,
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            graphQlMapper);

        var result = await sut.LoadForTestAsync(keys, cancellationToken);

        result[keys[0]].ShouldBeSameAs(refundDetails);
        result[keys[1]].ShouldBeNull();
        A.CallTo(() => marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdsAsync(keys, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Dispatch_Multiple_Loads_As_One_Batch(
        IMarketplaceRefundReadService marketplaceRefundReadService,
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var scheduler = new ManualBatchScheduler();
        var options = new DataLoaderOptions();
        var refund1 = new MarketplaceRefundReadModel
        {
            Id = "refund-1",
        };
        var refund2 = new MarketplaceRefundReadModel
        {
            Id = "refund-2",
        };
        var details1 = new MarketplaceRefundDetails
        {
            Id = refund1.Id,
        };
        var details2 = new MarketplaceRefundDetails
        {
            Id = refund2.Id,
        };
        A.CallTo(() => marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdsAsync(
                A<IReadOnlyCollection<string>>.That.Matches(ids =>
                    ids.Count == 2 && ids.Contains("subscription-1") && ids.Contains("subscription-2")),
                cancellationToken))
            .Returns(new Dictionary<string, MarketplaceRefundReadModel>
            {
                ["subscription-1"] = refund1,
                ["subscription-2"] = refund2,
            });
        A.CallTo(() => graphQlMapper.MapTo(refund1)).Returns(details1);
        A.CallTo(() => graphQlMapper.MapTo(refund2)).Returns(details2);
        await using var serviceProvider = new ServiceCollection()
            .AddSingleton(marketplaceRefundReadService)
            .BuildServiceProvider();
        var sut = new MarketplaceBookingSubscriptionRefundDataLoader(
            scheduler,
            options,
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            graphQlMapper);

        var first = sut.LoadAsync("subscription-1", cancellationToken);
        var second = sut.LoadAsync("subscription-2", cancellationToken);
        scheduler.ScheduledBatch.ShouldNotBeNull().Size.ShouldBe(2);
        await scheduler.DispatchAsync();

        (await first).ShouldBeSameAs(details1);
        (await second).ShouldBeSameAs(details2);
        A.CallTo(() => marketplaceRefundReadService.GetByMarketplaceBookingSubscriptionIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private sealed class ManualBatchScheduler : IBatchScheduler
    {
        public Batch? ScheduledBatch { get; private set; }

        public void Schedule(Batch batch) => ScheduledBatch = batch;

        public Task DispatchAsync() => ScheduledBatch is null
            ? throw new InvalidOperationException("No batch was scheduled.")
            : ScheduledBatch.DispatchAsync();
    }

    private sealed class TestDataLoader(
        IBatchScheduler batchScheduler,
        DataLoaderOptions options,
        IServiceScopeFactory serviceScopeFactory,
        IGraphQlMapper graphQlMapper) : MarketplaceBookingSubscriptionRefundDataLoader(
        batchScheduler,
        options,
        serviceScopeFactory,
        graphQlMapper)
    {
        public Task<IReadOnlyDictionary<string, MarketplaceRefundDetails?>> LoadForTestAsync(
            IReadOnlyList<string> keys,
            CancellationToken cancellationToken) => LoadBatchAsync(keys, cancellationToken);
    }
}
