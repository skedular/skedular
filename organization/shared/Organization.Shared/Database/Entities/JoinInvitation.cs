using Api.Shared;
using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class JoinInvitation : EntityBaseWithDeleted
{
    public string? Email { get; set; }
    public OldInvitationStatus Status { get; set; }
    public string NewStatus { get; set; }
    public OldOrganizationMembershipType MembershipType { get; set; }
    public string NewMembershipType { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual Customer CreatedBy { get; set; }
    public virtual Customer? Invitee { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class JoinInvitationConfiguration : IEntityTypeConfiguration<JoinInvitation>
{
    public void Configure(EntityTypeBuilder<JoinInvitation> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Email).HasMaxLength(Constants.MaxEmailLength);

        builder
            .Property(item => item.NewMembershipType)
            .HasMaxLength(Constants.MaxInvitationStatusLength)
            .HasComputedColumnSql(@"
                    CASE 
                        WHEN ""Status"" = 0 THEN 'PENDING'
                        WHEN ""Status"" = 1 THEN 'ACCEPTED'
                        WHEN ""Status"" = 2 THEN 'REJECTED'
                        WHEN ""Status"" = 3 THEN 'CANCELLED'
                        ELSE 'UNKNOWN'
                    END", true);

        builder
            .Property(item => item.NewMembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength)
            .HasComputedColumnSql(@"
                    CASE 
                        WHEN ""MembershipType"" = 0 THEN 'OWNER'
                        WHEN ""MembershipType"" = 1 THEN 'ADMINISTRATOR'
                        WHEN ""MembershipType"" = 2 THEN 'MEMBER'
                        ELSE 'UNKNOWN'
                    END", true);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.JoinInvitations);

        builder
            .HasOne(item => item.CreatedBy)
            .WithMany(item => item.JoinInvitationsCreatedBy);

        builder
            .HasOne(item => item.Invitee)
            .WithMany(item => item.JoinInvitationsInvitee);

        builder.HasIndex(item => item.Email);
        builder.HasIndex(item => item.Status);
    }
}
