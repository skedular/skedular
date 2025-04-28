using Microsoft.EntityFrameworkCore.Storage;

namespace Enterprise.Shared.Database;

public class OutboxTransactionBuilderDecorator(IDbTransactionBuilder transactionBuilder) : IDbTransactionBuilder
{
    public IDbContextTransaction BeginTransaction(IUnitOfWork unit) => new OutboxTransactionDecorator(transactionBuilder.BeginTransaction(unit));

    public async Task<IDbContextTransaction> BeginTransactionAsync(IUnitOfWork unit, CancellationToken cancellationToken) =>
        new OutboxTransactionDecorator(await transactionBuilder.BeginTransactionAsync(unit, cancellationToken));
}
