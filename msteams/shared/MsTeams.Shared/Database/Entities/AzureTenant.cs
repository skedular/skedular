using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureTenant : ReplicatedEntityBaseWithDeleted
{
    public virtual Organization Organization { get; set; }
    public virtual ICollection<AzureTenantTeam> AzureTenantTeams { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TenantConfiguration : IEntityTypeConfiguration<AzureTenant>
{
    public void Configure(EntityTypeBuilder<AzureTenant> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.HasOne(item => item.Organization).WithMany(item => item.AzureTenants);
    }
}
