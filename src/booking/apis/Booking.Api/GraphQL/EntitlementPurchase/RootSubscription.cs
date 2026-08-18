using System.Runtime.CompilerServices;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.GraphQL;
using Booking.Shared.Services.Cache;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.EntitlementPurchase;

[SubscriptionType]
public sealed class RootSubscription
{
    public async IAsyncEnumerable<EntitlementPurchaseDetails> OnEntitlementPurchaseUpdated(
        string purchaseId,
        [Service]
        ITopicEventReceiver topicEventReceiver,
        [Service]
        IServiceProvider serviceProvider,
        [EnumeratorCancellation]
        CancellationToken cancellationToken)
    {
        var sourceStream = await topicEventReceiver.SubscribeAsync<string>(Constants.EntitlementPurchaseTopicName, cancellationToken);

        var initialPurchase = await GetAuthorizedPurchaseAsync(purchaseId, serviceProvider, cancellationToken);
        if (initialPurchase is not null)
        {
            yield return initialPurchase;
        }

        await foreach (var changedPurchaseId in sourceStream.ReadEventsAsync().WithCancellation(cancellationToken))
        {
            if (changedPurchaseId != purchaseId)
            {
                continue;
            }

            var purchase = await GetAuthorizedPurchaseAsync(purchaseId, serviceProvider, cancellationToken);
            if (purchase is not null)
            {
                yield return purchase;
            }
        }
    }

    [UseResolverScope]
    [Subscribe(With = nameof(OnEntitlementPurchaseUpdated))]
    public EntitlementPurchaseDetails EntitlementPurchase([EventMessage] EntitlementPurchaseDetails item) => item;

    private static async Task<EntitlementPurchaseDetails?> GetAuthorizedPurchaseAsync(
        string purchaseId,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
        var purchaseReadService = scope.ServiceProvider.GetRequiredService<IEntitlementPurchaseReadService>();
        var graphQlMapper = scope.ServiceProvider.GetRequiredService<IEntitlementPurchaseGraphQlMapper>();
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var purchase = await purchaseReadService.GetAuthorizedAsync(purchaseId, customerId, cancellationToken);
        return purchase is null ? null : graphQlMapper.Map(purchase);
    }
}
