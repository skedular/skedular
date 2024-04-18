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

    public virtual Team? Team { get; set; }
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
            .WithMany(item => item.TeamMembers);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.TeamMembers);

        builder.HasIndex(item => item.MembershipType);
    }
}
