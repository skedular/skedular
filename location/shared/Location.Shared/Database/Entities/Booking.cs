using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Booking : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<Location> InvolvedLocations { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.HasMany(item => item.Resources).WithMany(item => item.Bookings);
        builder.HasMany(item => item.InvolvedLocations).WithMany(item => item.InvolvedBookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
    }
}
