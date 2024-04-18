namespace Enterprise.Shared.Database;

/// <summary>
///     Represents the methods that we want to expose from the DbContext
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
