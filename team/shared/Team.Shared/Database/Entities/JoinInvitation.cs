using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class JoinInvitation : EntityBaseWithDeleted
{
    public string? Email { get; set; }
    public string Status { get; set; }
    public string MembershipType { get; set; }

    public virtual Team Team { get; set; }
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
            .Property(item => item.Status)
            .HasMaxLength(Constants.MaxInvitationStatusLength);

        builder
            .Property(item => item.MembershipType)
            .HasMaxLength(Constants.MaxMembershipTypeLength);

        builder
            .HasOne(item => item.Team)
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
