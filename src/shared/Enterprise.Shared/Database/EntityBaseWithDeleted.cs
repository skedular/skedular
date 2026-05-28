using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Shared.Database;

public class EntityBaseWithDeleted : EntityBase
{
    public DateTimeOffset? DeletedAt { get; set; }
}

public class ReplicatedEntityBaseWithDeleted : ReplicatedEntityBase
{
    public DateTimeOffset? DeletedAt { get; set; }
}

public static class EntityBaseWithDeletedExtension
{
    extension<T>(EntityTypeBuilder<T> builder) where T : EntityBaseWithDeleted
    {
        public void ConfigureEntityBaseWithDeleted(int maxUniqueIdLength = Constants.MaxUniqueIdLength)
        {
            builder.ConfigureEntityBase(maxUniqueIdLength);

            builder.HasIndex(item => item.DeletedAt);
        }
    }
}

public static class ReplicatedEntityBaseWithDeletedExtension
{
    extension<T>(EntityTypeBuilder<T> builder) where T : ReplicatedEntityBaseWithDeleted
    {
        public void ConfigureReplicatedEntityBaseWithDeleted(int maxUniqueIdLength = Constants.MaxUniqueIdLength)
        {
            builder.ConfigureReplicatedEntityBase(maxUniqueIdLength);

            builder.HasIndex(item => item.DeletedAt);
        }
    }
}

public static class EntityBaseWithDeletedExtensions
{
    extension<TEntity>(TEntity entity) where TEntity : EntityBaseWithDeleted
    {
        public bool IsNotDeleted() => !entity.IsDeleted();
        public bool IsDeleted() => entity.DeletedAt.HasValue;
    }

    extension<TEntity>(TEntity entity) where TEntity : ReplicatedEntityBaseWithDeleted
    {
        public bool IsReplicatedNotDeleted() => !entity.IsReplicatedDeleted();

        public bool IsReplicatedDeleted() => entity.DeletedAt.HasValue;
    }
}
