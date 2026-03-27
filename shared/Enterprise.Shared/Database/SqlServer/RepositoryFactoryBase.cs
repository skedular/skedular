namespace Enterprise.Shared.Database.SqlServer;

public abstract class RepositoryFactoryBase<TDbContext> : IAsyncDisposable where TDbContext : DbContextBase<TDbContext>
{
    protected TDbContext? _dbContext;

    public TDbContext DbContext
    {
        get
        {
            ArgumentNullException.ThrowIfNull(_dbContext);
            return _dbContext;
        }
    }

    public IUnitOfWork UnitOfWork => DbContext;

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_dbContext is not null)
        {
            await DbContext.DisposeAsync();
        }

        _dbContext = null;
    }
}
