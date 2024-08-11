using Ardalis.GuardClauses;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Repositories;

namespace MsTeams.Api.Services;

public interface ITenantService
{
    Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken);
}

public class TenantService(
    IRepositoryFactory repositoryFactory,
    IContext context,
    IMemoryCache memoryCache) : ITenantService
{
    public async Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);

        var tenantId = context.PropertyBag.AzureTenantId;
        var key = $"tenant-exists-{tenantId}";
        if (memoryCache.Get<bool>(key))
        {
            return true;
        }

        memoryCache.Remove(key);
        return await memoryCache.GetOrCreateAsync(
            key,
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                return await repositoryFactory.TenantRepository.Query(
                    new Specification<Tenant>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Id == tenantId.ToString()
                    }).AsNoTracking().AnyAsync(cancellationToken);
            });
    }
}
