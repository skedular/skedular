using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccountRefreshCode : EntityBaseWithDeleted
{
    public string Code { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationStripeConnectAccountId { get; set; }
    public virtual OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripeConnectAccountRefreshCodeConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccountRefreshCode>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccountRefreshCode> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Code).HasMaxLength(Constants.MaxStripeConnectAccountRefreshCodeLength);

        builder
            .HasOne(item => item.OrganizationStripeConnectAccount)
            .WithMany(item => item.OrganizationStripeConnectAccountRefreshCodes)
            .HasForeignKey(item => item.OrganizationStripeConnectAccountId);

        builder.HasIndex(item => item.Code);
    }
}
