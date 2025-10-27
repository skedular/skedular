using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Location.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationTag : ReplicatedEntityBaseWithDeleted
{
    public string? Type { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<Resource> Resources { get; set; } = [];
    public virtual ICollection<Location> Locations { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersionProductTag { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersionLocationTags { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationTagConfiguration : IEntityTypeConfiguration<OrganizationTag>
{
    public void Configure(EntityTypeBuilder<OrganizationTag> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Type).HasMaxLength(Constants.MaxTagTypeLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.Tags);

        builder.HasIndex(item => item.Type);
    }
}
