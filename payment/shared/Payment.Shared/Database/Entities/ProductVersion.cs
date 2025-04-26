using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ProductVersion : EntityBase
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? PriceUnit { get; set; }
    public string? Currency { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OrganizationStripeConnectAccountId { get; set; }
    public virtual OrganizationStripeConnectAccount? OrganizationStripeConnectAccount { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeProductId { get; set; }
    public virtual StripeProduct? StripeProduct { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripePriceId { get; set; }
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
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxProductPriceCurrencyLength);

        builder.HasOne(item => item.Product).WithMany(item => item.ProductVersions).HasForeignKey(item => item.ProductId);
        builder
            .HasOne(item => item.OrganizationStripeConnectAccount)
            .WithMany(item => item.ProductVersions)
            .HasForeignKey(item => item.OrganizationStripeConnectAccountId);

        builder.HasOne(item => item.StripeProduct).WithOne(item => item.ProductVersion).HasForeignKey<ProductVersion>(item => item.StripeProductId);
        builder.HasOne(item => item.StripePrice).WithOne(item => item.ProductVersion).HasForeignKey<ProductVersion>(item => item.StripePriceId);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Currency);
    }
}
