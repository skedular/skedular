using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationMember : EntityBaseWithDeleted
{
    public OrganizationMembershipType MembershipType { get; set; } = OrganizationMembershipType.Member;

    public virtual Organization Organization { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationMembers);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.OrganizationMembers);

        builder.HasIndex(item => item.MembershipType);
    }
}
