using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICachedTeamService
{
    Task<Team> GetByIdAsync(string id, CancellationToken cancellationToken);
    void CleanCache(string id);
}

public class CachedTeamService(IRepositoryFactory repositoryFactory, IMemoryCache memoryCache) : ICachedTeamService
{
    public async Task<Team> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var team = await memoryCache.GetOrCreateAsync<Team>($"team-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);
                var team = await repositoryFactory.TeamRepository.GetByIdAsync(id, cancellationToken);
                if (team is null)
                {
                    throw new TeamNotFound();
                }

                return team;
            });

        if (team is null)
        {
            throw new TeamNotFound();
        }

        return team;
    }

    public void CleanCache(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        memoryCache.Remove($"team-id-{id}");
    }
}
