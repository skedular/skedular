using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICachedOrganizationService
{
    Task<Organization> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);
}

public class CachedOrganizationService(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache) : ICachedOrganizationService
{
    public async Task<Organization> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var organization = await memoryCache.GetOrCreateAsync<Organization>($"organization-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                           id,
                           organizationUniqueAlphanumericName,
                           false,
                           false,
                           cancellationToken) ??
                       throw new OrganizationNotFound();
            });

        return organization ?? throw new OrganizationNotFound();
    }
}
