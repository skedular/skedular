using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Location : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public string Type { get; set; }
    public OpeningHours? OpeningHours { get; set; }
    public ICollection<CdnImageFile>? FeatureImages { get; set; }
    public LocationExtraMetadata? ExtraMetadata { get; set; }
    public string? UniqueClaimCode { get; set; }
    public bool ContactedViaEmail { get; set; }
    public bool ContactedViaSms { get; set; }
    public bool ContactedViaCall { get; set; }
    public bool ContactedViaWhatsapp { get; set; }
    public ListingMetadata? ListingMetadata { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<FloorPlan> FloorPlans { get; set; } = [];
    public virtual ICollection<DailyDeskCountRecording> DailyDeskCountRecordings { get; set; } = [];
    public virtual ICollection<DailyRoomCountRecording> DailyRoomCountRecordings { get; set; } = [];
    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public virtual ICollection<Booking> InvolvedBookings { get; set; } = [];
    public virtual LocationPhysicalAddress? PhysicalAddress { get; set; }
    public virtual ICollection<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
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
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxLocationTypeLength).HasDefaultValue(LocationTypeConstants.Private);
        builder.Property(item => item.OpeningHours).HasColumnType("jsonb");
        builder.Property(item => item.FeatureImages).HasColumnType("jsonb");
        builder.Property(item => item.ExtraMetadata).HasColumnType("jsonb");
        builder.Property(item => item.UniqueClaimCode).HasMaxLength(Enterprise.Shared.Constants.MaxUniqueIdLength);
        builder.Property(item => item.ContactedViaEmail).HasDefaultValue(false);
        builder.Property(item => item.ContactedViaSms).HasDefaultValue(false);
        builder.Property(item => item.ContactedViaCall).HasDefaultValue(false);
        builder.Property(item => item.ContactedViaWhatsapp).HasDefaultValue(false);
        builder.Property(item => item.ListingMetadata).HasColumnType("jsonb");

        builder.HasOne(item => item.Organization).WithMany(item => item.Locations).HasForeignKey(item => item.OrganizationId);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.Locations);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Timezone);
        builder.HasIndex(item => item.Type);
        builder.HasIndex(item => item.UniqueClaimCode).IsUnique();
        builder.HasIndex(item => item.ContactedViaEmail);
        builder.HasIndex(item => item.ContactedViaCall);
        builder.HasIndex(item => item.ContactedViaWhatsapp);
        builder.HasIndex(item => item.ContactedViaSms);
    }
}
