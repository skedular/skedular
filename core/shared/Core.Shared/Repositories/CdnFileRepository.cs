using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Core.Shared.Repositories;

public interface ICdnFileRepository : IRepository<CdnFile>
{
    CdnFile Add(CdnFile cdnFile);
}

public class CdnFileRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, CdnFile>(dbContext, timeProvider), ICdnFileRepository
{
    public CdnFile Add(CdnFile cdnFile)
    {
        var now = TimeProvider.GetUtcNow();
        cdnFile.CreatedAt = now;
        return DbContext.CdnFile.Add(cdnFile).Entity;
    }
}
