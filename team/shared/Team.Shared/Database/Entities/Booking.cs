using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Booking : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }

    public virtual Team Team { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Team)
            .WithMany(item => item.Bookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.To);
    }
}
