using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ProductVersion : EntityBase
{
    public string Currency { get; set; }
    public ICollection<CdnImageFile>? FeatureImages { get; set; }
    public ICollection<ProductPricing> PricingOptions { get; set; } = [];
    public ListingMetadata? ListingMetadata { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }

    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ProductVersionConfiguration : IEntityTypeConfiguration<ProductVersion>
{
    public void Configure(EntityTypeBuilder<ProductVersion> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.FeatureImages).HasColumnType("jsonb");
        builder.Property(item => item.PricingOptions).HasColumnType("jsonb");
        builder.Property(item => item.ListingMetadata).HasColumnType("jsonb");

        builder.HasOne(item => item.Product).WithMany(item => item.ProductVersions).HasForeignKey(item => item.ProductId);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.ProductVersionOrganizationTags);

        builder.HasIndex(item => item.Currency);
    }
}
