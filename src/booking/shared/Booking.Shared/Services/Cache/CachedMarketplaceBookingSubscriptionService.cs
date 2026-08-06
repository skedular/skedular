using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;

namespace Booking.Shared.Services.Cache;

public interface ICachedMarketplaceBookingSubscriptionService
{
    ValueTask<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedMarketplaceBookingSubscriptionService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache)
    : ICachedMarketplaceBookingSubscriptionService
{
    public async ValueTask<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdUntrackedAsync(id, ct) ??
                            throw new MarketplaceBookingSubscriptionNotFound(),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromSeconds(30),
                },
                cancellationToken: cancellationToken);
        }
        catch (MarketplaceBookingSubscriptionNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken)
    {
        await RemoveByIdAsync(id, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdUntrackedAsync(id, cancellationToken) ??
            throw new MarketplaceBookingSubscriptionNotFound(),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromSeconds(30),
            },
            cancellationToken: cancellationToken);
    }


    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private string CreateKeyById(string id) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:marketplace-booking-subscription-id:{id}";
}
