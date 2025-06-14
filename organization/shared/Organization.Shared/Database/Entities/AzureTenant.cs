using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureTenant : EntityBaseWithDeleted
{
    public string? Name { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public string InstalledByUserId { get; set; } = string.Empty;

    public virtual ICollection<AzureTenantMember> AzureTenantMembers { get; set; } = [];
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TenantConfiguration : IEntityTypeConfiguration<AzureTenant>
{
    public void Configure(EntityTypeBuilder<AzureTenant> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTenantNameLength);
        builder.Property(item => item.InstalledByUserId).HasMaxLength(Constants.MaxVerifiableTokenLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.AzureTenants);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.MembersLastRefreshedAt);
        builder.HasIndex(item => item.InstalledByUserId);
    }
}
