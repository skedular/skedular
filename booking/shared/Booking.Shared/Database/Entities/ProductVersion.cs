using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ProductVersion : EntityBase
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? PriceUnit { get; set; }
    public bool? IsPriceTaxInclusive { get; set; }
    public decimal? PricePerMinute { get; set; }
    public string? Currency { get; set; }
    public int? MinDurationMinutes { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public bool? BookAllLocationResources { get; set; }
    public int? NumberOfResourcesToBook { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaCard { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaBankTransfer { get; set; }
    public ICollection<string>? AcceptedBookingPaymentMethods { get; set; } = [];

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }

    public virtual ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public virtual ICollection<OrganizationTag> LocationTags { get; set; } = [];
    public virtual ICollection<MarketplaceBooking> MarketplaceBookings { get; set; } = [];
    public virtual StripeProduct? StripeProduct { get; set; }
    public virtual StripePrice? StripePrice { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxProductNameLength);
        builder.Property(item => item.PriceUnit).HasMaxLength(Constants.MaxProductPriceUnitLength);
        builder.Property(item => item.Price).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.PricePerMinute).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxProductPriceCurrencyLength);
        builder.Property(item => item.BookAllLocationResources).HasDefaultValue(false);
        builder.Property(item => item.NumberOfResourcesToBook).HasDefaultValue(1);
        builder.Property(item => item.MaxAllowedResourcesLockTimePaidViaCard)
            .HasDefaultValue(Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard);
        builder
            .Property(item => item.MaxAllowedResourcesLockTimePaidViaBankTransfer)
            .HasDefaultValue(Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer);
        builder.Property(item => item.AcceptedBookingPaymentMethods).HasColumnType("jsonb");

        builder.HasOne(item => item.Product).WithMany(item => item.ProductVersions).HasForeignKey(item => item.ProductId);
        builder.HasMany(item => item.ProductTags).WithMany(item => item.ProductVersionProductTag);
        builder.HasMany(item => item.LocationTags).WithMany(item => item.ProductVersionLocationTags);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.PricePerMinute);
        builder.HasIndex(item => item.Currency);
    }
}
