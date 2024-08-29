using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class AzureTenantTeamChannel : EntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? WebUrl { get; set; }
    public string? Email { get; set; }

    public virtual AzureTenantTeam AzureTenantTeam { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class AzureTenantTeamChannelConfiguration : IEntityTypeConfiguration<AzureTenantTeamChannel>
{
    public void Configure(EntityTypeBuilder<AzureTenantTeamChannel> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();
        builder.Property(item => item.Name).HasMaxLength(Constants.MaxAzureTeamChannelNameLength);
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.WebUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);

        builder
            .HasOne(item => item.AzureTenantTeam)
            .WithMany(item => item.AzureTenantTeamChannels);

        builder.HasIndex(item => item.Name);
    }
}
