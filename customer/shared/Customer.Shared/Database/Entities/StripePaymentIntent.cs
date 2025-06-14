using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripePaymentIntent : EntityBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; }

    public virtual StripePaymentMethod StripePaymentMethod { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripePaymentIntentConfiguration : IEntityTypeConfiguration<StripePaymentIntent>
{
    public void Configure(EntityTypeBuilder<StripePaymentIntent> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);

        builder.HasOne(item => item.StripePaymentMethod).WithMany(item => item.StripePaymentIntents);

        builder.HasIndex(item => item.Amount);
        builder.HasIndex(item => item.Currency);
    }
}
