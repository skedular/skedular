using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;

namespace Booking.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Booking MapTo(Models.Booking src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Booking MapTo(Models.Booking src)
    {
        var booking = new Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.To.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            CustomerId = src.Customer.Id,
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id,
            LocationId = src.Location is null ? string.Empty : src.Location.Id,
            TeamId = src.Team is null ? string.Empty : src.Team.Id
        };

        booking.DeskIds.AddRange(src.Desks.Select(item => item.Id));

        return booking;
    }
}
