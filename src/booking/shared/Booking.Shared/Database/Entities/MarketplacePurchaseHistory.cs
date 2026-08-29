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
    public string? ProductTitle { get; set; }
    public DateTimeOffset PurchasedAt { get; set; }
    public DateTimeOffset ActivityAt { get; set; }
    public DateTimeOffset? BookingFrom { get; set; }
    public DateTimeOffset? BookingUntil { get; set; }
    public string? PaymentStatus { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? Currency { get; set; }
    public string? SubscriptionStatus { get; set; }
    public string? EntitlementStatus { get; set; }
    public int CreditQuantity { get; set; }
    public int GrantedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public bool IsDeleted { get; set; }
    public string? CancellationReason { get; set; }
    public string? LatestRefundStatus { get; set; }

    // Event fields. A row represents one immutable lifecycle event for eligible purchases.
    // EventId is the backend-owned event identifier. Id remains the standard entity primary key.
    public string? EventId { get; set; }
    public string? EventType { get; set; }
    public string? IdempotencyKey { get; set; }
    public DateTimeOffset? OccurredAt { get; set; }
    public DateTimeOffset? RecordedAt { get; set; }
    public string? CorrelationId { get; set; }
    public string? PreviousPaymentStatus { get; set; }
    public string? PreviousRefundStatus { get; set; }
    public string? RefundStatus { get; set; }
    public int? EventCreditQuantity { get; set; }
    public int? EventRemainingCreditQuantity { get; set; }
    public decimal? EventAmount { get; set; }
    public string? EventCurrency { get; set; }
    public DateTimeOffset? CancellationRequestedAt { get; set; }
    public DateTimeOffset? CancellationEffectiveAt { get; set; }
    public DateTimeOffset? RenewalAt { get; set; }
    public string? EventReason { get; set; }
    public string? EventSubscriptionStatus { get; set; }
    public string? EventEntitlementStatus { get; set; }
    public bool? EventAutoRenew { get; set; }
    public bool? EventCancelAtPeriodEnd { get; set; }
    public bool? EventIsDeleted { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? MarketplaceBookingId { get; set; }
    public virtual MarketplaceBooking? MarketplaceBooking { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? MarketplaceBookingSubscriptionId { get; set; }
    public virtual MarketplaceBookingSubscription? MarketplaceBookingSubscription { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? EntitlementPurchaseId { get; set; }
    public virtual EntitlementPurchase? EntitlementPurchase { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ProductVersionId { get; set; }
    public virtual ProductVersion? ProductVersion { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? CustomerId { get; set; }
    public virtual Customer? Customer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? DeletedByCustomerId { get; set; }
    public virtual Customer? DeletedByCustomer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? LatestRefundId { get; set; }
    public virtual MarketplaceRefund? LatestRefund { get; set; }
}
#pragma warning restore CS8618

public class MarketplacePurchaseHistoryConfiguration : IEntityTypeConfiguration<MarketplacePurchaseHistory>
{
    public void Configure(EntityTypeBuilder<MarketplacePurchaseHistory> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.SourceType).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.SourceId).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.ProductTitle).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.PaymentStatus).HasMaxLength(Constants.MaxBookingPaymentStatusLength);
        builder.Property(item => item.TotalAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.SubscriptionStatus).HasMaxLength(Constants.MaxMarketplaceBookingSubscriptionStatusLength);
        builder.Property(item => item.EntitlementStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.CancellationReason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.LatestRefundStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.EventType).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.EventId).HasMaxLength(512);
        // Refund lifecycle idempotency keys include the source type, source id, refund id, and status.
        builder.Property(item => item.IdempotencyKey).HasMaxLength(512);
        builder.Property(item => item.CorrelationId).HasMaxLength(Constants.MaxRefundCorrelationIdLength);
        builder.Property(item => item.PreviousPaymentStatus).HasMaxLength(Constants.MaxBookingPaymentStatusLength);
        builder.Property(item => item.PreviousRefundStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.RefundStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.EventAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.EventCurrency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.EventReason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.EventSubscriptionStatus).HasMaxLength(Constants.MaxMarketplaceBookingSubscriptionStatusLength);
        builder.Property(item => item.EventEntitlementStatus).HasMaxLength(Constants.MaxAccountingStatusLength);

        builder.HasOne(item => item.MarketplaceBooking).WithMany().HasForeignKey(item => item.MarketplaceBookingId);
        builder.HasOne(item => item.MarketplaceBookingSubscription).WithMany().HasForeignKey(item => item.MarketplaceBookingSubscriptionId);
        builder.HasOne(item => item.EntitlementPurchase).WithMany().HasForeignKey(item => item.EntitlementPurchaseId);
        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.ProductVersion).WithMany().HasForeignKey(item => item.ProductVersionId);
        builder.HasOne(item => item.Customer).WithMany().HasForeignKey(item => item.CustomerId);
        builder.HasOne(item => item.DeletedByCustomer).WithMany().HasForeignKey(item => item.DeletedByCustomerId);
        builder.HasOne(item => item.LatestRefund).WithMany().HasForeignKey(item => item.LatestRefundId);

        builder.HasIndex(item => new
            {
                item.SourceType,
                item.SourceId,
                item.IdempotencyKey,
            }).IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.ActivityAt,
            item.SourceType,
            item.SourceId,
        });
        builder.HasIndex(item => new
        {
            item.SourceType,
            item.SourceId,
            item.OccurredAt,
            item.RecordedAt,
            item.Id,
        }).HasFilter("\"EventType\" IS NOT NULL");
        builder.HasIndex(item => new
        {
            item.SourceType,
            item.SourceId,
            item.EventType,
            item.OccurredAt,
        }).HasFilter("\"EventType\" IS NOT NULL");
        builder.HasIndex(item => item.EventId).IsUnique().HasFilter("\"EventId\" IS NOT NULL");
        builder.HasIndex(item => item.MarketplaceBookingId).HasFilter("\"MarketplaceBookingId\" IS NOT NULL");
        builder.HasIndex(item => item.MarketplaceBookingSubscriptionId).HasFilter("\"MarketplaceBookingSubscriptionId\" IS NOT NULL");
        builder.HasIndex(item => item.EntitlementPurchaseId).HasFilter("\"EntitlementPurchaseId\" IS NOT NULL");
        builder.HasIndex(item => item.ProductVersionId);
        builder.HasIndex(item => item.CustomerId);
        builder.HasIndex(item => item.DeletedByCustomerId);
        builder.HasIndex(item => item.LatestRefundId)
            .HasFilter("\"LatestRefundId\" IS NOT NULL");
    }
}
