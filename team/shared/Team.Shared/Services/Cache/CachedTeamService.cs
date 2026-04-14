using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Team.Shared.Repositories;

namespace Team.Shared.Services.Cache;

public interface ICachedTeamService
{
    ValueTask<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateAsync(ICollection<Database.Entities.Team> teams, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
}

public class CachedTeamService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    HybridCache hybridCache,
    ILogger<CachedTeamService> logger)
    : ICachedTeamService
{
    public async ValueTask<Database.Entities.Team?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.TeamRepository.GetByIdUntrackedAsync(id, ct) ?? throw new TeamNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (TeamNotFound)
        {
            logger.LogDebug("Team lookup returned no result for team {TeamId}", id);
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken)
    {
        await RemoveByIdAsync(id, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.TeamRepository.GetByIdUntrackedAsync(id, cancellationToken) ?? throw new TeamNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);

        logger.LogDebug("Cache refresh for team {TeamId}", id);
    }

    public async ValueTask UpdateAsync(ICollection<Database.Entities.Team> teams, CancellationToken cancellationToken)
    {
        foreach (var item in teams)
        {
            await RemoveByIdAsync(item.Id, cancellationToken);

            await hybridCache.SetAsync(
                CreateKeyById(item.Id),
                item,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            logger.LogDebug("Cache refresh for team {TeamId}", item.Id);
        }
    }

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken)
    {
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);
        logger.LogDebug("Cache eviction for team {TeamId}", id);
    }

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:team-id:{id}";
}
