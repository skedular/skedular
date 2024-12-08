using Api.Shared;
using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationMember : EntityBaseWithDeleted
{
    public OldOrganizationMembershipType MembershipType { get; set; } = OldOrganizationMembershipType.Member;
    public string NewMembershipType { get; set; }
    public bool? IsOrganizationOnboardingDone { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; } = string.Empty;
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; } = string.Empty;
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
            .Property(item => item.NewMembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength)
            .HasComputedColumnSql(@"
                    CASE 
                        WHEN ""MembershipType"" = 0 THEN 'OWNER'
                        WHEN ""MembershipType"" = 1 THEN 'ADMINISTRATOR'
                        WHEN ""MembershipType"" = 2 THEN 'MEMBER'
                        ELSE 'UNKNOWN'
                    END", stored: true);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.OrganizationMembers)
            .HasForeignKey(item => item.OrganizationId);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.OrganizationMembers)
            .HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.MembershipType);
        builder.HasIndex(item => new { item.CustomerId, item.OrganizationId }).IsUnique();
    }
}
