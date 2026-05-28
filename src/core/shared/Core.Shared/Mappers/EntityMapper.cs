using Api.Shared.Services.Models;
using Core.Shared.Models;
using CdnFile = Core.Shared.Models.CdnFile;
using Customer = Core.Shared.Models.Customer;

namespace Core.Shared.Mappers;

public interface IEntityMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    CdnFile MapTo(Database.Entities.CdnFile src);
    PrivateFile MapTo(Database.Entities.PrivateFile src);
}

public class EntityMapper : IEntityMapper
{
    public Customer? MapTo(Database.Entities.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Type = src.Type.ToNullableCustomerType()
            };

    public CdnFile MapTo(Database.Entities.CdnFile src) =>
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

    public PrivateFile MapTo(Database.Entities.PrivateFile src) =>
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
}
