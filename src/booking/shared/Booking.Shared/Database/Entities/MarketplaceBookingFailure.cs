using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceBookingFailure : EntityBase
{
    public string FailureKey { get; set; }
    public string Category { get; set; }
    public string Scope { get; set; }
    public DateTimeOffset FinalizedAt { get; set; }
    public DateTimeOffset? RequestedFrom { get; set; }
    public DateTimeOffset? RequestedUntil { get; set; }
    public ICollection<string> RequestedResourceIds { get; set; } = [];
    public string? CustomerAction { get; set; }
    public string? CorrelationId { get; set; }
    public string? Reason { get; set; }
    public string? BookingId { get; set; }
    public string? RecurringBookingId { get; set; }
    public string? MarketplaceBookingSubscriptionId { get; set; }
    public DateTimeOffset? ResolutionDeadlineAt { get; set; }
    public DateTimeOffset? ResolutionDecidedAt { get; set; }
    public string? ResolutionDecision { get; set; }
    public string? ResolutionActorCustomerId { get; set; }
    public decimal? AllocatedRefundAmount { get; set; }
    public ICollection<string> CreatedOccurrenceIds { get; set; } = [];
    public ICollection<string> UnavailableOccurrenceIds { get; set; } = [];

    public virtual ICollection<MarketplaceBookingFailureEvent> Events { get; set; } = [];
    public virtual ICollection<MarketplaceBookingFailureDelivery> Deliveries { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceBookingFailureConfiguration : IEntityTypeConfiguration<MarketplaceBookingFailure>
{
    public void Configure(EntityTypeBuilder<MarketplaceBookingFailure> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.FailureKey).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.Category).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.Scope).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.CustomerAction).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.CorrelationId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.Reason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.RequestedResourceIds).HasColumnType("jsonb");
        builder.Property(item => item.BookingId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.RecurringBookingId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.MarketplaceBookingSubscriptionId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ResolutionDecision).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.ResolutionActorCustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.AllocatedRefundAmount).HasPrecision(18, 2);
        builder.Property(item => item.CreatedOccurrenceIds).HasColumnType("jsonb");
        builder.Property(item => item.UnavailableOccurrenceIds).HasColumnType("jsonb");

        builder.HasIndex(item => item.FailureKey).IsUnique();
        builder.HasIndex(item => item.BookingId);
        builder.HasIndex(item => item.RecurringBookingId);
        builder.HasIndex(item => item.MarketplaceBookingSubscriptionId);
        builder.HasIndex(item => item.FinalizedAt);
        // Expiry workers only need unresolved offers whose deadline has passed.
        builder.HasIndex(item => new
        {
            item.ResolutionDecision,
            item.ResolutionDeadlineAt,
        });
    }
}
