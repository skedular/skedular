using Booking.Shared.GraphQL;
using Booking.Shared.Services.Cache;
using HotChocolate.Subscriptions;

namespace Booking.Api.Services;

public interface IGraphQlHelperService
{
    Task RaiseGraphqlChange(string topicName, string id, CancellationToken cancellationToken);
}

public class GraphQlHelperService(ITopicEventSender topicEventSender, ICachedBookingService cachedBookingService) : IGraphQlHelperService
{
    public async Task RaiseGraphqlChange(string topicName, string id, CancellationToken cancellationToken)
    {
        if (topicName == Constants.BookingTopicName)
        {
            await cachedBookingService.RemoveByIdAsync(id, cancellationToken);
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);
    }
}
