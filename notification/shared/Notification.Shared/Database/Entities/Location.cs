using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Notification.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxLocationNameLength);
        
        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Locations);
    }
}
