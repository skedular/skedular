using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationBookingAccess : ReplicatedEntityBaseWithDeleted
{
    public string CustomerId { get; set; }
    public string LocationId { get; set; }
    public string OrganizationId { get; set; }
    public int ActiveBookingCount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationBookingAccessConfiguration : IEntityTypeConfiguration<LocationBookingAccess>
{
    public void Configure(EntityTypeBuilder<LocationBookingAccess> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.CustomerId).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.LocationId).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.OrganizationId).HasMaxLength(Constants.MaxUniqueIdLength);

        builder.HasIndex(item => item.CustomerId);
        builder.HasIndex(item => item.LocationId);
        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.ActiveBookingCount);
        builder.HasIndex(item => new { item.CustomerId, item.LocationId });
        builder.HasIndex(item => new { item.CustomerId, item.OrganizationId });
        builder.HasIndex(item => new { item.CustomerId, item.LocationId, item.OrganizationId }).IsUnique();
    }
}
