using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using CdnFile = Core.Shared.Models.CdnFile;
using Customer = Core.Shared.Models.Customer;

namespace Core.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    CdnFile MapTo(Shared.Database.Entities.CdnFile src);
    FileUploadResponse MapTo(CdnFile src);
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
            Original = new global::Api.Shared.Services.OpenApi.Skedular.Core.V1.CdnFile
            {
                Url = src.CdnUrl.ToString(), ContentType = src.ContentType, Width = src.Width, Height = src.Height
            },
            Thumbnail = src.ThumbnailCdnUrl is null
                ? null
                : new global::Api.Shared.Services.OpenApi.Skedular.Core.V1.CdnFile
                {
                    Url = src.ThumbnailCdnUrl.ToString(),
                    ContentType = src.ThumbnailContentType,
                    Width = src.ThumbnailWidth,
                    Height = src.ThumbnailHeight
                }
        };
}
