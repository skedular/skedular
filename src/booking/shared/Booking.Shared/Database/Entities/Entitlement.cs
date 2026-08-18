using Booking.Shared.Models.Entitlements;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Constants = Api.Shared.Services.Constants;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Entitlement : EntityBase
{
    public string PurchaseReference { get; set; }
    public string PricingId { get; set; }
    public int GrantedQuantity { get; set; }
    public DateTimeOffset ActivatesAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public EntitlementStatus Status { get; set; }
    public bool AutoRenew { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public DateTimeOffset? NextRenewalAt { get; set; }
    public string? RenewalFailureReason { get; set; }
    public decimal NetPurchaseAmount { get; set; }
    public string Currency { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public virtual EntitlementPurchase? EntitlementPurchase { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    public virtual ICollection<MarketplaceBooking> MarketplaceBookings { get; set; } = [];
    public virtual ICollection<CreditLedgerEntry> LedgerEntries { get; set; } = [];
    public virtual ICollection<EntitlementRefundLink> RefundLinks { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class EntitlementConfiguration : IEntityTypeConfiguration<Entitlement>
{
    public void Configure(EntityTypeBuilder<Entitlement> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.CustomerId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.OrganizationId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.PurchaseReference).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.PricingId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(item => item.RenewalFailureReason).HasMaxLength(Constants.MaxAccountingErrorLength);
        builder.Property(item => item.NetPurchaseAmount).HasColumnType("DECIMAL(18,4)");

        builder.HasOne(item => item.Customer).WithMany().HasForeignKey(item => item.CustomerId);
        builder.HasOne(item => item.Organization).WithMany().HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => new
        {
            item.CustomerId,
            item.Status,
        });
        builder.HasIndex(item => item.ExpiresAt);
        builder.HasIndex(item => item.NextRenewalAt);
        builder.HasIndex(item => item.PurchaseReference).IsUnique();
    }
}
