using Api.Shared;
using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationMember : ReplicatedEntityBaseWithDeleted
{
    public string? NewMembershipType { get; set; }
    public string? MembershipType { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; } = string.Empty;
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; } = string.Empty;
    public virtual Customer Customer { get; set; }

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .Property(item => item.NewMembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength);

        builder
            .Property(item => item.MembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationMembers)
            .HasForeignKey(item => item.OrganizationId);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.OrganizationMembers)
            .HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.NewMembershipType);
        builder.HasIndex(item => new { item.CustomerId, item.OrganizationId }).IsUnique();
    }
}
