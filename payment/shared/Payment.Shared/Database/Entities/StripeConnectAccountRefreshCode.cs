using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class StripeConnectAccountRefreshCode : EntityBaseWithDeleted
{
    public string Code { get; set; }
    public string RedirectUrl { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string StripeConnectAccountId { get; set; }
    public virtual StripeConnectAccount StripeConnectAccount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeConnectAccountRefreshCodeConfiguration : IEntityTypeConfiguration<StripeConnectAccountRefreshCode>
{
    public void Configure(EntityTypeBuilder<StripeConnectAccountRefreshCode> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Code).HasMaxLength(Constants.MaxStripeConnectAccountRefreshCodeLength);
        builder.Property(item => item.RedirectUrl).HasMaxLength(Constants.MaxUrlLength);

        builder
            .HasOne(item => item.StripeConnectAccount)
            .WithMany(item => item.StripeConnectAccountRefreshCodes)
            .HasForeignKey(item => item.StripeConnectAccountId);

        builder.HasIndex(item => item.Code);
    }
}
