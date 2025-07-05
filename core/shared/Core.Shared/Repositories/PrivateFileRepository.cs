using Core.Shared.Database;
using Core.Shared.Database.Entities;
using Enterprise.Shared.Database;

namespace Core.Shared.Repositories;

public interface IPrivateFileRepository : IRepository<PrivateFile>
{
    PrivateFile Add(PrivateFile cdnFile);
}

public class PrivateFileRepository(CoreDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CoreDbContext, PrivateFile>(dbContext, timeProvider), IPrivateFileRepository
{
    public PrivateFile Add(PrivateFile cdnFile)
    {
        var now = TimeProvider.GetUtcNow();
        cdnFile.CreatedAt = now;
        return DbContext.PrivateFile.Add(cdnFile).Entity;
    }
}
