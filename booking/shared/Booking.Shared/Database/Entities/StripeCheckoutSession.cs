using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeCheckoutSession : EntityBaseWithDeleted
{
    public string StripeCheckoutSessionId { get; set; }
    public string CheckoutUrl { get; set; }
    public decimal? AmountTotal { get; set; }
    public string? Currency { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string StripeCustomerCustomerId { get; set; }
    public virtual StripeCustomer StripeCustomer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string BookingId { get; set; }
    public virtual Booking Booking { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeCheckoutSessionConfiguration : IEntityTypeConfiguration<StripeCheckoutSession>
{
    public void Configure(EntityTypeBuilder<StripeCheckoutSession> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeCheckoutSessionId).HasMaxLength(Constants.MaxStripeCheckoutSessionIdLength);
        builder.Property(item => item.CheckoutUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.AmountTotal).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxProductPriceCurrencyLength);

        builder
            .HasOne(item => item.StripeCustomer)
            .WithMany(item => item.StripeCheckoutSessions)
            .HasForeignKey(item => item.StripeCustomerCustomerId);

        builder.HasOne(item => item.Booking).WithOne(item => item.StripeCheckoutSession).HasForeignKey<StripeCheckoutSession>(item => item.BookingId);

        builder.HasIndex(item => item.StripeCheckoutSessionId);
    }
}
