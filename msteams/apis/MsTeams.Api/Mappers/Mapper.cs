using MsTeams.Shared.Models;

namespace MsTeams.Api.Mappers;

public interface IMapper
{
    Organization MapTo(Shared.Database.Entities.Organization src);
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
}
