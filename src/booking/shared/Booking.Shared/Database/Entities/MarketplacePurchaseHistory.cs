using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

/// <summary>
///     Rebuildable operator-read projection. The referenced booking or subscription remains
///     authoritative; this row intentionally carries only history search and audit fields.
/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplacePurchaseHistory : EntityBase
{
    public string SourceType { get; set; }
    public string SourceId { get; set; }
    public string OrganizationId { get; set; }
    public string? ProductVersionId { get; set; }
    public string? ProductTitle { get; set; }
    public string? CustomerId { get; set; }
    public DateTimeOffset PurchasedAt { get; set; }
    public DateTimeOffset ActivityAt { get; set; }
    public DateTimeOffset? BookingFrom { get; set; }
    public DateTimeOffset? BookingUntil { get; set; }
    public string? PaymentStatus { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? SubscriptionStatus { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public bool IsDeleted { get; set; }
    public string? DeletedByCustomerId { get; set; }
    public string? CancellationReason { get; set; }
    public string? LatestRefundId { get; set; }
    public string? LatestRefundStatus { get; set; }

    public string? MarketplaceBookingId { get; set; }
    public virtual MarketplaceBooking? MarketplaceBooking { get; set; }

    public string? MarketplaceBookingSubscriptionId { get; set; }
    public virtual MarketplaceBookingSubscription? MarketplaceBookingSubscription { get; set; }
    public virtual Organization Organization { get; set; }
    public virtual ProductVersion? ProductVersion { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }
    public virtual MarketplaceRefund? LatestRefund { get; set; }
}
#pragma warning restore CS8618

public class MarketplacePurchaseHistoryConfiguration : IEntityTypeConfiguration<MarketplacePurchaseHistory>
{
    public void Configure(EntityTypeBuilder<MarketplacePurchaseHistory> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.SourceType).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.SourceId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.OrganizationId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.MarketplaceBookingId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.MarketplaceBookingSubscriptionId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ProductVersionId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ProductTitle).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.CustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.PaymentStatus).HasMaxLength(Constants.MaxBookingPaymentStatusLength);
        builder.Property(item => item.TotalAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.SubscriptionStatus).HasMaxLength(Constants.MaxMarketplaceBookingSubscriptionStatusLength);
        builder.Property(item => item.DeletedByCustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.CancellationReason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.LatestRefundId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.LatestRefundStatus).HasMaxLength(Constants.MaxAccountingStatusLength);

        builder
            .HasOne(item => item.MarketplaceBooking)
            .WithMany()
            .HasForeignKey(item => item.MarketplaceBookingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(item => item.MarketplaceBookingSubscription)
            .WithMany()
            .HasForeignKey(item => item.MarketplaceBookingSubscriptionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(item => item.Organization)
            .WithMany()
            .HasForeignKey(item => item.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        builder
            .HasOne(item => item.ProductVersion)
            .WithMany()
            .HasForeignKey(item => item.ProductVersionId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(item => item.Customer)
            .WithMany()
            .HasForeignKey(item => item.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(item => item.DeletedByCustomer)
            .WithMany()
            .HasForeignKey(item => item.DeletedByCustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder
            .HasOne(item => item.LatestRefund)
            .WithMany()
            .HasForeignKey(item => item.LatestRefundId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(item => new
        {
            item.SourceType,
            item.SourceId,
        }).IsUnique();
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.ActivityAt,
            item.SourceType,
            item.SourceId,
        });
        builder.HasIndex(item => item.MarketplaceBookingId).IsUnique().HasFilter("\"MarketplaceBookingId\" IS NOT NULL");
        builder.HasIndex(item => item.MarketplaceBookingSubscriptionId).IsUnique().HasFilter("\"MarketplaceBookingSubscriptionId\" IS NOT NULL");
        builder.HasIndex(item => item.ProductVersionId);
        builder.HasIndex(item => item.CustomerId);
        builder.HasIndex(item => item.DeletedByCustomerId);
        builder.HasIndex(item => item.LatestRefundId).IsUnique().HasFilter("\"LatestRefundId\" IS NOT NULL");
    }
}
