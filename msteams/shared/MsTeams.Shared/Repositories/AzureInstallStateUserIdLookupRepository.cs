using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IAzureInstallStateUserIdLookupRepository : IRepository<AzureInstallStateUserIdLookup>
{
    Task<AzureInstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken);
    AzureInstallStateUserIdLookup Add(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup);
    void Remove(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup);
}

public class AzureInstallStateUserIdLookupRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, AzureInstallStateUserIdLookup>(dbContext), IAzureInstallStateUserIdLookupRepository
{
    public async Task<AzureInstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.AzureInstallStateUserIdLookup
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public AzureInstallStateUserIdLookup Add(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup)
    {
        var now = timeProvider.GetUtcNow();
        azureInstallStateUserIdLookup.CreatedAt = now;
        return DbContext.AzureInstallStateUserIdLookup.Add(azureInstallStateUserIdLookup).Entity;
    }

    public void Remove(AzureInstallStateUserIdLookup azureInstallStateUserIdLookup) =>
        DbContext.AzureInstallStateUserIdLookup.Remove(azureInstallStateUserIdLookup);
}
