using Api.Shared;
using Api.Shared.Services.Models;
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
    public DateTimeOffset? DailyRoomCountLastRecordedAt { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? PhysicalAddressId { get; set; }
    public virtual Address? PhysicalAddress { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<Booking> Bookings { get; set; } = [];
    public virtual ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
    public virtual ICollection<DailyRoomCountRecording> DailyRoomCountRecordings { get; set; } = [];
    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public virtual ICollection<Booking> InvolvedBookings { get; set; } = [];
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
        builder.Property(item => item.OpeningHours).HasColumnType("jsonb");
        builder.Property(item => item.ContactEmail).HasMaxLength(Constants.MaxEmailLength);
        builder.Property(item => item.ContactPhone).HasMaxLength(Constants.MaxPhoneNumberLength);

        builder.HasOne(item => item.PhysicalAddress).WithOne(item => item.Location).HasForeignKey<Location>(item => item.PhysicalAddressId);
        builder.HasOne(item => item.Organization).WithMany(item => item.Locations);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.Locations);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.About);
        builder.HasIndex(item => item.Timezone);
        builder.HasIndex(item => item.DailyDeskCountLastRecordedAt);
        builder.HasIndex(item => item.DailyRoomCountLastRecordedAt);
    }
}
