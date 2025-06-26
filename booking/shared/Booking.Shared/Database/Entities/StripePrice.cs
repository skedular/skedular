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
    public string? ProductVersionId { get; set; }
    public virtual ProductVersion ProductVersion { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeProductId { get; set; }
    public virtual StripeProduct StripeProduct { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripePriceConfiguration : IEntityTypeConfiguration<StripePrice>
{
    public void Configure(EntityTypeBuilder<StripePrice> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripePriceId).HasMaxLength(Constants.MaxStripePriceIdLength);

        builder.HasOne(item => item.ProductVersion).WithOne(item => item.StripePrice).HasForeignKey<StripePrice>(item => item.ProductVersionId);
        builder.HasOne(item => item.StripeProduct).WithOne(item => item.StripePrice).HasForeignKey<StripePrice>(item => item.StripeProductId);
    }
}
