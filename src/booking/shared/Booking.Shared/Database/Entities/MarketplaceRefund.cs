using Api.Shared.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceRefund : EntityBase
{
    public string LocalEntityType { get; set; }
    public string LocalEntityId { get; set; }
    public string Status { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset ReferenceTime { get; set; }
    public int RefundPercentage { get; set; }
    public int? AppliedRuleMinutesBefore { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? Currency { get; set; }
    public string? Reason { get; set; }
    public string? AccountingProvider { get; set; }
    public string? ExternalRefundId { get; set; }
    public string? ExternalRefundNumber { get; set; }
    public DateTimeOffset? LastProcessedAt { get; set; }
    public string? LastError { get; set; }
    public string? PaymentProvider { get; set; }
    public string? ExternalPaymentRefundId { get; set; }
    public string? PaymentRefundStatus { get; set; }
    public DateTimeOffset? PaymentRefundLastProcessedAt { get; set; }
    public string? PaymentRefundLastError { get; set; }
    public string RefundKind { get; set; } = null!;
    public string IdempotencyKey { get; set; } = null!;
    public string? PolicySnapshotJson { get; set; }
    public string? CalculationResultJson { get; set; }
    public string? TimezoneId { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTimeOffset? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public string? BankTransferReference { get; set; }
    public DateTimeOffset? BankTransferSentAt { get; set; }
    public DateTimeOffset? ReconciledAt { get; set; }
    public DateTimeOffset? LastReconciledAt { get; set; }
    public string? ReconciliationStatus { get; set; }
    public string? LastNotificationStatus { get; set; }
    public bool PostPayoutRefund { get; set; }
    public string? StripeRefundPath { get; set; }
    public string? StripeAccountId { get; set; }
    public string? StripeChargeType { get; set; }
    public string? StripeTransferId { get; set; }
    public string? StripeChargeId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public DateTimeOffset? StripeRefundPathSelectedAt { get; set; }
    public string? ReconciliationLeaseOwner { get; set; }
    public DateTimeOffset? ReconciliationLeaseExpiresAt { get; set; }
    public DateTimeOffset? ReconciliationLeaseRenewedAt { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RequestedByCustomerId { get; set; }
    public virtual Customer? RequestedByCustomer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ApprovedByCustomerId { get; set; }
    public virtual Customer? ApprovedByCustomer { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RejectedByCustomerId { get; set; }
    public virtual Customer? RejectedByCustomer { get; set; }

    public virtual ICollection<MarketplaceRefundEvent> Events { get; set; } = [];
    public virtual ICollection<MarketplaceRefundPaymentAllocation> PaymentAllocations { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceRefundConfiguration : IEntityTypeConfiguration<MarketplaceRefund>
{
    public void Configure(EntityTypeBuilder<MarketplaceRefund> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.LocalEntityType).HasMaxLength(Constants.MaxAccountingEntityTypeLength);
        builder.Property(item => item.LocalEntityId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.Status).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.BaseAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.RefundAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.Reason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.AccountingProvider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.ExternalRefundId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ExternalRefundNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.PaymentProvider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.ExternalPaymentRefundId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.PaymentRefundStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.PaymentRefundLastError).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.RefundKind)
            .HasMaxLength(Constants.MaxAccountingStatusLength)
            .HasDefaultValue(MarketplaceRefundKindConstants.Cancellation).IsRequired();
        builder.Property(item => item.IdempotencyKey).HasMaxLength(Constants.MaxRefundIdempotencyKeyLength).IsRequired();
        builder.Property(item => item.PolicySnapshotJson).HasColumnType("text");
        builder.Property(item => item.CalculationResultJson).HasColumnType("text");
        builder.Property(item => item.TimezoneId).HasMaxLength(Constants.MaxRefundTimezoneIdLength);
        builder.Property(item => item.RejectionReason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.CancellationReason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.BankTransferReference).HasMaxLength(Constants.MaxRefundBankTransferReferenceLength);
        builder.Property(item => item.ReconciliationStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.LastNotificationStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.StripeRefundPath).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxRefundStripeAccountIdLength);
        builder.Property(item => item.StripeChargeType).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.StripeTransferId).HasMaxLength(Constants.MaxRefundStripeTransferIdLength);
        builder.Property(item => item.StripeChargeId).HasMaxLength(Constants.MaxRefundStripeChargeIdLength);
        builder.Property(item => item.StripePaymentIntentId).HasMaxLength(Constants.MaxRefundStripePaymentIntentIdLength);
        builder.Property(item => item.ReconciliationLeaseOwner).HasMaxLength(Constants.MaxRefundLeaseOwnerLength);
        builder.HasIndex(item => new { item.ReconciliationLeaseExpiresAt, item.Status });

        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.RequestedByCustomer).WithMany().HasForeignKey(item => item.RequestedByCustomerId);
        builder.HasOne(item => item.ApprovedByCustomer).WithMany().HasForeignKey(item => item.ApprovedByCustomerId);
        builder.HasOne(item => item.RejectedByCustomer).WithMany().HasForeignKey(item => item.RejectedByCustomerId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new { item.OrganizationId, item.LocalEntityType, item.LocalEntityId });
        builder.HasIndex(item => new { item.AccountingProvider, item.ExternalRefundId }).IsUnique();
        builder.HasIndex(item => new { item.PaymentProvider, item.ExternalPaymentRefundId }).IsUnique();
        builder.HasIndex(item => item.IdempotencyKey).IsUnique();
        builder.HasIndex(item => new { item.LocalEntityType, item.LocalEntityId, item.RefundKind })
            .IsUnique()
            .HasFilter(
                $"\"{nameof(MarketplaceRefund.RefundKind)}\" = 'Cancellation' AND \"{nameof(MarketplaceRefund.Status)}\" NOT IN ('Completed', 'Failed', 'Rejected', 'Cancelled')")
            .HasDatabaseName("IX_MarketplaceRefund_ActiveCancellation");
    }
}
