using Api.Shared.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceExternalRefundReconciliation : EntityBase
{
    public string? StripeAccountId { get; set; }
    public string Provider { get; set; }
    public string ExternalRefundId { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string Status { get; set; }
    public DateTimeOffset FirstSeenAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset? NextRetryAt { get; set; }
    public string? ResolutionReason { get; set; }
    public string? ResolutionCorrelationId { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? OrganizationId { get; set; }
    public virtual Organization? Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? ResolutionActorCustomerId { get; set; }
    public virtual Customer? ResolutionActorCustomer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceExternalRefundReconciliationConfiguration : IEntityTypeConfiguration<MarketplaceExternalRefundReconciliation>
{
    public void Configure(EntityTypeBuilder<MarketplaceExternalRefundReconciliation> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.StripeAccountId).HasMaxLength(Constants.MaxRefundStripeAccountIdLength);
        builder.Property(item => item.Provider).HasMaxLength(Constants.MaxRefundProviderLength);
        builder.Property(item => item.ExternalRefundId).HasMaxLength(Constants.MaxRefundExternalIdLength);
        builder.Property(item => item.Amount).HasColumnType("DECIMAL(18,4)");
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.Status)
            .HasMaxLength(Constants.MaxAccountingStatusLength)
            .HasDefaultValue(MarketplaceExternalRefundReconciliationStatusConstants.Open);
        builder.Property(item => item.ResolutionReason).HasMaxLength(Constants.MaxRefundResolutionReasonLength);
        builder.Property(item => item.ResolutionCorrelationId).HasMaxLength(Constants.MaxRefundCorrelationIdLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.ExternalRefundReconciliations).HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.ResolutionActorCustomer).WithMany().HasForeignKey(item => item.ResolutionActorCustomerId);

        builder.HasIndex(item => new
        {
            item.Status,
            item.NextRetryAt,
            item.RetryCount,
        });
        builder.HasIndex(item => new
        {
            item.Provider,
            item.ExternalRefundId,
        }).IsUnique();
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new
        {
            item.OrganizationId,
            item.Status,
        });
    }
}
