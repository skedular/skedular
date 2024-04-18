using Api.Shared;
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
    public static void ConfigureEntityBaseWithDeleted<T>(
        this EntityTypeBuilder<T> builder,
        int maxUniqueIdLength = Constants.MaxUniqueIdLength) where T : EntityBaseWithDeleted
    {
        builder.ConfigureEntityBase(maxUniqueIdLength);

        builder.HasIndex(item => item.DeletedAt);
    }
}

public static class ReplicatedEntityBaseWithDeletedExtension
{
    public static void ConfigureReplicatedEntityBaseWithDeleted<T>(
        this EntityTypeBuilder<T> builder,
        int maxUniqueIdLength = Constants.MaxUniqueIdLength) where T : ReplicatedEntityBaseWithDeleted
    {
        builder.ConfigureReplicatedEntityBase(maxUniqueIdLength);

        builder.HasIndex(item => item.DeletedAt);
    }
}
