using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ProductVersion : EntityBase
{
    public string? Name { get; set; }
    public string? Currency { get; set; }
    public ICollection<ProductPricing>? PricingOptions { get; set; } = [];

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }

    public virtual ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public virtual ICollection<OrganizationTag> LocationTags { get; set; } = [];
    public virtual ICollection<MarketplaceBooking> MarketplaceBookings { get; set; } = [];
    public virtual ICollection<StripeProduct> StripeProducts { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxProductNameLength);
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.PricingOptions).HasColumnType("jsonb");

        builder.HasOne(item => item.Product).WithMany(item => item.ProductVersions).HasForeignKey(item => item.ProductId);
        builder.HasMany(item => item.ProductTags).WithMany(item => item.ProductVersionProductTag);
        builder.HasMany(item => item.LocationTags).WithMany(item => item.ProductVersionLocationTags);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Currency);
    }
}
