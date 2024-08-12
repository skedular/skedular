using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Tenant : EntityBaseWithDeleted
{
    public string? Name { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public string InstalledByUserId { get; set; } = string.Empty;

    public virtual ICollection<TenantMember> TenantMembers { get; set; } = [];
    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTenantNameLength);
        builder.Property(item => item.InstalledByUserId).HasMaxLength(Constants.MaxVerifiableTokenLength);
        
        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Tenants);

    }
}
