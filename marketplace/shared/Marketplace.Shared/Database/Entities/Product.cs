using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Product : EntityBaseWithDeleted
{
    public bool Inactive { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string PriceUnit { get; set; }
    public decimal PricePerMinute { get; set; }
    public string Currency { get; set; }
    public int? MinDurationMinutes { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public bool BookAllLocationResources { get; set; }
    public int RecurrenceWindowDays { get; set; }
    public bool RequireConsecutiveDays { get; set; }
    public int? MaxBookingSpreadDays { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public CdnImageFile? PrimaryFeatureImage { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaCard { get; set; }
    public int MaxAllowedResourcesLockTimePaidViaBankTransfer { get; set; }
    public ICollection<string> AcceptedBookingPaymentMethods { get; set; } = [];

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    public virtual ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public virtual ICollection<OrganizationTag> LocationTags { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxProductNameLength);
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxProductDescriptionLength);
        builder.Property(item => item.PriceUnit).HasMaxLength(Constants.MaxProductPriceUnitLength);
        builder.Property(item => item.Price).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.PricePerMinute).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxProductPriceCurrencyLength);
        builder.Property(item => item.BookAllLocationResources).HasDefaultValue(false);
        builder.Property(item => item.RequireConsecutiveDays).HasDefaultValue(false);
        builder.Property(item => item.NumberOfResourcesToBook).HasDefaultValue(1);
        builder.Property(item => item.PrimaryFeatureImage).HasColumnType("jsonb");
        builder.Property(item => item.MaxAllowedResourcesLockTimePaidViaCard).HasDefaultValue(Constants.DefaultMaxAllowedResourcesLockTimePaidViaCard);
        builder
            .Property(item => item.MaxAllowedResourcesLockTimePaidViaBankTransfer)
            .HasDefaultValue(Constants.DefaultMaxAllowedResourcesLockTimePaidViaBankTransfer);
        builder.Property(item => item.AcceptedBookingPaymentMethods).HasColumnType("jsonb").HasDefaultValue(Array.Empty<string>());

        builder.HasOne(item => item.Organization).WithMany(item => item.Products).HasForeignKey(item => item.OrganizationId);
        builder.HasMany(item => item.ProductTags).WithMany(item => item.ProductProductTag);
        builder.HasMany(item => item.LocationTags).WithMany(item => item.ProductLocationTags);

        builder.HasIndex(item => item.Inactive);
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.PricePerMinute);
        builder.HasIndex(item => item.Currency);
    }
}
