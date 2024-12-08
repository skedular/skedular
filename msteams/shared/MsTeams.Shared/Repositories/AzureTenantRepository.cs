using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureTenantRepository : IRepository<AzureTenant>
{
    Task<AzureTenant> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken);
    AzureTenant Add(AzureTenant azureTenant);
    AzureTenant Update(AzureTenant azureTenant);
}

internal static class AzureTenantExtensions
{
    internal static IIncludableQueryable<AzureTenant, Organization> AddDependentObjects(
        this IQueryable<AzureTenant> originalQuery) =>
        originalQuery
            .Include(query => query.AzureTenantTeams)
            .ThenInclude(query => query.AzureTenantTeamChannels)
            .Include(query => query.Organization);
}

public class AzureTenantRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureTenant>(dbContext, timeProvider), IAzureTenantRepository
{
    public override async Task<AzureTenant> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureTenant
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public AzureTenant Add(AzureTenant azureTenant)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenant.CreatedAt = now;
        return DbContext.AzureTenant.Add(azureTenant).Entity;
    }

    public AzureTenant Update(AzureTenant azureTenant)
    {
        var now = TimeProvider.GetUtcNow();
        azureTenant.ModifiedAt = now;
        return DbContext.AzureTenant.Update(azureTenant).Entity;
    }
}
