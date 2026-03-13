using Api.Shared.Services.Models;
using Enterprise.Shared;
using Organization.Shared.Models;
using Stripe;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Organization.Shared.Models.Identity;
using Location = Organization.Shared.Models.Location;
using Team = Organization.Shared.Models.Team;
using Booking = Organization.Shared.Models.Booking;
using Customer = Organization.Shared.Models.Customer;
using CustomerType = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.CustomerType;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;
using OrganizationStripeConnectAccount = Organization.Shared.Database.Entities.OrganizationStripeConnectAccount;

namespace Organization.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src);
    Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src);
    Booking MapTo(Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event src);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Location MergeToEntity(
        Location src,
        Shared.Database.Entities.Location dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Team MergeToEntity(
        Team src,
        Shared.Database.Entities.Team dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        ICollection<Shared.Database.Entities.Organization> involvedOrganizations);

    Shared.Models.Organization MapTo(Shared.Database.Entities.Organization src);
    OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest);
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
            PhoneNumber = customer.PhoneNumber,
            Type = customer.Type switch
            {
                CustomerType.Guest => Api.Shared.Services.Models.CustomerType.Guest,
                CustomerType.Registered => Api.Shared.Services.Models.CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            },
            Identities = customer.Identities
                .Select(item => new Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified })
                .ToList()
        };
    }


    public Location MapTo(Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event src)
    {
        var location = src.Data.Location;
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

    public Team MapTo(Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event src)
    {
        var team = src.Data.Team;
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
            Until = booking.Until.ToDateTimeOffset(),
            InvolvedOrganizations = booking.InvolvedOrganizationIds.Select(item => new Shared.Models.Organization { Id = item }).ToList()
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
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
        dest.PhoneNumber = src.PhoneNumber;
        dest.Type = src.Type.ToNullableCustomerType();
        dest.Identities = identities;
        return dest;
    }

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

    public Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        ICollection<Shared.Database.Entities.Organization> involvedOrganizations)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.InvolvedOrganizations = involvedOrganizations;
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
            UniqueAlphanumericName = src.UniqueAlphanumericName,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            MarketplaceListingMetadata = src.MarketplaceListingMetadata ?? ListingMetadata.Empty,
            Website = src.Website,
            AgreedToTermsOfUse = src.AgreedToTermsOfUse,
            LogoUrl = src.LogoUrl,
            Type = src.Type.ToOrganizationType(),
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            IsOwnershipVerified = src.IsOwnershipVerified,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
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
        organization.OrganizationStripeCustomer = MapTo(src.OrganizationStripeCustomer, organization);
        organization.OrganizationStripePaymentMethods = MapTo(src.OrganizationStripePaymentMethods, organization).ToList();
        organization.OrganizationStripeConnectAccounts = MapTo(src.OrganizationStripeConnectAccounts, organization).ToList();

        return organization;
    }

    public OrganizationStripeConnectAccount MergeTo(Account src, OrganizationStripeConnectAccount dest)
    {
        dest.StripeAccountId = src.Id;
        dest.ChargesEnabled = src.ChargesEnabled;
        dest.PayoutsEnabled = src.PayoutsEnabled;
        dest.Type = src.Type.ToSafeString();
        dest.Country = src.Country;
        dest.DefaultCurrency = src.DefaultCurrency;
        dest.BusinessType = src.BusinessType;
        dest.CompanyName = src.BusinessProfile?.Name;
        dest.Url = src.BusinessProfile?.Url;
        dest.SupportUrl = src.BusinessProfile?.SupportUrl;
        dest.ContactEmail = src.Email;
        dest.ContactPhone = src.BusinessProfile?.SupportPhone;
        dest.DetailsSubmitted = src.DetailsSubmitted;
        dest.CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString();
        dest.CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString();
        return dest;
    }

    private static IEnumerable<Shared.Models.OrganizationMember> MapTo(
        IEnumerable<OrganizationMember> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationMember MapTo(OrganizationMember src, Shared.Models.Organization organization) =>
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
                PhoneNumber = src.PhoneNumber,
                Type = src.Type.ToNullableCustomerType(),
                Identities = MapTo(src.Identities).ToList()
            };

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

    private static Shared.Models.OrganizationOffering MapTo(OrganizationOffering src, Shared.Models.Organization organization)
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
            })
            .ToList();

        return organizationOffering;
    }

    private static IEnumerable<DailyMemberCountRecording> MapTo(IEnumerable<Shared.Database.Entities.DailyMemberCountRecording> src,
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

    private static IEnumerable<IndustrySubCategory> MapTo(IEnumerable<Shared.Database.Entities.IndustrySubCategory> src) => src.Select(MapTo)!;

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

    private static IEnumerable<JoinInvitation> MapTo(
        IEnumerable<Shared.Database.Entities.JoinInvitation> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static JoinInvitation MapTo(Shared.Database.Entities.JoinInvitation src, Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Email = src.Email,
            Status = src.Status.ToInvitationStatus(),
            Organization = organization,
            CreatedBy = MapTo(src.CreatedBy)!,
            Invitee = MapTo(src.Invitee)
        };

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

    private static OrganizationStripeCustomer? MapTo(
        Shared.Database.Entities.OrganizationStripeCustomer? src,
        Shared.Models.Organization organization) =>
        src is null
            ? null
            : new OrganizationStripeCustomer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                StripeCustomerId = src.StripeCustomerId,
                Organization = organization
            };

    private static IEnumerable<OrganizationStripePaymentMethod> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationStripePaymentMethod> src,
        Shared.Models.Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static OrganizationStripePaymentMethod MapTo(
        Shared.Database.Entities.OrganizationStripePaymentMethod src,
        Shared.Models.Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            SetupIntentId = src.SetupIntentId,
            PaymentMethodId = src.PaymentMethodId,
            CardBrand = src.CardBrand,
            CardCountry = src.CardCountry,
            CardDescription = src.CardDescription,
            CardExpiryMonth = src.CardExpiryMonth,
            CardExpiryYear = src.CardExpiryYear,
            CardFingerprint = src.CardFingerprint,
            CardFunding = src.CardFunding,
            CardIssuer = src.CardIssuer,
            CardLastFourDigit = src.CardLastFourDigit,
            Organization = organization
        };

    private static IEnumerable<Shared.Models.OrganizationStripeConnectAccount> MapTo(
        IEnumerable<OrganizationStripeConnectAccount> src,
        Shared.Models.Organization organization) => src.Select(item => MapTo(item, organization));

    private static Shared.Models.OrganizationStripeConnectAccount MapTo(
        OrganizationStripeConnectAccount src,
        Shared.Models.Organization organization) => new()
    {
        Id = src.Id,
        CreatedAt = src.CreatedAt,
        ModifiedAt = src.ModifiedAt,
        DeletedAt = src.DeletedAt,
        IsDefault = src.IsDefault,
        StripeAccountId = src.StripeAccountId,
        Name = src.Name,
        ChargesEnabled = src.ChargesEnabled,
        PayoutsEnabled = src.PayoutsEnabled,
        Type = src.Type,
        Country = src.Country,
        DefaultCurrency = src.DefaultCurrency,
        BusinessType = src.BusinessType,
        Url = src.Url,
        SupportUrl = src.SupportUrl,
        CompanyName = src.CompanyName,
        ContactEmail = src.ContactEmail,
        ContactPhone = src.ContactPhone,
        DetailsSubmitted = src.DetailsSubmitted,
        CapabilitiesCardPayments = src.CapabilitiesCardPayments,
        CapabilitiesTransfers = src.CapabilitiesTransfers,
        OnboardingUrl = src.OnboardingUrl,
        Organization = organization,
        OrganizationStripeConnectAccountAuthorization = MapTo(src.OrganizationStripeConnectAccountAuthorization)
    };

    private static OrganizationStripeConnectAccountAuthorization? MapTo(
        Shared.Database.Entities.OrganizationStripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new OrganizationStripeConnectAccountAuthorization
            {
                Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, IsAuthorized = src.IsAuthorized
            };
}
