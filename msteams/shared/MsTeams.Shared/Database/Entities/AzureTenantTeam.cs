using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureTenantTeam : EntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? WebUrl { get; set; }

    public virtual AzureTenant AzureTenant { get; set; }
    public virtual ICollection<AzureTenantTeamChannel> AzureTenantTeamChannels { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AzureTenantTeamConfiguration : IEntityTypeConfiguration<AzureTenantTeam>
{
    public void Configure(EntityTypeBuilder<AzureTenantTeam> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxAzureTeamNameLength);
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.WebUrl).HasMaxLength(Constants.MaxUrlLength);

        builder.HasOne(item => item.AzureTenant).WithMany(item => item.AzureTenantTeams);

        builder.HasIndex(item => item.Name);
    }
}
