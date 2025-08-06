namespace Enterprise.Shared.Models;

public class ModelBaseWithDeleted : ModelBase
{
    public DateTimeOffset? DeletedAt { get; set; }
}

public class ReplicatedModelBaseWithDeleted : ReplicatedModelBase
{
    public DateTimeOffset? DeletedAt { get; set; }
}

public static class ModelBaseWithDeletedExtensions
{
    public static bool IsNotDeleted<TEntity>(this TEntity entity) where TEntity : ModelBaseWithDeleted => !entity.IsDeleted();
    public static bool IsDeleted<TEntity>(this TEntity entity) where TEntity : ModelBaseWithDeleted => entity.DeletedAt.HasValue;

    public static bool IsReplicatedNotDeleted<TEntity>(this TEntity entity) where TEntity : ReplicatedModelBaseWithDeleted =>
        !entity.IsReplicatedDeleted();

    public static bool IsReplicatedDeleted<TEntity>(this TEntity entity) where TEntity : ReplicatedModelBaseWithDeleted => entity.DeletedAt.HasValue;
}
