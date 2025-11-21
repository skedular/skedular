using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enterprise.Shared.Database;

public class EntityBase
{
    public string Id { get; set; } = string.Empty;
    public uint EntityFrameworkVersion { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
}

public class ReplicatedEntityBase : EntityBase
{
    public DateTimeOffset? EventRaisedAt { get; set; }
}

public static class EntityBaseExtension
{
    extension<T>(EntityTypeBuilder<T> builder) where T : EntityBase
    {
        public void ConfigureEntityBase(int maxUniqueIdLength = Constants.MaxUniqueIdLength)
        {
            builder.HasKey(item => item.Id);

            builder.Property(item => item.EntityFrameworkVersion).IsRowVersion();
            builder.Property(item => item.Id).HasMaxLength(maxUniqueIdLength);

            builder.HasIndex(item => item.CreatedAt);
            builder.HasIndex(item => item.ModifiedAt);
        }
    }
}

public static class ReplicatedEntityBaseExtension
{
    extension<T>(EntityTypeBuilder<T> builder) where T : ReplicatedEntityBase
    {
        public void ConfigureReplicatedEntityBase(int maxUniqueIdLength = Constants.MaxUniqueIdLength) =>
            builder.ConfigureEntityBase(maxUniqueIdLength);
    }
}
