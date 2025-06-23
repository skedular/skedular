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
    public string? ContentType { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public string? ThumbnailStorageUrl { get; set; }
    public string? ThumbnailCdnUrl { get; set; }
    public string? ThumbnailContentType { get; set; }
    public int? ThumbnailWidth { get; set; }
    public int? ThumbnailHeight { get; set; }

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
        builder.Property(item => item.ContentType).HasMaxLength(Constants.MaxContentTypeLength);

        builder.Property(item => item.ThumbnailStorageUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.ThumbnailCdnUrl).HasMaxLength(Constants.MaxUrlLength);
        builder.Property(item => item.ThumbnailContentType).HasMaxLength(Constants.MaxContentTypeLength);

        builder.HasOne(item => item.UploadedBy).WithMany(item => item.CdnFiles);

        builder.HasIndex(item => item.ContentType);
    }
}
