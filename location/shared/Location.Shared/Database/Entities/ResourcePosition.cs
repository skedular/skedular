using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ResourcePosition : EntityBaseWithDeleted
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string? Shape { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ResourceId { get; set; }
    public virtual Resource Resource { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string FloorPlanId { get; set; }
    public virtual FloorPlan FloorPlan { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ResourcePositionConfiguration : IEntityTypeConfiguration<ResourcePosition>
{
    public void Configure(EntityTypeBuilder<ResourcePosition> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Shape).HasMaxLength(Constants.MaxShapeNameLength);
        builder.Property(item => item.Metadata).HasColumnType("jsonb");

        builder.HasOne(item => item.Resource)
            .WithOne(item => item.ResourcePosition)
            .HasForeignKey<ResourcePosition>(item => item.ResourceId)
            .IsRequired();

        builder.HasOne(item => item.FloorPlan)
            .WithMany(item => item.ResourcePositions)
            .HasForeignKey(item => item.FloorPlanId)
            .IsRequired();

        builder.HasIndex(item => item.ResourceId).IsUnique();
        builder.HasIndex(item => item.FloorPlanId);
    }
}
