using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Customer.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationMember : ReplicatedEntityBaseWithDeleted
{
    public LocationMembershipType? MembershipType { get; set; }

    public virtual Location Location { get; set; }
    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationMemberConfiguration : IEntityTypeConfiguration<LocationMember>
{
    public void Configure(EntityTypeBuilder<LocationMember> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.LocationMembers);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.LocationMemberships);

        builder.HasIndex(item => item.MembershipType);
    }
}
