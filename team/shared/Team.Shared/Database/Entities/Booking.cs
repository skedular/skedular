using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Booking : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }

    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.From).HasDefaultValue(DateTimeOffset.MinValue);
        builder.Property(item => item.Until).HasDefaultValue(DateTimeOffset.MinValue);

        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedBookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
    }
}
