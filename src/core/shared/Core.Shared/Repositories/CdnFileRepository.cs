using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;

namespace Core.Shared.Repositories;

public interface ICdnFileRepository : IRepository<CdnFile>
{
    void Add(CdnFile cdnFile);
}

public class CdnFileRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, CdnFile>(dbContext, timeProvider), ICdnFileRepository
{
    public void Add(CdnFile cdnFile)
    {
        var now = TimeProvider.GetUtcNow();
        cdnFile.CreatedAt = now;
        DbContext.CdnFile.Add(cdnFile);
    }
}
