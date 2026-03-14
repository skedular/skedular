using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBooking : EntityBase
{
    public string PaymentStatus { get; set; }
    public bool IsPaymentRequired { get; set; }
    public int Quantity { get; set; }
    public ProductPricing ProductPricing { get; set; }
    public string PaymentMethod { get; set; }
    public DateTimeOffset PaymentExpiry { get; set; }
    public decimal? TotalAmountExcludeTax { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? TaxRatePercentage { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public ICollection<string> InvoiceEmailList { get; set; } = [];
    public ProductPricingBillingSchedule BillingSchedule { get; set; } = ProductPricingBillingSchedule.Empty;

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? BookingId { get; set; }
    public virtual Booking? Booking { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RecurringBookingId { get; set; }
    public virtual RecurringBooking? RecurringBooking { get; set; }

    public virtual ProductVersion ProductVersion { get; set; }
    public virtual Customer? PaidByCustomer { get; set; }
    public virtual Organization? PaidByOrganization { get; set; }
    public virtual StripeCheckoutSession? StripeCheckoutSession { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingConfiguration : IEntityTypeConfiguration<MarketplaceBooking>
{
    public void Configure(EntityTypeBuilder<MarketplaceBooking> builder)
    {
        builder.ConfigureEntityBase();

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
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.InvoiceUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.InvoiceNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.BillingSchedule).HasColumnType("jsonb");
        builder.Property(item => item.ProductPricing).HasColumnType("jsonb");
        builder.Property(item => item.InvoiceEmailList).HasColumnType("jsonb");

        builder
            .HasOne(item => item.Booking)
            .WithOne(item => item.MarketplaceBooking)
            .HasForeignKey<MarketplaceBooking>(item => item.BookingId);

        builder
            .HasOne(item => item.RecurringBooking)
            .WithOne(item => item.MarketplaceBooking)
            .HasForeignKey<MarketplaceBooking>(item => item.RecurringBookingId);

        builder.HasOne(item => item.ProductVersion).WithMany(item => item.MarketplaceBookings);
        builder.HasOne(item => item.PaidByCustomer).WithMany(item => item.PaidMarketplaceBookings);
        builder.HasOne(item => item.PaidByOrganization).WithMany(item => item.PaidMarketplaceBookings);

        builder.HasIndex(item => item.PaymentStatus);
        builder.HasIndex(item => item.IsPaymentRequired);
        builder.HasIndex(item => item.Quantity);
        builder.HasIndex(item => item.PaymentMethod);
        builder.HasIndex(item => item.PaymentExpiry);
        builder.HasIndex(item => item.TotalAmountExcludeTax);
        builder.HasIndex(item => item.TaxAmount);
        builder.HasIndex(item => item.TaxRatePercentage);
        builder.HasIndex(item => item.TotalAmount);
        builder.HasIndex(item => item.Currency);
    }
}
