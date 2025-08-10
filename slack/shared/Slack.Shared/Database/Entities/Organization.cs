using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public string Type { get; set; }
    public string MemberVisibilityPolicy { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<Workspace> Workspaces { get; set; } = [];
    public virtual WorkspaceChannel? DailyUpdateChannel { get; set; }
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.UniqueAlphanumericName).HasMaxLength(Api.Shared.Services.Constants.MaxOrganizationUniqueAlphanumericNameLength);
        builder
            .Property(item => item.Type)
            .HasMaxLength(Api.Shared.Services.Constants.MaxOrganizationTypeLength)
            .HasDefaultValue(OrganizationTypeConstants.Private);
        builder
            .Property(item => item.MemberVisibilityPolicy)
            .HasMaxLength(Api.Shared.Services.Constants.MaxOrganizationMemberVisibilityPolicyLength)
            .HasDefaultValue(OrganizationMemberVisibilityPolicyConstants.FullAccess);

        builder.HasOne(item => item.DailyUpdateChannel).WithMany(item => item.OrganizationDailyUpdateChannels);

        builder.HasIndex(item => item.UniqueAlphanumericName).IsUnique();
        builder.HasIndex(item => item.SlackChannelDailyUpdateLastSentAt);
    }
}
