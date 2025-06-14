using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class CdnFile : EntityBase
{
    public string StorageUrl { get; set; }
    public string CdnUrl { get; set; }

    public virtual Customer UploadedBy { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class CdnFileConfiguration : IEntityTypeConfiguration<CdnFile>
{
    public void Configure(EntityTypeBuilder<CdnFile> builder)
    {
        builder.ConfigureEntityBase();

        builder.Property(item => item.StorageUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.CdnUrl).HasMaxLength(Constants.MaxUrlLength);

        builder.HasOne(item => item.UploadedBy).WithMany(item => item.CdnFiles);
    }
}
