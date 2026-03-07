using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class Tag : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Type { get; set; }
    public string? Color { get; set; }

    public virtual Organization Organization { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxTagNameLength);
        builder.Property(item => item.Description).HasMaxLength(Constants.MaxTagDescriptionLength);
        builder.Property(item => item.Type).HasMaxLength(Constants.MaxTagTypeLength);
        builder.Property(item => item.Color).HasMaxLength(Constants.MaxColorValueLength);

        builder.HasOne(item => item.Organization).WithMany(item => item.Tags);

        builder.HasIndex(item => item.Name);
        builder.HasIndex(item => item.Description);
        builder.HasIndex(item => item.Type);
    }
}
