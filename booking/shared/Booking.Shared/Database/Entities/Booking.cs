using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Booking : EntityBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public string Type { get; set; }
    public BookingSchedules? BookingSchedules { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual Organization? Organization { get; set; }
    public virtual Location? Location { get; set; }
    public virtual ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public virtual Team? Team { get; set; }
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Notes).HasMaxLength(Constants.MaxBookingNotesLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxBookingTypeLength).HasDefaultValue(BookingTypeConstants.WorkingFromOffice);

        builder.HasOne(item => item.Customer).WithMany(item => item.Bookings);
        builder.HasOne(item => item.Organization).WithMany(item => item.Bookings);
        builder.HasOne(item => item.Location).WithMany(item => item.Bookings);
        builder.HasMany(item => item.ResourceBookingSlots).WithMany(item => item.Bookings);
        builder.HasOne(item => item.Team).WithMany(item => item.Bookings);
        builder.HasMany(item => item.ProductVersions).WithMany(item => item.Bookings);
        builder.Property(item => item.BookingSchedules).HasColumnType("jsonb");

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
        builder.HasIndex(item => item.Notes);
        builder.HasIndex(item => item.Type);
    }
}
