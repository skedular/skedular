using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Database.SqlServer;

internal static class SqlServerUpsertCommandBuilder
{
    public static string BuildInsertIfMissing(string fullTableName, params string[] columnNames)
    {
        var columns = string.Join(", ", columnNames.Select(WrapIdentifier));
        var values = string.Join(", ", columnNames.Select(columnName => $"@{columnName}"));

        return
            $"""
             INSERT INTO {fullTableName} ({columns})
             SELECT {values}
             WHERE NOT EXISTS (
                 SELECT 1
                 FROM {fullTableName} WITH (UPDLOCK, HOLDLOCK)
                 WHERE [Id] = @Id
             );
             """;
    }

    private static string WrapIdentifier(string identifier) => $"[{identifier}]";
}

public abstract class RepositoryBase<TDbContext, TEntity>(TDbContext dbContext, TimeProvider timeProvider) : IRepository<TEntity>
    where TDbContext : DbContextBase<TDbContext>
    where TEntity : EntityBase
{
    protected readonly TDbContext DbContext = dbContext;
    protected readonly TimeProvider TimeProvider = timeProvider;

    public IUnitOfWork UnitOfWork => DbContext;

    public virtual IQueryable<TEntity> Query(ISpecification<TEntity>? specification = null) => ApplySpecification(specification);

    protected virtual IQueryable<TEntity> ApplySpecification(ISpecification<TEntity>? spec) =>
        SpecificationEvaluator<TEntity>.GetQuery(DbContext.Set<TEntity>().AsQueryable(), spec);

    public virtual async Task UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
        {
            throw new InvalidOperationException($"Entity type '{nameof(TEntity)}' is not part of the model.");
        }

        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema() ?? "dbo";
        var fullTableName = $"[{schema}].[{tableName}]";

        var sql = SqlServerUpsertCommandBuilder.BuildInsertIfMissing(fullTableName, "Id", "CreatedAt");

        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new SqlParameter("@Id", id),
                new SqlParameter("@CreatedAt", TimeProvider.GetUtcNow())
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
        var schema = entityType.GetSchema() ?? "dbo";
        var fullTableName = $"[{schema}].[{tableName}]";

        var sql = SqlServerUpsertCommandBuilder.BuildInsertIfMissing(fullTableName, "Id", "CreatedAt", foreignKeyColumnName);

        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new SqlParameter("@Id", id),
                new SqlParameter("@CreatedAt", TimeProvider.GetUtcNow()),
                new SqlParameter("@ForeignKeyId", foreignEntity.Id)
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
        var schema = entityType.GetSchema() ?? "dbo";
        var fullTableName = $"[{schema}].[{tableName}]";

        var sql = SqlServerUpsertCommandBuilder.BuildInsertIfMissing(
            fullTableName,
            "Id",
            "CreatedAt",
            foreignKeyColumnName1,
            foreignKeyColumnName2);

        await DbContext.Database.ExecuteSqlRawAsync(
            sql,
            [
                new SqlParameter("@Id", id),
                new SqlParameter("@CreatedAt", TimeProvider.GetUtcNow()),
                new SqlParameter("@ForeignKeyId1", foreignEntity1!.Id),
                new SqlParameter("@ForeignKeyId2", foreignEntity2!.Id)
            ],
            cancellationToken);
    }
}
