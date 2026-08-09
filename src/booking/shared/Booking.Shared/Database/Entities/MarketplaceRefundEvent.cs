using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceRefundEvent : EntityBase
{
    public string EventType { get; set; }
    public string? PreviousStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? CorrelationId { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? Reason { get; set; }
    public string? AccountingProvider { get; set; }
    public string? ExternalRefundId { get; set; }
    public string? ExternalRefundNumber { get; set; }
    public string? LastError { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceRefundId { get; set; }
    public virtual MarketplaceRefund MarketplaceRefund { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ActorCustomerId { get; set; }
    public virtual Customer? ActorCustomer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceRefundEventConfiguration : IEntityTypeConfiguration<MarketplaceRefundEvent>
{
    public void Configure(EntityTypeBuilder<MarketplaceRefundEvent> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EventType).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.PreviousStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.NewStatus).HasMaxLength(Constants.MaxAccountingStatusLength);
        builder.Property(item => item.CorrelationId).HasMaxLength(Constants.MaxRefundCorrelationIdLength);
        builder.Property(item => item.RefundAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Reason).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.AccountingProvider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(item => item.ExternalRefundId).HasMaxLength(Constants.MaxExternalRefundIdLength);
        builder.Property(item => item.ExternalRefundNumber).HasMaxLength(Constants.MaxInvoiceNumberLength);
        builder.Property(item => item.LastError).HasMaxLength(Constants.MaxAccountingErrorLength);

        builder.HasOne(item => item.MarketplaceRefund).WithMany(item => item.Events).HasForeignKey(item => item.MarketplaceRefundId);
        builder.HasOne(item => item.ActorCustomer).WithMany().HasForeignKey(item => item.ActorCustomerId);

        builder.HasIndex(item => new
        {
            item.MarketplaceRefundId,
            item.OccurredAt,
            item.CreatedAt,
        });
    }
}
