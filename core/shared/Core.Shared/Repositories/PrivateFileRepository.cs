using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;

namespace Core.Shared.Repositories;

public interface IPrivateFileRepository : IRepository<PrivateFile>
{
    void Add(PrivateFile cdnFile);
}

public class PrivateFileRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, PrivateFile>(dbContext, timeProvider), IPrivateFileRepository
{
    public void Add(PrivateFile cdnFile)
    {
        var now = TimeProvider.GetUtcNow();
        cdnFile.CreatedAt = now;
        DbContext.PrivateFile.Add(cdnFile);
    }
}
