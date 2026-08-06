using Api.Shared.Clients.Events.Skedular.Customer.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using CustomerBillingDetails = Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerBillingDetails;
using CustomerType = Api.Shared.Services.Models.CustomerType;
using Identity = Api.Shared.Clients.Events.Skedular.Customer.V1.Identity;
using Location = Api.Shared.Clients.Events.Skedular.Customer.V1.Location;
using OrganizationTag = Api.Shared.Clients.Events.Skedular.Customer.V1.OrganizationTag;
using PersonalInformationVisibility = Api.Shared.Services.Models.PersonalInformationVisibility;
using Resource = Api.Shared.Clients.Events.Skedular.Customer.V1.Resource;

namespace Customer.Shared.Mappers;

public interface IEventMapper
{
    Api.Shared.Clients.Events.Skedular.Customer.V1.Customer MapTo(Models.Customer src);
}

public class EventMapper : IEventMapper
{
    public Api.Shared.Clients.Events.Skedular.Customer.V1.Customer MapTo(Models.Customer src)
    {
        var customer = new Api.Shared.Clients.Events.Skedular.Customer.V1.Customer
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Title = src.Title.ToSafeString(),
            Designation = src.Designation.ToSafeString(),
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
            BillingDetails = MapTo(src.BillingDetails),
            Settings = new Settings
            {
                IsOnboardingDone = src.IsOnboardingDone,
            },
            PreferredOrganizationId = src.DefaultOrganization is null ? string.Empty : src.DefaultOrganization.Id,
            PersonalInformationVisibility = src.PersonalInformationVisibility switch
            {
                PersonalInformationVisibility.Visible => Api.Shared.Clients.Events.Skedular.Customer.V1.PersonalInformationVisibility.Visible,
                PersonalInformationVisibility.Redacted => Api.Shared.Clients.Events.Skedular.Customer.V1.PersonalInformationVisibility.Redacted,
                _ => throw new ArgumentOutOfRangeException(nameof(src.PersonalInformationVisibility), src.PersonalInformationVisibility,
                    $"Unexpected value for {nameof(src.PersonalInformationVisibility)}: {src.PersonalInformationVisibility}. Update enum mapping or caller input."),
            },
            Type = src.Type switch
            {
                CustomerType.Guest => Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerType.Guest,
                CustomerType.Registered => Api.Shared.Clients.Events.Skedular.Customer.V1.CustomerType.Registered,
                _ => throw new ArgumentOutOfRangeException(nameof(src.Type), src.Type,
                    $"Unexpected value for {nameof(src.Type)}: {src.Type}. Update enum mapping or caller input."),
            },
        };

        customer.Identities.AddRange(MapTo(src.Identities));
        customer.PreferredLocations.AddRange(
            src.PreferredLocations.Select(item =>
                new Location
                {
                    Id = item.Id,
                    OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id,
                })
        );
        customer.PreferredResources.AddRange(
            src.PreferredResources.Select(item =>
                new Resource
                {
                    Id = item.Id,
                    LocationId = item.Location is null ? string.Empty : item.Location.Id,
                }));
        customer.PreferredOrganizationTags.AddRange(
            src.PreferredOrganizationTags.Select(item => new OrganizationTag
            {
                Id = item.Id,
                OrganizationId = item.Organization.Id,
            })
        );
        customer.FavouriteLocations.AddRange(
            src.FavouriteLocations.Select(item =>
                new Location
                {
                    Id = item.Id,
                    OrganizationId = item.Organization is null ? string.Empty : item.Organization.Id,
                })
        );

        return customer;
    }

    private static IEnumerable<Identity> MapTo(IEnumerable<Models.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Models.Identity src) =>
        new()
        {
            Id = src.Id,
            Email = src.Email.ToSafeString(),
            EmailVerified = src.EmailVerified ?? false,
        };

    private static CustomerBillingDetails? MapTo(Models.CustomerBillingDetails? src) =>
        src is null
            ? null
            : new CustomerBillingDetails
            {
                Id = src.Id,
                CompanyName = src.CompanyName.ToSafeString(),
                Email = src.Email.ToSafeString(),
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString(),
                CountryCode = src.CountryCode.ToSafeString(),
                FormattedAddress = src.ToFormattedAddress(),
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null
                    ? null
                    : new Coordinates
                    {
                        Longitude = src.Coordinates.X,
                        Latitude = src.Coordinates.Y,
                    },
            };
}
