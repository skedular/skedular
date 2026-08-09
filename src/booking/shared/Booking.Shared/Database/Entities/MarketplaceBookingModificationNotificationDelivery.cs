using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingModificationNotificationDelivery : EntityBase
{
    public string DeliveryKey { get; set; }
    public string Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTimeOffset? LastAttemptAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public string? LastError { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientName { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceBookingModificationId { get; set; }
    public virtual MarketplaceBookingModification MarketplaceBookingModification { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RecipientCustomerId { get; set; }
    public virtual Customer? RecipientCustomer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingModificationNotificationDeliveryConfiguration
    : IEntityTypeConfiguration<MarketplaceBookingModificationNotificationDelivery>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingModificationNotificationDelivery> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.DeliveryKey).HasMaxLength(Constants.MaxExternalDeliverKeyLength);
        builder.Property(item => item.Status).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.RecipientEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.RecipientName).HasMaxLength(Constants.MaxDescriptionLength);

        builder.HasOne(item => item.MarketplaceBookingModification)
            .WithMany(item => item.NotificationDeliveries)
            .HasForeignKey(item => item.MarketplaceBookingModificationId);
        builder.HasOne(item => item.RecipientCustomer).WithMany().HasForeignKey(item => item.RecipientCustomerId);

        builder.HasIndex(item => new
        {
            item.MarketplaceBookingModificationId,
            item.DeliveryKey,
        }).IsUnique();
        builder.HasIndex(item => new
        {
            item.Status,
            item.LastAttemptAt,
        });
        builder.HasIndex(item => item.RecipientCustomerId);
    }
}
