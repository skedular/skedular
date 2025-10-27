using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class PrecomputedLocationProduct : EntityBase
{
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string OrganizationId { get; set; }
    public virtual Organization Organization { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string LocationId { get; set; }
    public virtual Location Location { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string ProductId { get; set; }
    public virtual Product Product { get; set; }

    public virtual ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class PrecomputedLocationProductConfiguration : IEntityTypeConfiguration<PrecomputedLocationProduct>
{
    public void Configure(EntityTypeBuilder<PrecomputedLocationProduct> builder)
    {
        builder.ConfigureEntityBase();

        builder.HasOne(item => item.Organization).WithMany(item => item.PrecomputedLocationProducts).HasForeignKey(item => item.OrganizationId);
        builder.HasOne(item => item.Location).WithMany(item => item.PrecomputedLocationProducts).HasForeignKey(item => item.LocationId);
        builder.HasOne(item => item.Product).WithMany(item => item.PrecomputedLocationProducts).HasForeignKey(item => item.ProductId);
        builder.HasMany(item => item.OrganizationTags).WithMany(item => item.PrecomputedLocationProducts);
    }
}
