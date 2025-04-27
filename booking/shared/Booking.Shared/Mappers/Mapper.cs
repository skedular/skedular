using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingSchedule;
using Resource = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Resource;

namespace Booking.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src)
    {
        var booking = new Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromOffice,
                BookingType.SickLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.SickLeave,
                BookingType.AnnualLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.AnnualLeave,
                BookingType.WellBeingLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WellBeingLeave,
                BookingType.ClientOffices => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.ClientOffices,
                BookingType.Vacation => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.Vacation,
                BookingType.TravelingForWork => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        booking.Resources.AddRange(MapTo(src.Resources));
        booking.Schedules.AddRange(MapTo(src.BookingSchedules));
        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));

        return booking;
    }

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item =>
        {
            var resource = new Resource { Id = item.Resource.Id };

            resource.CustomerIds.AddRange(item.Customers.Select(customer => customer.Id));

            return resource;
        });

    private static IEnumerable<BookingSchedule> MapTo(BookingSchedules src) => src.Schedules.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };
}
