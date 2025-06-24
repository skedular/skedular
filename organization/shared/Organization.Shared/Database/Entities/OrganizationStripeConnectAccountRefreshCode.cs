using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccountRefreshCode : EntityBaseWithDeleted
{
    public string Code { get; set; }
    public string RedirectUrl { get; set; }

    public virtual OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeConnectAccountRefreshCodeConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccountRefreshCode>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccountRefreshCode> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Code).HasMaxLength(Constants.MaxStripeConnectAccountRefreshCodeLength);
        builder.Property(item => item.RedirectUrl).HasMaxLength(Constants.MaxUrlLength);

        builder.HasOne(item => item.OrganizationStripeConnectAccount).WithMany(item => item.OrganizationStripeConnectAccountRefreshCodes);

        builder.HasIndex(item => item.Code);
    }
}
