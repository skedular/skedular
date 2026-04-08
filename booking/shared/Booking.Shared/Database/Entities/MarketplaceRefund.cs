using Api.Shared.Services;
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

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? RequestedByCustomerId { get; set; }
    public virtual Customer? RequestedByCustomer { get; set; }

    public virtual ICollection<MarketplaceRefundEvent> Events { get; set; } = [];
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

        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.RequestedByCustomer).WithMany().HasForeignKey(item => item.RequestedByCustomerId);

        builder.HasIndex(item => item.OrganizationId);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new { item.OrganizationId, item.LocalEntityType, item.LocalEntityId }).IsUnique();
        builder.HasIndex(item => new { item.AccountingProvider, item.ExternalRefundId }).IsUnique();
    }
}
