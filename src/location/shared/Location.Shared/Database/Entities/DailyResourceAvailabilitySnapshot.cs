using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class DailyResourceAvailabilitySnapshot : EntityBaseWithDeleted
{
    public DateTimeOffset Date { get; set; }
    public string Classification { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string LocationId { get; set; }
    public virtual Location Location { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ResourceId { get; set; }
    public virtual Resource Resource { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class DailyResourceAvailabilitySnapshotConfiguration : IEntityTypeConfiguration<DailyResourceAvailabilitySnapshot>
{
    public void Configure(EntityTypeBuilder<DailyResourceAvailabilitySnapshot> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Classification).HasMaxLength(Constants.MaxResourceAvailabilityClassificationLength);

        builder.HasOne(item => item.Location).WithMany(item => item.DailyResourceAvailabilitySnapshots).HasForeignKey(item => item.LocationId);
        builder.HasOne(item => item.Resource).WithMany().HasForeignKey(item => item.ResourceId);

        builder.HasIndex(item => item.Date);
        builder.HasIndex(item => item.Classification);
        builder.HasIndex(item => new
        {
            item.LocationId,
            item.Date,
        });
    }
}
