using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Enterprise.Shared.Models;
using Google.Protobuf.WellKnownTypes;
using Organization.Api.GraphQL;
using Organization.Shared.Models;
using AddCustomTagInput = Organization.Api.GraphQL.AddCustomTagInput;
using AddZoneInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.AddZoneInput;
using Booking = Organization.Shared.Models.Booking;
using Customer = Organization.Shared.Models.Customer;
using DailyMemberCountRecording = Organization.Shared.Models.DailyMemberCountRecording;
using Identity = Organization.Shared.Models.Identity;
using IndustryMainCategory = Organization.Shared.Models.IndustryMainCategory;
using IndustrySubCategory = Organization.Shared.Models.IndustrySubCategory;
using JoinInvitation = Organization.Shared.Models.JoinInvitation;
using Location = Organization.Shared.Models.Location;
using Offering = Api.Shared.Services.Offering.Offering;
using OrganizationDailyBookingsTotal = Organization.Shared.Models.OrganizationDailyBookingsTotal;
using OrganizationMember = Organization.Shared.Models.OrganizationMember;
using OrganizationMemberAttendancePercentage = Organization.Shared.Models.OrganizationMemberAttendancePercentage;
using OrganizationMemberStatus = Api.Shared.Services.Models.OrganizationMemberStatus;
using OrganizationOffering = Organization.Shared.Models.OrganizationOffering;
using Tag = Organization.Shared.Models.Tag;
using Team = Organization.Shared.Models.Team;
using TermsOfUse = Organization.Shared.Database.Entities.TermsOfUse;
using UpdateCustomTagInput = Organization.Api.GraphQL.UpdateCustomTagInput;
using UpdateZoneInput = Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateZoneInput;
using Member = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMember;

namespace Organization.Api.Mappers;

public interface IMapper
{
    Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src);
    OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization);
    JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src);

    Shared.Database.Entities.Organization MapTo(
        Shared.Models.Organization src,
        TermsOfUse termsOfUse,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories);

    Shared.Models.TermsOfUse? MapTo(TermsOfUse? src);
    Customer? MapTo(Shared.Database.Entities.Customer? src);
    IEnumerable<IndustryMainCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustryMainCategory> src);
    OrganizationTermsOfUse? MapTo(Shared.Models.TermsOfUse? src);
    IEnumerable<OrganizationIndustryMainCategoryReferenceDetails> MapTo(IEnumerable<IndustryMainCategory> src);
    IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src);
    OrganizationDetails? MapTo(Shared.Models.Organization? src);
    OrganizationMemberDetails MapTo(OrganizationMember src);

    OrganizationAnalytics MapTo(
        IEnumerable<OrganizationMemberAttendancePercentage> organizationMemberAttendancePercentages,
        IEnumerable<OrganizationDailyBookingsTotal> organizationDailyBookingsTotals);

    Shared.Models.Organization MapTo(AddOrganizationInput src);
    Shared.Models.Organization MapTo(UpdateOrganizationInput src);
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse MapToGrpcResponse(Shared.Models.TermsOfUse src);
    Shared.Models.Organization MapTo(Admin_AddInput src);
    global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization MapToGrpcResponse(Shared.Models.Organization src);

    Shared.Database.Entities.OrganizationMember MapToEntity(
        OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.OrganizationMember MergeToEntity(
        OrganizationMember src,
        Shared.Database.Entities.OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MapTo(Admin_AddMemberInput src);
    OrganizationEdge MapTo(Edge<Shared.Models.Organization> src);

    IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization);

    OrganizationMemberEdge MapTo(Edge<OrganizationMember> src);
    MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src);
    Tag MapTo(Shared.Database.Entities.Tag src);
    Shared.Database.Entities.Tag MapTo(Tag src, Shared.Database.Entities.Organization organization);
    Shared.Database.Entities.Tag MergeTo(Tag src, Shared.Database.Entities.Tag dest, Shared.Database.Entities.Organization organization);
    IEnumerable<Edge<Tag>> MapTo(IEnumerable<Edge<Shared.Database.Entities.Tag>> src, Shared.Models.Organization organization);
    Tag MapTo(AddCustomTagInput src);
    Tag MapTo(UpdateCustomTagInput src);
    Tag MapTo(GraphQL.AddZoneInput src);
    Tag MapTo(GraphQL.UpdateZoneInput src);
    OrganizationTagDetails? MapTo(Tag? src);
    OrganizationTagEdge MapTo(Edge<Tag> src);

    CustomTag MapToGrpcResponseCustomTag(Tag? src);
    CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src);
    Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.AddCustomTagInput src);
    Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateCustomTagInput src);

    Zone MapToGrpcResponseZone(Tag? src);
    ZoneEdge MapToGrpcResponseZone(Edge<Tag> src);
    Tag MapTo(AddZoneInput src);
    Tag MapTo(UpdateZoneInput src);

    IEnumerable<string> MapTo(Offering offering);
    OrganizationSsoSetting MapTo(UpdateOrganizationSsoSettingsInput src);
    OrganizationSsoSettingsDetails? MapTo(OrganizationSsoSetting? src);
    Shared.Database.Entities.OrganizationSsoSetting MapToEntity(OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.OrganizationSsoSetting MergeToEntity(
        OrganizationSsoSetting src,
        Shared.Database.Entities.OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting? MapTo(Shared.Database.Entities.OrganizationSsoSetting? src);
}

public class Mapper : IMapper
{
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
            Type = src.Type.ToOrganizationType(),
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            PaymentMethodEventRaisedAt = src.PaymentMethodEventRaisedAt,
            DailyMemberCountLastRecordedAt = src.DailyMemberCountLastRecordedAt,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories, null).ToList(),
            OrganizationSsoSettings = MapTo(src.OrganizationSsoSettings)
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.Bookings = MapTo(src.Bookings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.Locations = MapTo(src.Locations, organization).ToList();
        organization.Teams = MapTo(src.Teams, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();
        organization.AzureTenants = MapTo(src.AzureTenants, organization).ToList();
        organization.Tags = MapTo(src.Tags, organization).ToList();

        return organization;
    }

    public OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Role = src.Role.ToOrganizationMemberRole(),
            Status = src.Status.ToOrganizationMemberStatus(),
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            Customer = MapTo(src.Customer)!,
            Organization = organization
        };

    public JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Role = src.Role.ToOrganizationMemberRole(),
            Organization = MapTo(src.Organization),
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    public Shared.Database.Entities.Organization MapTo(
        Shared.Models.Organization src,
        TermsOfUse termsOfUse,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            TermsOfUse = termsOfUse,
            IndustrySubCategories = industrySubCategories,
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod
        };

    public Shared.Database.Entities.Organization MergeTo(
        Shared.Models.Organization src,
        Shared.Database.Entities.Organization dest,
        ICollection<Shared.Database.Entities.IndustrySubCategory> industrySubCategories)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.About = src.About;
        dest.Website = src.Website;
        dest.AgreedToTermsOfUse = src.AgreedToTermsOfUse;
        dest.LogoUrl = src.LogoUrl;
        dest.Type = src.Type.ToOrganizationType();
        dest.IndustrySubCategories = industrySubCategories;
        return dest;
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
                PhoneNumber = src.PhoneNumber,
                Identities = MapTo(src.Identities).ToList()
            };

    public Shared.Models.TermsOfUse? MapTo(TermsOfUse? src) =>
        src is null
            ? null
            : new Shared.Models.TermsOfUse
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                Active = src.Active,
                Terms = src.Terms
            };

    public IEnumerable<IndustryMainCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustryMainCategory> src) => src.Select(MapTo);

    public OrganizationTermsOfUse? MapTo(Shared.Models.TermsOfUse? src) =>
        src is null ? null : new OrganizationTermsOfUse { Id = src.Id, Terms = src.Terms };

    public IEnumerable<OrganizationIndustryMainCategoryReferenceDetails> MapTo(IEnumerable<IndustryMainCategory> src) => src.Select(MapTo);

    public IEnumerable<OrganizationDetails> MapTo(IEnumerable<Shared.Models.Organization> src) => src.Select(MapTo)!;

    public OrganizationDetails? MapTo(Shared.Models.Organization? src)
    {
        if (src is null)
        {
            return null;
        }

        var organizationOffering = src.OrganizationOfferings.FirstOrDefault();
        var availableOfferings = organizationOffering is null || organizationOffering.Code == OfferingCode.EarlyBirdV1
            ? []
            : Offerings.AllOfferings
                .Where(item => item != organizationOffering.Code)
                .Select(item =>
                {
                    var offering = item.GetOffering();
                    return new OrganizationOfferingDetails
                    {
                        IsEnterprise = item.IsEnterpriseOffering(),
                        Code = item.ToOfferingCode(),
                        Name = offering.Name,
                        UnitPrice = offering.UnitPrice,
                        FeatureSet = MapTo(offering),
                        UnderPriceLines = offering.UnderPriceLines,
                        Free = item.IsFreeOffering(),
                        EarlyBird = item.IsEarlyBirdOffering()
                    };
                });

        return new OrganizationDetails
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = new OrganizationTypeDetails
            {
                Type = src.Type,
                Name = src.Type.ToOrganizationTypeName(),
            },
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = src.IndustrySubCategories.Select(item => MapTo(item, null)),
            AvailableOfferings = availableOfferings,
            ActiveOffering = MapTo(organizationOffering),
            CanModify = src.CanModify,
            CanDelete = src.CanDelete,
            CanInvitePeople = src.CanInvitePeople,
            CanViewAnalytics = src.CanViewAnalytics,
            HasLocation = src.HasLocation,
            HasTeam = src.HasTeam,
            HasFutureBooking = src.HasFutureBooking,
            IsMyOnboardingDone = src.IsMyOnboardingDone,
            Members = MapTo(src.OrganizationMembers),
            ResourceTypes = src.Tags
                .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
                .Select(item => MapTo(item)!),
            SsoSettings = MapTo(src.OrganizationSsoSettings)
        };
    }

    public OrganizationMemberDetails MapTo(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role,
            Status = src.Status,
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            Customer = MapTo(src.Customer)
        };

    public OrganizationAnalytics MapTo(
        IEnumerable<OrganizationMemberAttendancePercentage> organizationMemberAttendancePercentages,
        IEnumerable<OrganizationDailyBookingsTotal> organizationDailyBookingsTotals) =>
        new()
        {
            MemberAttendancePercentage = organizationMemberAttendancePercentages
                .Select(item => new GraphQL.OrganizationMemberAttendancePercentage { Date = item.Date, Percentage = item.Percentage }),
            DailyBookingsTotals = organizationDailyBookingsTotals
                .Select(item => new GraphQL.OrganizationDailyBookingsTotal { Date = item.Date, Total = item.Total })
        };

    public Shared.Models.Organization MapTo(AddOrganizationInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList(),
            TermsOfUse = new Shared.Models.TermsOfUse { Id = src.TermsOfUseId }
        };

    public Shared.Models.Organization MapTo(UpdateOrganizationInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse
        MapToGrpcResponse(Shared.Models.TermsOfUse src) => new() { Id = src.Id, Terms = src.Terms };

    public Shared.Models.Organization MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            About = src.About,
            Website = src.Website,
            Type = src.Type.ToOrganizationType(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            TermsOfUse = string.IsNullOrWhiteSpace(src.TermsOfUseId) ? null : new Shared.Models.TermsOfUse { Id = src.TermsOfUseId },
            LogoUrl = src.LogoUrl,
            IndustrySubCategories = src.IndustrySubCategoryIds.Select(item => new IndustrySubCategory { Id = item }).ToList()
        };

    public global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization MapToGrpcResponse(Shared.Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
            .OrderByDescending(item => item.End).First();
        var organization = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            Type = src.Type.ToOrganizationType(),
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl.ToSafeString(),
            Offering = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            },
            HasAttachedPaymentMethod = src.HasAttachedPaymentMethod,
            HasFutureBooking = src.HasFutureBooking
        };

        organization.Tags.AddRange(MapToGrpcResponse(src.Tags));
        organization.ResourceTypes.AddRange(MapToGrpcResponseResourceType(src.Tags));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.IndustrySubCategories.AddRange(src.IndustrySubCategories.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.IndustrySubCategory
            {
                Id = item.Id, Name = item.Name, MainCategoryName = item.IndustryMainCategory.Name
            }));

        organization.Members.AddRange(MapToGrpcResponse(src.OrganizationMembers));

        return organization;
    }

    public Shared.Database.Entities.OrganizationMember MapToEntity(
        OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationMember(), organization, customer);

    public Shared.Database.Entities.OrganizationMember MergeToEntity(
        OrganizationMember src,
        Shared.Database.Entities.OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Role = src.Role.ToOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone;
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public OrganizationMember MapTo(Admin_AddMemberInput src) => MapTo(src.Member, new Shared.Models.Organization { Id = src.Id });

    public OrganizationEdge MapTo(Edge<Shared.Models.Organization> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public IEnumerable<Edge<OrganizationMember>> MapTo(
        IEnumerable<Edge<Shared.Database.Entities.OrganizationMember>> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    public OrganizationMemberEdge MapTo(Edge<OrganizationMember> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node) };

    public MemberEdge MapToGrpcResponse(Edge<OrganizationMember> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public Tag MapTo(Shared.Database.Entities.Tag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type.ToOrganizationTagType(),
            Color = src.Color
        };

    public Shared.Database.Entities.Tag MapTo(Tag src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Tag(), organization);

    public Shared.Database.Entities.Tag MergeTo(
        Tag src,
        Shared.Database.Entities.Tag dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Description = src.Description;
        dest.Type = src.Type.ToOrganizationTagType();
        dest.Color = src.Color;
        dest.Organization = organization;
        return dest;
    }

    public IEnumerable<Edge<Tag>> MapTo(IEnumerable<Edge<Shared.Database.Entities.Tag>> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    public Tag MapTo(AddCustomTagInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Tag MapTo(UpdateCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Tag MapTo(GraphQL.AddZoneInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Name = src.Name,
            Description = src.Description,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId },
            Type = OrganizationTagType.Zone,
            Color = src.Color
        };

    public Tag MapTo(GraphQL.UpdateZoneInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Description = src.Description,
            Type = OrganizationTagType.Zone,
            Color = src.Color
        };

    public OrganizationTagDetails? MapTo(Tag? src) =>
        src is null
            ? null
            : new OrganizationTagDetails
            {
                Id = src.Id,
                Name = src.Name,
                Description = src.Description,
                TagType = src.Type.ToOrganizationTagType(),
                Color = src.Color
            };

    public OrganizationTagEdge MapTo(Edge<Tag> src) => new() { Cursor = src.Cursor, Node = MapTo(src.Node)! };

    public CustomTag MapToGrpcResponseCustomTag(Tag? src) =>
        src is null
            ? new CustomTag()
            : new CustomTag
            {
                Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString()
            };

    public CustomTagEdge MapToGrpcResponseCustomTag(Edge<Tag> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponseCustomTag(src.Node) };

    public Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.AddCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Custom,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Tag MapTo(global::Api.Shared.Services.Grpc.Skedular.Organization.V1.UpdateCustomTagInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Custom,
            Color = src.Color
        };

    public Zone MapToGrpcResponseZone(Tag? src) =>
        src is null
            ? new Zone()
            : new Zone
            {
                Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString()
            };

    public ZoneEdge MapToGrpcResponseZone(Edge<Tag> src) => new() { Cursor = src.Cursor, Node = MapToGrpcResponseZone(src.Node) };

    public Tag MapTo(AddZoneInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Type = OrganizationTagType.Zone,
            Color = src.Color.ToSafeString(),
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public Tag MapTo(UpdateZoneInput src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Type = OrganizationTagType.Zone };

    public IEnumerable<string> MapTo(Offering offering) => offering.FeatureSets.Select(MapTo);

    public OrganizationSsoSetting MapTo(UpdateOrganizationSsoSettingsInput src) =>
        new()
        {
            Id = string.IsNullOrWhiteSpace(src.Id) ? string.Empty : src.Id,
            EntityId = src.EntityId,
            LoginUrl = src.LoginUrl,
            AppFederationMetadataUrl = src.AppFederationMetadataUrl,
            Organization = new Shared.Models.Organization { Id = src.OrganizationId }
        };

    public OrganizationSsoSettingsDetails? MapTo(OrganizationSsoSetting? src) =>
        src is null
            ? null
            : new OrganizationSsoSettingsDetails
            {
                Id = src.Id, EntityId = src.EntityId, LoginUrl = src.LoginUrl, AppFederationMetadataUrl = src.AppFederationMetadataUrl
            };

    public Shared.Database.Entities.OrganizationSsoSetting MapToEntity(
        OrganizationSsoSetting src,
        Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new Shared.Database.Entities.OrganizationSsoSetting(), organization);

    public Shared.Database.Entities.OrganizationSsoSetting MergeToEntity(
        OrganizationSsoSetting src,
        Shared.Database.Entities.OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EntityId = src.EntityId;
        dest.LoginUrl = src.LoginUrl;
        dest.AppFederationMetadataUrl = src.AppFederationMetadataUrl;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationSsoSetting? MapTo(Shared.Database.Entities.OrganizationSsoSetting? src) =>
        src is null
            ? null
            : new OrganizationSsoSetting
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                EntityId = src.EntityId,
                LoginUrl = src.LoginUrl,
                AppFederationMetadataUrl = src.AppFederationMetadataUrl
            };

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Tag> MapToGrpcResponse(IEnumerable<Tag> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Tag MapToGrpcResponse(Tag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Description = src.Description.ToSafeString(), Color = src.Color.ToSafeString() };

    private static IEnumerable<ResourceType> MapToGrpcResponseResourceType(IEnumerable<Tag> src) =>
        src
            .Where(item => OrganizationTagTypeConstants.ResourceTypes.Any(resourceType => resourceType == item.Type))
            .Select(MapToGrpcResponseResourceType);

    private static ResourceType MapToGrpcResponseResourceType(Tag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Description = src.Description.ToSafeString(),
            Color = src.Color.ToSafeString(),
            TagType = src.Type.ToOrganizationTagType()
        };

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Member MapToGrpcResponse(OrganizationMember src) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                OrganizationMemberRole.Owner => Role.Owner,
                OrganizationMemberRole.Administrator => Role.Administrator,
                OrganizationMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                OrganizationMemberStatus.Active => global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Active,
                OrganizationMemberStatus.Inactive => global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone ?? false,
            Customer = MapToGrpcResponse(src.Customer)
        };

    private IEnumerable<Member> MapToGrpcResponse(IEnumerable<OrganizationMember> src) => src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer MapToGrpcResponse(Customer src)
    {
        var customer = new global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Customer
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
            PhotoUrl512 = src.PhotoUrl512.ToSafeString(),
            PhoneNumber = src.PhoneNumber.ToSafeString()
        };

        customer.Identities.AddRange(MapToGrpcResponse(src.Identities));

        return customer;
    }

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Identity> MapToGrpcResponse(IEnumerable<Identity> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Identity MapToGrpcResponse(Identity src) =>
        new() { Id = src.Id, Email = src.Email.ToSafeString(), EmailVerified = src.EmailVerified ?? false };

    private static OrganizationMember MapTo(Member src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            Role = src.Role switch
            {
                Role.Owner => OrganizationMemberRole.Owner,
                Role.Administrator => OrganizationMemberRole.Administrator,
                Role.Member => OrganizationMemberRole.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = src.Status switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Active => OrganizationMemberStatus.Active,
                global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationMemberStatus.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsOrganizationOnboardingDone = src.IsOrganizationOnboardingDone,
            Customer = new Customer { Id = src.Customer.Id },
            Organization = organization
        };

    private static OrganizationCustomerDetails MapTo(Customer src) =>
        new()
        {
            UniqueId = src.Id,
            Email = src.Identities
                .Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
                .Select(item => item.Email!.ToLowerInvariant())
                .Distinct()
                .FirstOrDefault(),
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
            PhoneNumber = src.PhoneNumber
        };

    private OrganizationActiveOfferingDetails MapTo(OrganizationOffering? src)
    {
        if (src is null)
        {
            return new OrganizationActiveOfferingDetails();
        }

        var offering = src.Code.GetOffering();
        return new OrganizationActiveOfferingDetails
        {
            Id = src.Id,
            Code = src.Code.ToOfferingCode(),
            IsEnterprise = src.Code.IsEnterpriseOffering(),
            Name = offering.Name,
            Start = src.Start,
            End = src.End,
            UnitPrice = src.UnitPrice,
            FeatureSet = MapTo(offering),
            UnderPriceLines = offering.UnderPriceLines,
            Free = src.Code.IsFreeOffering(),
            EarlyBird = src.Code.IsEarlyBirdOffering()
        };
    }

    private static string MapTo(FeatureSetCode item) => Features.FeatureSet[item].Description;

    private static OrganizationIndustryMainCategoryReferenceDetails MapTo(IndustryMainCategory src)
    {
        var organizationIndustryMainCategoryReferenceDetails = new OrganizationIndustryMainCategoryReferenceDetails { Id = src.Id, Name = src.Name };

        organizationIndustryMainCategoryReferenceDetails.SubCategories =
            MapTo(src.IndustrySubCategories, organizationIndustryMainCategoryReferenceDetails);

        return organizationIndustryMainCategoryReferenceDetails;
    }

    private static IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> MapTo(
        IEnumerable<IndustrySubCategory> src,
        OrganizationIndustryMainCategoryReferenceDetails? organizationIndustryMainCategoryReferenceDetails) =>
        src.Select(item => MapTo(item, organizationIndustryMainCategoryReferenceDetails));

    private static OrganizationIndustrySubCategoryReferenceDetails MapTo(
        IndustrySubCategory src,
        OrganizationIndustryMainCategoryReferenceDetails? organizationIndustryMainCategoryReferenceDetails) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            MainCategoryName = organizationIndustryMainCategoryReferenceDetails is null
                ? src.IndustryMainCategory.Name
                : organizationIndustryMainCategoryReferenceDetails.Name
        };

    private IndustrySubCategory? MapTo(Shared.Database.Entities.IndustrySubCategory? src, IndustryMainCategory? industryMainCategory)
    {
        if (src is null)
        {
            return null;
        }

        var industrySubCategory = new IndustrySubCategory
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            IndustryMainCategory = industryMainCategory ?? MapTo(src.IndustryMainCategory)
        };

        return industrySubCategory;
    }

    private IEnumerable<IndustrySubCategory> MapTo(
        IEnumerable<Shared.Database.Entities.IndustrySubCategory> src,
        IndustryMainCategory? industryMainCategory) =>
        src.Select(item => MapTo(item, industryMainCategory))!;

    private IndustryMainCategory MapTo(Shared.Database.Entities.IndustryMainCategory src)
    {
        var industryMainCategory = new IndustryMainCategory
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name
        };

        industryMainCategory.IndustrySubCategories = MapTo(src.IndustrySubCategories, industryMainCategory).ToList();

        return industryMainCategory;
    }

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity> src) => src.Select(MapTo);

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

    private static IEnumerable<OrganizationOffering> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationOffering> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(Shared.Database.Entities.OrganizationOffering src,
        Shared.Models.Organization organization) =>
        new()
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

    private static IEnumerable<Booking> MapTo(IEnumerable<Shared.Database.Entities.Booking> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Booking MapTo(Shared.Database.Entities.Booking src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            From = src.From,
            Until = src.Until,
            Organization = organization
        };

    private static IEnumerable<DailyMemberCountRecording> MapTo(
        IEnumerable<Shared.Database.Entities.DailyMemberCountRecording> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static DailyMemberCountRecording MapTo(Shared.Database.Entities.DailyMemberCountRecording src, Shared.Models.Organization organization) =>
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

    private static IEnumerable<Location> MapTo(IEnumerable<Shared.Database.Entities.Location> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Location MapTo(Shared.Database.Entities.Location src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<Team> MapTo(IEnumerable<Shared.Database.Entities.Team> src, Shared.Models.Organization organization) =>
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

    private IEnumerable<JoinInvitation> MapTo(IEnumerable<Shared.Database.Entities.JoinInvitation> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Organization = organization,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

    private Edge<OrganizationMember> MapTo(Edge<Shared.Database.Entities.OrganizationMember> src, Shared.Models.Organization organization) =>
        new(src.Cursor, MapTo(src.Node, organization));

    private static IEnumerable<AzureTenant> MapTo(IEnumerable<Shared.Database.Entities.AzureTenant> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static AzureTenant MapTo(Shared.Database.Entities.AzureTenant src, Shared.Models.Organization organization)
    {
        var azureTenant = new AzureTenant
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Name = src.Name,
            MembersLastRefreshedAt = src.MembersLastRefreshedAt,
            Organization = organization
        };

        azureTenant.AzureTenantMembers = MapTo(src.AzureTenantMembers, azureTenant).ToList();

        return azureTenant;
    }

    private static IEnumerable<AzureTenantMember> MapTo(IEnumerable<Shared.Database.Entities.AzureTenantMember> src, AzureTenant azureTenant) =>
        src.Select(item => MapTo(item, azureTenant));

    private static AzureTenantMember MapTo(Shared.Database.Entities.AzureTenantMember src, AzureTenant azureTenant) =>
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
            PreferredLanguage = src.PreferredLanguage,
            PhotoUrl = src.PhotoUrl,
            PhotoUrl48 = src.PhotoUrl48,
            PhotoUrl64 = src.PhotoUrl64,
            PhotoUrl96 = src.PhotoUrl96,
            PhotoUrl120 = src.PhotoUrl120,
            PhotoUrl240 = src.PhotoUrl240,
            PhotoUrl360 = src.PhotoUrl360,
            PhotoUrl432 = src.PhotoUrl432,
            PhotoUrl504 = src.PhotoUrl504,
            PhotoUrl648 = src.PhotoUrl648,
            AzureTenant = azureTenant
        };

    private Edge<Tag> MapTo(Edge<Shared.Database.Entities.Tag> src, Shared.Models.Organization organization)
    {
        var tag = MapTo(src.Node);
        tag.Organization = organization;
        return new Edge<Tag>(src.Cursor, tag);
    }

    private IEnumerable<OrganizationMemberDetails> MapTo(IEnumerable<OrganizationMember> src) => src.Select(MapTo);

    private static IEnumerable<Tag> MapTo(IEnumerable<Shared.Database.Entities.Tag> src, Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Tag MapTo(Shared.Database.Entities.Tag src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Description = src.Description,
            Type = src.Type.ToOrganizationTagType(),
            Color = src.Color,
            Organization = organization
        };
}
