using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Team.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Team : EntityBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
    public virtual ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public virtual Location? PrimaryLocation { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTeamNameLength);
        builder.Property(item => item.About).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.Timezone).HasMaxLength(Constants.MaxTimezoneLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.Teams);
        builder.HasOne(item => item.PrimaryLocation).WithMany(item => item.PrimaryLocationForTeams);
        
        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.About);
        builder.HasIndex(item => item.Timezone);
    }
}
