using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Workspace : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string BotUserId { get; set; }
    public string BotUserScope { get; set; }
    public string BotUserAccessToken { get; set; }
    public string BotRefreshToken { get; set; }
    public string AuthedUserId { get; set; }
    public string AuthedUserScope { get; set; }
    public string AuthedUserAccessToken { get; set; }
    public string AuthedRefreshToken { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public DateTimeOffset? ChannelsLastRefreshedAt { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<WorkspaceChannel> Channels { get; set; } = [];
    public virtual ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();
        builder.Property(item => item.Name).HasMaxLength(Api.Shared.Constants.MaxSlackWorkspaceNameLength);
        builder.Property(item => item.BotUserId).HasMaxLength(1000);
        builder.Property(item => item.BotUserScope).HasMaxLength(Api.Shared.Constants.MaxSlackScopeLength);
        builder.Property(item => item.BotUserAccessToken).HasMaxLength(Api.Shared.Constants.MaxTokenLength);
        builder.Property(item => item.BotRefreshToken).HasMaxLength(Api.Shared.Constants.MaxTokenLength);
        builder.Property(item => item.AuthedUserId).HasMaxLength(1000);
        builder.Property(item => item.AuthedUserScope).HasMaxLength(Api.Shared.Constants.MaxSlackScopeLength);
        builder.Property(item => item.AuthedUserAccessToken).HasMaxLength(Api.Shared.Constants.MaxTokenLength);
        builder.Property(item => item.AuthedRefreshToken).HasMaxLength(Api.Shared.Constants.MaxTokenLength);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Workspaces);

        builder.HasIndex(item => item.AuthedUserScope);
        builder.HasIndex(item => item.BotUserScope);
        builder.HasIndex(item => item.MembersLastRefreshedAt);
        builder.HasIndex(item => item.ChannelsLastRefreshedAt);
    }
}
