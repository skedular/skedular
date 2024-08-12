using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Tenant?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Tenant Add(Tenant tenant);
    Tenant Update(Tenant tenant);
}

internal static class TenantExtensions
{
    internal static IIncludableQueryable<Tenant, ICollection<TenantMember>> AddDependentObjects(
        this IQueryable<Tenant> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers)
            .ThenInclude(query => query.Customer)
            .Include(query => query.TenantMembers);
}

public class TenantRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, Tenant>(dbContext), ITenantRepository
{
    public async Task<Tenant> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Tenant.Add(new Tenant { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Tenant?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Tenant
            .AddDependentObjects()
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Tenant Add(Tenant tenant)
    {
        var now = timeProvider.GetUtcNow();
        tenant.CreatedAt = now;
        return DbContext.Tenant.Add(tenant).Entity;
    }

    public Tenant Update(Tenant tenant)
    {
        var now = timeProvider.GetUtcNow();
        tenant.ModifiedAt = now;
        return DbContext.Tenant.Update(tenant).Entity;
    }
}
