using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MsTeams.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Organization : ReplicatedEntityBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string Type { get; set; }
    public string MemberVisibilityPolicy { get; set; }

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public virtual ICollection<AzureTenant> AzureTenants { get; set; } = [];
    public virtual OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.UniqueAlphanumericName).HasMaxLength(Constants.MaxOrganizationUniqueAlphanumericNameLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxOrganizationTypeLength).HasDefaultValue(OrganizationTypeConstants.Private);
        builder
            .Property(item => item.MemberVisibilityPolicy)
            .HasMaxLength(Constants.MaxOrganizationMemberVisibilityPolicyLength)
            .HasDefaultValue(OrganizationMemberVisibilityPolicyConstants.FullAccess);

        builder.HasIndex(item => item.UniqueAlphanumericName).IsUnique();
    }
}
