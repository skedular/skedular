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
    extension<TEntity>(TEntity entity) where TEntity : ModelBaseWithDeleted
    {
        public bool IsNotDeleted() => !entity.IsDeleted();
        public bool IsDeleted() => entity.DeletedAt.HasValue;
    }

    extension<TEntity>(TEntity entity) where TEntity : ReplicatedModelBaseWithDeleted
    {
        public bool IsReplicatedNotDeleted() => !entity.IsReplicatedDeleted();
        public bool IsReplicatedDeleted() => entity.DeletedAt.HasValue;
    }
}
