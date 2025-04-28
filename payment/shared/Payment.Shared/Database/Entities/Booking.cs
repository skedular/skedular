using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Booking : ReplicatedEntityBaseWithDeleted
{
    public ICollection<BookingSchedule>? Schedules { get; set; }
    public ICollection<ProductVersionLineItem>? LineItems { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? StripeCheckoutSessionId { get; set; }
    public virtual StripeCheckoutSession? StripeCheckoutSession { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Schedules).HasColumnType("jsonb");
        builder.Property(item => item.LineItems).HasColumnType("jsonb");

        builder.HasOne(item => item.StripeCheckoutSession).WithOne(item => item.Booking);
    }
}
