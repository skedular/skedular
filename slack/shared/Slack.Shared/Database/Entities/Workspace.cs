using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Workspace : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string? Domain { get; set; }
    public string? EmailDomain { get; set; }
    public string? EnterpriseId { get; set; }
    public string? EnterpriseName { get; set; }
    public string BotUserId { get; set; }
    public string BotUserScope { get; set; }
    public string BotUserAccessToken { get; set; }
    public string BotRefreshToken { get; set; }
    public string AuthedUserId { get; set; }
    public string AuthedUserScope { get; set; }
    public string AuthedUserAccessToken { get; set; }
    public string AuthedRefreshToken { get; set; }
    public DateTimeOffset? LastRefreshedAt { get; set; }
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
        builder.Property(item => item.Name).HasMaxLength(Constants.Constants.MaxSlackWorkspaceNameLength);
        builder.Property(item => item.Domain).HasMaxLength(Constants.Constants.MaxSlackWorkspaceDomainLength);
        builder.Property(item => item.EmailDomain).HasMaxLength(Constants.Constants.MaxSlackWorkspaceEmailDomainLength);
        builder.Property(item => item.EnterpriseId).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.EnterpriseName).HasMaxLength(Constants.Constants.MaxSlackWorkspaceEnterpriseNameLength);
        builder.Property(item => item.BotUserId).HasMaxLength(Constants.Constants.MaxSlackBotUserIdLength);
        builder.Property(item => item.BotUserScope).HasMaxLength(Constants.Constants.MaxSlackScopeLength);
        builder.Property(item => item.BotUserAccessToken).HasMaxLength(Constants.Constants.MaxSlackTokenLength);
        builder.Property(item => item.BotRefreshToken).HasMaxLength(Constants.Constants.MaxSlackTokenLength);
        builder.Property(item => item.AuthedUserId).HasMaxLength(Constants.Constants.MaxSlackAuthedUserIdLength);
        builder.Property(item => item.AuthedUserScope).HasMaxLength(Constants.Constants.MaxSlackScopeLength);
        builder.Property(item => item.AuthedUserAccessToken).HasMaxLength(Constants.Constants.MaxSlackTokenLength);
        builder.Property(item => item.AuthedRefreshToken).HasMaxLength(Constants.Constants.MaxSlackTokenLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.Workspaces);

        builder.HasIndex(item => item.AuthedUserScope);
        builder.HasIndex(item => item.BotUserScope);
        builder.HasIndex(item => item.MembersLastRefreshedAt);
        builder.HasIndex(item => item.ChannelsLastRefreshedAt);
    }
}
