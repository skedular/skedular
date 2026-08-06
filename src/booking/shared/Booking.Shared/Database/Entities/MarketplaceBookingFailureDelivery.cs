using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingFailureDelivery : EntityBase
{
    public string MarketplaceBookingFailureId { get; set; }
    public string RecipientKey { get; set; }
    public string? RecipientCustomerId { get; set; }
    public string? RecipientEmail { get; set; }
    public string Audience { get; set; }
    public string Channel { get; set; }
    public string Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTimeOffset? LastAttemptAt { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public string? LastError { get; set; }
    public virtual MarketplaceBookingFailure MarketplaceBookingFailure { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingFailureDeliveryConfiguration : IEntityTypeConfiguration<MarketplaceBookingFailureDelivery>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingFailureDelivery> builder)
    {
        builder.ConfigureEntityBase();
        builder.Property(item => item.RecipientKey).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.RecipientCustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.RecipientEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.Audience).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.Channel).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.Status).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder.HasOne(item => item.MarketplaceBookingFailure)
            .WithMany(item => item.Deliveries)
            .HasForeignKey(item => item.MarketplaceBookingFailureId);

        builder.HasIndex(item => new
        {
            item.MarketplaceBookingFailureId,
            item.RecipientKey,
            item.Channel,
        }).IsUnique();
        builder.HasIndex(item => new
        {
            item.Status,
            item.LastAttemptAt,
        });
    }
}
