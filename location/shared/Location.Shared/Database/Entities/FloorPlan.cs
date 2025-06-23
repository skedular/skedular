using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class FloorPlan : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public CdnImageFile Image { get; set; }

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

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxFloorPlanNameLength);
        builder.Property(item => item.Image).HasColumnType("jsonb");

        builder.HasOne(item => item.Location)
            .WithMany(item => item.FloorPlans)
            .HasForeignKey(item => item.LocationId);

        builder.HasIndex(item => item.Name);
    }
}
