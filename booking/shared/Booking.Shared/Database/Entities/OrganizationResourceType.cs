using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class OrganizationResourceType : ReplicatedEntityBaseWithDeleted
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Color { get; set; }

    public virtual Organization Organization { get; set; }
    public virtual ICollection<LocationResource> LocationResources { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class OrganizationResourceTypeConfiguration : IEntityTypeConfiguration<OrganizationResourceType>
{
    public void Configure(EntityTypeBuilder<OrganizationResourceType> builder)
    {
        builder.ConfigureReplicatedEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTagNameLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxTagTypeLength);
        builder.Property(item => item.Color).HasMaxLength(Constants.MaxColorValueLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.OrganizationResourceTypes);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Type);
    }
}
