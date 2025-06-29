using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeProduct : EntityBaseWithDeleted
{
    public string StripeProductId { get; set; }
    public string StripeAccountId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ProductVersionId { get; set; }
    public virtual ProductVersion ProductVersion { get; set; }

    public virtual StripePrice? StripePrice { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeProductConfiguration : IEntityTypeConfiguration<StripeProduct>
{
    public void Configure(EntityTypeBuilder<StripeProduct> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeProductId).HasMaxLength(Constants.MaxStripeProductIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxStripeConnectAccountIdLength);

        builder.HasOne(item => item.ProductVersion).WithOne(item => item.StripeProduct).HasForeignKey<StripeProduct>(item => item.ProductVersionId);

        builder.HasIndex(item => item.StripeAccountId);
    }
}
