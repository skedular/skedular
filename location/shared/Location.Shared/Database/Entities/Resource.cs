using Api.Shared;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Resource : EntityBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public bool Inactive { get; set; }
    public bool RequireBookingApproval { get; set; }
    public string? Color { get; set; }
    public bool? IsOpeningHoursOverriden { get; set; }
    public OpeningHours? OpeningHours { get; set; }

    public virtual Location Location { get; set; }
    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxResourceNameLength);
        builder.Property(item => item.Color).HasMaxLength(Constants.MaxColorValueLength);
        builder.Property(item => item.IsOpeningHoursOverriden).HasDefaultValue(false);
        builder.Property(item => item.OpeningHours).HasColumnType("jsonb");

        builder.HasOne(item => item.Location).WithMany(item => item.Resources);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.Resources);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Inactive);
        builder.HasIndex(item => item.RequireBookingApproval);
        builder.HasIndex(item => item.IsOpeningHoursOverriden);
    }
}
