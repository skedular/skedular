using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Shared.GraphQL;
using Booking.Shared.Services.Cache;
using FakeItEasy;
using HotChocolate.Subscriptions;
using Testing.Shared;

namespace Booking.Api.UnitTests.Services.GraphQlTopicEventSenderTests;

public class RaiseGraphqlChangeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Remove_Booking_Cache_And_Send_Event_When_Topic_Is_Booking(
        [Frozen] ITopicEventSender topicEventSender,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
        GraphQlTopicEventSender sut,
        string id,
        CancellationToken cancellationToken)
    {
        await sut.RaiseGraphqlChangeAsync(Constants.BookingTopicName, id, cancellationToken);

        A.CallTo(() => cachedBookingService.RemoveByIdAsync(id, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedMarketplaceBookingSubscriptionService.RemoveByIdAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => topicEventSender.SendAsync(Constants.BookingTopicName, id, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Remove_Subscription_Cache_And_Send_Event_When_Topic_Is_Marketplace_Booking_Subscription(
        [Frozen] ITopicEventSender topicEventSender,
        [Frozen] ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
        [Frozen] ICachedBookingService cachedBookingService,
        GraphQlTopicEventSender sut,
        string id,
        CancellationToken cancellationToken)
    {
        await sut.RaiseGraphqlChangeAsync(Constants.MarketplaceBookingSubscriptionTopicName, id, cancellationToken);

        A.CallTo(() => cachedMarketplaceBookingSubscriptionService.RemoveByIdAsync(id, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedBookingService.RemoveByIdAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => topicEventSender.SendAsync(Constants.MarketplaceBookingSubscriptionTopicName, id, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Only_Send_Event_When_Topic_Does_Not_Require_Cache_Invalidation(
        [Frozen] ITopicEventSender topicEventSender,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
        GraphQlTopicEventSender sut,
        string topicName,
        string id,
        CancellationToken cancellationToken)
    {
        await sut.RaiseGraphqlChangeAsync(topicName, id, cancellationToken);

        A.CallTo(() => cachedBookingService.RemoveByIdAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => cachedMarketplaceBookingSubscriptionService.RemoveByIdAsync(A<string>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => topicEventSender.SendAsync(topicName, id, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
