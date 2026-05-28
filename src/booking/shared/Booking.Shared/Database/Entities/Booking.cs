using Api.Shared.Services;
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
    public string Category { get; set; }
    public string Channel { get; set; }
    public ICollection<BookingSchedule> Schedules { get; set; }
    public bool? HasRecurringInstanceOverrides { get; set; }

    public virtual MarketplaceBooking? MarketplaceBooking { get; set; }
    public virtual ICollection<OrganizationArrearsInvoiceLine> OrganizationArrearsInvoiceLines { get; set; } = [];
    public virtual ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public virtual ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public virtual ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public virtual ICollection<Location> InvolvedLocations { get; set; } = [];
    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
    public virtual ICollection<Resource> InvolvedResources { get; set; } = [];
    public virtual Customer? CreatedByCustomer { get; set; }
    public virtual Customer? LastModifiedByCustomer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
    public virtual RecurringBooking? RecurringBooking { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Notes).HasMaxLength(Constants.MaxBookingNotesLength);
        builder.Property(item => item.Category).HasMaxLength(Constants.MaxBookingCategoryLength);
        builder.Property(item => item.Channel).HasMaxLength(Constants.MaxBookingChannelLength);
        builder.Property(item => item.Schedules).HasColumnType("jsonb");

        builder.HasMany(item => item.ResourceBookingSlots).WithMany(item => item.Bookings);
        builder.HasMany(item => item.InvolvedCustomers).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedOrganizations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedLocations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedResources).WithMany(item => item.InvolvedBookings);
        builder.HasOne(item => item.CreatedByCustomer).WithMany(item => item.CreatedBookings);
        builder.HasOne(item => item.LastModifiedByCustomer).WithMany(item => item.LastModifiedBookings);
        builder.HasOne(item => item.DeletedByCustomer).WithMany(item => item.DeletedBookings);
        builder.HasOne(item => item.RecurringBooking).WithMany(item => item.Bookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
        builder.HasIndex(item => item.Notes);
        builder.HasIndex(item => item.Category);
        builder.HasIndex(item => item.Channel);
        builder.HasIndex(item => item.HasRecurringInstanceOverrides);
    }
}
