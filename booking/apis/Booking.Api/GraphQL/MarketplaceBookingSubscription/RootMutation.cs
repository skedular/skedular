using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[MutationType]
public class RootMutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionPayload> AddMarketplaceBookingSubscriptionAsync(
        AddMarketplaceBookingSubscriptionInput input,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var subscription = await marketplaceBookingSubscriptionService.AddAsync(mapper.MapTo(input), cancellationToken);
        return new MarketplaceBookingSubscriptionPayload
        {
            ClientMutationId = input.ClientMutationId, MarketplaceBookingSubscription = mapper.MapTo(subscription)
        };
    }

    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionPayload> DeleteMarketplaceBookingSubscriptionAsync(
        DeleteMarketplaceBookingSubscriptionInput input,
        [Service] IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var subscription = await marketplaceBookingSubscriptionService.DeleteAsync(input.Id, input.CancellationMode, cancellationToken);
        return new MarketplaceBookingSubscriptionPayload
        {
            ClientMutationId = input.ClientMutationId, MarketplaceBookingSubscription = mapper.MapTo(subscription)
        };
    }
}
