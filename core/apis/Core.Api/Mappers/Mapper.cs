using Core.Shared.Models;
using Customer = Core.Shared.Models.Customer;

namespace Core.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    CdnFile MapTo(Shared.Database.Entities.CdnFile src);
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
            UploadedBy = MapTo(src.UploadedBy)!
        };
}
