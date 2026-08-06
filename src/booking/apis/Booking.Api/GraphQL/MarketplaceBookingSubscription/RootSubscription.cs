using System.Runtime.CompilerServices;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.GraphQL;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[SubscriptionType]
public class RootSubscription(IGraphQlMapper graphQlMapper)
{
    public async IAsyncEnumerable<MarketplaceBookingSubscriptionDetails> OnMarketplaceBookingSubscriptionUpdated(
        string id,
        [Service]
        ITopicEventReceiver topicEventReceiver,
        [Service]
        IServiceProvider serviceProvider,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(Constants.MarketplaceBookingSubscriptionTopicName, cancellationToken);

        yield return await GetMarketplaceBookingSubscriptionByIdAsync(id, serviceProvider, cancellationToken);

        await foreach (var _ in sourceStream.ReadEventsAsync().Where(item => item == id).WithCancellation(cancellationToken))
        {
            yield return await GetMarketplaceBookingSubscriptionByIdAsync(id, serviceProvider, cancellationToken);
        }
    }

    [UseResolverScope]
    [Subscribe(With = nameof(OnMarketplaceBookingSubscriptionUpdated))]
    public MarketplaceBookingSubscriptionDetails MarketplaceBookingSubscription([EventMessage] MarketplaceBookingSubscriptionDetails item) => item;

    private async Task<MarketplaceBookingSubscriptionDetails> GetMarketplaceBookingSubscriptionByIdAsync(
        string id,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var marketplaceBookingSubscriptionService = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingSubscriptionService>();
        return graphQlMapper.MapTo(await marketplaceBookingSubscriptionService.GetByIdAsync(id, cancellationToken));
    }
}
