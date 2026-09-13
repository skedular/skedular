using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripePrice : EntityBaseWithDeleted
{
    public string StripePriceId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeProductId { get; set; }
    public bool IsRecurring { get; set; }
    public string? ProductPricingId { get; set; }
    public string? StripeAccountId { get; set; }
    public string? Currency { get; set; }
    public decimal? UnitAmount { get; set; }
    public string? MembershipTerm { get; set; }
    public string? BillingMode { get; set; }
    public bool? IsTaxInclusive { get; set; }

    public virtual StripeProduct? StripeProduct { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripePriceConfiguration : IEntityTypeConfiguration<StripePrice>
{
    public void Configure(EntityTypeBuilder<StripePrice> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripePriceId).HasMaxLength(Constants.MaxStripePriceIdLength);

        builder.Property(item => item.ProductPricingId).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.UnitAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.MembershipTerm).HasMaxLength(Constants.MaxMembershipTermLength);
        builder.Property(item => item.BillingMode).HasMaxLength(Constants.MaxProductPricingBillingModeLength);

        builder.HasOne(item => item.StripeProduct).WithMany(item => item.StripePrices).HasForeignKey(item => item.StripeProductId);

        builder.HasIndex(item => new
        {
            item.StripeAccountId,
            item.StripePriceId,
        }).IsUnique();

        builder.HasIndex(item => new
        {
            item.StripeAccountId,
            item.ProductPricingId,
            item.UnitAmount,
            item.Currency,
            item.MembershipTerm,
            item.BillingMode,
            item.IsTaxInclusive,
            item.IsRecurring,
        }).IsUnique();
    }
}
