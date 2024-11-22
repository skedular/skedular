using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Microsoft.Graph.Models;
using Organization.Shared.Models;
using AzureTenant = Organization.Shared.Database.Entities.AzureTenant;
using AzureTenantMember = Organization.Shared.Database.Entities.AzureTenantMember;
using Event = Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event;
using Identity = Organization.Shared.Models.Identity;
using Location = Organization.Shared.Models.Location;
using Team = Organization.Shared.Models.Team;
using Booking = Organization.Shared.Models.Booking;
using Customer = Organization.Shared.Models.Customer;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;
using Tag = Organization.Shared.Database.Entities.Tag;

namespace Organization.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Shared.Models.Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src);
    Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src);
    Booking MapTo(Api.Shared.Clients.Events.UnityHub.Booking.V1.Value.Event src);

    Shared.Database.Entities.Customer MapToEntity(
        Customer src,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    IEnumerable<Shared.Database.Entities.Identity> MapToEntity(
        IEnumerable<Identity> src,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Organization MapToEntity(Shared.Models.Organization src);

    Shared.Database.Entities.Organization MergeToEntity(Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationOffering MapToEntity(
        Shared.Models.OrganizationOffering src,
        Shared.Database.Entities.Organization organization);

    OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Location MapToEntity(Location src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Booking MapToEntity(Booking src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Organization organization);

    Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src);
    IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src);
    Admin_AddIdentityInput MapTo(AzureTenantMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(AzureTenantMember src, string customerId);

    Admin_AddInput MapTo(
        AzureTenantMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations);

    Shared.Models.AzureTenantMember MapTo(User src);

    AzureTenantMember MapTo(
        Shared.Models.AzureTenantMember src,
        AzureTenant azureTenant);

    AzureTenantMember MergeToEntity(
        Shared.Models.AzureTenantMember src,
        AzureTenantMember dest,
        AzureTenant azureTenant);

    Tag MapToEntity(Shared.Models.Tag src, Shared.Database.Entities.Organization location);

    Tag MergeToEntity(
        Shared.Models.Tag src,
        Tag dest,
        Shared.Database.Entities.Organization location);
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
                    new Identity
                    {
                        Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified
                    })
                .ToList()
        };
    }

    public Shared.Models.Organization MapTo(Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.OrganizationAfterState;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();

        var organization = new Shared.Models.Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            Name = organizationAfterState.Name,
            About = organizationAfterState.About,
            Website = organizationAfterState.Website,
            LogoUrl = organizationAfterState.LogoUrl
        };
        
        organization.Tags = organizationAfterState.Tags.Select(item => new Shared.Models.Tag
        {
            Id = item.Id,
            DeletedAt = deletedAt,
            Name = item.Name,
            Description = item.Description,
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
                IsOrganizationOnboardingDone = true,
                Customer = new Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        organization.OrganizationOfferings =
        [
            new Shared.Models.OrganizationOffering
            {
                Id = organizationAfterState.Offering.Id,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                AutoRenew = organizationAfterState.Offering.AutoRenew,
                UnitPrice = organizationAfterState.Offering.UnitPrice,
                Organization = organization
            }
        ];

        return organization;
    }

    public Location MapTo(Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event src)
    {
        var location = src.Data.LocationAfterState;
        var deletedAt = location.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Location
        {
            Id = location.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Shared.Models.Organization { Id = location.OrganizationId }
        };
    }

    public Team MapTo(Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event src)
    {
        var team = src.Data.TeamAfterState;
        var deletedAt = team.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Team
        {
            Id = team.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Shared.Models.Organization { Id = team.OrganizationId }
        };
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
            Organization = new Shared.Models.Organization { Id = booking.OrganizationId }
        };
    }

    public Shared.Database.Entities.Customer MapToEntity(Customer src,
        ICollection<Shared.Database.Entities.Identity> identities) =>
        MergeToEntity(src, new Shared.Database.Entities.Customer(), identities);

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities)
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

    public IEnumerable<Shared.Database.Entities.Identity>
        MapToEntity(IEnumerable<Identity> src, Shared.Database.Entities.Customer? customer) =>
        src.Select(identity => MapToEntity(identity, customer));

    public Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Shared.Database.Entities.Identity(), customer);

    public Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
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

    public Shared.Database.Entities.Organization MapToEntity(Shared.Models.Organization src) =>
        MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Website = src.Website;
        dest.LogoUrl = src.LogoUrl;
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
        dest.MembershipType = src.MembershipType;
        dest.IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public OrganizationOffering MapToEntity(
        Shared.Models.OrganizationOffering src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationOffering(), organization);

    public OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.Code = src.Code;
        dest.Start = src.Start;
        dest.End = src.End;
        dest.AutoRenew = src.AutoRenew;
        dest.UnitPrice = src.UnitPrice;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.Location MapToEntity(Location src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Location(), organization);

    public Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.Team MapToEntity(Team src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Team(), organization);

    public Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.Booking
        MapToEntity(Booking src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.Booking(), organization);

    public Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.From = src.From;
        dest.To = src.To;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src)
    {
        var organization = new Shared.Models.Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            PaymentMethodEventRaisedAt = src.PaymentMethodEventRaisedAt,
            DailyMemberCountLastRecordedAt = src.DailyMemberCountLastRecordedAt,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories).ToList()
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.Bookings = MapTo(src.Bookings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.Locations = MapTo(src.Locations, organization).ToList();
        organization.Teams = MapTo(src.Teams, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();

        return organization;
    }

    public IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src) =>
        src.Select(MapTo);

    public Admin_AddIdentityInput MapTo(AzureTenantMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(AzureTenantMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_AddInput MapTo(
        AzureTenantMember src,
        string customerId,
        Shared.Database.Entities.Organization defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations)
    {
        var input = new Admin_AddInput
        {
            Id = customerId,
            Designation = src.Designation.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            IsOrganizationOnboardingDone = true,
            IsLocationOnboardingDone = true,
            IsDefaultOrganizationOnboardingDone = true,
            IsDefaultLocationOnboardingDone = true,
            IsPreferredZoneOnboardingDone = false,
            IsPreferredDeskOnboardingDone = false,
            DefaultOrganization =
                new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization { Id = defaultOrganization.Id }
        };

        input.Identities.Add(
            new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Identity
            {
                Id = src.Id, Email = src.Email, EmailVerified = true
            });

        input.DefaultLocations.AddRange(defaultLocations.Select(item =>
            new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Location
            {
                Id = item.Id,
                Organization =
                    new Api.Shared.Services.Grpc.UnityHub.Customer.V1.Organization { Id = defaultOrganization.Id }
            }));

        return input;
    }

    public Shared.Models.AzureTenantMember MapTo(User src) =>
        new()
        {
            Id = src.Id!,
            Email = src.Mail,
            Designation = src.JobTitle,
            Name = src.DisplayName,
            GivenName = src.GivenName,
            FamilyName = src.Surname,
            PreferredLanguage = src.PreferredLanguage
        };

    public AzureTenantMember MapTo(Shared.Models.AzureTenantMember src, AzureTenant azureTenant) =>
        MergeToEntity(src, new AzureTenantMember(), azureTenant);

    public AzureTenantMember MergeToEntity(
        Shared.Models.AzureTenantMember src,
        AzureTenantMember dest,
        AzureTenant azureTenant)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.Designation = src.Designation;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.FamilyName = src.FamilyName;
        dest.PreferredLanguage = src.PreferredLanguage;
        dest.PhotoUrl = src.PhotoUrl;
        dest.PhotoUrl48 = src.PhotoUrl48;
        dest.PhotoUrl64 = src.PhotoUrl64;
        dest.PhotoUrl96 = src.PhotoUrl96;
        dest.PhotoUrl120 = src.PhotoUrl120;
        dest.PhotoUrl240 = src.PhotoUrl240;
        dest.PhotoUrl360 = src.PhotoUrl360;
        dest.PhotoUrl432 = src.PhotoUrl432;
        dest.PhotoUrl504 = src.PhotoUrl504;
        dest.PhotoUrl648 = src.PhotoUrl648;
        dest.AzureTenant = azureTenant;
        return dest;
    }

    public Tag MapToEntity(Shared.Models.Tag src, Shared.Database.Entities.Organization location) =>
        MergeToEntity(src, new Tag(), location);

    public Tag MergeToEntity(
        Shared.Models.Tag src,
        Tag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Type = src.Type;
        dest.Organization = organization;
        return dest;
    }

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status,
            Organization = MapTo(src.Organization),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(IEnumerable<OrganizationMember> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationMember
        MapTo(OrganizationMember src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            MembershipType = src.MembershipType,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
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

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Shared.Database.Entities.Identity src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Email = src.Email,
            EmailVerified = src.EmailVerified
        };

    private static TermsOfUse? MapTo(Shared.Database.Entities.TermsOfUse? src) =>
        src is null
            ? null
            : new TermsOfUse
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Active = src.Active,
                Terms = src.Terms
            };

    private static IEnumerable<Shared.Models.OrganizationOffering> MapTo(
        IEnumerable<OrganizationOffering> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationOffering MapTo(
        OrganizationOffering src,
        Shared.Models.Organization organization)
    {
        var organizationOffering = new Shared.Models.OrganizationOffering
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Code = src.Code,
            Start = src.Start,
            End = src.End,
            AutoRenew = src.AutoRenew,
            UnitPrice = src.UnitPrice,
            Organization = organization
        };

        organizationOffering.OrganizationOfferingActiveMembers = src.OrganizationOfferingActiveMembers
            .Select(item => new OrganizationOfferingActiveMember
            {
                Id = item.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                OrganizationMember = MapTo(item.OrganizationMember, organization),
                OrganizationOffering = organizationOffering
            }).ToList();

        return organizationOffering;
    }

    private static IEnumerable<Booking> MapTo(IEnumerable<Shared.Database.Entities.Booking> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Booking MapTo(Shared.Database.Entities.Booking src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            From = src.From,
            To = src.To,
            Organization = organization
        };

    private static IEnumerable<DailyMemberCountRecording> MapTo(
        IEnumerable<Shared.Database.Entities.DailyMemberCountRecording> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static DailyMemberCountRecording MapTo(
        Shared.Database.Entities.DailyMemberCountRecording src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Organization = organization,
            Date = src.Date,
            Count = src.Count
        };

    private static IEnumerable<IndustrySubCategory>
        MapTo(IEnumerable<Shared.Database.Entities.IndustrySubCategory> src) => src.Select(MapTo)!;

    private static IndustrySubCategory? MapTo(Shared.Database.Entities.IndustrySubCategory? src) =>
        src is null
            ? null
            : new IndustrySubCategory
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Name = src.Name,
                IndustryMainCategory = MapTo(src.IndustryMainCategory)
            };

    private static IndustryMainCategory MapTo(Shared.Database.Entities.IndustryMainCategory src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name
        };

    private static IEnumerable<Location> MapTo(
        IEnumerable<Shared.Database.Entities.Location> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Location MapTo(Shared.Database.Entities.Location src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Team MapTo(Shared.Database.Entities.Team src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<JoinInvitation> MapTo(
        IEnumerable<Shared.Database.Entities.JoinInvitation> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static JoinInvitation MapTo(
        Shared.Database.Entities.JoinInvitation src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status,
            Organization = organization,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };
}
