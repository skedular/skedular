using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Shared.UnitTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class FilterByStatusesShould
{
    private static IQueryable<MarketplaceBookingSubscription> BuildQueryable(
        IEnumerable<string> statuses) =>
        statuses
            .Select(status => new MarketplaceBookingSubscription { Status = status })
            .AsQueryable();

    private static MarketplaceBookingSubscriptionSearchCriteria CriteriaWith(
        IReadOnlyList<MarketplaceBookingSubscriptionStatus> statuses) =>
        new(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            [],
            [],
            statuses,
            []);

    [Fact]
    public void Return_All_When_Statuses_Is_Empty()
    {
        var queryable = BuildQueryable([
            MarketplaceBookingSubscriptionStatusConstants.Active,
            MarketplaceBookingSubscriptionStatusConstants.Cancelled,
            MarketplaceBookingSubscriptionStatusConstants.Paused
        ]);

        var result = queryable.AddSearchCriteria(CriteriaWith([]), null).ToList();

        result.Count.ShouldBe(3);
    }

    [Fact]
    public void Return_Only_Matching_When_Single_Status_Provided()
    {
        var queryable = BuildQueryable([
            MarketplaceBookingSubscriptionStatusConstants.Active,
            MarketplaceBookingSubscriptionStatusConstants.Cancelled,
            MarketplaceBookingSubscriptionStatusConstants.Active
        ]);

        var result = queryable
            .AddSearchCriteria(CriteriaWith([MarketplaceBookingSubscriptionStatus.Active]), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldAllBe(item => item.Status == MarketplaceBookingSubscriptionStatusConstants.Active);
    }

    [Fact]
    public void Return_Union_When_Multiple_Statuses_Provided()
    {
        var queryable = BuildQueryable([
            MarketplaceBookingSubscriptionStatusConstants.Active,
            MarketplaceBookingSubscriptionStatusConstants.Cancelled,
            MarketplaceBookingSubscriptionStatusConstants.Paused,
            MarketplaceBookingSubscriptionStatusConstants.Expired
        ]);

        var result = queryable
            .AddSearchCriteria(CriteriaWith([
                MarketplaceBookingSubscriptionStatus.Active,
                MarketplaceBookingSubscriptionStatus.Paused
            ]), null)
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item => item.Status == MarketplaceBookingSubscriptionStatusConstants.Active);
        result.ShouldContain(item => item.Status == MarketplaceBookingSubscriptionStatusConstants.Paused);
    }
}
