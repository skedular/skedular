using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationPhysicalAddress : EntityBase
{
    public Point? Coordinates { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? LocationId { get; set; }
    public virtual Location Location { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationPhysicalAddressConfiguration : IEntityTypeConfiguration<LocationPhysicalAddress>
{
    public void Configure(EntityTypeBuilder<LocationPhysicalAddress> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Coordinates).HasColumnType("geometry (point, 4326)");

        builder
            .HasOne(item => item.Location)
            .WithOne(item => item.PhysicalAddress)
            .HasForeignKey<LocationPhysicalAddress>(item => item.LocationId);

        builder.HasIndex(item => item.Coordinates);
    }
}
