using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationStripeConnectAccount : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationStripeConnectAccountConfiguration : IEntityTypeConfiguration<OrganizationStripeConnectAccount>
{
    public void Configure(EntityTypeBuilder<OrganizationStripeConnectAccount> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxStripeConnectAccountNameLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.StripeConnectAccounts).HasForeignKey(item => item.OrganizationId);
    }
}
