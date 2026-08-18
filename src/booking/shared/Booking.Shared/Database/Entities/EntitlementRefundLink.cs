using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Constants = Api.Shared.Services.Constants;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EntitlementRefundLink : EntityBase
{
    public int UnusedCreditQuantity { get; set; }
    public decimal RefundAmount { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string EntitlementId { get; set; }
    public virtual Entitlement Entitlement { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceRefundId { get; set; }
    public virtual MarketplaceRefund MarketplaceRefund { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class EntitlementRefundLinkConfiguration : IEntityTypeConfiguration<EntitlementRefundLink>
{
    public void Configure(EntityTypeBuilder<EntitlementRefundLink> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.EntitlementId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.MarketplaceRefundId).HasMaxLength(Constants.MaxLocalEntityLength);
        builder.Property(item => item.RefundAmount).HasColumnType("DECIMAL(18,4)");

        builder.HasOne(item => item.Entitlement).WithMany(item => item.RefundLinks).HasForeignKey(item => item.EntitlementId);
        builder.HasOne(item => item.MarketplaceRefund).WithMany().HasForeignKey(item => item.MarketplaceRefundId);

        builder.HasIndex(item => item.EntitlementId).IsUnique();
        builder.HasIndex(item => item.MarketplaceRefundId).IsUnique();
    }
}
