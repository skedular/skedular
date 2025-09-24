using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : ReplicatedEntityBaseWithDeleted
{
    public OpeningHours? OpeningHours { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<Customer> PreferredByCustomers { get; set; } = [];
    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public virtual ICollection<Booking> InvolvedBookings { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.OpeningHours).HasColumnType("jsonb");

        builder.HasOne(item => item.Organization).WithMany(item => item.Locations);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.Locations);
    }
}
