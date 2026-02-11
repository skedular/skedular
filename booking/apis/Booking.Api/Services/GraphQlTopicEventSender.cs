using Booking.Shared.GraphQL;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.GraphQL;
using HotChocolate.Subscriptions;

namespace Booking.Api.Services;

public class GraphQlTopicEventSender(ITopicEventSender topicEventSender, ICachedBookingService cachedBookingService) : IGraphQlTopicEventSender
{
    public async Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken)
    {
        if (topicName == Constants.BookingTopicName)
        {
            await cachedBookingService.RemoveByIdAsync(id, cancellationToken);
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);
    }
}
