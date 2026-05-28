using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationMember : ReplicatedEntityBaseWithDeleted
{
    public string? Role { get; set; }
    public string Status { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; }
    public virtual Customer Customer { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Role).HasMaxLength(Constants.MaxRoleLength);
        builder
            .Property(item => item.Status)
            .HasMaxLength(Constants.MaxOrganizationMemberStatusLength)
            .HasDefaultValue(OrganizationMemberStatusConstants.Active);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationMembers).HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.Customer).WithMany(item => item.OrganizationMembers).HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.Role);
        builder.HasIndex(item => item.Status);
        builder.HasIndex(item => new { item.CustomerId, item.OrganizationId }).IsUnique();
    }
}
