using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Customer.Api.GraphQL.Billing;
using Customer.Api.GraphQL.Customer;
using Customer.Api.GraphQL.Feedback;
using Customer.Api.GraphQL.Payment;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using HotChocolate.Types.Pagination;
using NetTopologySuite.Geometries;
using Stripe;
using CustomerBillingDetails = Customer.Shared.Models.CustomerBillingDetails;
using CustomerFeedback = Customer.Shared.Models.CustomerFeedback;
using Identity = Customer.Shared.Database.Entities.Identity;
using IdentityType = Api.Shared.Services.Grpc.Skedular.Customer.V1.IdentityType;
using Location = Customer.Shared.Models.Location;
using Organization = Customer.Shared.Models.Organization;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using PersonalInformationVisibility = Api.Shared.Services.Models.PersonalInformationVisibility;
using Resource = Customer.Shared.Database.Entities.Resource;

namespace Customer.Api.Mappers;

public interface IMapper
{
    Shared.Models.Customer MapTo(IContext context);
    Identity MapToIdentity(IContext context);
    CustomerDetails MapTo(Shared.Models.Customer src);
    CustomerFeedback MapTo(SubmitCustomerFeedbackInput src);
    Shared.Models.Customer MapTo(Shared.Database.Entities.Customer src);

    Shared.Database.Entities.Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Resource> preferredResources,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags,
        ICollection<Shared.Database.Entities.Location> favouriteLocations);

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
    CustomerCreateOptions MapToStripeCustomerCreateOption(Shared.Database.Entities.Customer src);
    Shared.Database.Entities.CustomerBillingDetails MapTo(CustomerBillingDetails src, Shared.Database.Entities.Customer customer);

    Shared.Database.Entities.CustomerBillingDetails MergeToEntity(
        CustomerBillingDetails src,
        Shared.Database.Entities.CustomerBillingDetails dest,
        Shared.Database.Entities.Customer customer);

    CustomerBillingDetails MapTo(AddMyBillingDetailsInput src);
    CustomerBillingDetails MapTo(UpdateMyBillingDetailsInput src);
    IEnumerable<CustomerPaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src);
    IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Shared.Database.Entities.StripePaymentMethod> src);
    CustomerBillingDetails? MapTo(Shared.Database.Entities.CustomerBillingDetails? src);
    GraphQL.Billing.CustomerBillingDetails? MapToGraphQl(CustomerBillingDetails? src);
}

public class Mapper : IMapper
{
    public Shared.Models.Customer MapTo(IContext context) =>
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
                        EmailVerified = context.GetEmailVerified(),
                        Type = global::Api.Shared.Services.Models.IdentityType.Registered
                    }
                },
            IsOnboardingDone = false,
            DefaultOrganization = null,
            PreferredLocations = [],
            PreferredOrganizationTags = [],
            PreferredResources = [],
            FavouriteLocations = [],
            PersonalInformationVisibility = PersonalInformationVisibility.Visible
        };

    public Identity MapToIdentity(IContext context) =>
        new()
        {
            Id = context.GetVerifiableToken().ToSafeString(),
            Email = context.GetEmail(),
            EmailVerified = context.GetEmailVerified(),
            Type = IdentityTypeConstants.Registered
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
            Email = src.Identities.ToFirstEmail(),
            Emails = src.Identities.ToEmails(),
            Identities = MapTo(src.Identities),
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganizationId = src.DefaultOrganization?.Id,
            DefaultOrganizationUniqueAlphanumericName = src.DefaultOrganization?.UniqueAlphanumericName,
            PreferredLocationIds = src.PreferredLocations.Select(item => item.Id),
            PreferredZoneIds = src.PreferredOrganizationTags.Where(item => item.Type == OrganizationTagType.Zone).Select(item => item.Id),
            PreferredCustomTagIds = src.PreferredOrganizationTags.Where(item => item.Type == OrganizationTagType.Custom).Select(item => item.Id),
            PreferredResourceIds = src.PreferredResources.Select(item => item.Id),
            FavouriteLocationIds = src.FavouriteLocations.Select(item => item.Id),
            PersonalInformationVisibility = new PersonalInformationVisibilityDetails
            {
                Type = src.PersonalInformationVisibility, Name = src.PersonalInformationVisibility.ToPersonalInformationVisibilityName()
            }
        };

    public CustomerFeedback MapTo(SubmitCustomerFeedbackInput src) =>
        new() { Id = src.Id.ToSafeString(), Content = src.FeedbackContent, Channel = src.Channel };

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
            BillingDetails = MapTo(src.BillingDetails),
            IsOnboardingDone = src.IsOnboardingDone,
            Identities = MapTo(src.Identities).ToList(),
            DefaultOrganization = MapTo(src.DefaultOrganization),
            PreferredLocations = MapTo(src.PreferredLocations).ToList(),
            PreferredResources = MapTo(src.PreferredResources).ToList(),
            PreferredOrganizationTags = MapTo(src.PreferredOrganizationTags).ToList(),
            FavouriteLocations = MapTo(src.FavouriteLocations).ToList(),
            StripeCustomer = MapTo(src.StripeCustomer),
            StripePaymentMethods = MapTo(src.StripePaymentMethods).ToList(),
            PersonalInformationVisibility = src.PersonalInformationVisibility.ToPersonalInformationVisibility()
        };

    public CustomerFeedback MapTo(Shared.Database.Entities.CustomerFeedback src) =>
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

    public Shared.Database.Entities.CustomerFeedback MapTo(
        CustomerFeedback src,
        Shared.Database.Entities.Customer customer) =>
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
            Identities = src.Identities
                .Select(item => new Shared.Models.Identity
                {
                    Id = item.Id,
                    Email = item.Email.ToSafeString(),
                    EmailVerified = item.EmailVerified,
                    Type = item.Type switch
                    {
                        IdentityType.Guest => global::Api.Shared.Services.Models.IdentityType.Guest,
                        IdentityType.Registered => global::Api.Shared.Services.Models.IdentityType.Registered,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                })
                .ToList(),
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganization = string.IsNullOrWhiteSpace(src.DefaultOrganizationId) ? null : new Organization { Id = src.DefaultOrganizationId },
            PreferredLocations = src.PreferredLocations.Select(item =>
                    new Location { Id = item.Id, Organization = new Organization { Id = item.Organization.Id } })
                .ToList(),
            PreferredResources = src.PreferredResources
                .Select(item => new Shared.Models.Resource { Id = item.Id, Location = new Location { Id = item.Location.Id } })
                .ToList(),
            PreferredOrganizationTags = src.PreferredOrganizationTags
                .Select(item => new OrganizationTag { Id = item.Id, Organization = new Organization { Id = item.Organization.Id } })
                .ToList(),
            FavouriteLocations = [],
            PersonalInformationVisibility = src.PersonalInformationVisibility switch
            {
                global::Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility.Visible => PersonalInformationVisibility.Visible,
                global::Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility.Redacted =>
                    PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException()
            }
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
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganizationId = src.DefaultOrganization is null ? string.Empty : src.DefaultOrganization.Id.ToSafeString(),
            DisplayableName = src.DisplayableName.ToSafeString(),
            PersonalInformationVisibility = src.PersonalInformationVisibility switch
            {
                PersonalInformationVisibility.Visible => global::Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility.Visible,
                PersonalInformationVisibility.Redacted =>
                    global::Api.Shared.Services.Grpc.Skedular.Customer.V1.PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        customer.Identities.AddRange(src.Identities.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Identity
            {
                Id = item.Id,
                Email = item.Email.ToSafeString(),
                EmailVerified = item.EmailVerified ?? false,
                Type = item.Type switch
                {
                    global::Api.Shared.Services.Models.IdentityType.Guest => IdentityType.Guest,
                    global::Api.Shared.Services.Models.IdentityType.Registered => IdentityType.Registered,
                    _ => throw new ArgumentOutOfRangeException()
                }
            }));

        customer.PreferredLocationIds.AddRange(src.PreferredLocations.Select(item => item.Id));
        customer.PreferredResourceIds.AddRange(src.PreferredResources.Select(item => item.Id));
        customer.PreferredOrganizationTagIds.AddRange(src.PreferredOrganizationTags.Select(item => item.Id));
        customer.FavouriteLocationIds.AddRange(src.FavouriteLocations.Select(item => item.Id));

        return customer;
    }

    public Identity MapTo(Shared.Models.Identity src, Shared.Database.Entities.Customer customer) => MergeTo(src, new Identity(), customer);

    public Identity MergeTo(Shared.Models.Identity src, Identity dest, Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.Email = src.Email;
        dest.EmailVerified = src.EmailVerified;
        dest.Type = src.Type.ToIdentityType();
        dest.Customer = customer;
        return dest;
    }

    public Shared.Models.Identity MapTo(Admin_AddIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Type = src.Type switch
            {
                IdentityType.Guest => global::Api.Shared.Services.Models.IdentityType.Guest,
                IdentityType.Registered => global::Api.Shared.Services.Models.IdentityType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = src.CustomerId }
        };

    public Shared.Models.Identity MapTo(Admin_UpdateIdentityInput src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified,
            Type = src.Type switch
            {
                IdentityType.Guest => global::Api.Shared.Services.Models.IdentityType.Guest,
                IdentityType.Registered => global::Api.Shared.Services.Models.IdentityType.Registered,
                _ => throw new ArgumentOutOfRangeException()
            },
            Customer = new Shared.Models.Customer { Id = src.CustomerId }
        };

    public Edge<Shared.Models.Customer> MapTo(Edge<Shared.Database.Entities.Customer> src) => new(MapTo(src.Node), src.Cursor);

    public CustomerEdge MapTo(Edge<Shared.Models.Customer> src) => new(MapTo(src.Node), src.Cursor);

    public CustomerCreateOptions MapToStripeCustomerCreateOption(Shared.Database.Entities.Customer src) =>
        new()
        {
            Name = src.ToDisplayableName(),
            Email = src.Identities.ToSingleEmail(),
            Phone = src.PhoneNumber.ToSafeString(),
            PreferredLocales = string.IsNullOrWhiteSpace(src.Locale) ? [] : [src.Locale],
            Metadata = new Dictionary<string, string> { { "type", "customer" }, { "customerId", src.Id } }
        };

    public Shared.Database.Entities.CustomerBillingDetails MapTo(CustomerBillingDetails src, Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new Shared.Database.Entities.CustomerBillingDetails(), customer);

    public Shared.Database.Entities.CustomerBillingDetails MergeToEntity(
        CustomerBillingDetails src,
        Shared.Database.Entities.CustomerBillingDetails dest,
        Shared.Database.Entities.Customer customer)
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

    public CustomerBillingDetails MapTo(AddMyBillingDetailsInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            CompanyName = src.CompanyName,
            Email = src.Email,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode
        };

    public CustomerBillingDetails MapTo(UpdateMyBillingDetailsInput src) =>
        new()
        {
            Id = src.Id,
            CompanyName = src.CompanyName,
            Email = src.Email,
            OsmType = src.OsmType,
            OsmId = src.OsmId,
            PlaceId = src.PlaceId,
            Coordinates = src.Longitude is null || src.Latitude is null ? null : new Point(new Coordinate(src.Longitude.Value, src.Latitude.Value)),
            FormattedAddress = src.FormattedAddress,
            AddressLine1 = src.AddressLine1,
            AddressLine2 = src.AddressLine2,
            Suburb = src.Suburb,
            City = src.City,
            Province = src.Province,
            Zipcode = src.Zipcode,
            Country = src.Country,
            CountryCode = src.CountryCode
        };

    public IEnumerable<CustomerPaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src) => src.Select(MapTo);

    public IEnumerable<StripePaymentMethod> MapTo(IEnumerable<Shared.Database.Entities.StripePaymentMethod> src) => src.Select(MapTo);

    public CustomerBillingDetails? MapTo(Shared.Database.Entities.CustomerBillingDetails? src) =>
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

    public GraphQL.Billing.CustomerBillingDetails? MapToGraphQl(CustomerBillingDetails? src) =>
        src is null
            ? null
            : new GraphQL.Billing.CustomerBillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName,
                Email = src.Email,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Longitude = src.Coordinates?.X,
                Latitude = src.Coordinates?.Y,
                FormattedAddress = src.ToFormattedAddress(),
                MultilinesFormattedAddress = src.ToMultilinesFormattedAddress(),
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode
            };

    public IEnumerable<Identity> MapToEntity(IEnumerable<Shared.Models.Identity> src) => src.Select(MapToEntity);

    public Shared.Database.Entities.Customer MapToEntity(
        Shared.Models.Customer src,
        ICollection<Identity> identities,
        Shared.Database.Entities.Organization? defaultOrganization,
        ICollection<Shared.Database.Entities.Location> preferredLocations,
        ICollection<Resource> preferredResources,
        ICollection<Shared.Database.Entities.OrganizationTag> preferredOrganizationTags,
        ICollection<Shared.Database.Entities.Location> favouriteLocations) =>
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
            Identities = identities,
            DefaultOrganization = defaultOrganization,
            PreferredLocations = preferredLocations,
            PreferredResources = preferredResources,
            PreferredOrganizationTags = preferredOrganizationTags,
            FavouriteLocations = favouriteLocations,
            PersonalInformationVisibility = src.PersonalInformationVisibility.ToPersonalInformationVisibility()
        };

    private static Identity MapToEntity(Shared.Models.Identity src) => new()
    {
        Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified, Type = src.Type.ToIdentityType()
    };

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
                EmailVerified = src.EmailVerified,
                Type = src.Type.ToIdentityType()
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
                UniqueAlphanumericName = src.UniqueAlphanumericName,
                Type = src.Type.ToOrganizationType(),
                IsOwnershipVerified = src.IsOwnershipVerified
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
                Organization = MapTo(src.Organization),
                Type = src.Type.ToNullableLocationType(),
                Resources = MapTo(src.Resources).ToList()
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
                Type = src.Type.ToNullableOrganizationTagType(),
                Organization = new Organization { Id = src.Organization.Id }
            };

    private static IEnumerable<Shared.Models.Resource> MapTo(IEnumerable<Resource?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Shared.Models.Resource? MapTo(Resource? src) =>
        src is null
            ? null
            : new Shared.Models.Resource
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Location = src.Location is null ? null : new Location { Id = src.Location.Id }
            };

    private static IEnumerable<CustomerIdentity> MapTo(IEnumerable<Shared.Models.Identity> src) => src.Select(MapTo);

    private static CustomerIdentity MapTo(Shared.Models.Identity src) =>
        new() { Id = src.Id, Email = src.Email, Verified = src.EmailVerified ?? false, Type = src.Type };

    private static StripeCustomer? MapTo(Shared.Database.Entities.StripeCustomer? src) =>
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

    private static StripePaymentMethod MapTo(Shared.Database.Entities.StripePaymentMethod src) =>
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

    private static CustomerPaymentMethod MapTo(StripePaymentMethod src) =>
        new()
        {
            Id = src.Id,
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
