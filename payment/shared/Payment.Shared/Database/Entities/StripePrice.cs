using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripePrice : EntityBaseWithDeleted
{
    public string StripePriceId { get; set; }
    public virtual ProductVersion? ProductVersion { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripePriceConfiguration : IEntityTypeConfiguration<StripePrice>
{
    public void Configure(EntityTypeBuilder<StripePrice> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripePriceId).HasMaxLength(Constants.StripePriceIdLength);

        builder.HasIndex(item => item.StripePriceId);
    }
}
