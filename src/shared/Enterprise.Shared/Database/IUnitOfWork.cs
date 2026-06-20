namespace Enterprise.Shared.Database;

/// <summary>
///     Represents the methods that we want to expose from the DbContext
/// </summary>
public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
