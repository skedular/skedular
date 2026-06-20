using System.Runtime.CompilerServices;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Location.Api.Services;
using Location.Shared.GraphQL;

namespace Location.Api.GraphQL.Location;

[SubscriptionType]
public class RootSubscription
{
    public async IAsyncEnumerable<HostListingProductReadyDetails> OnListingProductReady(
        string locationId,
        [Service] ITopicEventReceiver topicEventReceiver,
        [Service] IServiceProvider serviceProvider,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(Constants.ListingProductReadyTopicName, cancellationToken);

        yield return await GetListingProductReadyAsync(locationId, serviceProvider, cancellationToken);

        await foreach (var _ in sourceStream.ReadEventsAsync().Where(item => item == locationId).WithCancellation(cancellationToken))
        {
            yield return await GetListingProductReadyAsync(locationId, serviceProvider, cancellationToken);
        }
    }

    [UseResolverScope]
    [Subscribe(With = nameof(OnListingProductReady))]
    public HostListingProductReadyDetails ListingProductReady([EventMessage] HostListingProductReadyDetails item) => item;

    private static async Task<HostListingProductReadyDetails> GetListingProductReadyAsync(
        string locationId,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var locationService = scope.ServiceProvider.GetRequiredService<ILocationService>();
        var location = await locationService.GetByIdAsync(locationId, false, cancellationToken);
        var product = location?.PrecomputedLocationProducts.Select(item => item.Product).FirstOrDefault();

        return new HostListingProductReadyDetails
        {
            LocationId = locationId,
            Product = product is null
                ? null
                : new HostListingProductReadyProductDetails { Id = product.Id, Inactive = product.Inactive }
        };
    }
}
