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
    public string PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public ICollection<ProductVersionLineItem> LineItems { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? TotalAmountExcludeTax { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? TaxRatePercentage { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public ICollection<string>? InvoiceEmailList { get; set; }

    public virtual ICollection<ResourceBookingSlot> ResourceBookingSlots { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
    public virtual ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public virtual ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public virtual ICollection<Location> InvolvedLocations { get; set; } = [];
    public virtual ICollection<Team> InvolvedTeams { get; set; } = [];
    public virtual ICollection<Resource> InvolvedResources { get; set; } = [];
    public virtual Customer? PaidByCustomer { get; set; }
    public virtual Organization? PaidByOrganization { get; set; }
    public virtual Customer? CreatedByCustomer { get; set; }
    public virtual Customer? LastModifiedByCustomer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
    public virtual StripeCheckoutSession? StripeCheckoutSession { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Notes).HasMaxLength(Constants.MaxBookingNotesLength);
        builder.Property(item => item.Category)
            .HasMaxLength(Constants.MaxBookingCategoryLength)
            .HasDefaultValue(BookingCategoryConstants.WorkingFromOffice);

        builder.Property(item => item.Channel)
            .HasMaxLength(Constants.MaxBookingChannelLength)
            .HasDefaultValue(BookingChannelConstants.Private);

        builder
            .Property(item => item.PaymentStatus)
            .HasMaxLength(Constants.MaxBookingPaymentStatusLength)
            .HasDefaultValue(PaymentStatusConstants.Confirmed);

        builder.Property(item => item.IsPaymentRequired).HasDefaultValue(false);
        builder.Property(item => item.PaymentMethod).HasMaxLength(Constants.MaxBookingMethodLength);
        builder.Property(item => item.TotalAmountExcludeTax).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.TaxAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.TaxRatePercentage).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.TotalAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxProductPriceCurrencyLength);
        builder.Property(item => item.InvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.InvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.Schedules).HasColumnType("jsonb");
        builder.Property(item => item.LineItems).HasColumnType("jsonb");
        builder.Property(item => item.InvoiceEmailList).HasColumnType("jsonb");

        builder.HasMany(item => item.ResourceBookingSlots).WithMany(item => item.Bookings);
        builder.HasMany(item => item.ProductVersions).WithMany(item => item.Bookings);
        builder.HasMany(item => item.InvolvedCustomers).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedOrganizations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedLocations).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedTeams).WithMany(item => item.InvolvedBookings);
        builder.HasMany(item => item.InvolvedResources).WithMany(item => item.InvolvedBookings);

        builder.HasOne(item => item.PaidByCustomer).WithMany(item => item.PaidBookings);
        builder.HasOne(item => item.PaidByOrganization).WithMany(item => item.PaidBookings);
        builder.HasOne(item => item.CreatedByCustomer).WithMany(item => item.CreatedBookings);
        builder.HasOne(item => item.LastModifiedByCustomer).WithMany(item => item.LastModifiedBookings);
        builder.HasOne(item => item.DeletedByCustomer).WithMany(item => item.DeletedBookings);

        builder.HasIndex(item => item.From);
        builder.HasIndex(item => item.Until);
        builder.HasIndex(item => item.Notes);
        builder.HasIndex(item => item.Category);
        builder.HasIndex(item => item.Channel);
        builder.HasIndex(item => item.PaymentStatus);
        builder.HasIndex(item => item.IsPaymentRequired);
        builder.HasIndex(item => item.PaymentMethod);
        builder.HasIndex(item => item.TotalAmountExcludeTax);
        builder.HasIndex(item => item.TaxAmount);
        builder.HasIndex(item => item.TaxRatePercentage);
        builder.HasIndex(item => item.TotalAmount);
        builder.HasIndex(item => item.Currency);
    }
}
