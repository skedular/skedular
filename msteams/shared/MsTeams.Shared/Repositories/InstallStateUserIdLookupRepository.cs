using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database;
using MsTeams.Shared.Database.Entities;

namespace MsTeams.Shared.Repositories;

public interface IInstallStateUserIdLookupRepository : IRepository<InstallStateUserIdLookup>
{
    Task<InstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken);
    InstallStateUserIdLookup Add(InstallStateUserIdLookup installStateUserIdLookup);
    void Remove(InstallStateUserIdLookup installStateUserIdLookup);
}

public class InstallStateUserIdLookupRepository(MsTeamsDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MsTeamsDbContext, InstallStateUserIdLookup>(dbContext), IInstallStateUserIdLookupRepository
{
    public async Task<InstallStateUserIdLookup?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.InstallStateUserIdLookup
            .Where(query => query.Id == id)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public InstallStateUserIdLookup Add(InstallStateUserIdLookup installStateUserIdLookup)
    {
        var now = timeProvider.GetUtcNow();
        installStateUserIdLookup.CreatedAt = now;
        return DbContext.InstallStateUserIdLookup.Add(installStateUserIdLookup).Entity;
    }

    public void Remove(InstallStateUserIdLookup installStateUserIdLookup) =>
        DbContext.InstallStateUserIdLookup.Remove(installStateUserIdLookup);
}
