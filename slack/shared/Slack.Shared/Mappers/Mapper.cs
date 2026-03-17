using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using NetTopologySuite.Geometries;
using Slack.Shared.Models;
using SlackNet;
using Booking = Slack.Shared.Models.Booking;
using Location = Slack.Shared.Models.Location;
using Team = Slack.Shared.Models.Team;
using Organization = Slack.Shared.Models.Organization;
using Customer = Slack.Shared.Models.Customer;
using Identity = Slack.Shared.Models.Identity;
using OrganizationMember = Slack.Shared.Database.Entities.OrganizationMember;
using Workspace = Slack.Shared.Database.Entities.Workspace;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;
using Admin_AddInput = Api.Shared.Services.Grpc.Skedular.Customer.V1.Admin_AddInput;
using BookingCategory = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory;
using BookingChannel = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingChannel;
using CustomerType = Api.Shared.Services.Models.CustomerType;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using LocationType = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationType;
using Models_OrganizationCustomTag = Slack.Shared.Models.OrganizationCustomTag;
using OrganizationBillingCycle = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationBillingCycle;
using OrganizationMemberRole = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus;
using OrganizationTag = Slack.Shared.Models.OrganizationTag;
using OrganizationType = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationType;
using PersonalInformationVisibility = Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility;
using Resource = Slack.Shared.Models.Resource;
using ResourceType = Slack.Shared.Models.ResourceType;
using Role = Api.Shared.Services.Grpc.Skedular.Team.V1.Role;
using TeamMemberStatus = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMemberStatus;
using WorkspaceChannel = Slack.Shared.Database.Entities.WorkspaceChannel;

namespace Slack.Shared.Mappers;

public interface IMapper
{
    Customer? MapTo(Database.Entities.Customer? src);
    Booking MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src);
    Models.Workspace MapTo(Workspace src);
    WorkspaceMember MapToEntity(User src, Workspace workspace);
    WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace);
    WorkspaceChannel MapToEntity(Conversation src, Workspace workspace);
    WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace);
    Workspace MergeToEntity(SlackNet.Team src, Workspace dest);
    Admin_AddIdentityInput MapToAddIdentityInput(WorkspaceMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId);
    Admin_AddInput MapTo(WorkspaceMember src, string customerId, string defaultOrganizationId, ICollection<string> preferredLocationIds);
    Customer? MapTo(Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer? src);
    Organization MapTo(Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization src);
    Location MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Location src);
    Team MapTo(Api.Shared.Services.Grpc.Skedular.Team.V1.Team src);
    OrganizationPermissions MapTo(Permissions src);
    LocationPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions src);
    TeamPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Team.V1.Permissions src);
    Models.OrganizationMember MapTo(Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMember src);
    OrganizationZone MapTo(Zone src);
    Models_OrganizationCustomTag MapTo(CustomTag src);
    OrganizationBillingDetails MapTo(BillingDetails src);
    Resource MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Resource src);
    OrganizationProductTag MapTo(ProductTag src);
    OrganizationTag MapTo(Tag src);
    TeamBookingPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.TeamPermissions src);
    OrganizationBookingPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationPermissions src);
}

public class Mapper : IMapper
{
    public Customer? MapTo(Database.Entities.Customer? src)
    {
        if (src is null)
        {
            return null;
        }

        var customer = new Customer
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Timezone = src.Timezone,
            Type = src.Type.ToNullableCustomerType()
        };

        customer.Identities = MapTo(src.Identities, customer).ToList();

        return customer;
    }

    public Models.Workspace MapTo(Workspace src)
    {
        var workspace = new Models.Workspace
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Name = src.Name,
            Domain = src.Domain,
            EmailDomain = src.EmailDomain,
            EnterpriseId = src.EnterpriseId,
            EnterpriseName = src.EnterpriseName,
            BotUserId = src.BotUserId,
            BotUserScope = src.BotUserScope,
            BotUserAccessToken = src.BotUserAccessToken,
            BotRefreshToken = src.BotRefreshToken,
            AuthedUserId = src.AuthedUserId,
            AuthedUserScope = src.AuthedUserScope,
            AuthedUserAccessToken = src.AuthedUserAccessToken,
            AuthedRefreshToken = src.AuthedRefreshToken,
            Organization = MapTo(src.Organization)
        };

        workspace.WorkspaceMembers = MapTo(src.WorkspaceMembers, workspace).ToList();

        return workspace;
    }

    public Booking MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.To.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Category = src.Category switch
            {
                BookingCategory.WorkingFromHome => Api.Shared.Services.Models.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => Api.Shared.Services.Models.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => Api.Shared.Services.Models.BookingCategory.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => Api.Shared.Services.Models.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => Api.Shared.Services.Models.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => Api.Shared.Services.Models.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => Api.Shared.Services.Models.BookingCategory.ClientOffice,
                BookingCategory.Vacation => Api.Shared.Services.Models.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => Api.Shared.Services.Models.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => Api.Shared.Services.Models.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Channel = src.Channel switch
            {
                BookingChannel.Private => Api.Shared.Services.Models.BookingChannel.Private,
                BookingChannel.Marketplace => Api.Shared.Services.Models.BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            Resources = src.Resources.Select(item => new Resource { Id = item.Id }).ToList(),
            InvolvedCustomers = src.InvolvedCustomerIds.Select(item => new Customer { Id = item }).ToList(),
            InvolvedOrganizations = src.InvolvedOrganizationIds.Select(item => new Organization { Id = item }).ToList(),
            InvolvedLocations = src.InvolvedLocationIds.Select(item => new Location { Id = item }).ToList(),
            InvolvedTeams = src.InvolvedTeamIds.Select(item => new Team { Id = item }).ToList()
        };

    public WorkspaceMember MapToEntity(User src, Workspace workspace) => MergeToEntity(src, new WorkspaceMember(), workspace);

    public WorkspaceMember MergeToEntity(User src, WorkspaceMember dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Email = src.Profile.Email.ToSafeString();
        dest.Designation = src.Profile.Title.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxPersonDesignationLength);
        dest.Name = src.Profile.RealName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxPersonNameLength);
        dest.GivenName = src.Profile.FirstName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxGivenNameLength);
        dest.FamilyName = src.Profile.LastName.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxFamilyNameLength);
        dest.Timezone = src.Tz.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxTimezoneLength);
        dest.IsAdmin = src.IsAdmin;
        dest.IsOwner = src.IsOwner;
        dest.IsPrimaryOwner = src.IsPrimaryOwner;
        dest.Locale = src.Locale.ToSafeString().Truncate(Api.Shared.Services.Constants.MaxLocaleLength);
        dest.PhotoUrl = src.Profile.ImageOriginal.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl24 = src.Profile.Image24.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl32 = src.Profile.Image32.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl48 = src.Profile.Image48.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl72 = src.Profile.Image72.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl192 = src.Profile.Image192.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.PhotoUrl512 = src.Profile.Image512.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.Workspace = workspace;
        return dest;
    }

    public WorkspaceChannel MapToEntity(Conversation src, Workspace workspace) => MergeToEntity(src, new WorkspaceChannel(), workspace);

    public WorkspaceChannel MergeToEntity(Conversation src, WorkspaceChannel dest, Workspace workspace)
    {
        dest.Id = src.Id;
        dest.Name = src.Name.Truncate(Api.Shared.Services.Constants.MaxUrlLength);
        dest.Topic = src.Topic.Value;
        dest.Purpose = src.Purpose.Value;
        dest.IsPrivate = src.IsPrivate;
        dest.IsGeneral = src.IsGeneral;
        dest.IsGroup = src.IsGroup;
        dest.IsShared = src.IsShared;
        dest.IsMember = src.IsMember;
        dest.Workspace = workspace;
        return dest;
    }

    public Workspace MergeToEntity(SlackNet.Team src, Workspace dest)
    {
        dest.Name = src.Name;
        dest.Domain = src.Domain;
        dest.EmailDomain = src.EmailDomain;
        dest.EnterpriseId = src.EnterpriseId;
        dest.EnterpriseName = src.EnterpriseName;
        return dest;
    }

    public Admin_AddIdentityInput MapToAddIdentityInput(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_AddInput MapTo(WorkspaceMember src, string customerId, string defaultOrganizationId, ICollection<string> preferredLocationIds)
    {
        var input = new Admin_AddInput
        {
            Id = customerId,
            Designation = src.Designation.ToSafeString(),
            Name = src.Name.ToSafeString(),
            GivenName = src.GivenName.ToSafeString(),
            FamilyName = src.FamilyName.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            PhotoUrl = src.PhotoUrl.ToSafeString(),
            PhotoUrl24 = src.PhotoUrl24.ToSafeString(),
            PhotoUrl32 = src.PhotoUrl32.ToSafeString(),
            PhotoUrl48 = src.PhotoUrl48.ToSafeString(),
            PhotoUrl72 = src.PhotoUrl72.ToSafeString(),
            PhotoUrl192 = src.PhotoUrl192.ToSafeString(),
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            IsOnboardingDone = true,
            DefaultOrganizationId = defaultOrganizationId.ToSafeString(),
            PersonalInformationVisibility = PersonalInformationVisibility.Visible,
            Type = Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerType.Registered
        };

        input.Identities.Add(new Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity { Id = src.Id, Email = src.Email, EmailVerified = true });

        input.PreferredLocations.AddRange(preferredLocationIds.Select(item =>
            new Api.Shared.Services.Grpc.Skedular.Customer.V1.Location
            {
                Id = item, Organization = new Api.Shared.Services.Grpc.Skedular.Customer.V1.Organization { Id = defaultOrganizationId }
            }));

        return input;
    }

    public Organization MapTo(Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization src) =>
        new()
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name.ToSafeString(),
            ListingMetadata = MapTo(src.ListingMetadata),
            MarketplaceListingMetadata = MapTo(src.MarketplaceListingMetadata),
            Website = src.Website.ToSafeString(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl.ToSafeString(),
            Type = src.Type switch
            {
                OrganizationType.Private => Api.Shared.Services.Models.OrganizationType.Private,
                OrganizationType.Marketplace => Api.Shared.Services.Models.OrganizationType.Marketplace,
                OrganizationType.Individual => Api.Shared.Services.Models.OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            },
            BillingCycle = src.BillingCycle switch
            {
                OrganizationBillingCycle.Weekly => Api.Shared.Services.Models.OrganizationBillingCycle.Weekly,
                OrganizationBillingCycle.Fortnightly => Api.Shared.Services.Models.OrganizationBillingCycle.Fortnightly,
                OrganizationBillingCycle.Monthly => Api.Shared.Services.Models.OrganizationBillingCycle.Monthly,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOwnershipVerified = src.IsOwnershipVerified,
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            HasFutureBooking = src.HasFutureBooking,
            Tags = MapToOrganizationCustomTag(src.Tags).ToList(),
            ResourceTypes = MapTo(src.ResourceTypes).ToList()
        };

    public Location MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Location src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone.ToSafeString(),
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Organization { Id = src.OrganizationId },
            Type = src.Type switch
            {
                LocationType.Private => Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            Resources = MapTo(src.Resources).ToList()
        };

    public Team MapTo(Api.Shared.Services.Grpc.Skedular.Team.V1.Team src)
    {
        var team = new Team
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Organization = string.IsNullOrWhiteSpace(src.OrganizationId) ? null : new Organization { Id = src.OrganizationId },
            PrimaryLocation = string.IsNullOrWhiteSpace(src.PrimaryLocationId) ? null : new Location { Id = src.PrimaryLocationId },
            Permissions = new TeamPermissions
            {
                CanView = src.Permissions.CanView,
                CanModify = src.Permissions.CanModify,
                CanDelete = src.Permissions.CanDelete,
                CanInvitePeople = src.Permissions.CanInvitePeople,
                CanCancelPeopleExistingInvitations = src.Permissions.CanCancelPeopleExistingInvitations
            }
        };

        team.TeamMembers = MapTo(src.Members, team).ToList();

        return team;
    }

    public Customer? MapTo(Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer? src) =>
        src is null
            ? null
            : new Customer
            {
                Id = src.Id,
                DisplayableName = src.DisplayableName.ToSafeString(),
                Designation = src.Designation.ToSafeString(),
                Title = src.Title.ToSafeString(),
                Timezone = src.Timezone.ToSafeString(),
                Locale = src.Locale.ToSafeString(),
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
                IsOnboardingDone = src.IsOnboardingDone,
                Identities = MapTo(src.Identities).ToList(),
                DefaultOrganization =
                    string.IsNullOrWhiteSpace(src.DefaultOrganizationId)
                        ? null
                        : new Organization { Id = src.DefaultOrganizationId.ToSafeString() },
                PreferredLocations = src.PreferredLocationIds.Select(item => new Location { Id = item }).ToList(),
                PreferredResources = src.PreferredResourceIds.Select(item => new Resource { Id = item }).ToList(),
                PreferredOrganizationTags = src.PreferredOrganizationTagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
                Type = src.Type switch
                {
                    Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerType.Guest => CustomerType.Guest,
                    Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerType.Registered => CustomerType.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

    public OrganizationPermissions MapTo(Permissions src) =>
        new()
        {
            CanView = src.CanView,
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations,
            CanViewAnalytics = src.CanViewAnalytics
        };

    public LocationPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Permissions src) =>
        new() { CanView = src.CanView, CanModify = src.CanModify, CanDelete = src.CanDelete, CanViewAnalytics = src.CanViewAnalytics };

    public TeamPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Team.V1.Permissions src) =>
        new()
        {
            CanView = src.CanView,
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations
        };

    public Models.OrganizationMember MapTo(Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                OrganizationMemberRole.Owner => Api.Shared.Services.Models.OrganizationMemberRole.Owner,
                OrganizationMemberRole.Administrator => Api.Shared.Services.Models.OrganizationMemberRole.Administrator,
                OrganizationMemberRole.Member => Api.Shared.Services.Models.OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                OrganizationMemberStatus.Active => Api.Shared.Services.Models.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => Api.Shared.Services.Models.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId.ToSafeString() }
        };

    public OrganizationZone MapTo(Zone src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    public Models_OrganizationCustomTag MapTo(CustomTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    public OrganizationProductTag MapTo(ProductTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    public OrganizationTag MapTo(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
            Type = src.TagType.ToSafeString().ToOrganizationTagType()
        };

    public TeamBookingPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.TeamPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking
        };

    public OrganizationBookingPermissions MapTo(Api.Shared.Services.Grpc.Skedular.Booking.V1.OrganizationPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking
        };

    public OrganizationBillingDetails MapTo(BillingDetails src) =>
        new()
        {
            Id = src.Id,
            CompanyName = src.CompanyName,
            Email = src.Email,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Coordinates is null ? null : new Point(new Coordinate(src.Coordinates.Longitude, src.Coordinates.Latitude)),
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode,
            FormattedAddress = src.FormattedAddress
        };

    public Resource MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            ResourceType = new ResourceType { Id = src.ResourceTypeId },
            CustomTags = src.CustomTagIds.Select(item => new Models_OrganizationCustomTag { Id = item }).ToList(),
            Zones = src.ZoneIds.Select(item => new OrganizationZone { Id = item }).ToList(),
            ProductTags = src.ProductTagIds.Select(item => new OrganizationProductTag { Id = item }).ToList()
        };

    private static IEnumerable<Models_OrganizationCustomTag> MapToOrganizationCustomTag(IEnumerable<Tag> src) =>
        src.Select(MapToOrganizationCustomTag);

    private static Models_OrganizationCustomTag MapToOrganizationCustomTag(Tag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<OrganizationResourceType> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Organization.V1.ResourceType> src) =>
        src.Select(MapToResourceType);

    private static OrganizationResourceType MapToResourceType(Api.Shared.Services.Grpc.Skedular.Organization.V1.ResourceType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private Organization MapTo(Database.Entities.Organization src)
    {
        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            CustomDomain = src.CustomDomain,
            Type = src.Type.ToOrganizationType(),
            IsOwnershipVerified = src.IsOwnershipVerified,
            SlackChannelDailyUpdateLastSentAt = src.SlackChannelDailyUpdateLastSentAt
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();

        return organization;
    }

    private static IEnumerable<Models.WorkspaceMember> MapTo(IEnumerable<WorkspaceMember> src, Models.Workspace workspace) =>
        src.Select(item => MapTo(item, workspace));

    private static Models.WorkspaceMember MapTo(WorkspaceMember src, Models.Workspace workspace) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Email = src.Email,
            Designation = src.Designation,
            Name = src.Name,
            GivenName = src.GivenName,
            FamilyName = src.FamilyName,
            Timezone = src.Timezone,
            IsAdmin = src.IsAdmin,
            IsOwner = src.IsOwner,
            IsPrimaryOwner = src.IsPrimaryOwner,
            Locale = src.Locale,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl24 = src.PhotoUrl24,
            PhotoUrl32 = src.PhotoUrl32,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl72 = src.PhotoUrl72,
            PhotoUrl192 = src.PhotoUrl192,
            PhotoUrl512 = src.PhotoUrl512,
            LastProfileStatusUpdatedAt = src.LastProfileStatusUpdatedAt,
            AutomaticallyUpdateProfileStatus = src.AutomaticallyUpdateProfileStatus,
            Workspace = workspace
        };

    private IEnumerable<Models.OrganizationMember> MapTo(IEnumerable<OrganizationMember> src, Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private Models.OrganizationMember MapTo(OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization,
            Customer = MapTo(src.Customer)!
        };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src, Customer customer) =>
        src.Select(item => MapTo(item, customer));

    private static Identity MapTo(Database.Entities.Identity src, Customer customer) =>
        new() { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, Customer = customer };

    private static IEnumerable<Identity> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private IEnumerable<Resource> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Location.V1.Resource> src) =>
        src.Select(MapTo);

    private static IEnumerable<TeamMember> MapTo(IEnumerable<Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember> src, Team team) =>
        src.Select(item => MapTo(item, team));

    private static TeamMember MapTo(Api.Shared.Services.Grpc.Skedular.Team.V1.TeamMember src, Team team) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => TeamMemberRole.Owner,
                Role.Administrator => TeamMemberRole.Administrator,
                Role.Member => TeamMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                TeamMemberStatus.Active => Api.Shared.Services.Models.TeamMemberStatus.Active,
                TeamMemberStatus.Inactive => Api.Shared.Services.Models.TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Customer { Id = src.CustomerId },
            OrganizationMember = src.OrganizationMember is null || string.IsNullOrWhiteSpace(src.OrganizationMember.Id)
                ? null
                : new Models.OrganizationMember
                {
                    Id = src.OrganizationMember.Id, Customer = new Customer { Id = src.OrganizationMember.CustomerId }
                },
            Team = team
        };

    private static ListingMetadata MapTo(Api.Shared.Services.Grpc.Skedular.Organization.V1.ListingMetadata src) =>
        new(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(), src.IncludedFeatures);

    private static ListingMetadata MapTo(Api.Shared.Services.Grpc.Skedular.Location.V1.ListingMetadata src) =>
        new(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(), src.IncludedFeatures);
}
