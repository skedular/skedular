using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccount : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public string Type { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; } = string.Empty;
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripeConnectAccountConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccount>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccount> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxStripeConnectAccountNameLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxStripeConnectAccountTypeLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationStripeConnectAccounts).HasForeignKey(item => item.OrganizationId);

        builder.HasIndex(item => item.ChargesEnabled);
        builder.HasIndex(item => item.PayoutsEnabled);
        builder.HasIndex(item => item.Type);
    }
}
