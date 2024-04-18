using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : EntityBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public DateTimeOffset? DailyDeskCountLastRecordedAt { get; set; }

    public virtual Organization? Organization { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = [];
    public virtual ICollection<Desk> Desks { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<LocationMember> LocationMembers { get; set; } = [];
    public virtual ICollection<JoinInvitation> JoinInvitations { get; set; } = [];
    public virtual ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxLocationNameLength);
        builder.Property(item => item.About).HasMaxLength(Constants.MaxDescriptionLength);
        builder.Property(item => item.Timezone).HasMaxLength(Constants.MaxTimezoneLength);

        builder
            .HasOne(item => item.Organization)
            .WithMany(item => item.Locations);
    }
}
