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

internal static class TenantExtensions
{
    internal static IIncludableQueryable<AzureTenant, ICollection<AzureTenantMember>> AddDependentObjects(
        this IQueryable<AzureTenant> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers)
            .ThenInclude(query => query.Customer)
            .Include(query => query.AzureTenantMembers);
}

public class AzureTenantRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureTenant>(dbContext), IAzureTenantRepository
{
    public async Task<AzureTenant> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.AzureTenant.Add(new AzureTenant { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureTenant
            .AddDependentObjects()
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public AzureTenant Add(AzureTenant azureTenant)
    {
        var now = timeProvider.GetUtcNow();
        azureTenant.CreatedAt = now;
        return DbContext.AzureTenant.Add(azureTenant).Entity;
    }

    public AzureTenant Update(AzureTenant azureTenant)
    {
        var now = timeProvider.GetUtcNow();
        azureTenant.ModifiedAt = now;
        return DbContext.AzureTenant.Update(azureTenant).Entity;
    }
}
