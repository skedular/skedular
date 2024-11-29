using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IAzureTenantRepository : IRepository<AzureTenant>
{
    Task<AzureTenant> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken);
    AzureTenant Add(AzureTenant azureTenant);
    AzureTenant Update(AzureTenant azureTenant);
}

internal static class AzureTenantExtensions
{
    internal static IIncludableQueryable<AzureTenant, ICollection<AzureTenantMember>> AddDependentObjects(
        this IQueryable<AzureTenant> originalQuery) =>
        originalQuery
            .Include(query => query.Organization)
            .ThenInclude(query => query.OrganizationMembers)
            .ThenInclude(query => query.Customer)
            .Include(query => query.AzureTenantMembers);
}

public class AzureTenantRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, AzureTenant>(dbContext, timeProvider), IAzureTenantRepository
{
    public override async Task<AzureTenant> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        await base.UpsertNakedAsync(id, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<AzureTenant?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureTenant
            .AddDependentObjects()
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

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
