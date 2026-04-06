using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DailyBookingCountRecording : EntityBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DailyBookingCountRecordingConfiguration : IEntityTypeConfiguration<DailyBookingCountRecording>
{
    public void Configure(EntityTypeBuilder<DailyBookingCountRecording> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.HasOne(item => item.Organization).WithMany(item => item.DailyBookingCountRecordings);

        builder.HasIndex(item => item.Date);
        builder.HasIndex(item => item.Count);
    }
}
