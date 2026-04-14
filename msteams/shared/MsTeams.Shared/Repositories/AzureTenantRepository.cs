using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureTenantRepository : IRepository<AzureTenant>
{
    Task<AzureTenant> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<AzureTenant>> GetAllAsync(CancellationToken cancellationToken);
    AzureTenant Add(AzureTenant azureTenant);
    AzureTenant Update(AzureTenant azureTenant);
}

internal static class AzureTenantExtensions
{
    extension(IQueryable<AzureTenant> originalQuery)
    {
        internal IIncludableQueryable<AzureTenant, Organization> AddDependentObjects() =>
            originalQuery
                .Include(query => query.AzureTenantTeams)
                .ThenInclude(query => query.AzureTenantTeamChannels)
                .Include(query => query.Organization);
    }
}

public class AzureTenantRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureTenant>(dbContext, timeProvider), IAzureTenantRepository
{
    public async Task<AzureTenant> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureTenant
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<AzureTenant>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.AzureTenant
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

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
