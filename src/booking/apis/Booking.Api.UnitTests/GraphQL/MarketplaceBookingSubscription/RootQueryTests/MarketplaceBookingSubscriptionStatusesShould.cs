using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionStatusesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Subscription_Status_Options(RootQuery sut)
    {
        var result = sut.MarketplaceBookingSubscriptionStatuses().ToList();

        result.Count.ShouldBe(5);
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.Active &&
            !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.Cancelled &&
            !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.Expired &&
            !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.RenewalFailed &&
            !string.IsNullOrWhiteSpace(item.Name));
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.Paused &&
            !string.IsNullOrWhiteSpace(item.Name));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Status_Options_With_Correct_Names(RootQuery sut)
    {
        var result = sut.MarketplaceBookingSubscriptionStatuses().ToList();

        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.Active &&
            item.Name == MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatusName());
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionStatus.RenewalFailed &&
            item.Name == MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatusName());
    }
}
