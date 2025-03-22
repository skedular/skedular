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
    public static bool IsNotDeleted<TEntity>(this TEntity entity) where TEntity : ModelBaseWithDeleted => !entity.DeletedAt.HasValue;
}
