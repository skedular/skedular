using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Location.Shared.Models;
using Customer = Location.Shared.Models.Customer;
using Desk = Location.Shared.Models.Desk;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Location.Shared.Database.Entities.Identity;
using LocationMember = Location.Shared.Database.Entities.LocationMember;
using Offering = Location.Shared.Models.Offering;
using Organization = Location.Shared.Models.Organization;
using OrganizationMember = Location.Shared.Database.Entities.OrganizationMember;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;
using Tag = Location.Shared.Database.Entities.Tag;

namespace Location.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Shared.Models.Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Booking MapTo(Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event src);

    Shared.Database.Entities.Customer MapToEntity(Customer src, ICollection<Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities);

    IEnumerable<Identity> MapToEntity(
        IEnumerable<Shared.Models.Identity> src,
        Shared.Database.Entities.Customer? customer);

    Identity MapToEntity(Shared.Models.Identity src, Shared.Database.Entities.Customer? customer);

    Identity MergeToEntity(
        Shared.Models.Identity src,
        Identity dest,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Booking MapToEntity(
        Booking src,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks);

    Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks);

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

    Shared.Database.Entities.Location MapToEntity(
        Shared.Models.Location src,
        Shared.Database.Entities.Organization? organization);

    Shared.Database.Entities.Location MergeToEntity(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization);

    LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer);

    Tag MapToEntity(Shared.Models.Tag src, Shared.Database.Entities.Location location);

    Tag MergeToEntity(
        Shared.Models.Tag src,
        Tag dest,
        Shared.Database.Entities.Location location);

    Shared.Database.Entities.Desk MapToEntity(
        Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<OrganizationTag> organizationTags);

    Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<OrganizationTag> organizationTags);

    OrganizationTag MergeToEntity(
        Shared.Models.OrganizationTag src,
        OrganizationTag dest,
        Shared.Database.Entities.Organization organization);

    OrganizationTag MapToEntity(
        Shared.Models.OrganizationTag src,
        Shared.Database.Entities.Organization organization);
}

public class Mapper : IMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.AfterState;
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
            Identities = customer.Identities.Select(item =>
                    new Shared.Models.Identity
                    {
                        Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified
                    })
                .ToList()
        };
    }

    public Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.OrganizationAfterState;
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
            Type = item.TagType,
            Organization = organization
        }).ToList();

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                MembershipType = item.MembershipType switch
                {
                    MembershipType.Owner => OrganizationMembershipType.Owner,
                    MembershipType.Administrator => OrganizationMembershipType.Administrator,
                    MembershipType.Member => OrganizationMembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Customer = new Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        return organization;
    }

    public Shared.Models.Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src)
    {
        var locationAfterState = src.Data.LocationAfterState;
        var deletedAt = locationAfterState.DeletedAt?.ToDateTimeOffset();

        var location = new Shared.Models.Location
        {
            Id = locationAfterState.Id,
            DeletedAt = deletedAt,
            Name = locationAfterState.Name,
            About = locationAfterState.About,
            Timezone = locationAfterState.Timezone,
            Organization = string.IsNullOrWhiteSpace(locationAfterState.OrganizationId)
                ? null
                : new Organization { Id = locationAfterState.OrganizationId },
            LocationMembers = locationAfterState.Members.Select(item =>
            {
                return new Shared.Models.LocationMember
                {
                    Id = item.Id,
                    MembershipType = item.MembershipType switch
                    {
                        Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Owner =>
                            LocationMembershipType.Owner,
                        Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Administrator =>
                            LocationMembershipType.Administrator,
                        Api.Shared.Clients.Events.UnityHub.Location.V1.Value.MembershipType.Member =>
                            LocationMembershipType.Member,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Customer = new Customer { Id = item.CustomerId }
                };
            }).ToList()
        };

        location.Tags = locationAfterState.Tags.Select(item => new Shared.Models.Tag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            Name = item.Name,
            Description = item.Description,
            Type = item.TagType,
            Location = location
        }).ToList();

        location.Desks = locationAfterState.Desks.Select(item => new Desk
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            Name = item.Name,
            Tags =
                item.LocationTagIds.Select(tagId =>
                    new Shared.Models.Tag { Id = tagId, Location = location }).ToList()
        }).ToList();

        return location;
    }

    public Booking MapTo(Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event src)
    {
        var booking = src.Data.AfterState;
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
            Desks = booking.DeskIds.Select(item => new Desk { Id = item }).ToList()
        };
    }

    public Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Identity> identities)
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

    public Identity MergeToEntity(Shared.Models.Identity src, Identity dest,
        Shared.Database.Entities.Customer? customer)
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
        ICollection<Shared.Database.Entities.Desk> desks) =>
        MergeToEntity(src, new Shared.Database.Entities.Booking(), location, desks);

    public Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Location location,
        ICollection<Shared.Database.Entities.Desk> desks)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.From = src.From;
        dest.To = src.To;
        dest.Location = location;
        dest.Desks = desks;
        return dest;
    }

    public IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src) =>
        src.Select(MapTo);

    public Shared.Database.Entities.Organization MapToEntity(Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src,
        Shared.Database.Entities.Organization dest)
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
        dest.MembershipType = src.MembershipType;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public Shared.Database.Entities.Location MapToEntity(
        Shared.Models.Location src,
        Shared.Database.Entities.Organization? organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Location(), organization);

    public Shared.Database.Entities.Location MergeToEntity(
        Shared.Models.Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization? organization)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Timezone = src.Timezone;
        dest.Organization = organization;
        return dest;
    }

    public LocationMember MapToEntity(
        Shared.Models.LocationMember src,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new LocationMember(), location, customer);

    public LocationMember MergeToEntity(
        Shared.Models.LocationMember src,
        LocationMember dest,
        Shared.Database.Entities.Location location,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.MembershipType = src.MembershipType;
        dest.Location = location;
        dest.Customer = customer;
        return dest;
    }

    public Tag MapToEntity(Shared.Models.Tag src, Shared.Database.Entities.Location location) =>
        MergeToEntity(src, new Tag(), location);

    public Tag MergeToEntity(
        Shared.Models.Tag src,
        Tag dest,
        Shared.Database.Entities.Location location)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Type = src.Type;
        dest.Location = location;
        return dest;
    }

    public Shared.Database.Entities.Desk MapToEntity(
        Desk src,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<OrganizationTag> organizationTags) =>
        MergeToEntity(src, new Shared.Database.Entities.Desk(), location, tags, organizationTags);

    public Shared.Database.Entities.Desk MergeToEntity(
        Desk src,
        Shared.Database.Entities.Desk dest,
        Shared.Database.Entities.Location location,
        ICollection<Tag> tags,
        ICollection<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Location = location;
        dest.Tags = tags;
        dest.OrganizationTags = organizationTags;
        return dest;
    }

    public OrganizationTag MergeToEntity(
        Shared.Models.OrganizationTag src,
        OrganizationTag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Type = src.Type;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationTag MapToEntity(Shared.Models.OrganizationTag src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationTag(), organization);

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
            Organization = MapTo(src.Organization)
        };

        location.LocationMembers = MapTo(src.LocationMembers, location).ToList();
        location.Tags = MapTo(src.Tags, location).ToList();
        location.Desks = MapTo(src.Desks, location).ToList();

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
            Status = src.Status,
            Location = MapTo(src.Location),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private static IEnumerable<Shared.Models.Tag> MapTo(IEnumerable<Tag> src, Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Shared.Models.Tag MapTo(Tag src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type,
            Location = location
        };

    private static IEnumerable<Desk> MapTo(IEnumerable<Shared.Database.Entities.Desk> src,
        Shared.Models.Location location) =>
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
            Tags = MapTo(src.Tags, location).ToList(),
            Location = location
        };

    private static IEnumerable<Shared.Models.LocationMember> MapTo(
        IEnumerable<LocationMember> src,
        Shared.Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static Shared.Models.LocationMember
        MapTo(LocationMember src, Shared.Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            MembershipType = src.MembershipType,
            Customer = MapTo(src.Customer)!,
            Location = location
        };

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(
        IEnumerable<OrganizationMember> src,
        Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationMember MapTo(OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            MembershipType = src.MembershipType,
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

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity> src) =>
        src.Select(MapTo);

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
