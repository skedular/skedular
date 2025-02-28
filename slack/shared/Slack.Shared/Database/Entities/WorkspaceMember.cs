using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Slack.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class WorkspaceMember : EntityBaseWithDeleted
{
    public string Email { get; set; }
    public string Designation { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Timezone { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; }
    public bool IsPrimaryOwner { get; set; }
    public string Locale { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }
    public DateTimeOffset? LastProfileStatusUpdatedAt { get; set; }
    public bool? AutomaticallyUpdateProfileStatus { get; set; }

    public virtual Workspace Workspace { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();
        builder.Property(item => item.Email).HasMaxLength(Api.Shared.Constants.MaxEmailLength);
        builder.Property(item => item.Designation).HasMaxLength(Api.Shared.Constants.MaxDesignationLength);
        builder.Property(item => item.Name).HasMaxLength(Api.Shared.Constants.MaxPersonNameLength);
        builder.Property(item => item.GivenName).HasMaxLength(Api.Shared.Constants.MaxGivenNameLength);
        builder.Property(item => item.FamilyName).HasMaxLength(Api.Shared.Constants.MaxFamilyNameLength);
        builder.Property(item => item.Timezone).HasMaxLength(Api.Shared.Constants.MaxTimezoneLength);
        builder.Property(item => item.Locale).HasMaxLength(Api.Shared.Constants.MaxLocaleLength);
        builder.Property(item => item.PhotoUrl).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl24).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl32).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl48).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl72).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl192).HasMaxLength(Api.Shared.Constants.MaxUrlLength);
        builder.Property(item => item.PhotoUrl512).HasMaxLength(Api.Shared.Constants.MaxUrlLength);

        builder.HasOne(item => item.Workspace).WithMany(item => item.WorkspaceMembers);

        builder.HasIndex(item => item.LastProfileStatusUpdatedAt);
        builder.HasIndex(item => item.AutomaticallyUpdateProfileStatus);
    }
}
