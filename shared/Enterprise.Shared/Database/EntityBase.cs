using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Shared.Database;

public class EntityBase
{
    public string Id { get; set; } = string.Empty;
    public uint Version { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
}

public class ReplicatedEntityBase : EntityBase
{
    public DateTimeOffset? EventRaisedAt { get; set; }
}

public static class EntityBaseExtension
{
    public static void ConfigureEntityBase<T>(this EntityTypeBuilder<T> builder, int maxUniqueIdLength = Constants.MaxUniqueIdLength)
        where T : EntityBase
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Version).IsRowVersion();
        builder.Property(item => item.Id).HasMaxLength(maxUniqueIdLength);

        builder.HasIndex(item => item.CreatedAt);
        builder.HasIndex(item => item.ModifiedAt);
    }
}

public static class ReplicatedEntityBaseExtension
{
    public static void ConfigureReplicatedEntityBase<T>(this EntityTypeBuilder<T> builder, int maxUniqueIdLength = Constants.MaxUniqueIdLength)
        where T : ReplicatedEntityBase =>
        builder.ConfigureEntityBase(maxUniqueIdLength);
}
