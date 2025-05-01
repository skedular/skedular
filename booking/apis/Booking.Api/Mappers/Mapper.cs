using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Sanitization;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using BookingCheckoutSession = Booking.Shared.Database.Entities.BookingCheckoutSession;
using BookingEdge = Booking.Api.GraphQL.BookingEdge;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using BookingType = Api.Shared.Services.Models.BookingType;
using BookingStatus = Api.Shared.Services.Models.BookingStatus;
using Customer = Booking.Shared.Models.Customer;
using Identity = Booking.Shared.Models.Identity;
using LineItem = Api.Shared.Services.Grpc.Skedular.Booking.V1.LineItem;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationTag = Booking.Shared.Models.OrganizationTag;
using PaymentStatus = Api.Shared.Services.Models.PaymentStatus;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using Team = Booking.Shared.Database.Entities.Team;
using Resource = Booking.Shared.Models.Resource;

namespace Booking.Api.Mappers;

public interface IMapper
{
    Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src);
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
        BookingCheckoutSession? bookingCheckoutSession);

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
        BookingCheckoutSession? bookingCheckoutSession);

    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddInput src);
    Shared.Models.Booking MapTo(UpdateInput src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src);
    IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src);
    IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src);
    IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource> MapToGrpcResponse(IEnumerable<Resource> src);
}

public class Mapper : IMapper
{
    public Shared.Models.Booking MapTo(Shared.Database.Entities.Booking src)
    {
        var team = new Shared.Models.Booking
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type.ToBookingType(),
            Status = src.Status.ToBookingStatus(),
            IsPaymentRequired = src.IsPaymentRequired,
            Schedules = src.Schedules,
            LineItems = src.LineItems,
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
            BookingCheckoutSession = MapTo(src.BookingCheckoutSession)
        };

        return team;
    }

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

    public BookingDetails MapTo(Shared.Models.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = new BookingTypeDetails { Type = src.Type, Name = src.Type.ToBookingTypeName() },
            Status = new BookingStatusDetails { Type = src.Status, Name = src.Status.ToBookingStatusName() },
            IsPaymentRequired = src.IsPaymentRequired,
            Resources = MapTo(src.Resources),
            InvolvedCustomers = MapTo(src.InvolvedCustomers),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations),
            InvolvedLocations = MapTo(src.InvolvedLocations),
            InvolvedTeams = MapTo(src.InvolvedTeams),
            PaidByCustomer = MapTo(src.PaidByCustomer),
            PaidByOrganization = MapTo(src.PaidByOrganization),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            LineItems = src.LineItems.Select(item => new LineItemDetails
            {
                ProductVersionDetails = MapTo(src.ProductVersions.First(productVersion => productVersion.Id == item.ProductVersionId)),
                Quantity = item.Quantity
            }),
            BookingCheckoutSession = MapTo(src.BookingCheckoutSession)
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
            InvolvedOrganizations = src.OrganizationIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Organization { Id = item }).ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds()!.Select(item => new Shared.Models.Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList(),
            LineItems = src.LineItems.Select(item => new ProductVersionLineItem(item.ProductVersionId, item.Quantity)).ToList()
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
            InvolvedOrganizations = src.OrganizationIds.Select(item => new Shared.Models.Organization { Id = item }).ToList(),
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
        BookingCheckoutSession? bookingCheckoutSession) =>
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
            bookingCheckoutSession);

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
        BookingCheckoutSession? bookingCheckoutSession)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.Notes = src.Notes;
        dest.Type = src.Type.ToBookingType();
        dest.Status = src.Status.ToBookingStatus();
        dest.IsPaymentRequired = src.IsPaymentRequired;
        dest.Schedules = src.Schedules;
        dest.LineItems = src.LineItems;
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
        dest.BookingCheckoutSession = bookingCheckoutSession;
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
            Status = src.Status switch
            {
                BookingStatus.Pending => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingStatus.Pending,
                BookingStatus.Rejected => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingStatus.Rejected,
                BookingStatus.Confirmed => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingStatus.Confirmed,
                BookingStatus.PaymentExpired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingStatus.PaymentExpired,
                BookingStatus.PaymentRecordNeverCreated => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingStatus
                    .PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            PaidByCustomer = MapToGrpcResponse(src.PaidByCustomer),
            PaidByOrganization = MapToGrpcResponse(src.PaidByOrganization),
            CreatedByCustomer = MapToGrpcResponse(src.CreatedByCustomer),
            LastModifiedByCustomer = MapToGrpcResponse(src.LastModifiedByCustomer),
            DeletedByCustomer = MapToGrpcResponse(src.DeletedByCustomer),
            BookingCheckoutSession = MapToGrpcResponse(src.BookingCheckoutSession)
        };

        booking.InvolvedCustomers.AddRange(MapToGrpcResponse(src.InvolvedCustomers));
        booking.InvolvedOrganizations.AddRange(MapToGrpcResponse(src.InvolvedOrganizations));
        booking.InvolvedLocations.AddRange(MapToGrpcResponse(src.InvolvedLocations));
        booking.InvolvedTeams.AddRange(MapToGrpcResponse(src.InvolvedTeams));
        booking.Resources.AddRange(MapToGrpcResponse(src.Resources));
        booking.Schedules.AddRange(MapToGrpcResponse(src.Schedules));
        booking.LineItems.AddRange(MapToGrpcResponse(src.LineItems));

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
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
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
                Name = src.Name,
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src) => new(MapTo(src.Node), src.Cursor);
    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) => new(MapTo(src.Node), src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src) => src.Select(MapTo);
    public IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src) => src.Select(item => MapTo(item, []));

    public IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource> MapToGrpcResponse(IEnumerable<Resource> src) =>
        src.Select(item => MapToGrpcResponse(item, []));

    private static ProductVersionDetails MapTo(Shared.Models.ProductVersion src) =>
        new()
        {
            UniqueId = src.Id,
            Name = src.Name,
            Price = src.Price.ToRoundedPrice(),
            PriceUnit = new PriceUnitDetails { Type = src.PriceUnit, Name = src.PriceUnit.ToPriceUnitName() },
            Currency = new CurrencyDetails { Type = src.Currency, Name = src.Currency.ToCurrencyName() }
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer> MapToGrpcResponse(IEnumerable<Customer> src) =>
        src.Select(MapToGrpcResponse)!;

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization> MapToGrpcResponse(
        IEnumerable<Shared.Models.Organization> src) =>
        src.Select(MapToGrpcResponse)!;

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location> MapToGrpcResponse(
        IEnumerable<Shared.Models.Location> src) =>
        src.Select(MapToGrpcResponse)!;

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team> MapToGrpcResponse(IEnumerable<Shared.Models.Team> src) =>
        src.Select(MapToGrpcResponse)!;

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer? MapToGrpcResponse(Customer? src)
    {
        if (src is null)
        {
            return null;
        }

        var customer = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            MiddleName = src.MiddleName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString()
        };

        customer.Identities.AddRange(MapToGrpcResponse(src.Identities));

        return customer;
    }

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity> MapToGrpcResponse(IEnumerable<Identity> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Identity MapToGrpcResponse(Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization? MapToGrpcResponse(Shared.Models.Organization? src) =>
        src is null ? null : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Organization { Id = src.Id, Name = src.Name.ToSafeString() };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location? MapToGrpcResponse(Shared.Models.Location? src) =>
        src is null ? null : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location { Id = src.Id, Name = src.Name.ToSafeString() };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team? MapToGrpcResponse(Shared.Models.Team? src) =>
        src is null ? null : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Team { Id = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationCustomTag> MapToGrpcResponseCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Custom).Select(MapToGrpcResponseCustomTag);

    private static OrganizationCustomTag MapToGrpcResponseCustomTag(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<OrganizationZone> MapToGrpcResponseZones(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Zone).Select(MapToGrpcResponseZone);

    private static OrganizationZone MapToGrpcResponseZone(OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static CustomerDetails? MapTo(Customer? src) =>
        src is null
            ? null
            : new CustomerDetails
            {
                UniqueId = src.Id,
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
                PhotoUrl512 = src.PhotoUrl512
            };

    private static OrganizationDetails? MapTo(Shared.Models.Organization? src) =>
        src is null ? null : new OrganizationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static LocationDetails? MapTo(Shared.Models.Location? src) =>
        src is null ? null : new LocationDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static TeamDetails MapTo(Shared.Models.Team src) => new() { UniqueId = src.Id, Name = src.Name.ToSafeString() };

    private static IEnumerable<OrganizationCustomTagDetails> MapToCustomTags(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Custom).Select(MapToCustomTag);

    private static OrganizationCustomTagDetails MapToCustomTag(OrganizationTag src) =>
        new() { UniqueId = src.Id, Name = src.Name, Color = src.Color };

    private static IEnumerable<OrganizationZoneDetails> MapToZones(IEnumerable<OrganizationTag> src) =>
        src.Where(item => item.Type == OrganizationTagType.Zone).Select(MapToZone);

    private static OrganizationZoneDetails MapToZone(OrganizationTag src) => new() { UniqueId = src.Id, Name = src.Name, Color = src.Color };

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
                Name = src.Name,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
            };

    private static Shared.Models.Team? MapTo(Team? src) =>
        src is null
            ? null
            : new Shared.Models.Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name, Type = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static Resource MapTo(Shared.Database.Entities.Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Capacity = src.Capacity,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static BookingResourceDetails MapTo(Resource src, IEnumerable<Customer> customers) =>
        new()
        {
            UniqueId = src.Id,
            Name = src.Name.ToSafeString(),
            CustomTags = MapToCustomTags(src.OrganizationTags),
            Zones = MapToZones(src.OrganizationTags),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            Location = MapTo(src.Location),
            Customers = MapTo(customers)
        };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource MapToGrpcResponse(Resource src, IEnumerable<Customer> customers)
    {
        var resource = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            ResourceType = MapTo(src.OrganizationTags.First(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type))),
            Location = src.Location is null
                ? null
                : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Location { Id = src.Location.Id, Name = src.Location.Name.ToSafeString() }
        };

        resource.OrganizationCustomTags.AddRange(MapToGrpcResponseCustomTags(src.OrganizationTags));
        resource.OrganizationZones.AddRange(MapToGrpcResponseZones(src.OrganizationTags));
        resource.Customers.Add(MapToGrpcResponse(customers));

        return resource;
    }

    private static ResourceType MapTo(OrganizationTag src) => new() { Id = src.Id, Name = src.Name.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<BookingResourceDetails> MapTo(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item => MapTo(item.Resource, item.Customers));

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

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource>
        MapToGrpcResponse(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item => MapToGrpcResponse(item.Resource, item.Customers));

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule> MapToGrpcResponse(
        IEnumerable<BookingSchedule> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule MapToGrpcResponse(BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static IEnumerable<LineItem> MapToGrpcResponse(IEnumerable<ProductVersionLineItem> src) =>
        src.Select(MapToGrpcResponse);

    private static LineItem MapToGrpcResponse(ProductVersionLineItem src) =>
        new() { ProductVersionId = src.ProductVersionId, Quantity = src.Quantity };

    private IEnumerable<ResourceBookingSlot> MapTo(IEnumerable<Shared.Database.Entities.ResourceBookingSlot> src) => src.Select(MapTo);
    private IEnumerable<Customer> MapTo(IEnumerable<Shared.Database.Entities.Customer> src) => src.Select(MapTo)!;
    private static IEnumerable<Shared.Models.Organization> MapTo(IEnumerable<Organization> src) => src.Select(MapTo)!;
    private IEnumerable<Shared.Models.Location> MapTo(IEnumerable<Location> src) => src.Select(MapTo)!;
    private static IEnumerable<Shared.Models.Team> MapTo(IEnumerable<Team> src) => src.Select(MapTo)!;
    private static IEnumerable<CustomerDetails> MapTo(IEnumerable<Customer> src) => src.Select(MapTo)!;
    private static IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src) => src.Select(MapTo)!;
    private static IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;
    private static IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src) => src.Select(MapTo);

    private static BookingCheckoutSessionDetails? MapTo(Shared.Models.BookingCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSessionDetails
            {
                Id = src.Id,
                PaymentReferenceId = src.PaymentReferenceId,
                PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
                CheckoutUrl = src.CheckoutUrl
            };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCheckoutSession? MapToGrpcResponse(
        Shared.Models.BookingCheckoutSession? src) =>
        src is null
            ? null
            : new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCheckoutSession
            {
                Id = src.Id,
                PaymentReferenceId = src.PaymentReferenceId,
                PaymentStatus = src.PaymentStatus switch
                {
                    PaymentStatus.NoPaymentRequired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.NoPaymentRequired,
                    PaymentStatus.Paid => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Paid,
                    PaymentStatus.Unpaid => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Unpaid,
                    _ => throw new ArgumentOutOfRangeException()
                },
                CheckoutUrl = src.CheckoutUrl
            };

    private static Shared.Models.BookingCheckoutSession? MapTo(BookingCheckoutSession? src) =>
        src is null
            ? null
            : new Shared.Models.BookingCheckoutSession
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                PaymentReferenceId = src.PaymentReferenceId,
                PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
                CheckoutUrl = src.CheckoutUrl
            };
}
