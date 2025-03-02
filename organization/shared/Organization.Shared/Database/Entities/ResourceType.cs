using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class ResourceType : EntityBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public string? SystemType { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class ResourceTypeConfiguration : IEntityTypeConfiguration<ResourceType>
{
    public void Configure(EntityTypeBuilder<ResourceType> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxResourceTypeNameLength);
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxResourceTypeDescriptionLength);
        builder.Property(item => item.Color).HasMaxLength(Constants.MaxColorValueLength);
        builder.Property(item => item.SystemType).HasMaxLength(Constants.MaxResourceTypeSystemTypeLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.ResourceTypes);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Description);
        builder.HasIndex(item => item.SystemType);
    }
}
