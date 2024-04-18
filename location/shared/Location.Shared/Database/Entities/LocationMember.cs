using Api.Shared.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationMember : EntityBaseWithDeleted
{
    public LocationMembershipType MembershipType { get; set; } = LocationMembershipType.Member;
    public virtual Location Location { get; set; }
    public virtual Customer Customer { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationMemberConfiguration : IEntityTypeConfiguration<LocationMember>
{
    public void Configure(EntityTypeBuilder<LocationMember> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder
            .HasOne(item => item.Location)
            .WithMany(item => item.LocationMembers);

        builder
            .HasOne(item => item.Customer)
            .WithMany(item => item.LocationMembers);

        builder.HasIndex(item => item.MembershipType);
    }
}
