using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TeamMember : ReplicatedEntityBaseWithDeleted
{
    public TeamMembershipType? MembershipType { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string TeamId { get; set; }
    public virtual Team Team { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string CustomerId { get; set; } = string.Empty;
    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Team)
            .WithMany(item => item.TeamMembers)
            .HasForeignKey(item => item.TeamId);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.TeamMembers)
            .HasForeignKey(item => item.CustomerId);

        builder.HasIndex(item => item.MembershipType);
        builder.HasIndex(item => new { item.CustomerId, item.TeamId }).IsUnique();
    }
}
