using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Enterprise.Shared.Database.PostgreSql;

public abstract class RepositoryBase<TDbContext, TEntity>(TDbContext dbContext, TimeProvider timeProvider) : IRepository<TEntity>
    where TDbContext : DbContextBase<TDbContext>
    where TEntity : EntityBase
{
    protected readonly TDbContext DbContext = dbContext;
    protected readonly TimeProvider TimeProvider = timeProvider;

    public IUnitOfWork UnitOfWork => DbContext;

    public virtual async Task UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TEntity)}' is not part of the model.");
        }

        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema();
        var fullTableName = schema == null ? $"public.\"{tableName}\"" : $"{schema}.\"{tableName}\"";

        var sql = $"INSERT INTO {fullTableName} (\"Id\", \"CreatedAt\") VALUES (@Id, @CreatedAt) ON CONFLICT (\"Id\") DO NOTHING;";
        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new NpgsqlParameter("@Id", id),
                new NpgsqlParameter("@CreatedAt", TimeProvider.GetUtcNow())
            ],
            cancellationToken);
    }

    public async Task UpsertNakedAsync<TForeignEntity>(string id, TForeignEntity? foreignEntity, CancellationToken cancellationToken)
        where TForeignEntity : EntityBase
    {
        if (foreignEntity is null)
        {
            await UpsertNakedAsync(id, cancellationToken);

            return;
        }

        var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TEntity)}' is not part of the model.");
        }

        var foreignEntityType = DbContext.Model.FindEntityType(typeof(TForeignEntity));
        if (foreignEntityType == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TForeignEntity)}' is not part of the model.");
        }

        var foreignKey = entityType.GetForeignKeys().Single(item => item.PrincipalEntityType == foreignEntityType);
        var foreignKeyColumnName = foreignKey.Properties.Select(item => item.GetColumnName()).Single();

        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema();
        var fullTableName = schema == null ? $"public.\"{tableName}\"" : $"{schema}.\"{tableName}\"";

        var sql =
            $"INSERT INTO {fullTableName}  (\"Id\", \"CreatedAt\", \"{foreignKeyColumnName}\") VALUES (@Id, @CreatedAt, @ForeignKeyId) ON CONFLICT (\"Id\") DO NOTHING;";
        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new NpgsqlParameter("@Id", id),
                new NpgsqlParameter("@CreatedAt", TimeProvider.GetUtcNow()),
                new NpgsqlParameter("@ForeignKeyId", foreignEntity.Id)
            ],
            cancellationToken);
    }

    public async Task UpsertNakedAsync<TForeignEntity1, TForeignEntity2>(
        string id,
        TForeignEntity1? foreignEntity1,
        TForeignEntity2? foreignEntity2,
        CancellationToken cancellationToken) where TForeignEntity1 : EntityBase where TForeignEntity2 : EntityBase
    {
        if (foreignEntity1 is null && foreignEntity2 is null)
        {
            await UpsertNakedAsync(id, cancellationToken);

            return;
        }

        if (foreignEntity1 is not null && foreignEntity2 is null)
        {
            await UpsertNakedAsync(id, foreignEntity1, cancellationToken);

            return;
        }

        if (foreignEntity1 is null && foreignEntity2 is not null)
        {
            await UpsertNakedAsync(id, foreignEntity2, cancellationToken);

            return;
        }

        var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TEntity)}' is not part of the model.");
        }

        var foreignEntityType1 = DbContext.Model.FindEntityType(typeof(TForeignEntity1));
        if (foreignEntityType1 == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TForeignEntity1)}' is not part of the model.");
        }

        var foreignKey1 = entityType.GetForeignKeys().Single(item => item.PrincipalEntityType == foreignEntityType1);
        var foreignKeyColumnName1 = foreignKey1.Properties.Select(item => item.GetColumnName()).Single();

        var foreignEntityType2 = DbContext.Model.FindEntityType(typeof(TForeignEntity2));
        if (foreignEntityType2 == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TForeignEntity2)}' is not part of the model.");
        }

        var foreignKey2 = entityType.GetForeignKeys().Single(item => item.PrincipalEntityType == foreignEntityType2);
        var foreignKeyColumnName2 = foreignKey2.Properties.Select(item => item.GetColumnName()).Single();

        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema();
        var fullTableName = schema == null ? $"public.\"{tableName}\"" : $"{schema}.\"{tableName}\"";

        var sql =
            $"INSERT INTO {fullTableName}  (\"Id\", \"CreatedAt\", \"{foreignKeyColumnName1}\", \"{foreignKeyColumnName2}\") VALUES (@Id, @CreatedAt, @ForeignKeyId1, @ForeignKeyId2) ON CONFLICT (\"Id\") DO NOTHING;";
        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new NpgsqlParameter("@Id", id),
                new NpgsqlParameter("@CreatedAt", TimeProvider.GetUtcNow()),
                new NpgsqlParameter("@ForeignKeyId1", foreignEntity1!.Id),
                new NpgsqlParameter("@ForeignKeyId2", foreignEntity2!.Id)
            ],
            cancellationToken);
    }
}
