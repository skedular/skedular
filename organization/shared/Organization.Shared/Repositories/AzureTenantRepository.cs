using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IAzureTenantRepository : IRepository<AzureTenant>
{
    Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<AzureTenant>> GetAllAsync(CancellationToken cancellationToken);
    AzureTenant Add(AzureTenant azureTenant);
    AzureTenant Update(AzureTenant azureTenant);
}

internal static class AzureTenantExtensions
{
    extension(IQueryable<AzureTenant> originalQuery)
    {
        internal IIncludableQueryable<AzureTenant, ICollection<AzureTenantMember>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Organization)
                .ThenInclude(query => query.OrganizationMembers)
                .ThenInclude(query => query.Customer)
                .Include(query => query.AzureTenantMembers);
    }
}

public class AzureTenantRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, AzureTenant>(dbContext, timeProvider), IAzureTenantRepository
{
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
