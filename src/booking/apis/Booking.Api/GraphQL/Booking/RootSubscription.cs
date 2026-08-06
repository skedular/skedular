using System.Runtime.CompilerServices;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.GraphQL;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[SubscriptionType]
public class RootSubscription(IGraphQlMapper graphQlMapper)
{
    public async IAsyncEnumerable<BookingDetails> OnBookingUpdated(
        string id,
        [Service]
        ITopicEventReceiver topicEventReceiver,
        [Service]
        IServiceProvider serviceProvider,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(Constants.BookingTopicName, cancellationToken);

        yield return await GetBookingByIdAsync(id, serviceProvider, cancellationToken);

        await foreach (var _ in sourceStream.ReadEventsAsync().Where(item => item == id).WithCancellation(cancellationToken))
        {
            yield return await GetBookingByIdAsync(id, serviceProvider, cancellationToken);
        }
    }

    [UseResolverScope]
    [Subscribe(With = nameof(OnBookingUpdated))]
    public BookingDetails Booking([EventMessage] BookingDetails item) => item;

    private async Task<BookingDetails> GetBookingByIdAsync(string id, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        return graphQlMapper.MapTo(await bookingService.GetByIdAsync(id, cancellationToken));
    }
}
