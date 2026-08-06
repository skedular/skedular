using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.BookingSchedule;
using Resource = Api.Shared.Clients.Events.Skedular.Booking.V1.Resource;

namespace Booking.Shared.Mappers;

public interface IEventMapper
{
    Api.Shared.Clients.Events.Skedular.Booking.V1.Booking MapTo(Models.Booking src);
}

public class EventMapper : IEventMapper
{
    public Api.Shared.Clients.Events.Skedular.Booking.V1.Booking MapTo(Models.Booking src)
    {
        var booking = new Api.Shared.Clients.Events.Skedular.Booking.V1.Booking
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Category = src.Category switch
            {
                BookingCategory.WorkingFromHome => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.ClientOffice,
                BookingCategory.Vacation => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            Channel = src.Channel switch
            {
                BookingChannel.Private => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingChannel.Private,
                BookingChannel.Marketplace => Api.Shared.Clients.Events.Skedular.Booking.V1.BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
        };

        if (src.CreatedByCustomer is not null)
        {
            booking.CreatedByCustomerId = src.CreatedByCustomer.Id;
        }

        if (src.LastModifiedByCustomer is not null)
        {
            booking.LastModifiedByCustomerId = src.LastModifiedByCustomer.Id;
        }

        if (src.DeletedByCustomer is not null)
        {
            booking.DeletedByCustomerId = src.DeletedByCustomer.Id;
        }

        if (src.HasRecurringInstanceOverrides.HasValue)
        {
            booking.HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides.Value;
        }

        booking.Resources.AddRange(MapTo(src.Resources));
        booking.Schedules.AddRange(MapTo(src.Schedules));
        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));

        return booking;
    }

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) => src.Select(item => new Resource
    {
        Id = item.Resource.Id,
    });

    private static IEnumerable<BookingSchedule> MapTo(IEnumerable<Api.Shared.Services.Models.BookingSchedule> src) => src.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new()
        {
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
        };
}
