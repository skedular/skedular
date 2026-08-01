using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingFailureEvent : EntityBase
{
    public string EventType { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string? Reason { get; set; }
    public string? LastError { get; set; }
    public string? ActorCustomerId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceBookingFailureId { get; set; }
    public virtual MarketplaceBookingFailure MarketplaceBookingFailure { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingFailureEventConfiguration : IEntityTypeConfiguration<MarketplaceBookingFailureEvent>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingFailureEvent> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EventType).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.Reason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.ActorCustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);

        builder.HasOne(item => item.MarketplaceBookingFailure).WithMany(item => item.Events).HasForeignKey(item => item.MarketplaceBookingFailureId);

        builder.HasIndex(item => new { item.MarketplaceBookingFailureId, item.OccurredAt, item.CreatedAt });
    }
}
