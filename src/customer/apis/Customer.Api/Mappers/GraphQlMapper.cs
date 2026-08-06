using Api.Shared.Services.Models;
using Customer.Api.GraphQL.Billing;
using Customer.Api.GraphQL.Customer;
using Customer.Api.GraphQL.Feedback;
using Customer.Api.GraphQL.Payment;
using Customer.Shared.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using NetTopologySuite.Geometries;
using CustomerBillingDetails = Customer.Shared.Models.CustomerBillingDetails;
using CustomerFeedback = Customer.Shared.Models.CustomerFeedback;
using OrganizationTag = Customer.Shared.Models.OrganizationTag;
using StripePaymentMethod = Customer.Shared.Models.StripePaymentMethod;

namespace Customer.Api.Mappers;

public interface IGraphQlMapper
{
    CustomerDetails MapTo(Shared.Models.Customer src);
    CustomerFeedback MapTo(SubmitCustomerFeedbackInput src);
    CustomerBillingDetails MapTo(AddMyBillingDetailsInput src);
    CustomerBillingDetails MapTo(UpdateMyBillingDetailsInput src);
    CustomerEdge MapTo(Edge<Shared.Models.Customer> src);
    GraphQL.Billing.CustomerBillingDetails? MapToGraphQl(CustomerBillingDetails? src);
    IEnumerable<CustomerPaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src);
}

public class GraphQlMapper : IGraphQlMapper
{
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
            Identities = MapToIdentities(src.Identities),
            IsOnboardingDone = src.IsOnboardingDone,
            DefaultOrganizationId = src.DefaultOrganization?.Id,
            DefaultOrganizationCustomDomain = src.DefaultOrganization?.CustomDomain,
            PreferredLocationIds = src.PreferredLocations.Select(item => item.Id),
            PreferredZones = MapToTagDetails(src.PreferredOrganizationTags.Where(item => item.Type == OrganizationTagType.Zone)),
            PreferredCustomTags = MapToTagDetails(src.PreferredOrganizationTags.Where(item => item.Type == OrganizationTagType.Custom)),
            PreferredResourceIds = src.PreferredResources.Select(item => item.Id),
            FavouriteLocationIds = src.FavouriteLocations.Select(item => item.Id),
            PersonalInformationVisibility = new PersonalInformationVisibilityDetails
            {
                Type = src.PersonalInformationVisibility,
                Name = src.PersonalInformationVisibility.ToPersonalInformationVisibilityName(),
            },
            Type = src.Type,
        };

    public CustomerFeedback MapTo(SubmitCustomerFeedbackInput src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Content = src.FeedbackContent,
            Channel = src.Channel,
        };

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
            CountryCode = src.CountryCode,
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
            CountryCode = src.CountryCode,
        };

    public CustomerEdge MapTo(Edge<Shared.Models.Customer> src) => new(MapTo(src.Node), src.Cursor);

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
                CountryCode = src.CountryCode,
            };

    public IEnumerable<CustomerPaymentMethod> MapTo(IEnumerable<StripePaymentMethod> src) => src.Select(MapTo);

    private static IEnumerable<CustomerIdentity> MapToIdentities(IEnumerable<Identity> src) => src.Select(MapToIdentity);

    private static CustomerIdentity MapToIdentity(Identity src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email,
            Verified = src.EmailVerified ?? false,
        };

    private static IEnumerable<OrganizationTagDetails> MapToTagDetails(IEnumerable<OrganizationTag> src) => src.Select(MapToTagDetails);

    private static OrganizationTagDetails MapToTagDetails(OrganizationTag src) => new()
    {
        Id = src.Id,
        Name = src.Name.ToSafeString(),
        Type = src.Type,
        Color = src.Color.ToSafeString(),
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
            CardLastFourDigit = src.CardLastFourDigit,
        };
}
