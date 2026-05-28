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

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string StripeCustomerCustomerId { get; set; }
    public virtual StripeCustomer StripeCustomer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceBookingId { get; set; }
    public virtual MarketplaceBooking MarketplaceBooking { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeCheckoutSessionConfiguration : IEntityTypeConfiguration<StripeCheckoutSession>
{
    public void Configure(EntityTypeBuilder<StripeCheckoutSession> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.StripeCheckoutSessionId).HasMaxLength(Constants.MaxStripeCheckoutSessionIdLength);
        builder.Property(item => item.CheckoutUrl).HasMaxLength(Constants.MaxUrlLength);

        builder
            .HasOne(item => item.StripeCustomer)
            .WithMany(item => item.StripeCheckoutSessions)
            .HasForeignKey(item => item.StripeCustomerCustomerId);

        builder
            .HasOne(item => item.MarketplaceBooking)
            .WithOne(item => item.StripeCheckoutSession)
            .HasForeignKey<StripeCheckoutSession>(item => item.MarketplaceBookingId);

        builder.HasIndex(item => item.StripeCheckoutSessionId);
    }
}
