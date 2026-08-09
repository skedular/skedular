using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingModification : EntityBase
{
    public string ActorKind { get; set; }
    public string? Reason { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public DateTimeOffset OriginalFrom { get; set; }
    public DateTimeOffset OriginalUntil { get; set; }
    public DateTimeOffset ResultFrom { get; set; }
    public DateTimeOffset ResultUntil { get; set; }
    public ICollection<string> OriginalResourceIds { get; set; } = [];
    public ICollection<string> ResultResourceIds { get; set; } = [];
    public bool SubscriptionOccurrenceOverride { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string BookingId { get; set; }
    public virtual Booking Booking { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ActorCustomerId { get; set; }
    public virtual Customer ActorCustomer { get; set; }

    public virtual ICollection<MarketplaceBookingModificationNotificationDelivery> NotificationDeliveries { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingModificationConfiguration : IEntityTypeConfiguration<MarketplaceBookingModification>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingModification> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.ActorKind).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.Reason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.OriginalResourceIds).HasColumnType("jsonb");
        builder.Property(item => item.ResultResourceIds).HasColumnType("jsonb");

        builder.HasOne(item => item.Booking).WithMany().HasForeignKey(item => item.BookingId);
        builder.HasOne(item => item.ActorCustomer).WithMany().HasForeignKey(item => item.ActorCustomerId);

        builder.HasIndex(item => new
        {
            item.BookingId,
            item.OccurredAt,
        });
        builder.HasIndex(item => item.ActorCustomerId);
    }
}
