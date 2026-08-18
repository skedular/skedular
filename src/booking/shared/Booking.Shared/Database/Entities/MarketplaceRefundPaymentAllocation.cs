using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class MarketplaceRefundPaymentAllocation : EntityBase
{
    public string SourcePaymentProvider { get; set; }
    public string SourcePaymentReference { get; set; }
    public decimal SourceCapturedAmount { get; set; }
    public decimal AllocatedRefundAmount { get; set; }
    public bool IsSourcePayment { get; set; }
    public string Currency { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string MarketplaceRefundId { get; set; }
    public virtual MarketplaceRefund MarketplaceRefund { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class MarketplaceRefundPaymentAllocationConfiguration : IEntityTypeConfiguration<MarketplaceRefundPaymentAllocation>
{
    public void Configure(EntityTypeBuilder<MarketplaceRefundPaymentAllocation> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(x => x.SourcePaymentProvider).HasMaxLength(Constants.MaxAccountingProviderLength);
        builder.Property(x => x.SourcePaymentReference).HasMaxLength(Constants.MaxSourcePaymentReferenceLength);
        builder.Property(x => x.Currency).HasMaxLength(Constants.MaxCurrencyLength);
        builder.Property(x => x.SourceCapturedAmount).HasColumnType("DECIMAL(18,4)");
        builder.Property(x => x.AllocatedRefundAmount).HasColumnType("DECIMAL(18,4)");

        builder.HasOne(x => x.MarketplaceRefund).WithMany(x => x.PaymentAllocations).HasForeignKey(x => x.MarketplaceRefundId);

        builder.HasIndex(x => new
        {
            x.MarketplaceRefundId,
            x.SourcePaymentProvider,
            x.SourcePaymentReference,
            x.IsSourcePayment,
        }).IsUnique();
        builder.HasIndex(x => new
            {
                x.SourcePaymentProvider,
                x.SourcePaymentReference,
            })
            .IsUnique()
            .HasFilter("\"IsSourcePayment\" = TRUE");
    }
}
