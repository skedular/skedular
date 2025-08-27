using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationPhysicalAddress : EntityBaseWithDeleted
{
    public string? OsmType { get; set; }
    public string? OsmId { get; set; }
    public string? PlaceId { get; set; }
    public Point? Coordinates { get; set; }
    public string? FormattedAddress { get; set; }
    public string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Suburb { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string Zipcode { get; set; }
    public string Country { get; set; }
    public string? CountryCode { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? LocationId { get; set; }
    public virtual Location Location { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationPhysicalAddressConfiguration : IEntityTypeConfiguration<LocationPhysicalAddress>
{
    public void Configure(EntityTypeBuilder<LocationPhysicalAddress> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.OsmType).HasMaxLength(Constants.MaxOsmTypeLength);
        builder.Property(item => item.OsmId).HasMaxLength(Constants.MaxOsmIdLength);
        builder.Property(item => item.PlaceId).HasMaxLength(Constants.MaxPlaceIdLength);
        builder.Property(item => item.Coordinates).HasColumnType("geometry (point, 4326)");
        builder.Property(item => item.FormattedAddress).HasMaxLength(Constants.MaxFormattedAddressLength);
        builder.Property(item => item.AddressLine1).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.AddressLine2).HasMaxLength(Constants.MaxAddressLineLength);
        builder.Property(item => item.Suburb).HasMaxLength(Constants.MaxSuburbLength);
        builder.Property(item => item.City).HasMaxLength(Constants.MaxCityLength);
        builder.Property(item => item.Province).HasMaxLength(Constants.MaxProvinceLength);
        builder.Property(item => item.Zipcode).HasMaxLength(Constants.MaxZipcodeLength);
        builder.Property(item => item.Country).HasMaxLength(Constants.MaxCountryLength);
        builder.Property(item => item.CountryCode).HasMaxLength(Constants.MaxCountryCodeLength);

        builder
            .HasOne(item => item.Location)
            .WithOne(item => item.PhysicalAddress)
            .HasForeignKey<LocationPhysicalAddress>(item => item.LocationId);

        builder.HasIndex(item => item.OsmType);
        builder.HasIndex(item => item.OsmId);
        builder.HasIndex(item => item.PlaceId);
        builder.HasIndex(item => item.Coordinates);
    }
}
