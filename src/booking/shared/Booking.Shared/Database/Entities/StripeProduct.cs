using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeProduct : EntityBaseWithDeleted
{
    public string ProductPricingId { get; set; }
    public string PricingCadence { get; set; }
    public string BillingMode { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public string StripeProductId { get; set; }
    public string StripeAccountId { get; set; }

    public virtual ProductVersion ProductVersion { get; set; }
    public virtual StripePrice? StripePrice { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeProductConfiguration : IEntityTypeConfiguration<StripeProduct>
{
    public void Configure(EntityTypeBuilder<StripeProduct> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.PricingCadence).HasMaxLength(Constants.MaxProductPricingCadenceLength);
        builder.Property(item => item.BillingMode).HasMaxLength(Constants.MaxProductPricingBillingModeLength);
        builder.Property(item => item.ProductPricingId).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.StripeProductId).HasMaxLength(Constants.MaxStripeProductIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);

        builder.HasOne(item => item.ProductVersion).WithMany(item => item.StripeProducts);

        builder.HasIndex(item => item.ProductPricingId);
        builder.HasIndex(item => item.PricingCadence);
        builder.HasIndex(item => item.BillingMode);
        builder.HasIndex(item => item.StripeProductId);
        builder.HasIndex(item => item.StripeAccountId);
        builder.HasIndex(item => item.NumberOfResourcesToBook);
    }
}
