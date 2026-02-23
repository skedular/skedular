using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class RecurringBooking : EntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string Channel { get; set; }
    public string Frequency { get; set; }
    public int Interval { get; set; }
    public ICollection<string> ByWeekDays { get; set; } = [];
    public int? ByMonthDay { get; set; }
    public int? BySetPosition { get; set; }
    public string EndType { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }
    public ICollection<DateTimeOffset> SkippedDates { get; set; } = [];

    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual MarketplaceBooking? MarketplaceBooking { get; set; }
    public virtual ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public virtual ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
    public virtual Customer? CreatedByCustomer { get; set; }
    public virtual Customer? LastModifiedByCustomer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class RecurringBookingConfiguration : IEntityTypeConfiguration<RecurringBooking>
{
    public void Configure(EntityTypeBuilder<RecurringBooking> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Channel).HasMaxLength(Constants.MaxBookingChannelLength);
        builder.Property(item => item.Frequency).HasMaxLength(Constants.MaxRecurringBookingFrequencyLength);
        builder.Property(item => item.Interval);
        builder.Property(item => item.ByWeekDays).HasColumnType("jsonb");
        builder.Property(item => item.EndType).HasMaxLength(Constants.MaxRecurringBookingEndTypeLength);
        builder.Property(item => item.SkippedDates).HasColumnType("jsonb");

        builder.HasMany(item => item.InvolvedCustomers).WithMany(item => item.InvolvedRecurringBooking);
        builder.HasMany(item => item.InvolvedOrganizations).WithMany(item => item.InvolvedRecurringBooking);
        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedRecurringBooking);
        builder.HasOne(item => item.CreatedByCustomer).WithMany(item => item.CreatedRecurringBookings);
        builder.HasOne(item => item.LastModifiedByCustomer).WithMany(item => item.LastModifiedRecurringBookings);
        builder.HasOne(item => item.DeletedByCustomer).WithMany(item => item.DeletedRecurringBookings);

        builder.HasIndex(item => item.Channel);
        builder.HasIndex(item => item.Frequency);
        builder.HasIndex(item => item.EndType);
        builder.HasIndex(item => item.StartDate);
        builder.HasIndex(item => item.EndDate);
    }
}
