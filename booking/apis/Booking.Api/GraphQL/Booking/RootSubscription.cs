using System.Runtime.CompilerServices;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.GraphQL;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[SubscriptionType]
public class RootSubscription(IServiceProvider serviceProvider, IMapper mapper)
{
    public async IAsyncEnumerable<BookingDetails> OnBookingUpdated(
        string id,
        [Service] ITopicEventReceiver topicEventReceiver,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(Constants.BookingTopicName, cancellationToken);

        yield return await GetBookingByIdAsync(id, cancellationToken);

        await foreach (var _ in sourceStream.ReadEventsAsync().Where(item => item == id).WithCancellation(cancellationToken))
        {
            yield return await GetBookingByIdAsync(id, cancellationToken);
        }
    }

    [UseResolverScope]
    [Subscribe(With = nameof(OnBookingUpdated))]
    public BookingDetails Booking([EventMessage] BookingDetails item) => item;

    private async Task<BookingDetails> GetBookingByIdAsync(string id, CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        return mapper.MapTo(await bookingService.GetByIdAsync(id, cancellationToken));
    }
}
