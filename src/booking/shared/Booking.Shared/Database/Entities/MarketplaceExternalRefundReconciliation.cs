using Api.Shared.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

public class MarketplaceExternalRefundReconciliation : EntityBase
{
    public string? OrganizationId { get; set; }
    public string? StripeAccountId { get; set; }
    public string Provider { get; set; } = null!;
    public string ExternalRefundId { get; set; } = null!;
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? NextRetryAt { get; set; }
    public string? ResolutionReason { get; set; }
    public string? ResolutionActorCustomerId { get; set; }
    public string? ResolutionCorrelationId { get; set; }
}

public class MarketplaceExternalRefundReconciliationConfiguration
    : IEntityTypeConfiguration<MarketplaceExternalRefundReconciliation>
{
    public void Configure(EntityTypeBuilder<MarketplaceExternalRefundReconciliation> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.OrganizationId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxRefundStripeAccountIdLength);
        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxRefundProviderLength).IsRequired();
        builder.Property(item => item.ExternalRefundId).HasMaxLength(Constants.MaxRefundExternalIdLength).IsRequired();
        builder.Property(item => item.Amount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.Status)
            .HasMaxLength(Constants.MaxAccountingStatusLength)
            .HasDefaultValue(MarketplaceExternalRefundReconciliationStatusConstants.Open).IsRequired();
        builder.Property(item => item.ResolutionReason).HasMaxLength(Constants.MaxRefundResolutionReasonLength);
        builder.Property(item => item.ResolutionActorCustomerId).HasMaxLength(Constants.MaxAccountingExternalIdLength);
        builder.Property(item => item.ResolutionCorrelationId).HasMaxLength(Constants.MaxRefundCorrelationIdLength);

        builder.HasIndex(item => new { item.Status, item.NextRetryAt, item.RetryCount });
        builder.HasIndex(item => new { item.Provider, item.ExternalRefundId }).IsUnique();
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new { item.OrganizationId, item.Status });
    }
}
