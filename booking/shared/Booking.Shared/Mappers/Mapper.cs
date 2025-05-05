using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using BookingCheckoutSession = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCheckoutSession;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingSchedule;
using LineItem = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.LineItem;
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
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromCoworkingSpace,
                BookingType.SickLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.SickLeave,
                BookingType.AnnualLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.AnnualLeave,
                BookingType.WellbeingLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WellbeingLeave,
                BookingType.ClientOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.ClientOffice,
                BookingType.Vacation => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.Vacation,
                BookingType.TravelingForWork => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                BookingStatus.Pending => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingStatus.Pending,
                BookingStatus.Rejected => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingStatus.Rejected,
                BookingStatus.Confirmed => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingStatus.Confirmed,
                BookingStatus.PaymentExpired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingStatus.PaymentExpired,
                BookingStatus.PaymentRecordNeverCreated =>
                    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingStatus.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            BookedOnMarketplace = src.BookedOnMarketplace,
            BookingCheckoutSession = MapTo(src.BookingCheckoutSession)
        };

        if (src.PaidByCustomer is not null)
        {
            booking.PaidByCustomerId = src.PaidByCustomer.Id;
        }

        if (src.PaidByOrganization is not null)
        {
            booking.PaidByOrganizationId = src.PaidByOrganization.Id;
        }

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

        booking.Resources.AddRange(MapTo(src.Resources));
        booking.Schedules.AddRange(MapTo(src.Schedules));
        booking.LineItems.AddRange(MapTo(src.LineItems));
        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));

        return booking;
    }

    private static BookingCheckoutSession? MapTo(Models.BookingCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession
            {
                Id = src.Id,
                CheckoutUrl = src.CheckoutUrl.ToSafeString(),
                PaymentStatus = src.PaymentStatus switch
                {
                    PaymentStatus.NoPaymentRequired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.NoPaymentRequired,
                    PaymentStatus.Pending => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Pending,
                    PaymentStatus.Paid => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Paid,
                    PaymentStatus.Unpaid => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Unpaid,
                    PaymentStatus.Expired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Expired,
                    _ => throw new ArgumentOutOfRangeException()
                },
                AmountTotal = src.AmountTotal is null ? string.Empty : src.AmountTotal.Value.ToRoundedPrice(),
                Currency = src.Currency.ToSafeString()
            };

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item =>
        {
            var resource = new Resource { Id = item.Resource.Id };

            resource.CustomerIds.AddRange(item.Customers.Select(customer => customer.Id));

            return resource;
        });

    private static IEnumerable<BookingSchedule> MapTo(IEnumerable<Api.Shared.Services.Models.BookingSchedule> src) => src.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static IEnumerable<LineItem> MapTo(IEnumerable<ProductVersionLineItem> src) => src.Select(MapTo);

    private static LineItem MapTo(ProductVersionLineItem src) =>
        new() { ProductVersionId = src.ProductVersionId, Quantity = src.Quantity };
}
