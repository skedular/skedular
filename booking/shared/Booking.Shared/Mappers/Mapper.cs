using Api.Shared.Models;
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
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.WorkingFromOffice,
                BookingType.SickLeave => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.SickLeave,
                BookingType.AnnualLeave => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.AnnualLeave,
                BookingType.WellBeingLeave => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.WellBeingLeave,
                BookingType.ClientOffices => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.ClientOffices,
                BookingType.Vacation => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.Vacation,
                BookingType.TravelingForWork => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => 
                    Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            CustomerId = src.Customer.Id,
            OrganizationId = src.Organization is null ? string.Empty : src.Organization.Id,
            LocationId = src.Location is null ? string.Empty : src.Location.Id,
            TeamId = src.Team is null ? string.Empty : src.Team.Id
        };

        booking.DeskIds.AddRange(src.Desks.Select(item => item.Id));

        return booking;
    }
}
