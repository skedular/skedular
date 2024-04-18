using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Team : ReplicatedEntityBaseWithDeleted
{
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public string? Timezone { get; set; }

    public virtual WorkspaceChannel? DailyUpdateChannel { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Timezone).HasMaxLength(Api.Shared.Constants.MaxTimezoneLength);

        builder
            .HasOne(item => item.DailyUpdateChannel)
            .WithMany(item => item.TeamDailyUpdateChannels);

        builder.HasIndex(item => item.SlackChannelDailyUpdateLastSentAt);
    }
}
