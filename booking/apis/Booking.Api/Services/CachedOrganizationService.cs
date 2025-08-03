using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICachedOrganizationService
{
    Task<Organization> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task CleanCacheAsync(string id, CancellationToken cancellationToken);
}

public class CachedOrganizationService(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache)
    : ICachedOrganizationService
{
    public async Task<Organization> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var organization = await memoryCache.GetOrCreateAsync<Organization>($"organization-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return await repositoryFactory.OrganizationRepository.GetByIdAsync(id, false, false, cancellationToken) ??
                       throw new OrganizationNotFound();
            });

        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organization;
    }

    public Task CleanCacheAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        memoryCache.Remove($"organization-id-{id}");

        return Task.CompletedTask;
    }
}
