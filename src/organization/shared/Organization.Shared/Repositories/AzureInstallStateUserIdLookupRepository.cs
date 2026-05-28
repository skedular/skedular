using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Organization.Shared.Database;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Repositories;

public interface IAzureInstallStateUserIdLookupRepository : IRepository<AzureInstallStateUserIdLookup>
{
    Task<AzureInstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken);
    AzureInstallStateUserIdLookup Add(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup);
    void Remove(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup);
}

public class AzureInstallStateUserIdLookupRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, AzureInstallStateUserIdLookup>(dbContext, timeProvider),
        IAzureInstallStateUserIdLookupRepository
{
    public async Task<AzureInstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureInstallStateUserIdLookup.FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public AzureInstallStateUserIdLookup Add(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup)
    {
        var now = TimeProvider.GetUtcNow();
        azureInstallStateUserIdLookup.CreatedAt = now;
        return DbContext.AzureInstallStateUserIdLookup.Add(azureInstallStateUserIdLookup).Entity;
    }

    public void Remove(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup) =>
        DbContext.AzureInstallStateUserIdLookup.Remove(azureInstallStateUserIdLookup);
}
