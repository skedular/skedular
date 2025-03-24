using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public OpeningHours? OpeningHours { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<LocationMember> LocationMembers { get; set; } = [];
    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<Customer> PreferredByCustomers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxLocationNameLength);
        builder.Property(item => item.OpeningHours).HasColumnType("jsonb");

        builder.HasOne(item => item.Organization).WithMany(item => item.Locations);

        builder.HasIndex(item => item.Name);
    }
}
