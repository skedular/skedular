using Enterprise.Shared.Outbox;
using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public class OutboxTransactionDecorator(IDbContextTransaction dbContextTransaction) : IDbContextTransaction
{
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync() => dbContextTransaction.DisposeAsync();

    public void Commit()
    {
        dbContextTransaction.Commit();
        OutboxEvents.OnTransactionCommit();
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await dbContextTransaction.CommitAsync(cancellationToken);
        OutboxEvents.OnTransactionCommit();
    }

    public void Rollback() => dbContextTransaction.Rollback();

    public Task RollbackAsync(CancellationToken cancellationToken) => dbContextTransaction.RollbackAsync(cancellationToken);

    public void CreateSavepoint(string name) => dbContextTransaction.CreateSavepoint(name);

    public Task CreateSavepointAsync(string name, CancellationToken cancellationToken) =>
        dbContextTransaction.CreateSavepointAsync(name, cancellationToken);

    public void RollbackToSavepoint(string name) => dbContextTransaction.RollbackToSavepoint(name);

    public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken) =>
        dbContextTransaction.RollbackToSavepointAsync(name, cancellationToken);

    public void ReleaseSavepoint(string name) => dbContextTransaction.ReleaseSavepoint(name);

    public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken) =>
        dbContextTransaction.ReleaseSavepointAsync(name, cancellationToken);

    public Guid TransactionId => dbContextTransaction.TransactionId;

    public bool SupportsSavepoints => dbContextTransaction.SupportsSavepoints;

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            dbContextTransaction.Dispose();
        }

        _disposed = true;
    }

    ~OutboxTransactionDecorator() => Dispose(false);
}
