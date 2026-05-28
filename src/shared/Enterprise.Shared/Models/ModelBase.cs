namespace Enterprise.Shared.Models;

public class ModelBase
{
    public string Id { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
}

public class ReplicatedModelBase : ModelBase
{
    public DateTimeOffset? EventRaisedAt { get; set; }
}

public static class ModelBaseExtensions
{
    extension<TEntity>(TEntity entity) where TEntity : ModelBase
    {
        public bool IsNotModified() => !entity.ModifiedAt.HasValue;
    }
}
