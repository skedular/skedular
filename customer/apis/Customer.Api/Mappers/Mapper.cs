using Api.Shared.Models;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Customer.Api.GraphQL;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Models;
using CustomerFeedback = Customer.Shared.Models.CustomerFeedback;
using Desk = Customer.Shared.Models.Desk;
using FeedbackChannel = Customer.Api.GraphQL.FeedbackChannel;
using Identity = Customer.Shared.Database.Entities.Identity;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using Team = Customer.Shared.Models.Team;

namespace Customer.Api.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo();
    Identity MapToIdentity();
    CustomerDetails MapTo(Shared.Models.Customer src);
    CustomerPayload MapTo(Shared.Models.Customer src, string? clientMutationId);
    CustomerFeedback MapTo(SubmitCustomerFeedbackInput src);
    SubmitCustomerFeedbackPayload MapTo(CustomerFeedback src, string? clientMutationId);
    Shared.Models.Customer MapTo(Shared.Database.Entities.Customer src);

    Shared.Database.Entities.Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> defaultLocations,
        ICollection<Shared.Database.Entities.Team> defaultTeams,
        ICollection<Shared.Database.Entities.Desk> preferredDesks,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags);

    IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src);
    CustomerFeedback MapTo(Shared.Database.Entities.CustomerFeedback src);
    Shared.Database.Entities.CustomerFeedback MapTo(CustomerFeedback src, Shared.Database.Entities.Customer customer);
    Shared.Models.Customer MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer MapToGrpcResponse(Shared.Models.Customer src);
    Identity MapTo(Shared.Models.Identity src, Shared.Database.Entities.Customer customer);
    Identity MergeTo(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer customer);
    Shared.Models.Identity MapTo(Admin_AddIdentityInput src);
    Shared.Models.Identity MapTo(Admin_UpdateIdentityInput src);
    Edge<Shared.Models.Customer> MapTo(Edge<Shared.Database.Entities.Customer> src);
    CustomerEdge MapTo(Edge<Shared.Models.Customer> src);
}

public class Mapper(IContext context) : IMapper
{
    public Shared.Models.Customer MapTo() =>
        new()
        {
            Designation = context.GetDesignation(),
            Title = context.GetTitle(),
            Name = context.GetName(),
            GivenName = context.GetGivenName(),
            MiddleName = context.GetMiddleName(),
            FamilyName = context.GetFamilyName(),
            PhotoUrl = context.GetPhotoUrl(),
            PhotoUrl24 = context.GetPhotoUrl24(),
            PhotoUrl32 = context.GetPhotoUrl32(),
            PhotoUrl48 = context.GetPhotoUrl48(),
            PhotoUrl72 = context.GetPhotoUrl72(),
            PhotoUrl192 = context.GetPhotoUrl192(),
            PhotoUrl512 = context.GetPhotoUrl512(),
            Timezone = context.GetTimezone(),
            Locale = context.GetLocale(),
            PhoneNumber = null,
            Identities =
                new List<Shared.Models.Identity>
                {
                    new()
                    {
                        Id = context.GetVerifiableToken().ToSafeString(),
                        Email = context.GetEmail(),
                        EmailVerified = context.GetEmailVerified()
                    }
                },
            IsOrganizationOnboardingDone = false,
            IsLocationOnboardingDone = false,
            IsTeamOnboardingDone = false,
            IsDefaultOrganizationOnboardingDone = false,
            IsDefaultLocationOnboardingDone = false,
            IsPreferredZoneOnboardingDone = false,
            IsPreferredDeskOnboardingDone = false,
            DefaultOrganization = null,
            DefaultLocations = [],
            DefaultTeams = [],
            PreferredOrganizationTags = [],
            PreferredDesks = []
        };

    public Identity MapToIdentity() =>
        new()
        {
            Id = context.GetVerifiableToken().ToSafeString(),
            Email = context.GetEmail(),
            EmailVerified = context.GetEmailVerified()
        };

    public CustomerDetails MapTo(Shared.Models.Customer src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            Designation = src.Designation,
            Title = src.Title,
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
            Timezone = src.Timezone,
            Locale = src.Locale,
            PhoneNumber = src.PhoneNumber,
            Email = src.Identities
                .Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
                .Select(item => item.Email!.ToLowerInvariant())
                .Distinct()
                .FirstOrDefault(),
            Identities = MapTo(src.Identities).ToArray(),
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone ?? false,
            IsTeamOnboardingDone = src.IsTeamOnboardingDone ?? false,
            IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone ?? false,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone ?? false,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone ?? false,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone ?? false,
            DefaultOrganization = src.DefaultOrganization is null
                ? null
                : new CustomerOrganizationDetails
                {
                    UniqueId = src.DefaultOrganization.Id,
                    Name = src.DefaultOrganization.Name,
                    LogoUrl = src.DefaultOrganization.LogoUrl
                },
            DefaultLocations = src.DefaultLocations.Select(item => new CustomerLocationDetails
            {
                UniqueId = item.Id,
                Name = item.Name,
                Organization =
                    item.Organization is null
                        ? null
                        : new CustomerOrganizationDetails
                        {
                            UniqueId = item.Organization.Id,
                            Name = item.Organization.Name,
                            LogoUrl = item.Organization.LogoUrl
                        }
            }).ToArray(),
            PreferredZones =
                src.PreferredOrganizationTags
                    .Where(item => item.Type == OrganizationTagType.Zone)
                    .Select(item => new CustomerOrganizationTagDetails { UniqueId = item.Id, Name = item.Name })
                    .ToArray(),
            PreferredDeskTypes =
                src.PreferredOrganizationTags
                    .Where(item => item.Type == OrganizationTagType.DeskType)
                    .Select(item => new CustomerOrganizationTagDetails { UniqueId = item.Id, Name = item.Name })
                    .ToArray(),
            PreferredDesks =
                src.PreferredDesks
                    .Select(item => new CustomerDeskDetails { UniqueId = item.Id, Name = item.Name })
                    .ToArray(),
            DefaultTeams = src.DefaultTeams.Select(item => new CustomerTeamDetails
            {
                UniqueId = item.Id,
                Name = item.Name,
                Organization =
                    item.Organization is null
                        ? null
                        : new CustomerOrganizationDetails
                        {
                            UniqueId = item.Organization.Id,
                            Name = item.Organization.Name,
                            LogoUrl = item.Organization.LogoUrl
                        }
            }).ToArray()
        };

    public CustomerPayload MapTo(Shared.Models.Customer src, string? clientMutationId) => new()
    {
        ClientMutationId = clientMutationId, Customer = MapTo(src)
    };

    public CustomerFeedback MapTo(SubmitCustomerFeedbackInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Content = src.FeedbackContent,
            Channel = src.Channel switch
            {
                FeedbackChannel.Web => FeedbackChannelType.Web,
                FeedbackChannel.Slack => FeedbackChannelType.Slack,
                FeedbackChannel.MsTeams => FeedbackChannelType.MsTeams,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

    public SubmitCustomerFeedbackPayload MapTo(CustomerFeedback src, string? clientMutationId) =>
        new() { Id = src.Id, ClientMutationId = clientMutationId };

    public Shared.Models.Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Title = src.Title,
            Designation = src.Designation,
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
            Timezone = src.Timezone,
            Locale = src.Locale,
            PhoneNumber = src.PhoneNumber,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone,
            IsTeamOnboardingDone = src.IsTeamOnboardingDone,
            IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone,
            Identities = MapTo(src.Identities).ToList(),
            DefaultOrganization = MapTo(src.DefaultOrganization),
            DefaultLocations = MapTo(src.DefaultLocations).ToList(),
            PreferredDesks = MapTo(src.PreferredDesks).ToList(),
            DefaultTeams = MapTo(src.DefaultTeams).ToList(),
            PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList()
        };

    public CustomerFeedback MapTo(Shared.Database.Entities.CustomerFeedback src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Content = src.Content,
            Channel = src.Channel,
            Customer = MapTo(src.Customer)
        };

    public Shared.Database.Entities.CustomerFeedback MapTo(CustomerFeedback src,
        Shared.Database.Entities.Customer customer) =>
        new() { Id = src.Id, Content = src.Content, Channel = src.Channel, Customer = customer };

    public Shared.Models.Customer MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Designation = src.Designation,
            Title = src.Title,
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
            Timezone = src.Timezone,
            Locale = src.Locale,
            PhoneNumber = src.PhoneNumber,
            Identities =
                src.Identities.Select(item =>
                        new Shared.Models.Identity
                        {
                            Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified
                        })
                    .ToList(),
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone,
            IsTeamOnboardingDone = src.IsTeamOnboardingDone,
            IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone,
            DefaultOrganization =
                string.IsNullOrWhiteSpace(src.DefaultOrganization?.Id)
                    ? null
                    : new Organization { Id = src.DefaultOrganization.Id },
            DefaultLocations = src.DefaultLocations.Select(item =>
                    new Location
                    {
                        Id = item.Id,
                        Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                            ? null
                            : new Organization { Id = item.Organization.Id }
                    })
                .ToList(),
            DefaultTeams = src.DefaultTeams.Select(item =>
                    new Team
                    {
                        Id = item.Id,
                        Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                            ? null
                            : new Organization { Id = item.Organization.Id }
                    })
                .ToList(),
            PreferredDesks = src.PreferredDesks.Select(item =>
                    new Desk { Id = item.Id, Location = new Location { Id = item.Location.Id } })
                .ToList(),
            PreferredOrganizationTags = src.PreferredOrganizationTags.Select(item =>
                    new OrganizationTag { Id = item.Id, Organization = new Organization { Id = item.Organization.Id } })
                .ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer MapToGrpcResponse(Shared.Models.Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer
        {
            Id = src.Id,
            Designation = src.Designation.ToSafeString(),
            Title = src.Title.ToSafeString(),
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
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Locale = src.Locale.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString(),
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone ?? false,
            IsTeamOnboardingDone = src.IsTeamOnboardingDone ?? false,
            IsDefaultOrganizationOnboardingDone = src.IsDefaultOrganizationOnboardingDone ?? false,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone ?? false,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone ?? false,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone ?? false,
            DefaultOrganization =
                string.IsNullOrWhiteSpace(src.DefaultOrganization?.Id)
                    ? new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization()
                    : new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization
                    {
                        Id = src.DefaultOrganization.Id, Name = src.DefaultOrganization.Name.ToSafeString()
                    }
        };

        customer.Identities.AddRange(src.Identities.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity
            {
                Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified ?? false
            }));
        customer.DefaultLocations.AddRange(src.DefaultLocations.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Location
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                    ? new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization()
                    : new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization
                    {
                        Id = item.Organization.Id
                    }
            }));
        customer.DefaultTeams.AddRange(src.DefaultTeams.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Team
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Organization = string.IsNullOrWhiteSpace(item.Organization?.Id)
                    ? new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization()
                    : new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization
                    {
                        Id = item.Organization.Id
                    }
            }));
        customer.PreferredDesks.AddRange(src.PreferredDesks.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Desk
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Location = new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Location
                {
                    Id = item.Location.Id
                }
            }));
        customer.PreferredOrganizationTags.AddRange(src.PreferredOrganizationTags.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.OrganizationTag
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Type = item.Type.ToSafeString(),
                Organization = new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization
                {
                    Id = item.Organization.Id
                }
            }));
        return customer;
    }

    public Identity MapTo(Shared.Models.Identity src, Shared.Database.Entities.Customer customer) =>
        MergeTo(src, new Identity(), customer);

    public Identity MergeTo(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.EmailVerified = src.EmailVerified;
        dest.Customer = customer;
        return dest;
    }

    public Shared.Models.Identity MapTo(Admin_AddIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Customer = new Shared.Models.Customer { Id = src.CustomerId }
        };

    public Shared.Models.Identity MapTo(Admin_UpdateIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Customer = new Shared.Models.Customer { Id = src.CustomerId }
        };

    public Edge<Shared.Models.Customer> MapTo(Edge<Shared.Database.Entities.Customer> src) =>
        new(src.Cursor, MapTo(src.Node));

    public CustomerEdge MapTo(Edge<Shared.Models.Customer> src) =>
        new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src) => src.Select(MapToEntity);

    public Shared.Database.Entities.Customer
        MapToEntity(Shared.Models.Customer src,
            ICollection<Identity> identities,
            Shared.Database.Entities.Organization? defaultOrganization,
            ICollection<Shared.Database.Entities.Location> defaultLocations,
            ICollection<Shared.Database.Entities.Team> defaultTeams,
            ICollection<Shared.Database.Entities.Desk> preferredDesks,
            ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags) =>
        new()
        {
            Id = src.Id,
            Designation = src.Designation,
            Title = src.Title,
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
            Timezone = src.Timezone,
            Locale = src.Locale,
            PhoneNumber = src.PhoneNumber,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            IsLocationOnboardingDone = src.IsLocationOnboardingDone,
            IsTeamOnboardingDone = src.IsTeamOnboardingDone,
            IsDefaultOrganizationOnboardingDone =
                src.IsDefaultOrganizationOnboardingDone,
            IsDefaultLocationOnboardingDone = src.IsDefaultLocationOnboardingDone,
            IsPreferredZoneOnboardingDone = src.IsPreferredZoneOnboardingDone,
            IsPreferredDeskOnboardingDone = src.IsPreferredDeskOnboardingDone,
            Identities = identities,
            DefaultOrganization = defaultOrganization,
            DefaultLocations = defaultLocations,
            DefaultTeams = defaultTeams,
            PreferredDesks = preferredDesks,
            PreferredOrganizationTags = preferredOrganizationTags
        };

    private static Identity MapToEntity(Shared.Models.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static IEnumerable<Shared.Models.Identity> MapTo(IEnumerable<Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Shared.Models.Identity
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                Email = src.Email,
                EmailVerified = src.EmailVerified
            };

    private static Organization? MapTo(Shared.Database.Entities.Organization? src) =>
        src is null
            ? null
            : new Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                LogoUrl = src.LogoUrl
            };

    private static Location? MapTo(Shared.Database.Entities.Location? src) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Organization = MapTo(src.Organization),
                Desks = MapTo(src.Desks).ToList()
            };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static OrganizationTag? MapTo(Shared.Database.Entities.OrganizationTag? src) =>
        src is null
            ? null
            : new OrganizationTag
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Type = src.Type,
                Organization = new Organization { Id = src.Organization.Id }
            };

    private static IEnumerable<Desk> MapTo(IEnumerable<Shared.Database.Entities.Desk?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Desk? MapTo(Shared.Database.Entities.Desk? src) =>
        src is null
            ? null
            : new Desk
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Location = new Location { Id = src.Location.Id }
            };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Team? MapTo(Shared.Database.Entities.Team? src) =>
        src is null
            ? null
            : new Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                Organization = MapTo(src.Organization)
            };

    private static IEnumerable<CustomerIdentity> MapTo(IEnumerable<Shared.Models.Identity> src) =>
        src.Select(MapTo)!;

    private static CustomerIdentity MapTo(Shared.Models.Identity src) =>
        new() { Id = src.Id, Email = src.Email, Verified = src.EmailVerified ?? false };
}
