using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Location.Shared.Models;
using Customer = Location.Shared.Models.Customer;
using Desk = Location.Shared.Models.Desk;
using Room = Location.Shared.Models.Room;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Location.Shared.Database.Entities.Identity;
using LocationMember = Location.Shared.Database.Entities.LocationMember;
using Offering = Api.Shared.Services.Models.Offering;
using Organization = Location.Shared.Models.Organization;
using OrganizationMember = Location.Shared.Database.Entities.OrganizationMember;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Booking MapTo(Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event src);
    Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Identity> identities);
    Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities);
    IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src, Shared.Database.Entities.Customer? customer);
    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);
    Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Booking MapToEntity(
        Booking src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks,
        ICollection<Shared.Database.Entities.Room> rooms);

    Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks,
        ICollection<Shared.Database.Entities.Room> rooms);

    IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization);
    OrganizationTag MergeToEntity(Shared.Models.OrganizationTag src, OrganizationTag dest, Shared.Database.Entities.Organization organization);
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
            Name = customer.Name,
            GivenName = customer.GivenName,
            MiddleName = customer.MiddleName,
            FamilyName = customer.FamilyName,
            PhotoUrl = customer.PhotoUrl,
            PhotoUrl24 = customer.PhotoUrl24,
            PhotoUrl32 = customer.PhotoUrl32,
            PhotoUrl48 = customer.PhotoUrl48,
            PhotoUrl72 = customer.PhotoUrl72,
            PhotoUrl192 = customer.PhotoUrl192,
            PhotoUrl512 = customer.PhotoUrl512,
            Identities = customer.Identities
                .Select(item => new Shared.Models.Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified })
                .ToList()
        };
    }

    public Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.Organization;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = organizationAfterState.Name,
            LogoUrl = organizationAfterState.LogoUrl,
            Offering = new Offering
            {
                Id = organizationAfterState.Offering.Id,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                ActiveCustomerIds = organizationAfterState.Offering.ActiveCustomerIds.ToArray()
            }
        };

        organization.Tags = organizationAfterState.Tags.Select(item => new Shared.Models.OrganizationTag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = item.Name,
            Type = item.Type.ToNullableOrganizationTagType(),
            Color = item.Color,
            Organization = organization
        }).ToList();

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                Role = item.Role switch
                {
                    Role.Owner => OrganizationMemberRole.Owner,
                    Role.Administrator => OrganizationMemberRole.Administrator,
                    Role.Member => OrganizationMemberRole.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Status = item.Status switch
                {
                    Status.Active => OrganizationMemberStatus.Active,
                    Status.Inactive => OrganizationMemberStatus.Inactive,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Customer = new Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        return organization;
    }


    public Booking MapTo(Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event src)
    {
        var booking = src.Data.Booking;
        var deletedAt = booking.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Booking
        {
            Id = booking.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            From = booking.From.ToDateTimeOffset(),
            To = booking.To.ToDateTimeOffset(),
            Location = new Shared.Models.Location { Id = booking.LocationId },
            Desks = booking.DeskIds.Select(item => new Desk { Id = item }).ToList(),
            Rooms = booking.RoomIds.Select(item => new Room { Id = item }).ToList()
        };
    }

    public Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest, ICollection<Identity> identities)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.MiddleName = src.MiddleName;
        dest.FamilyName = src.FamilyName;
        dest.PhotoUrl = src.PhotoUrl;
        dest.PhotoUrl24 = src.PhotoUrl24;
        dest.PhotoUrl32 = src.PhotoUrl32;
        dest.PhotoUrl48 = src.PhotoUrl48;
        dest.PhotoUrl72 = src.PhotoUrl72;
        dest.PhotoUrl192 = src.PhotoUrl192;
        dest.PhotoUrl512 = src.PhotoUrl512;
        dest.Identities = identities;
        return dest;
    }

    public IEnumerable<Identity> MapToEntity(
        IEnumerable<Shared.Models.Identity> src,
        Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Identity(), customer);

    public Identity MergeToEntity(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer? customer)
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

    public Shared.Database.Entities.Booking MapToEntity(
        Booking src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks,
        ICollection<Shared.Database.Entities.Room> rooms) =>
        MergeToEntity(src, new Shared.Database.Entities.Booking(), location, desks, rooms);

    public Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks,
        ICollection<Shared.Database.Entities.Room> rooms)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.From = src.From;
        dest.To = src.To;
        dest.Location = location;
        dest.Desks = desks;
        dest.Rooms = rooms;
        return dest;
    }

    public IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src) => src.Select(MapTo);

    public Shared.Database.Entities.Organization MapToEntity(Organization src) => MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.LogoUrl = src.LogoUrl;
        dest.Offering = src.Offering;
        return dest;
    }

    public OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationTag(), organization);

    public OrganizationTag MergeToEntity(Shared.Models.OrganizationTag src, OrganizationTag dest, Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type.ToNullableOrganizationTagType();
        dest.Color = src.Color;
        dest.Organization = organization;
        return dest;
    }

    private static Shared.Models.Location MapTo(Shared.Database.Entities.Location src)
    {
        var location = new Shared.Models.Location
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            About = src.About,
            Timezone = src.Timezone,
            OpeningHours = src.OpeningHours,
            Organization = MapTo(src.Organization)
        };

        location.LocationMembers = MapTo(src.LocationMembers, location).ToList();
        location.Resources = MapTo(src.Resources, location).ToList();
        location.Desks = MapTo(src.Desks, location).ToList();
        location.Rooms = MapTo(src.Rooms, location).ToList();

        return location;
    }

    private static Organization? MapTo(Shared.Database.Entities.Organization? src)
    {
        if (src is null)
        {
            return null;
        }

        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            LogoUrl = src.LogoUrl,
            Offering = src.Offering
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();

        return organization;
    }

    private static JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Location = MapTo(src.Location),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private static IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Resource MapTo(Shared.Database.Entities.Resource src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            IsOpeningHoursOverriden = src.IsOpeningHoursOverriden ?? false,
            AvailableHours = src.AvailableHours,
            Location = location
        };

    private static IEnumerable<Desk> MapTo(IEnumerable<Shared.Database.Entities.Desk> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Desk MapTo(Shared.Database.Entities.Desk src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Location = location
        };

    private static IEnumerable<Room> MapTo(IEnumerable<Shared.Database.Entities.Room> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Room MapTo(Shared.Database.Entities.Room src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Deactivated = src.Deactivated,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Location = location
        };

    private static IEnumerable<Shared.Models.LocationMember> MapTo(IEnumerable<LocationMember> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Shared.Models.LocationMember MapTo(LocationMember src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToLocationMemberRole(),
            Customer = MapTo(src.Customer)!,
            Location = location
        };

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(IEnumerable<OrganizationMember> src, Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationMember MapTo(OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToNullableOrganizationMemberRole(),
            Status = src.Status.ToOrganizationMemberStatus(),
            Customer = MapTo(src.Customer)!,
            Organization = organization
        };

    private static Customer? MapTo(Shared.Database.Entities.Customer? src) =>
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

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity> src) => src.Select(MapTo);

    private static Shared.Models.Identity MapTo(Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };
}
