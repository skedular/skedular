using Api.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CdnFile : ReplicatedEntityBase
{
    public string Url { get; set; }

    public virtual ICollection<Product> Products { get; set; } = [];
    public virtual ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CdnFileConfiguration : IEntityTypeConfiguration<CdnFile>
{
    public void Configure(EntityTypeBuilder<CdnFile> builder)
    {
        builder.ConfigureReplicatedEntityBase();

        builder.Property(item => item.Url).HasMaxLength(Constants.MaxUrlLength);
    }
}
