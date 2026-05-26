using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Zones.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using NetTopologySuite.Geometries;
using Slack.Shared.Models;
using Admin_AddInput = Api.Shared.Grpc.Skedular.Customer.Admin.V1.Admin_AddInput;
using Booking = Slack.Shared.Models.Booking;
using BookingCategory = Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingCategory;
using BookingChannel = Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingChannel;
using Customer = Slack.Shared.Models.Customer;
using CustomerType = Api.Shared.Services.Models.CustomerType;
using Identity = Slack.Shared.Models.Identity;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using Location = Slack.Shared.Models.Location;
using LocationType = Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType;
using Models_OrganizationCustomTag = Slack.Shared.Models.OrganizationCustomTag;
using Organization = Slack.Shared.Models.Organization;
using OrganizationBillingCycle = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationBillingCycle;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus;
using OrganizationTag = Slack.Shared.Models.OrganizationTag;
using OrganizationType = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationType;
using PersonalInformationVisibility = Api.Shared.Grpc.Skedular.Customer.Core.V1.PersonalInformationVisibility;
using Resource = Slack.Shared.Models.Resource;
using ResourceType = Slack.Shared.Models.ResourceType;
using Role = Api.Shared.Grpc.Skedular.Team.Core.V1.Role;
using Team = Slack.Shared.Models.Team;
using TeamMemberStatus = Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMemberStatus;
using WorkspaceMember = Slack.Shared.Database.Entities.WorkspaceMember;

namespace Slack.Shared.Mappers;

public interface IGrpcMapper
{
    Admin_AddIdentityInput MapToAddIdentityInput(WorkspaceMember src, string customerId);
    Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId);
    Admin_AddInput MapTo(WorkspaceMember src, string customerId, string defaultOrganizationId, IReadOnlyList<string> preferredLocationIds);
    Customer? MapTo(Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer? src);
    Organization MapTo(Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization src);
    Location MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Location src);
    Team MapTo(Api.Shared.Grpc.Skedular.Team.Core.V1.Team src);
    Booking MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking src);
    OrganizationPermissions MapTo(Permissions src);
    LocationPermissions MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Permissions src);
    TeamPermissions MapTo(Api.Shared.Grpc.Skedular.Team.Core.V1.Permissions src);
    OrganizationMember MapTo(Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMember src);
    OrganizationZone MapTo(Zone src);
    Models_OrganizationCustomTag MapTo(CustomTag src);
    OrganizationBillingDetails MapTo(BillingDetails src);
    Resource MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Resource src);
    OrganizationProductTag MapTo(ProductTag src);
    OrganizationTag MapTo(Tag src);
    TeamBookingPermissions MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.TeamPermissions src);
    OrganizationBookingPermissions MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.OrganizationPermissions src);
}

public class GrpcMapper : IGrpcMapper
{
    public Admin_AddIdentityInput MapToAddIdentityInput(WorkspaceMember src, string customerId) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };

    public Admin_UpdateIdentityInput MapToUpdateIdentityInput(WorkspaceMember src, string customerId)
    {
        var input = new Admin_UpdateIdentityInput { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = true, CustomerId = customerId };
        input.FieldsToUpdate.AddRange([IdentityPatchField.Email, IdentityPatchField.EmailVerified]);
        return input;
    }

    public Admin_AddInput MapTo(WorkspaceMember src, string customerId, string defaultOrganizationId, IReadOnlyList<string> preferredLocationIds)
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
            Type = Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Registered
        };

        input.Identities.Add(new Api.Shared.Grpc.Skedular.Customer.Core.V1.Identity { Id = src.Id, Email = src.Email, EmailVerified = true });

        input.PreferredLocations.AddRange(preferredLocationIds.Select(item =>
            new Api.Shared.Grpc.Skedular.Customer.Core.V1.Location
            {
                Id = item, Organization = new Api.Shared.Grpc.Skedular.Customer.Core.V1.Organization { Id = defaultOrganizationId }
            }));

        return input;
    }

    public Customer? MapTo(Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer? src) =>
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
                    Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Guest => CustomerType.Guest,
                    Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerType.Registered => CustomerType.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };

    public Organization MapTo(Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization src) =>
        new()
        {
            Id = src.Id,
            CustomDomain = src.CustomDomain.ToSafeString(),
            Name = src.Name.ToSafeString(),
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

    public Location MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Location src) =>
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

    public Team MapTo(Api.Shared.Grpc.Skedular.Team.Core.V1.Team src)
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

    public Booking MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.Booking src) =>
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

    public LocationPermissions MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Permissions src) =>
        new() { CanView = src.CanView, CanModify = src.CanModify, CanDelete = src.CanDelete, CanViewAnalytics = src.CanViewAnalytics };

    public TeamPermissions MapTo(Api.Shared.Grpc.Skedular.Team.Core.V1.Permissions src) =>
        new()
        {
            CanView = src.CanView,
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanCancelPeopleExistingInvitations = src.CanCancelPeopleExistingInvitations
        };

    public OrganizationMember MapTo(Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMember src) =>
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

    public TeamBookingPermissions MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.TeamPermissions src) =>
        new()
        {
            CanViewBookings = src.CanViewBookings,
            CanAddBooking = src.CanAddBooking,
            CanUpdateBooking = src.CanUpdateBooking,
            CanDeleteBooking = src.CanDeleteBooking
        };

    public OrganizationBookingPermissions MapTo(Api.Shared.Grpc.Skedular.Booking.Core.V1.OrganizationPermissions src) =>
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

    public Resource MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.Resource src) =>
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

    private static IEnumerable<OrganizationResourceType> MapTo(IEnumerable<Api.Shared.Grpc.Skedular.Organization.Core.V1.ResourceType> src) =>
        src.Select(MapToResourceType);

    private static OrganizationResourceType MapToResourceType(Api.Shared.Grpc.Skedular.Organization.Core.V1.ResourceType src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<Identity> MapTo(IEnumerable<Api.Shared.Grpc.Skedular.Customer.Core.V1.Identity> src) =>
        src.Select(MapTo);

    private static Identity MapTo(Api.Shared.Grpc.Skedular.Customer.Core.V1.Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified };

    private IEnumerable<Resource> MapTo(IEnumerable<Api.Shared.Grpc.Skedular.Location.Core.V1.Resource> src) =>
        src.Select(MapTo);

    private static IEnumerable<TeamMember> MapTo(IEnumerable<Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMember> src, Team team) =>
        src.Select(item => MapTo(item, team));

    private static TeamMember MapTo(Api.Shared.Grpc.Skedular.Team.Core.V1.TeamMember src, Team team) =>
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
                : new OrganizationMember { Id = src.OrganizationMember.Id, Customer = new Customer { Id = src.OrganizationMember.CustomerId } },
            Team = team
        };

    private static ListingMetadata MapTo(Api.Shared.Grpc.Skedular.Organization.Core.V1.ListingMetadata src) =>
        new(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(), src.IncludedFeatures);

    private static ListingMetadata MapTo(Api.Shared.Grpc.Skedular.Location.Core.V1.ListingMetadata src) =>
        new(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(), src.IncludedFeatures);
}
