using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Core.Shared.Models;
using CdnFile = Core.Shared.Models.CdnFile;
using Customer = Core.Shared.Models.Customer;
using File = Api.Shared.Services.OpenApi.Skedular.Core.V1.File;

namespace Core.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    CdnFile MapTo(Shared.Database.Entities.CdnFile src);
    FileUploadResponse MapTo(CdnFile src);
    PrivateFile MapTo(Shared.Database.Entities.PrivateFile src);
    FileUploadResponse MapTo(PrivateFile src);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Shared.Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Timezone = src.Timezone
            };

    public CdnFile MapTo(Shared.Database.Entities.CdnFile src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            StorageUrl = new Uri(src.StorageUrl),
            CdnUrl = new Uri(src.CdnUrl),
            ContentType = src.ContentType,
            Width = src.Width,
            Height = src.Height,
            ThumbnailStorageUrl = string.IsNullOrWhiteSpace(src.ThumbnailStorageUrl) ? null : new Uri(src.ThumbnailStorageUrl),
            ThumbnailCdnUrl = string.IsNullOrWhiteSpace(src.ThumbnailCdnUrl) ? null : new Uri(src.ThumbnailCdnUrl),
            ThumbnailContentType = src.ThumbnailContentType,
            ThumbnailWidth = src.ThumbnailWidth,
            ThumbnailHeight = src.ThumbnailHeight,
            UploadedBy = MapTo(src.UploadedBy)!
        };

    public FileUploadResponse MapTo(CdnFile src) =>
        new()
        {
            Id = src.Id,
            Original = new File { Url = src.CdnUrl.ToString(), ContentType = src.ContentType, Width = src.Width, Height = src.Height },
            Thumbnail = src.ThumbnailCdnUrl is null
                ? null
                : new File
                {
                    Url = src.ThumbnailCdnUrl.ToString(),
                    ContentType = src.ThumbnailContentType,
                    Width = src.ThumbnailWidth,
                    Height = src.ThumbnailHeight
                }
        };

    public PrivateFile MapTo(Shared.Database.Entities.PrivateFile src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            StorageUrl = new Uri(src.StorageUrl),
            ContentType = src.ContentType,
            Width = src.Width,
            Height = src.Height,
            ThumbnailStorageUrl = string.IsNullOrWhiteSpace(src.ThumbnailStorageUrl) ? null : new Uri(src.ThumbnailStorageUrl),
            ThumbnailContentType = src.ThumbnailContentType,
            ThumbnailWidth = src.ThumbnailWidth,
            ThumbnailHeight = src.ThumbnailHeight,
            UploadedBy = MapTo(src.UploadedBy)!
        };

    public FileUploadResponse MapTo(PrivateFile src) =>
        new()
        {
            Id = src.Id,
            Original = new File { Url = src.StorageUrl.ToString(), ContentType = src.ContentType, Width = src.Width, Height = src.Height },
            Thumbnail = src.ThumbnailStorageUrl is null
                ? null
                : new File
                {
                    Url = src.ThumbnailStorageUrl.ToString(),
                    ContentType = src.ThumbnailContentType,
                    Width = src.ThumbnailWidth,
                    Height = src.ThumbnailHeight
                }
        };
}
