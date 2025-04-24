using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TeamMember : EntityBaseWithDeleted
{
    public string Role { get; set; }
    public string Status { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string TeamId { get; set; }
    public virtual Team Team { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public virtual OrganizationMember? OrganizationMember { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Role).HasMaxLength(Constants.MaxRoleLength);
        builder
            .Property(item => item.Status)
            .HasMaxLength(Constants.MaxTeamMemberStatusLength)
            .HasDefaultValue(TeamMemberStatusConstants.Active);

        builder.HasOne(item => item.Team).WithMany(item => item.TeamMembers).HasForeignKey(item => item.TeamId);
        builder.HasOne(item => item.Customer).WithMany(item => item.TeamMembers).HasForeignKey(item => item.CustomerId);
        builder.HasOne(item => item.OrganizationMember).WithMany(item => item.TeamMembers);

        builder.HasIndex(item => item.Role);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new { item.CustomerId, item.TeamId }).IsUnique();
    }
}
