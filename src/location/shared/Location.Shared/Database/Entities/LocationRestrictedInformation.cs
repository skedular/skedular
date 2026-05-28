using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class LocationRestrictedInformation : EntityBase
{
    public string Title { get; set; }
    public string Category { get; set; }
    public string Content { get; set; }
    public bool Active { get; set; }
    public int SortOrder { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string LocationId { get; set; }
    public virtual Location Location { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class LocationRestrictedInformationConfiguration : IEntityTypeConfiguration<LocationRestrictedInformation>
{
    public void Configure(EntityTypeBuilder<LocationRestrictedInformation> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.Title).HasMaxLength(Constants.MaxLocationRestrictedInformationTitleLength);
        builder.Property(item => item.Category).HasMaxLength(Constants.MaxLocationRestrictedInformationCategoryLength);
        builder.Property(item => item.Content).HasMaxLength(Constants.MaxLocationRestrictedInformationContentLength);
        builder.Property(item => item.Active).HasDefaultValue(true);

        builder.HasOne(item => item.Location).WithMany(item => item.RestrictedInformation).HasForeignKey(item => item.LocationId);

        builder.HasIndex(item => item.Category);
        builder.HasIndex(item => item.Active);
        builder.HasIndex(item => item.SortOrder);
        builder.HasIndex(item => item.LocationId);
    }
}
