using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.Payment;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Sanitization;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using StripeCheckoutSession = Booking.Shared.Database.Entities.StripeCheckoutSession;
using BookingEdge = Booking.Api.GraphQL.Booking.BookingEdge;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using BookingType = Api.Shared.Services.Models.BookingType;
using Customer = Booking.Shared.Models.Customer;
using Identity = Booking.Shared.Models.Identity;
using LineItem = Api.Shared.Services.Grpc.Skedular.Booking.V1.LineItem;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationTag = Booking.Shared.Models.OrganizationTag;
using PaymentMethod = Api.Shared.Services.Models.PaymentMethod;
using PaymentStatus = Api.Shared.Services.Models.PaymentStatus;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using Team = Booking.Shared.Database.Entities.Team;
using Resource = Booking.Shared.Models.Resource;

namespace Booking.Api.Mappers;

public interface IMapper
{
    Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src, DateTimeOffset paymentExpiry);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    BookingDetails MapTo(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddBookingInput src);
    Shared.Models.Booking MapTo(UpdateBookingInput src);
    Shared.Models.Location? MapTo(Location? src);

    Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        ICollection<Shared.Database.Entities.Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Shared.Database.Entities.Resource> resources,
        Shared.Database.Entities.Customer? paidByCustomer,
        Organization? paidByOrganization,
        Shared.Database.Entities.Customer? createdByCustomer,
        Shared.Database.Entities.Customer? lastModifiedByCustomer,
        Shared.Database.Entities.Customer? deletedByCustomer,
        ICollection<ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession);

    Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        ICollection<Shared.Database.Entities.Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Shared.Database.Entities.Resource> resources,
        Shared.Database.Entities.Customer? paidByCustomer,
        Organization? paidByOrganization,
        Shared.Database.Entities.Customer? createdByCustomer,
        Shared.Database.Entities.Customer? lastModifiedByCustomer,
        Shared.Database.Entities.Customer? deletedByCustomer,
        ICollection<ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession);

    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddInput src);
    Shared.Models.Booking MapTo(UpdateInput src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src, DateTimeOffset paymentExpiry);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src);
    IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src);
    IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src);
}

public class Mapper : IMapper
{
    public Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src, DateTimeOffset paymentExpiry) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type.ToBookingType(),
            PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
            IsPaymentRequired = src.IsPaymentRequired,
            Schedules = src.Schedules,
            LineItems = src.LineItems,
            BookedOnMarketplace = src.BookedOnMarketplace,
            ResourceBookingSlots = MapTo(src.ResourceBookingSlots).ToList(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            PaidByCustomer = MapTo(src.PaidByCustomer),
            PaidByOrganization = MapTo(src.PaidByOrganization),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            StripeCheckoutSession = MapTo(src.StripeCheckoutSession),
            ProductVersions = MapTo(src.ProductVersions).ToList(),
            PaymentExpiry = paymentExpiry,
            PaymentMethod = src.PaymentMethod.ToNullablePaymentMethod(),
            TotalAmountExcludeTax = src.TotalAmountExcludeTax,
            TaxAmount = src.TaxAmount,
            TaxRatePercentage = src.TaxRatePercentage,
            TotalAmount = src.TotalAmount,
            Currency = src.Currency,
            InvoiceUrl = src.InvoiceUrl,
            InvoiceNumber = src.InvoiceNumber,
            InvoiceEmailList = src.InvoiceEmailList
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
                Designation = src.Designation,
                Title = src.Title,
                Timezone = src.Timezone,
                Locale = src.Locale,
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
                PhoneNumber = src.PhoneNumber,
                Identities = MapTo(src.Identities).ToList()
            };

    public BookingDetails MapTo(Shared.Models.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = new BookingTypeDetails { Type = src.Type, Name = src.Type.ToBookingTypeName() },
            PaymentStatus = new PaymentStatusDetails { Type = src.PaymentStatus, Name = src.PaymentStatus.ToPaymentStatusName() },
            IsPaymentRequired = src.IsPaymentRequired,
            BookingResources = MapTo(src.Resources),
            InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
            InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => item.Id),
            InvolvedLocationIds = src.InvolvedLocations.Select(item => item.Id),
            InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
            PaidByCustomerId = src.PaidByCustomer?.Id,
            PaidByOrganizationId = src.PaidByOrganization?.Id,
            CreatedByCustomerId = src.CreatedByCustomer?.Id,
            LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
            DeletedByCustomerId = src.DeletedByCustomer?.Id,
            LineItems =
                src.LineItems.Select(item => new LineItemDetails
                {
                    ProductVersionId = src.ProductVersions.First(productVersion => productVersion.Id == item.ProductVersionId).Id,
                    Quantity = item.Quantity
                }),
            BookedOnMarketplace = src.BookedOnMarketplace,
            BookingCheckoutSession = MapTo(src.StripeCheckoutSession),
            PaymentExpiry = src.PaymentExpiry,
            PaymentMethod =
                src.PaymentMethod is null
                    ? null
                    : new PaymentMethodTypeDetails { Type = src.PaymentMethod.Value, Name = src.PaymentMethod.Value.ToPaymentMethodName() },
            InvoiceUrl = src.InvoiceUrl,
            InvoiceNumber = src.InvoiceNumber,
            InvoiceEmailList = src.InvoiceEmailList,
            TotalAmountExcludeTax = src.TotalAmountExcludeTax,
            TotalAmountExcludeTaxToDisplay =
                src.TotalAmountExcludeTax is null || string.IsNullOrWhiteSpace(src.Currency)
                    ? "N/A"
                    : src.TotalAmountExcludeTax.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.ToCurrency()),
            TaxAmount = src.TaxAmount,
            TaxAmountToDisplay =
                src.TaxAmount is null || string.IsNullOrWhiteSpace(src.Currency)
                    ? "N/A"
                    : src.TaxAmount.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.ToCurrency()),
            TaxRatePercentage = src.TaxRatePercentage,
            TaxRatePercentageToDisplay = src.TaxRatePercentage is null ? "N/A" : src.TaxRatePercentage.Value.ToRoundedDecimal(),
            TotalAmount = src.TotalAmount,
            TotalAmountToDisplay =
                src.TotalAmount is null || string.IsNullOrWhiteSpace(src.Currency)
                    ? "N/A"
                    : src.TotalAmount.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.ToCurrency()),
            Currency = src.Currency,
            CurrencyToDisplay = string.IsNullOrWhiteSpace(src.Currency) ? "N/A" : src.Currency.ToCurrencyName()
        };

    public Shared.Models.Booking MapTo(AddBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds()!.Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type,
            Schedules = new List<BookingSchedule> { new(src.From, src.Until) },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
                src.OrganizationIds.ToSafeCollection().RemoveInvalidIds()!.Select(item => new Shared.Models.Organization { Id = item })
                    .Concat(src.OrganizationUniqueAlphanumericNames.ToSafeCollection().RemoveInvalidIds()!.Select(item =>
                        new Shared.Models.Organization { UniqueAlphanumericName = item }))
                    .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList(),
            LineItems = src.LineItems.Select(item => new ProductVersionLineItem(item.ProductVersionId, item.Quantity)).ToList(),
            PaymentMethod = src.PaymentMethod,
            InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection()
        };
    }

    public Shared.Models.Booking MapTo(UpdateBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds()!.Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type,
            Schedules = new List<BookingSchedule> { new(src.From, src.Until) },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
                src.OrganizationIds.ToSafeCollection().RemoveInvalidIds()!.Select(item => new Shared.Models.Organization { Id = item })
                    .Concat(src.OrganizationUniqueAlphanumericNames.ToSafeCollection().RemoveInvalidIds()!.Select(item =>
                        new Shared.Models.Organization { UniqueAlphanumericName = item }))
                    .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Team { Id = item }).ToList(),
            Resources = src.ResourceIds.RemoveInvalidIds()!.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        ICollection<Shared.Database.Entities.Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Shared.Database.Entities.Resource> resources,
        Shared.Database.Entities.Customer? paidByCustomer,
        Organization? paidByOrganization,
        Shared.Database.Entities.Customer? createdByCustomer,
        Shared.Database.Entities.Customer? lastModifiedByCustomer,
        Shared.Database.Entities.Customer? deletedByCustomer,
        ICollection<ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession) =>
        MergeTo(
            src,
            new Shared.Database.Entities.Booking(),
            involvedCustomers,
            involvedOrganizations,
            involvedLocations,
            involvedTeams,
            resources,
            paidByCustomer,
            paidByOrganization,
            createdByCustomer,
            lastModifiedByCustomer,
            deletedByCustomer,
            productVersions,
            stripeCheckoutSession);

    public Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        ICollection<Shared.Database.Entities.Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Shared.Database.Entities.Resource> resources,
        Shared.Database.Entities.Customer? paidByCustomer,
        Organization? paidByOrganization,
        Shared.Database.Entities.Customer? createdByCustomer,
        Shared.Database.Entities.Customer? lastModifiedByCustomer,
        Shared.Database.Entities.Customer? deletedByCustomer,
        ICollection<ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.Notes = src.Notes;
        dest.Type = src.Type.ToBookingType();
        dest.PaymentStatus = src.PaymentStatus.ToPaymentStatus();
        dest.IsPaymentRequired = src.IsPaymentRequired;
        dest.Schedules = src.Schedules;
        dest.LineItems = src.LineItems;
        dest.BookedOnMarketplace = src.BookedOnMarketplace;
        dest.ResourceBookingSlots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        dest.InvolvedCustomers = involvedCustomers;
        dest.InvolvedOrganizations = involvedOrganizations;
        dest.InvolvedLocations = involvedLocations;
        dest.InvolvedTeams = involvedTeams;
        dest.PaidByCustomer = paidByCustomer;
        dest.PaidByOrganization = paidByOrganization;
        dest.CreatedByCustomer = createdByCustomer;
        dest.LastModifiedByCustomer = lastModifiedByCustomer;
        dest.DeletedByCustomer = deletedByCustomer;
        dest.ProductVersions = productVersions;
        dest.StripeCheckoutSession = stripeCheckoutSession;
        dest.PaymentMethod = src.PaymentMethod.ToNullablePaymentMethod();
        dest.TotalAmountExcludeTax = src.TotalAmountExcludeTax;
        dest.TaxAmount = src.TaxAmount;
        dest.TaxRatePercentage = src.TaxRatePercentage;
        dest.TotalAmount = src.TotalAmount;
        dest.Currency = src.Currency;
        dest.InvoiceUrl = src.InvoiceUrl;
        dest.InvoiceNumber = src.InvoiceNumber;
        dest.InvoiceEmailList = src.InvoiceEmailList;
        return dest;
    }

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src)
    {
        var booking = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromCoworkingSpace,
                BookingType.SickLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave,
                BookingType.AnnualLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave,
                BookingType.WellbeingLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellbeingLeave,
                BookingType.ClientOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffice,
                BookingType.Vacation => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation,
                BookingType.TravelingForWork => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            PaymentStatus = src.PaymentStatus switch
            {
                PaymentStatus.Pending => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Pending,
                PaymentStatus.Rejected => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Rejected,
                PaymentStatus.Confirmed => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Confirmed,
                PaymentStatus.Expired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Expired,
                PaymentStatus.RecordNeverCreated => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            PaidByCustomerId = src.PaidByCustomer is null ? string.Empty : src.PaidByCustomer.Id.ToSafeString(),
            PaidByOrganizationId = src.PaidByOrganization is null ? string.Empty : src.PaidByOrganization.Id.ToSafeString(),
            CreatedByCustomerId = src.CreatedByCustomer is null ? string.Empty : src.CreatedByCustomer.Id.ToSafeString(),
            LastModifiedByCustomerId = src.LastModifiedByCustomer is null ? string.Empty : src.LastModifiedByCustomer.Id.ToSafeString(),
            DeletedByCustomerId = src.DeletedByCustomer is null ? string.Empty : src.DeletedByCustomer.Id.ToSafeString(),
            BookingCheckoutSession = MapToGrpcResponse(src.StripeCheckoutSession),
            PaymentExpiry = src.PaymentExpiry.ToTimestamp(),
            BookedOnMarketplace = src.BookedOnMarketplace,
            TotalAmountExcludeTax = src.TotalAmountExcludeTax.ToNullDouble(),
            TaxAmount = src.TaxAmount.ToNullDouble(),
            TaxRatePercentage = src.TaxRatePercentage.ToNullDouble(),
            TotalAmount = src.TotalAmount.ToNullDouble(),
            Currency = src.Currency.ToSafeString(),
            InvoiceUrl = src.InvoiceUrl.ToSafeString(),
            InvoiceNumber = src.InvoiceNumber.ToSafeString()
        };

        if (src.PaymentMethod is not null)
        {
            booking.PaymentMethod = src.PaymentMethod switch
            {
                PaymentMethod.Card => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentMethod.Card,
                PaymentMethod.BankTransfer => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));
        booking.ResourceIds.AddRange(src.Resources.Select(item => item.Resource.Id));
        booking.Schedules.AddRange(MapToGrpcResponse(src.Schedules));
        booking.LineItems.AddRange(MapToGrpcResponse(src.LineItems));
        booking.InvoiceEmailList.AddRange(src.InvoiceEmailList.ToSafeCollection());

        return booking;
    }

    public Shared.Models.Booking MapTo(AddInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds()!.Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.Until.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome => BookingType.WorkingFromHome,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice => BookingType.WorkingFromOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromCoworkingSpace => BookingType.WorkingFromCoworkingSpace,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellbeingLeave => BookingType.WellbeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffice => BookingType.ClientOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Schedules = new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) },
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Organization { Id = item }).ToList(),
            InvolvedLocations = [],
            InvolvedTeams = src.TeamIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList(),
            BookedOnMarketplace = false
        };
    }

    public Shared.Models.Booking MapTo(UpdateInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds()!.Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.Until.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome => BookingType.WorkingFromHome,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice => BookingType.WorkingFromOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromCoworkingSpace => BookingType.WorkingFromCoworkingSpace,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellbeingLeave => BookingType.WellbeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffice => BookingType.ClientOffice,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Schedules = new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) },
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Organization { Id = item }).ToList(),
            InvolvedLocations = [],
            InvolvedTeams = src.TeamIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public Shared.Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Shared.Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src, DateTimeOffset paymentExpiry) =>
        new(MapTo(src.Node, paymentExpiry), src.Cursor);

    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) => new(MapTo(src.Node), src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src) => src.Select(MapTo);
    public IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src) => src.Select(item => MapTo(item, []));

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<Shared.Models.Organization> MapTo(IEnumerable<Organization> src) => src.Select(MapTo)!;

    private static Shared.Models.Organization? MapTo(Organization? src) =>
        src is null
            ? null
            : new Shared.Models.Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                UniqueAlphanumericName = src.UniqueAlphanumericName,
                Name = src.Name,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType()
            };

    private static IEnumerable<Shared.Models.Team> MapTo(IEnumerable<Team> src) => src.Select(MapTo)!;

    private static Shared.Models.Team? MapTo(Team? src) =>
        src is null
            ? null
            : new Shared.Models.Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt
            };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new() { Id = src.Id, Type = src.Type.ToNullableOrganizationTagType() };

    private static Resource MapTo(Shared.Database.Entities.Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Capacity = src.Capacity,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static BookingResourceDetails MapTo(Resource src, IEnumerable<Customer> customers) =>
        new() { ResourceId = src.Id, LocationId = src.Location?.Id, CustomerIds = customers.Select(item => item.Id) };

    private static IEnumerable<BookingResourceDetails> MapTo(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item => MapTo(item.Resource, item.Customers));

    private IEnumerable<ResourceBookingSlot> MapTo(IEnumerable<Shared.Database.Entities.ResourceBookingSlot> src) => src.Select(MapTo);

    private ResourceBookingSlot MapTo(Shared.Database.Entities.ResourceBookingSlot src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Available = src.Available,
            Start = src.Start,
            Customers = MapTo(src.Customers).ToList(),
            Resource = MapTo(src.Resource)
        };

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule> MapToGrpcResponse(
        IEnumerable<BookingSchedule> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule MapToGrpcResponse(BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static IEnumerable<LineItem> MapToGrpcResponse(IEnumerable<ProductVersionLineItem> src) => src.Select(MapToGrpcResponse);

    private static LineItem MapToGrpcResponse(ProductVersionLineItem src) =>
        new() { ProductVersionId = src.ProductVersionId, Quantity = src.Quantity };

    private IEnumerable<Customer> MapTo(IEnumerable<Shared.Database.Entities.Customer> src) => src.Select(MapTo)!;
    private IEnumerable<Shared.Models.Location> MapTo(IEnumerable<Location> src) => src.Select(MapTo)!;

    private static BookingCheckoutSessionDetails? MapTo(Shared.Models.StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSessionDetails { UniqueId = src.Id, CheckoutUrl = src.CheckoutUrl };

    private static BookingCheckoutSession? MapToGrpcResponse(Shared.Models.StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession { Id = src.Id, CheckoutUrl = src.CheckoutUrl };

    private static Shared.Models.StripeCheckoutSession? MapTo(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new Shared.Models.StripeCheckoutSession
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                DeletedAt = src.DeletedAt,
                CheckoutUrl = src.CheckoutUrl.ToSafeString()
            };

    private static IEnumerable<Shared.Models.ProductVersion> MapTo(IEnumerable<ProductVersion> src) =>
        src.Select(MapTo);

    private static Shared.Models.ProductVersion MapTo(ProductVersion src)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(src.PriceUnit);

        if (!src.PricePerMinute.HasValue)
        {
            throw new ArgumentNullException(nameof(src.PricePerMinute));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(src.Currency);

        if (!src.BookAllLocationResources.HasValue)
        {
            throw new ArgumentNullException(nameof(src.BookAllLocationResources));
        }

        if (!src.RecurrenceWindowDays.HasValue)
        {
            throw new ArgumentNullException(nameof(src.RecurrenceWindowDays));
        }

        if (!src.RequireConsecutiveDays.HasValue)
        {
            throw new ArgumentNullException(nameof(src.RequireConsecutiveDays));
        }

        if (!src.NumberOfResourcesToBook.HasValue)
        {
            throw new ArgumentNullException(nameof(src.NumberOfResourcesToBook));
        }

        if (!src.IsPriceTaxInclusive.HasValue)
        {
            throw new ArgumentNullException(nameof(src.IsPriceTaxInclusive));
        }

        return new Shared.Models.ProductVersion
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name.ToSafeString(),
            Price = src.Price ?? 0,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive.Value,
            PricePerMinute = src.PricePerMinute.Value,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources.Value,
            RecurrenceWindowDays = src.RecurrenceWindowDays.Value,
            RequireConsecutiveDays = src.RequireConsecutiveDays.Value,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook.Value
        };
    }
}
