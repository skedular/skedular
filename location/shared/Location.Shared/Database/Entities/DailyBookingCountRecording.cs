using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618
public class DailyBookingCountRecording : EntityBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public int Count { get; set; }

    public virtual Location Location { get; set; }
}
#pragma warning restore CS8618

public class DailyBookingCountRecordingConfiguration : IEntityTypeConfiguration<DailyBookingCountRecording>
{
    public void Configure(EntityTypeBuilder<DailyBookingCountRecording> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.HasOne(item => item.Location).WithMany(item => item.DailyBookingCountRecordings);

        builder.HasIndex(item => item.Date);
        builder.HasIndex(item => item.Count);
    }
}
