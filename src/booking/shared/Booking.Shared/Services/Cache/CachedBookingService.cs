using Api.Shared.Services;
using Booking.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;

namespace Booking.Shared.Services.Cache;

public interface ICachedBookingService
{
    ValueTask<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedBookingService(ApplicationConfiguration applicationConfiguration, IRepositoryFactory repositoryFactory, HybridCache hybridCache)
    : ICachedBookingService
{
    public async ValueTask<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.BookingRepository.GetByIdUntrackedAsync(id, ct) ?? throw new BookingNotFound(),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromSeconds(30),
                },
                cancellationToken: cancellationToken);
        }
        catch (BookingNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken)
    {
        await RemoveByIdAsync(id, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.BookingRepository.GetByIdUntrackedAsync(id, cancellationToken) ?? throw new BookingNotFound(),
            new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromSeconds(30),
            },
            cancellationToken: cancellationToken);
    }


    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:booking-id:{id}";
}
