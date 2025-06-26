using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Organization.Shared.Models;
using Stripe;
using Customer = Organization.Shared.Models.Customer;
using Location = Organization.Shared.Models.Location;
using Offering = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Offering;
using OrganizationMember = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.OrganizationMember;
using OrganizationSsoSetting = Organization.Shared.Models.OrganizationSsoSetting;
using OrganizationStripePaymentMethod = Organization.Shared.Database.Entities.OrganizationStripePaymentMethod;
using Tag = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Tag;

namespace Organization.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src);
    InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride);
    OrganizationStripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization);
    Models.Organization MapTo(Database.Entities.Organization src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization MapTo(Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First();
        var organization = new Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Organization
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            LogoUrl = src.LogoUrl.ToSafeString(),
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail.ToSafeString(),
            ContactPhone = src.ContactPhone.ToSafeString(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            Offering = new Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            },
            SsoSettings = MapTo(src.OrganizationSsoSettings)
        };

        organization.AzureTenantIds.AddRange(src.AzureTenants.Select(item => item.Id));

        organization.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            Type = item.Type.ToOrganizationTagType(),
            Color = item.Color.ToSafeString()
        }));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.Members.AddRange(src.OrganizationMembers.Select(item => new OrganizationMember
        {
            Id = item.Id,
            CustomerId = item.Customer.Id,
            Role = item.Role switch
            {
                OrganizationMemberRole.Owner => Role.Owner,
                OrganizationMemberRole.Administrator => Role.Administrator,
                OrganizationMemberRole.Member => Role.Member,
                _ => throw new ArgumentOutOfRangeException()
            },
            Status = item.Status switch
            {
                OrganizationMemberStatus.Active => Status.Active,
                OrganizationMemberStatus.Inactive => Status.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            }
        }));

        return organization;
    }

    public InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            OrganizationId = src.Organization.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };

    public OrganizationStripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Organization organization) =>
        new()
        {
            SetupIntentId = setupIntentId,
            PaymentMethodId = paymentMethod.Id,
            CardBrand = paymentMethod.Card?.Brand,
            CardCountry = paymentMethod.Card?.Country,
            CardDescription = paymentMethod.Card?.Description,
            CardExpiryMonth = paymentMethod.Card is null ? null : (byte)paymentMethod.Card.ExpMonth,
            CardExpiryYear = paymentMethod.Card is null ? null : (short)paymentMethod.Card.ExpYear,
            CardFingerprint = paymentMethod.Card?.Fingerprint,
            CardFunding = paymentMethod.Card?.Funding,
            CardIssuer = paymentMethod.Card?.Issuer,
            CardLastFourDigit = paymentMethod.Card?.Last4,
            Organization = organization
        };

    public Models.Organization MapTo(Database.Entities.Organization src)
    {
        var organization = new Models.Organization
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
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            PaymentMethodEventRaisedAt = src.PaymentMethodEventRaisedAt,
            DailyMemberCountLastRecordedAt = src.DailyMemberCountLastRecordedAt,
            TermsOfUse = MapTo(src.TermsOfUse),
            IndustrySubCategories = MapTo(src.IndustrySubCategories).ToList()
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();
        organization.DailyMemberCountRecordings = MapTo(src.DailyMemberCountRecordings, organization).ToList();
        organization.Locations = MapTo(src.Locations, organization).ToList();
        organization.Teams = MapTo(src.Teams, organization).ToList();
        organization.JoinInvitations = MapTo(src.JoinInvitations, organization).ToList();
        organization.Tags = MapTo(src.Tags, organization).ToList();

        return organization;
    }

    private static OrganizationSsoSettings? MapTo(OrganizationSsoSetting? src) =>
        src is null
            ? null
            : new OrganizationSsoSettings
            {
                Id = src.Id,
                EntityId = src.EntityId.ToSafeString(),
                LoginUrl = src.LoginUrl.ToSafeString(),
                AppFederationMetadataUrl = src.AppFederationMetadataUrl.ToSafeString(),
                IsActive = src.IsActive
            };

    private static TermsOfUse? MapTo(Database.Entities.TermsOfUse? src) =>
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

    private static IEnumerable<IndustrySubCategory> MapTo(IEnumerable<Database.Entities.IndustrySubCategory> src) => src.Select(MapTo)!;

    private static IndustrySubCategory? MapTo(Database.Entities.IndustrySubCategory? src) =>
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

    private static IndustryMainCategory MapTo(Database.Entities.IndustryMainCategory src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name
        };

    private static IEnumerable<Models.OrganizationMember> MapTo(
        IEnumerable<Database.Entities.OrganizationMember> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.OrganizationMember MapTo(Database.Entities.OrganizationMember src, Models.Organization organization) =>
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

    private static Customer? MapTo(Database.Entities.Customer? src) =>
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

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Database.Entities.Identity src) =>
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
        IEnumerable<Database.Entities.OrganizationOffering> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(Database.Entities.OrganizationOffering src, Models.Organization organization)
    {
        var organizationOffering = new OrganizationOffering
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
            })
            .ToList();

        return organizationOffering;
    }

    private static IEnumerable<DailyMemberCountRecording> MapTo(IEnumerable<Database.Entities.DailyMemberCountRecording> src,
        Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static DailyMemberCountRecording MapTo(Database.Entities.DailyMemberCountRecording src, Models.Organization organization) =>
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

    private static IEnumerable<Location> MapTo(IEnumerable<Database.Entities.Location> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Location MapTo(Database.Entities.Location src, Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<Team> MapTo(IEnumerable<Database.Entities.Team> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Team MapTo(Database.Entities.Team src, Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization
        };

    private static IEnumerable<JoinInvitation> MapTo(IEnumerable<Database.Entities.JoinInvitation> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static JoinInvitation MapTo(Database.Entities.JoinInvitation src, Models.Organization organization) =>
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

    private static IEnumerable<Models.Tag> MapTo(IEnumerable<Database.Entities.Tag> src, Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Models.Tag MapTo(Database.Entities.Tag src, Models.Organization organization) =>
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
