using MsTeams.Shared.Models;

namespace MsTeams.Api.Mappers;

public interface IMapper
{
    Organization MapTo(Shared.Database.Entities.Organization src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
}

public class Mapper : IMapper
{
    public Organization MapTo(Shared.Database.Entities.Organization src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            EventRaisedAt = src.EventRaisedAt
        };
    
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
                Timezone = src.Timezone,
            };
}
