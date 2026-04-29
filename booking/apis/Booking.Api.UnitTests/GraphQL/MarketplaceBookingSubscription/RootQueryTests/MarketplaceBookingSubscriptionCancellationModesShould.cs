using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Mappers;
using Microsoft.Extensions.Logging;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionCancellationModesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Available_Cancellation_Modes(
        IMapper mapper,
        ILogger<RootQuery> logger)
    {
        var sut = new RootQuery(mapper, logger);

        var result = sut.MarketplaceBookingSubscriptionCancellationModes().ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionCancellationMode.Immediate &&
            item.Name == MarketplaceBookingSubscriptionCancellationMode.Immediate.ToMarketplaceBookingSubscriptionCancellationModeName());
        result.ShouldContain(item =>
            item.Type == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd &&
            item.Name == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd.ToMarketplaceBookingSubscriptionCancellationModeName());
    }
}
