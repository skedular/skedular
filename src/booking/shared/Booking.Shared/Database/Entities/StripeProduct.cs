using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeProduct : EntityBaseWithDeleted
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductVersionId { get; set; }
    public virtual ProductVersion ProductVersion { get; set; }

    public string ProductPricingId { get; set; }
    public string MembershipTerm { get; set; }
    public string BillingMode { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public string StripeProductId { get; set; }
    public string StripeAccountId { get; set; }

    public virtual ICollection<StripePrice> StripePrices { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeProductConfiguration : IEntityTypeConfiguration<StripeProduct>
{
    public void Configure(EntityTypeBuilder<StripeProduct> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.MembershipTerm).HasMaxLength(Constants.MaxMembershipTermLength);
        builder.Property(item => item.BillingMode).HasMaxLength(Constants.MaxProductPricingBillingModeLength);
        builder.Property(item => item.ProductPricingId).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.StripeProductId).HasMaxLength(Constants.MaxStripeProductIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);
        builder.HasOne(item => item.ProductVersion).WithMany(item => item.StripeProducts).HasForeignKey(item => item.ProductVersionId);

        builder.HasIndex(item => item.ProductPricingId);
        builder.HasIndex(item => item.MembershipTerm);
        builder.HasIndex(item => item.BillingMode);
        builder.HasIndex(item => item.StripeProductId);
        builder.HasIndex(item => item.StripeAccountId);
        builder.HasIndex(item => item.NumberOfResourcesToBook);
    }
}
