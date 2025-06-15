using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICachedTeamService
{
    Task<Team> GetByIdAsync(string id, CancellationToken cancellationToken);
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
                return await repositoryFactory.TeamRepository.GetByIdAsync(id, false, cancellationToken) ?? throw new TeamNotFound();
            });

        if (team is null)
        {
            throw new TeamNotFound();
        }

        return team;
    }
}
