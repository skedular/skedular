using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Mappers;
using Booking.Api.Services;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddMarketplaceBookingSubscriptionShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Forward_The_Mapped_Weekly_Selected_Days_To_The_Subscription_Service(
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        RootMutation sut,
        CancellationToken cancellationToken)
    {
        var input = new AddMarketplaceBookingSubscriptionInput { WeeklySelectedDays = [DayOfWeek.Tuesday, DayOfWeek.Thursday] };
        var subscription = new Shared.Models.MarketplaceBookingSubscription { WeeklySelectedDays = [DayOfWeek.Tuesday, DayOfWeek.Thursday] };
        var details = new MarketplaceBookingSubscriptionDetails();
        var expectedDays = new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday };
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(subscription);
        A.CallTo(() => marketplaceBookingSubscriptionService.AddAsync(subscription, cancellationToken)).Returns(subscription);
        A.CallTo(() => graphQlMapper.MapTo(subscription)).Returns(details);

        var result = await sut.AddMarketplaceBookingSubscriptionAsync(input, marketplaceBookingSubscriptionService, cancellationToken);

        result.MarketplaceBookingSubscription.ShouldBeSameAs(details);
        A.CallTo(() => marketplaceBookingSubscriptionService.AddAsync(
                A<Shared.Models.MarketplaceBookingSubscription>.That.Matches(item =>
                    item.WeeklySelectedDays.SequenceEqual(expectedDays)),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
