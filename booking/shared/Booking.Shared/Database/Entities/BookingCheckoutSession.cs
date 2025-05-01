using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class BookingCheckoutSession : EntityBase
{
    public string PaymentReferenceId { get; set; }
    public string CheckoutUrl { get; set; }
    public string PaymentStatus { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string BookingId { get; set; }
    public virtual Booking Booking { get; set; }
}

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingCheckoutSessionConfiguration : IEntityTypeConfiguration<BookingCheckoutSession>
{
    public void Configure(EntityTypeBuilder<BookingCheckoutSession> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.PaymentReferenceId).HasMaxLength(Constants.MaxUniqueIdLength);
        builder.Property(item => item.CheckoutUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.PaymentStatus).HasMaxLength(Constants.MaxPaymentStatusLength);

        builder
            .HasOne(item => item.Booking)
            .WithOne(item => item.BookingCheckoutSession)
            .HasForeignKey<BookingCheckoutSession>(item => item.BookingId);

        builder.HasIndex(item => item.PaymentReferenceId);
        builder.HasIndex(item => item.PaymentStatus);
    }
}
