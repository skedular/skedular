using Notification.Shared.Models;

namespace Notification.Api.Mappers;

public interface IMapper
{
    Customer? MapTo(Shared.Database.Entities.Customer? src);
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
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                Identities = MapTo(src.Identities).ToList()
            };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };
}
