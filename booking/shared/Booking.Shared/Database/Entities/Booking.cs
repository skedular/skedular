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
    public BookingSchedules BookingSchedules { get; set; }
    public string Status { get; set; }
    public bool IsPaymentRequired { get; set; }

    public virtual ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
    public virtual ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public virtual ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public virtual ICollection<Location> InvolvedLocations { get; set; } = [];
    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
    public virtual Customer? PaidByCustomer { get; set; }
    public virtual Organization? PaidByOrganization { get; set; }
    public virtual Customer? CreatedByCustomer { get; set; }
    public virtual Customer? LastModifiedByCustomer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Notes).HasMaxLength(Constants.MaxBookingNotesLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxBookingTypeLength).HasDefaultValue(BookingTypeConstants.WorkingFromOffice);
        builder.Property(item => item.Status).HasMaxLength(Constants.MaxBookingStatusLength).HasDefaultValue(BookingStatusConstants.Confirmed);
        builder.Property(item => item.IsPaymentRequired).HasDefaultValue(false);

        builder.HasMany(item => item.ResourceBookingSlots).WithMany(item => item.Bookings);
        builder.HasMany(item => item.ProductVersions).WithMany(item => item.Bookings);
        builder.Property(item => item.BookingSchedules).HasColumnType("jsonb");
        builder.HasMany(item => item.InvolvedCustomers).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedOrganizations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedLocations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedBookings);

        builder.HasOne(item => item.PaidByCustomer).WithMany(item => item.PaidBookings);
        builder.HasOne(item => item.PaidByOrganization).WithMany(item => item.PaidBookings);
        builder.HasOne(item => item.CreatedByCustomer).WithMany(item => item.CreatedBookings);
        builder.HasOne(item => item.LastModifiedByCustomer).WithMany(item => item.LastModifiedBookings);
        builder.HasOne(item => item.DeletedByCustomer).WithMany(item => item.DeletedBookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
        builder.HasIndex(item => item.Notes);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => item.IsPaymentRequired);
    }
}
