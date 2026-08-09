using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Sanitization;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using BookingCategory = Api.Shared.Services.Models.BookingCategory;
using BookingChannel = Api.Shared.Services.Models.BookingChannel;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using Customer = Booking.Shared.Models.Customer;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using Organization = Booking.Shared.Models.Organization;
using OrganizationTag = Booking.Shared.Models.OrganizationTag;
using PaymentMethod = Api.Shared.Services.Models.PaymentMethod;
using PaymentStatus = Api.Shared.Services.Models.PaymentStatus;
using Resource = Booking.Shared.Models.Resource;
using StripeCheckoutSession = Booking.Shared.Models.StripeCheckoutSession;
using Team = Booking.Shared.Models.Team;

namespace Booking.Api.Mappers;

public interface IGrpcMapper
{
    global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src);
    IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src);
    Shared.Models.Booking MapTo(AddPrivateInput src);
    Shared.Models.Booking MapTo(UpdatePrivateInput src);
}

public class GrpcMapper : IGrpcMapper
{
    public global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking MapToGrpcResponse(Shared.Models.Booking src)
    {
        var booking = new global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Category = src.Category switch
            {
                BookingCategory.WorkingFromHome => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory
                    .WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.ClientOffice,
                BookingCategory.Vacation => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            Channel = src.Channel switch
            {
                BookingChannel.Private => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingChannel.Private,
                BookingChannel.Marketplace => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            MarketplaceBooking = MapToGrpcResponse(src.MarketplaceBooking),
            CreatedByCustomerId = src.CreatedByCustomer is null ? string.Empty : src.CreatedByCustomer.Id.ToSafeString(),
            LastModifiedByCustomerId = src.LastModifiedByCustomer is null ? string.Empty : src.LastModifiedByCustomer.Id.ToSafeString(),
            DeletedByCustomerId = src.DeletedByCustomer is null ? string.Empty : src.DeletedByCustomer.Id.ToSafeString(),
        };

        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));
        booking.Resources.AddRange(src.InvolvedResources.Select(item =>
            new global::Api.Shared.Grpc.Skedular.Booking.Core.V1.Resource
            {
                Id = item.Id,
            }));

        booking.Schedules.AddRange(MapToGrpcResponse(src.Schedules));

        if (src.HasRecurringInstanceOverrides.HasValue)
        {
            booking.HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides.Value;
        }

        return booking;
    }

    public BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src) =>
        new()
        {
            Cursor = src.Cursor,
            Node = MapToGrpcResponse(src.Node),
        };

    public IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src) => src.Select(MapTo);

    public Shared.Models.Booking MapTo(AddPrivateInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.Until.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Category = MapToCategory(src.Category),
            Schedules = new List<BookingSchedule>
            {
                new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()),
            },
            InvolvedCustomers = customers,
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),
            ],
            InvolvedLocations = [],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            Resources =
            [
                .. src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource
                {
                    Id = item,
                }, customers)),
            ],
        };
    }

    public Shared.Models.Booking MapTo(UpdatePrivateInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();
        var from = src.From;
        var until = src.Until;
        var hasSchedule = from is not null && until is not null;

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = hasSchedule ? from!.ToDateTimeOffset() : default,
            Until = hasSchedule ? until!.ToDateTimeOffset() : default,
            Notes = src.Notes.ToSafeString(),
            Category = MapToCategory(src.Category),
            Schedules = hasSchedule
                ? new List<BookingSchedule>
                {
                    new(from!.ToDateTimeOffset(), until!.ToDateTimeOffset()),
                }
                : [],
            InvolvedCustomers = customers,
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),
            ],
            InvolvedLocations = [],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            Resources =
            [
                .. src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource
                {
                    Id = item,
                }, customers)),
            ],
        };
    }

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color,
        };

    private static Resource MapTo(Shared.Database.Entities.Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Capacity = src.Capacity,
            Name = src.Name.ToSafeString(),
            Color = src.Color,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationTags = [.. MapTo(src.OrganizationTags)],
        };

    private static IEnumerable<global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingSchedule> MapToGrpcResponse(
        IEnumerable<BookingSchedule> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingSchedule MapToGrpcResponse(BookingSchedule src) =>
        new()
        {
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
        };

    private static BookingCheckoutSession? MapToGrpcResponse(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession
            {
                Id = src.Id,
                CheckoutUrl = src.CheckoutUrl,
            };

    private static global::Api.Shared.Grpc.Skedular.Booking.Core.V1.MarketplaceBooking? MapToGrpcResponse(MarketplaceBooking? src)
    {
        if (src is null)
        {
            return null;
        }

        var marketplaceBooking = new global::Api.Shared.Grpc.Skedular.Booking.Core.V1.MarketplaceBooking
        {
            Id = src.Id,
            PaymentStatus = src.PaymentStatus switch
            {
                PaymentStatus.NotSet => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.NotSet,
                PaymentStatus.Pending => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.Pending,
                PaymentStatus.Rejected => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.Rejected,
                PaymentStatus.Confirmed => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.Confirmed,
                PaymentStatus.Expired => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.Expired,
                PaymentStatus.RecordNeverCreated => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            IsPaymentRequired = src.IsPaymentRequired,
            PaidByCustomerId = src.PaidByCustomer is null ? string.Empty : src.PaidByCustomer.Id.ToSafeString(),
            PaidByOrganizationId = src.PaidByOrganization is null ? string.Empty : src.PaidByOrganization.Id.ToSafeString(),
            BookingCheckoutSession = MapToGrpcResponse(src.StripeCheckoutSession),
            PaymentExpiry = src.PaymentExpiry.ToTimestamp(),
            TotalAmountExcludeTax = src.TotalAmountExcludeTax.ToNullDouble(),
            TaxAmount = src.TaxAmount.ToNullDouble(),
            TaxRatePercentage = src.TaxRatePercentage.ToNullDouble(),
            TotalAmount = src.TotalAmount.ToNullDouble(),
            Currency = src.Currency.ToNullableCurrency(),
            InvoiceUrl = src.InvoiceUrl.ToSafeString(),
            InvoiceNumber = src.InvoiceNumber.ToSafeString(),
            PaymentMethod = src.PaymentMethod switch
            {
                PaymentMethod.Card => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentMethod.Card,
                PaymentMethod.BankTransfer => global::Api.Shared.Grpc.Skedular.Booking.Core.V1.PaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            },
            Quantity = src.Quantity,
            ProductVersionId = src.ProductVersion.Id.ToSafeString(),
        };

        marketplaceBooking.InvoiceEmailList.AddRange(src.InvoiceEmailList.ToSafeCollection());

        return marketplaceBooking;
    }

    private static BookingCategory MapToCategory(global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory src) =>
        src switch
        {
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WorkingFromHome => BookingCategory.WorkingFromHome,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WorkingFromOffice => BookingCategory.WorkingFromOffice,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WorkingFromCoworkingSpace => BookingCategory.WorkingFromCoworkingSpace,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.SickLeave => BookingCategory.SickLeave,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.AnnualLeave => BookingCategory.AnnualLeave,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.WellbeingLeave => BookingCategory.WellbeingLeave,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.ClientOffice => BookingCategory.ClientOffice,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.Vacation => BookingCategory.Vacation,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.TravelingForWork => BookingCategory.TravelingForWork,
            global::Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory.NonWorkingDay => BookingCategory.NonWorkingDay,
            _ => throw new ArgumentOutOfRangeException(null,
                "Unexpected value encountered. Update enum mapping or caller input to include this case."),
        };
}
