using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Pagination;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionsWithStatusFilterShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Pass_Statuses_From_SearchCriteria_To_Repository(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, [], [],
            [MarketplaceBookingSubscriptionStatus.Active],
            []);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository)
            .Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>.That.Matches(c =>
                    c.Statuses != null && c.Statuses.Contains(MarketplaceBookingSubscriptionStatus.Active)),
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<MarketplaceBookingSubscriptionAccessScope?>._,
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedMarketplaceBookingSubscriptionsAsync(
            new PaginationInputParam(null, null, null, null),
            searchCriteria,
            [],
            true,
            cancellationToken);

        result.Item3.ShouldBe(0);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>.That.Matches(c =>
                    c.Statuses != null && c.Statuses.Contains(MarketplaceBookingSubscriptionStatus.Active)),
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<MarketplaceBookingSubscriptionAccessScope?>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Pass_Empty_Statuses_From_SearchCriteria_To_Repository(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = new MarketplaceBookingSubscriptionSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, [], [], [], []);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository)
            .Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>.That.Matches(c => c.Statuses == null || c.Statuses.Count == 0),
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<MarketplaceBookingSubscriptionAccessScope?>._,
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedMarketplaceBookingSubscriptionsAsync(
            new PaginationInputParam(null, null, null, null),
            searchCriteria,
            [],
            true,
            cancellationToken);

        result.Item3.ShouldBe(0);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>.That.Matches(c => c.Statuses == null || c.Statuses.Count == 0),
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<MarketplaceBookingSubscriptionAccessScope?>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
