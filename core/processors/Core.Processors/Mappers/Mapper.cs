using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Customer = Core.Shared.Models.Customer;
using Identity = Core.Shared.Database.Entities.Identity;

namespace Core.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);

    Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities);
    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);
}

public class Mapper : IMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Identities = customer.Identities
                .Select(item => new Shared.Models.Identity { Id = item.Id, Email = item.Email, EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Timezone = src.Timezone;
        dest.Identities = identities;

        return dest;
    }

    public Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(Shared.Models.Identity src,
        Identity dest,
        Shared.Database.Entities.Customer? customer)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.EmailVerified = src.EmailVerified;
        if (customer is not null)
        {
            dest.Customer = customer;
        }

        return dest;
    }
}
