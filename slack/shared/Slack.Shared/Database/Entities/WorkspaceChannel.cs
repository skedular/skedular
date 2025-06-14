using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class WorkspaceChannel : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string Topic { get; set; }
    public string Purpose { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsGeneral { get; set; }
    public bool IsGroup { get; set; }
    public bool IsShared { get; set; }
    public bool IsMember { get; set; }

    public virtual Workspace Workspace { get; set; }
    public virtual ICollection<Organization> OrganizationDailyUpdateChannels { get; set; } = [];
    public virtual ICollection<Location> LocationDailyUpdateChannels { get; set; } = [];
    public virtual ICollection<Team> TeamDailyUpdateChannels { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class WorkspaceChannelConfiguration : IEntityTypeConfiguration<WorkspaceChannel>
{
    public void Configure(EntityTypeBuilder<WorkspaceChannel> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();
        builder.Property(item => item.Name).HasMaxLength(Constants.Constants.MaxSlackChannelNameLength);
        builder.Property(item => item.Topic).HasMaxLength(Constants.Constants.MaxSlackChannelTopicLength);
        builder.Property(item => item.Purpose).HasMaxLength(Constants.Constants.MaxSlackChannelPurposeLength);

        builder.HasOne(item => item.Workspace).WithMany(item => item.Channels);

        builder.HasIndex(item => item.Name);
    }
}
