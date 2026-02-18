using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class BookingRecurrence : EntityBaseWithDeleted
{
    public string Frequency { get; set; } // (Daily, Weekly, Monthly, etc.)
    public int Interval { get; set; } // (every N units, e.g. every 2 weeks)
    public ICollection<string> ByWeekDays { get; set; } = []; // (for weekly patterns, e.g. Mon/Wed/Fri)
    public int? ByMonthDay { get; set; } // (for monthly “day 15” style)
    public int? BySetPosition { get; set; } // (for “first Monday”, “last Friday” patterns)
    public string EndType { get; set; } // (Never, UntilDate, AfterOccurrences)
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset? Until { get; set; }
    public int? OccurrenceCount { get; set; }
    public ICollection<DateTimeOffset> SkippedDates { get; set; } = [];

    public virtual ICollection<Booking> Bookings { get; set; } = [];
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingRecurrenceConfiguration : IEntityTypeConfiguration<BookingRecurrence>
{
    public void Configure(EntityTypeBuilder<BookingRecurrence> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Frequency).HasMaxLength(Constants.MaxBookingRecurrenceFrequencyLength);
        builder.Property(item => item.Interval);
        builder.Property(item => item.ByWeekDays).HasColumnType("jsonb");
        builder.Property(item => item.EndType).HasMaxLength(Constants.MaxBookingRecurrenceEndTypeLength);
        builder.Property(item => item.SkippedDates).HasColumnType("jsonb");

        builder.HasIndex(item => item.Frequency);
        builder.HasIndex(item => item.EndType);
        builder.HasIndex(item => item.Start);
        builder.HasIndex(item => item.Until);
    }
}
