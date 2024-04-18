using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Team : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTeamNameLength);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Teams);

        builder.HasIndex(item => item.Name);
    }
}
