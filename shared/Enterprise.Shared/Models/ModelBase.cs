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
    public static bool IsNotModified<TEntity>(this TEntity entity) where TEntity : ModelBase => !entity.ModifiedAt.HasValue;
}
