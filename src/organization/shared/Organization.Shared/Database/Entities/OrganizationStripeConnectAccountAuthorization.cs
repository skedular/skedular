using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccountAuthorization : EntityBase
{
    public bool IsAuthorized { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationStripeConnectAccountId { get; set; }
    public virtual OrganizationStripeConnectAccount OrganizationStripeConnectAccount { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class StripeConnectAccountAuthorizationConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccountAuthorization>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccountAuthorization> builder)
    {
        builder.ConfigureEntityBase();

        builder
            .HasOne(item => item.OrganizationStripeConnectAccount)
            .WithOne(item => item.OrganizationStripeConnectAccountAuthorization)
            .HasForeignKey<OrganizationStripeConnectAccountAuthorization>(item => item.OrganizationStripeConnectAccountId);

        builder.HasIndex(item => item.IsAuthorized);
    }
}
