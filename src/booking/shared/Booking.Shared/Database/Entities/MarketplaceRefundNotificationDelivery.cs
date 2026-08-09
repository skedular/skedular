using Api.Shared.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

public class MarketplaceRefundNotificationDelivery : EntityBase
{
    public string MarketplaceRefundId { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string RecipientId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public int AttemptCount { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public string? LastError { get; set; }
    public virtual MarketplaceRefund MarketplaceRefund { get; set; } = null!;
}

public class MarketplaceRefundNotificationDeliveryConfiguration
    : IEntityTypeConfiguration<MarketplaceRefundNotificationDelivery>
{
    public void Configure(EntityTypeBuilder<MarketplaceRefundNotificationDelivery> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EventType).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.RecipientId).HasMaxLength(Constants.MaxRefundNotificationRecipientIdLength);
        builder.Property(item => item.Status)
            .HasMaxLength(Constants.MaxRefundNotificationStatusLength)
            .HasDefaultValue(MarketplaceRefundNotificationDeliveryStatusConstants.Pending);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxRefundResolutionReasonLength);

        builder.HasOne(item => item.MarketplaceRefund).WithMany().HasForeignKey(item => item.MarketplaceRefundId);

        builder.HasIndex(item => new
        {
            item.MarketplaceRefundId,
            item.EventType,
            item.RecipientId,
        }).IsUnique();
        builder.HasIndex(item => item.Status);
    }
}
