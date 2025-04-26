using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL;
using Booking.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using BookingEdge = Booking.Api.GraphQL.BookingEdge;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using BookingType = Api.Shared.Services.Models.BookingType;
using Customer = Booking.Shared.Models.Customer;
using Identity = Booking.Shared.Models.Identity;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;
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
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Shared.Database.Entities.Resource> resources);

    Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Shared.Database.Entities.Resource> resources);

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
            BookingSchedules = src.BookingSchedules,
            Customer = MapTo(src.Customer)!,
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            ResourceBookingSlots = MapTo(src.ResourceBookingSlots).ToList(),
            Team = MapTo(src.Team),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
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
            Type = src.Type,
            Customer = MapTo(src.Customer),
            Organization = MapTo(src.Organization),
            Location = MapTo(src.Location),
            Resources = MapTo(src.Resources),
            Team = MapTo(src.Team),
            InvolvedCustomers = MapTo(src.InvolvedCustomers),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations),
            InvolvedLocations = MapTo(src.InvolvedLocations),
            InvolvedTeams = MapTo(src.InvolvedTeams)
        };

    public Shared.Models.Booking MapTo(AddBookingInput src)
    {
        var customer = new Customer { Id = src.CustomerId };

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type,
            BookingSchedules = new BookingSchedules(new List<BookingSchedule> { new(src.From, src.Until) }),
            Customer = customer,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, [customer])).ToList(),
            ProductVersions = src.ProductVersionIds.Select(item => new ProductVersion { Id = item }).ToList()
        };
    }

    public Shared.Models.Booking MapTo(UpdateBookingInput src)
    {
        var customer = new Customer { Id = src.CustomerId };

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Type = src.Type,
            BookingSchedules = new BookingSchedules(new List<BookingSchedule> { new(src.From, src.Until) }),
            Customer = customer,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, [customer])).ToList(),
            ProductVersions = src.ProductVersionIds.Select(item => new ProductVersion { Id = item }).ToList()
        };
    }

    public Shared.Database.Entities.Booking MapTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Shared.Database.Entities.Resource> resources) =>
        MergeTo(src, new Shared.Database.Entities.Booking(), customer, organization, location, team, resources);

    public Shared.Database.Entities.Booking MergeTo(
        Shared.Models.Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Customer customer,
        Organization? organization,
        Location? location,
        Team? team,
        ICollection<Shared.Database.Entities.Resource> resources)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.Notes = src.Notes;
        dest.Type = src.Type.ToBookingType();
        dest.BookingSchedules = src.BookingSchedules;
        dest.Customer = customer;
        dest.Organization = organization;
        dest.Location = location;
        dest.Team = team;
        dest.ResourceBookingSlots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
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
            Customer = MapToGrpcResponse(src.Customer),
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WorkingFromOffice,
                BookingType.SickLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave,
                BookingType.AnnualLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave,
                BookingType.WellBeingLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave,
                BookingType.ClientOffices => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices,
                BookingType.Vacation => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation,
                BookingType.TravelingForWork => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Organization = MapToGrpcResponse(src.Organization),
            Location = MapToGrpcResponse(src.Location),
            Team = MapToGrpcResponse(src.Team)
        };

        booking.Resources.AddRange(MapToGrpcResponse(src.Resources));
        booking.Schedules.AddRange(MapToGrpcResponse(src.BookingSchedules));

        return booking;
    }

    public Shared.Models.Booking MapTo(AddInput src)
    {
        var customer = new Customer { Id = src.CustomerId };

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
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave => BookingType.WellBeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices => BookingType.ClientOffices,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            BookingSchedules = new BookingSchedules(new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) }),
            Customer = new Customer { Id = src.CustomerId },
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, [customer])).ToList()
        };
    }

    public Shared.Models.Booking MapTo(UpdateInput src)
    {
        var customer = new Customer { Id = src.CustomerId };

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
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.SickLeave => BookingType.SickLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.AnnualLeave => BookingType.AnnualLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.WellBeingLeave => BookingType.WellBeingLeave,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.ClientOffices => BookingType.ClientOffices,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.Vacation => BookingType.Vacation,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.TravelingForWork => BookingType.TravelingForWork,
                global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            BookingSchedules = new BookingSchedules(new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) }),
            Customer = customer,
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Shared.Models.Organization { Id = src.OrganizationId },
            Location = string.IsNullOrWhiteSpace(src.LocationId) ? null : new Shared.Models.Location { Id = src.LocationId },
            Team = string.IsNullOrWhiteSpace(src.TeamId) ? null : new Shared.Models.Team { Id = src.TeamId },
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, [customer])).ToList()
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

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer> MapToGrpcResponse(IEnumerable<Customer> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Customer MapToGrpcResponse(Customer src)
    {
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

    private static CustomerDetails MapTo(Customer src) =>
        new()
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

    private static TeamDetails? MapTo(Shared.Models.Team? src) =>
        src is null ? null : new TeamDetails { UniqueId = src.Id, Name = src.Name.ToSafeString() };

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

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule> MapToGrpcResponse(BookingSchedules src) =>
        src.Schedules.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule MapToGrpcResponse(BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private IEnumerable<ResourceBookingSlot> MapTo(IEnumerable<Shared.Database.Entities.ResourceBookingSlot> src) => src.Select(MapTo);
    private IEnumerable<Customer> MapTo(IEnumerable<Shared.Database.Entities.Customer> src) => src.Select(MapTo)!;
    private static IEnumerable<Shared.Models.Organization> MapTo(IEnumerable<Organization> src) => src.Select(MapTo)!;
    private IEnumerable<Shared.Models.Location> MapTo(IEnumerable<Location> src) => src.Select(MapTo)!;
    private static IEnumerable<Shared.Models.Team> MapTo(IEnumerable<Team> src) => src.Select(MapTo)!;
    private static IEnumerable<CustomerDetails> MapTo(IEnumerable<Customer> src) => src.Select(MapTo);
    private static IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src) => src.Select(MapTo)!;
    private static IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;
    private static IEnumerable<TeamDetails> MapTo(IEnumerable<Shared.Models.Team> src) => src.Select(MapTo)!;
}
