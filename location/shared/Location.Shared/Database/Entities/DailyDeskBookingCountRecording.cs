using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DailyDeskBookingCountRecording : EntityBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public virtual Location Location { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DailyDeskBookingCountRecordingConfiguration : IEntityTypeConfiguration<DailyDeskBookingCountRecording>
{
    public void Configure(EntityTypeBuilder<DailyDeskBookingCountRecording> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.HasOne(item => item.Location).WithMany(item => item.DailyDeskBookingCountRecordings);

        builder.HasIndex(item => item.Date);
        builder.HasIndex(item => item.Count);
    }
}
