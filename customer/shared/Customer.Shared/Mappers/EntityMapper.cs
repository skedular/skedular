using Api.Shared.Services.Models;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using HotChocolate.Types.Pagination;
using Stripe;
using CustomerBillingDetails = Customer.Shared.Models.CustomerBillingDetails;
using CustomerFeedback = Customer.Shared.Models.CustomerFeedback;
using CustomerType = Api.Shared.Services.Models.CustomerType;
using Identity = Customer.Shared.Database.Entities.Identity;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using PaymentMethod = Stripe.PaymentMethod;
using PersonalInformationVisibility = Api.Shared.Services.Models.PersonalInformationVisibility;
using Resource = Customer.Shared.Database.Entities.Resource;

namespace Customer.Shared.Mappers;

public interface IEntityMapper
{
    Models.Customer MapTo(IContext context);
    Identity MapToIdentity(IContext context);
    Models.Customer MapTo(Database.Entities.Customer src);

    Database.Entities.Customer MapToEntity(
        Models.Customer src,
        IEnumerable<Identity> identities,
        Database.Entities.Organization? defaultOrganization,
        IReadOnlyList<Database.Entities.Location> preferredLocations,
        IReadOnlyList<Resource> preferredResources,
        IReadOnlyList<Database.Entities.OrganizationTag> preferredOrganizationTags,
        IReadOnlyList<Database.Entities.Location> favouriteLocations);

    IEnumerable<Identity> MapToEntity(IEnumerable<Models.Identity> src);
    CustomerFeedback MapTo(Database.Entities.CustomerFeedback src);
    Database.Entities.CustomerFeedback MapTo(CustomerFeedback src, Database.Entities.Customer customer);
    Identity MapTo(Models.Identity src, Database.Entities.Customer customer);
    Identity MergeTo(Models.Identity src, Identity dest, Database.Entities.Customer customer);
    Database.Entities.CustomerBillingDetails MapTo(CustomerBillingDetails src, Database.Entities.Customer customer);

    Database.Entities.CustomerBillingDetails MergeToEntity(
        CustomerBillingDetails src,
        Database.Entities.CustomerBillingDetails dest,
        Database.Entities.Customer customer);

    CustomerBillingDetails? MapTo(Database.Entities.CustomerBillingDetails? src);
    Edge<Models.Customer> MapTo(Edge<Database.Entities.Customer> src);
    IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Database.Entities.StripePaymentMethod> src);
    CustomerCreateOptions MapToStripeCustomerCreateOption(Database.Entities.Customer src);
    Database.Entities.StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Customer customer);
}

public class EntityMapper : IEntityMapper
{
    public Models.Customer MapTo(IContext context) =>
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
                new List<Models.Identity>
                {
                    new()
                    {
                        Id = context.GetVerifiableToken().ToSafeString(),
                        Email = context.GetEmail(),
                        EmailVerified = context.GetEmailVerified()
                    }
                },
            IsOnboardingDone = false,
            DefaultOrganization = null,
            PreferredLocations = [],
            PreferredOrganizationTags = [],
            PreferredResources = [],
            FavouriteLocations = [],
            PersonalInformationVisibility = PersonalInformationVisibility.Visible,
            Type = CustomerType.Registered
        };

    public Identity MapToIdentity(IContext context) =>
        new() { Id = context.GetVerifiableToken().ToSafeString(), Email = context.GetEmail(), EmailVerified = context.GetEmailVerified() };

    public Models.Customer MapTo(Database.Entities.Customer src) =>
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
            BillingDetails = MapTo(src.BillingDetails),
            IsOnboardingDone = src.IsOnboardingDone,
            Identities = MapToIdentities(src.Identities).ToList(),
            DefaultOrganization = MapTo(src.DefaultOrganization),
            PreferredLocations = MapToLocations(src.PreferredLocations).ToList(),
            PreferredResources = MapToResources(src.PreferredResources).ToList(),
            PreferredOrganizationTags = MapToOrganizationTags(src.PreferredOrganizationTags).ToList(),
            FavouriteLocations = MapToLocations(src.FavouriteLocations).ToList(),
            StripeCustomer = MapTo(src.StripeCustomer),
            StripePaymentMethods = MapTo(src.StripePaymentMethods).ToList(),
            PersonalInformationVisibility = src.PersonalInformationVisibility.ToPersonalInformationVisibility(),
            Type = src.Type.ToCustomerType()
        };

    public CustomerFeedback MapTo(Database.Entities.CustomerFeedback src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Content = src.Content,
            Channel = src.Channel switch
            {
                FeedbackChannelTypeConstants.Web => FeedbackChannelType.Web,
                FeedbackChannelTypeConstants.Slack => FeedbackChannelType.Slack,
                FeedbackChannelTypeConstants.MsTeams => FeedbackChannelType.MsTeams,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = MapTo(src.Customer)
        };

    public Database.Entities.CustomerFeedback MapTo(CustomerFeedback src, Database.Entities.Customer customer) =>
        new()
        {
            Id = src.Id,
            Content = src.Content,
            Channel = src.Channel switch
            {
                FeedbackChannelType.Web => FeedbackChannelTypeConstants.Web,
                FeedbackChannelType.Slack => FeedbackChannelTypeConstants.Slack,
                FeedbackChannelType.MsTeams => FeedbackChannelTypeConstants.MsTeams,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = customer
        };

    public Database.Entities.Customer MapToEntity(
        Models.Customer src,
        IEnumerable<Identity> identities,
        Database.Entities.Organization? defaultOrganization,
        IReadOnlyList<Database.Entities.Location> preferredLocations,
        IReadOnlyList<Resource> preferredResources,
        IReadOnlyList<Database.Entities.OrganizationTag> preferredOrganizationTags,
        IReadOnlyList<Database.Entities.Location> favouriteLocations) =>
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
            IsOnboardingDone = src.IsOnboardingDone,
            Identities = identities.ToList(),
            DefaultOrganization = defaultOrganization,
            PreferredLocations = preferredLocations.ToList(),
            PreferredResources = preferredResources.ToList(),
            PreferredOrganizationTags = preferredOrganizationTags.ToList(),
            FavouriteLocations = favouriteLocations.ToList(),
            PersonalInformationVisibility = src.PersonalInformationVisibility.ToPersonalInformationVisibility(),
            Type = src.Type.ToCustomerType()
        };

    public IEnumerable<Identity> MapToEntity(IEnumerable<Models.Identity> src) => src.Select(MapToEntity);

    public Identity MapTo(Models.Identity src, Database.Entities.Customer customer) => MergeTo(src, new Identity(), customer);

    public Identity MergeTo(Models.Identity src, Identity dest, Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.EmailVerified = src.EmailVerified;
        dest.Customer = customer;
        return dest;
    }

    public Database.Entities.CustomerBillingDetails MapTo(CustomerBillingDetails src, Database.Entities.Customer customer) =>
        MergeToEntity(src, new Database.Entities.CustomerBillingDetails(), customer);

    public Database.Entities.CustomerBillingDetails MergeToEntity(
        CustomerBillingDetails src,
        Database.Entities.CustomerBillingDetails dest,
        Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.CompanyName = src.CompanyName;
        dest.Email = src.Email;
        dest.OsmType = src.OsmType;
        dest.OsmId = src.OsmId;
        dest.PlaceId = src.PlaceId;
        dest.Coordinates = src.Coordinates;
        dest.FormattedAddress = src.FormattedAddress;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.CountryCode = src.CountryCode;
        dest.Customer = customer;
        return dest;
    }

    public CustomerBillingDetails? MapTo(Database.Entities.CustomerBillingDetails? src) =>
        src is null
            ? null
            : new CustomerBillingDetails
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                CompanyName = src.CompanyName,
                Email = src.Email,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode
            };

    public Edge<Models.Customer> MapTo(Edge<Database.Entities.Customer> src) => new(MapTo(src.Node), src.Cursor);

    public IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Database.Entities.StripePaymentMethod> src) => src.Select(MapToModel);

    public CustomerCreateOptions MapToStripeCustomerCreateOption(Database.Entities.Customer src) =>
        new()
        {
            Name = src.ToDisplayableName(),
            Email = src.Identities.ToSingleEmail(),
            Phone = src.PhoneNumber.ToSafeString(),
            PreferredLocales = string.IsNullOrWhiteSpace(src.Locale) ? [] : [src.Locale],
            Metadata = new Dictionary<string, string> { { "type", "customer" }, { "customerId", src.Id } }
        };

    public Database.Entities.StripePaymentMethod MapTo(PaymentMethod paymentMethod, string setupIntentId, Database.Entities.Customer customer) =>
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
            Customer = customer
        };

    private static Identity MapToEntity(Models.Identity src) => new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<Location> MapToLocations(IEnumerable<Database.Entities.Location?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static IEnumerable<Models.Identity> MapToIdentities(IEnumerable<Identity>? src) =>
        (src is null ? [] : src.Select(MapTo))!;

    private static Models.Identity? MapTo(Identity? src) =>
        src is null
            ? null
            : new Models.Identity
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                Email = src.Email,
                EmailVerified = src.EmailVerified
            };

    private static Organization? MapTo(Database.Entities.Organization? src) =>
        src is null
            ? null
            : new Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                CustomDomain = src.CustomDomain,
                Type = src.Type.ToOrganizationType(),
                IsOwnershipVerified = src.IsOwnershipVerified
            };

    private static Location? MapTo(Database.Entities.Location? src) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Organization = MapTo(src.Organization),
                Type = src.Type.ToNullableLocationType(),
                Resources = MapToResources(src.Resources).ToList()
            };

    private static IEnumerable<OrganizationTag> MapToOrganizationTags(IEnumerable<Database.Entities.OrganizationTag?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static OrganizationTag? MapTo(Database.Entities.OrganizationTag? src) =>
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
                Type = src.Type.ToNullableOrganizationTagType(),
                Color = src.Color,
                Organization = new Organization { Id = src.Organization.Id }
            };

    private static IEnumerable<Models.Resource> MapToResources(IEnumerable<Resource?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Models.Resource? MapTo(Resource? src) =>
        src is null
            ? null
            : new Models.Resource
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Location = MapTo(src.Location)!
            };

    private static StripeCustomer? MapTo(Database.Entities.StripeCustomer? src) =>
        src is null
            ? null
            : new StripeCustomer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                StripeCustomerId = src.StripeCustomerId
            };

    private static StripePaymentMethod MapToModel(Database.Entities.StripePaymentMethod src) =>
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
            CardLastFourDigit = src.CardLastFourDigit
        };
}
