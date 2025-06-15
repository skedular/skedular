using Api.Shared;
using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class FloorPlan : EntityBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public int FloorLevel { get; set; }
    public string? FloorName { get; set; }
    public string ImagePath { get; set; } = string.Empty;
    public string? ThumbnailPath { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool IsActive { get; set; } = true;

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string LocationId { get; set; }
    public virtual Location Location { get; set; }

    public virtual ICollection<ResourcePosition> ResourcePositions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class FloorPlanConfiguration : IEntityTypeConfiguration<FloorPlan>
{
    public void Configure(EntityTypeBuilder<FloorPlan> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxResourceNameLength);
        builder.Property(item => item.FloorLevel).HasDefaultValue(0);
        builder.Property(item => item.FloorName).HasMaxLength(Constants.MaxResourceNameLength);
        builder.Property(item => item.ImagePath).HasMaxLength(Constants.MaxFilePathLength);
        builder.Property(item => item.ThumbnailPath).HasMaxLength(Constants.MaxFilePathLength);
        builder.Property(item => item.IsActive).HasDefaultValue(true);

        builder.HasOne(item => item.Location)
            .WithMany(item => item.FloorPlans)
            .HasForeignKey(item => item.LocationId);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.LocationId);
        builder.HasIndex(item => item.FloorLevel);
        builder.HasIndex(item => item.IsActive);
        builder.HasIndex(item => new { item.LocationId, item.FloorLevel })
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL");
    }
}