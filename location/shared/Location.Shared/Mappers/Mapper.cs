using Api.Shared.Clients.Events.Skedular.Location.V1.Value;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Location.Shared.Models;
using CdnFile = Api.Shared.Clients.Events.Skedular.Location.V1.Value.CdnFile;
using CdnImageFile = Api.Shared.Clients.Events.Skedular.Location.V1.Value.CdnImageFile;
using LocationType = Api.Shared.Services.Models.LocationType;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using Resource = Api.Shared.Clients.Events.Skedular.Location.V1.Value.Resource;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;

namespace Location.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location MapTo(Models.Location src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location MapTo(Models.Location src)
    {
        var location = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.Location
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Timezone = src.Timezone.ToSafeString(),
            Type = src.Type switch
            {
                LocationType.Private => Api.Shared.Clients.Events.Skedular.Location.V1.Value.LocationType.Private,
                LocationType.Marketplace => Api.Shared.Clients.Events.Skedular.Location.V1.Value.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            OrganizationId = src.Organization.Id,
            OpeningHours = MapTo(src.OpeningHours),
            PhysicalAddress = MapTo(src.PhysicalAddress)
        };

        location.Resources.AddRange(src.Resources.Select(item =>
        {
            var resource = new Resource
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Inactive = item.Inactive,
                RequireBookingApproval = item.RequireBookingApproval,
                Color = item.Color.ToSafeString(),
                Capacity = item.Capacity,
                IsAvailableHoursOverridden = item.IsAvailableHoursOverridden,
                AvailableHours = item.AvailableHours is null ? null : MapTo(item.AvailableHours)
            };

            resource.TagIds.AddRange(item.Tags.Select(tag => tag.Id));

            return resource;
        }));

        location.TagIds.AddRange(src.SpaceTypes.Select(tag => tag.Id));
        location.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return location;
    }

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours MapTo(OpeningHours? src)
    {
        if (src is null)
        {
            return new Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours
            {
                WeekOpeningHours = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.WeekOpeningHours
                {
                    Monday = MapToDefault(),
                    Tuesday = MapToDefault(),
                    Wednesday = MapToDefault(),
                    Thursday = MapToDefault(),
                    Friday = MapToDefault(),
                    Saturday = MapToDefault(),
                    Sunday = MapToDefault()
                }
            };
        }

        var openingHours = new Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHours { WeekOpeningHours = MapTo(src.WeekOpeningHours) };
        openingHours.ClosedDates.AddRange(src.ClosedDates.Select(item => item.ToTimestamp()));
        openingHours.DatesWithVariedOpeningHours.AddRange(src.DatesWithVariedOpeningHours.ToList().Select(item => new VariedDateOpeningHours
        {
            Date = item.Key.ToTimestamp(), OpeningHoursDetails = MapTo(item.Value)
        }));

        return openingHours;
    }

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.WeekOpeningHours MapTo(WeekOpeningHours src) =>
        new()
        {
            Monday = MapTo(src.Monday),
            Tuesday = MapTo(src.Tuesday),
            Wednesday = MapTo(src.Wednesday),
            Thursday = MapTo(src.Thursday),
            Friday = MapTo(src.Friday),
            Saturday = MapTo(src.Saturday),
            Sunday = MapTo(src.Sunday)
        };

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails MapTo(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static Api.Shared.Clients.Events.Skedular.Location.V1.Value.OpeningHoursDetails MapToDefault() =>
        new() { Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty };

    private static PhysicalAddress? MapTo(LocationPhysicalAddress? src) =>
        src is null
            ? null
            : new PhysicalAddress
            {
                Id = src.Id,
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
                Coordinates = src.Coordinates is null ? null : new Coordinates { Longitude = src.Coordinates.X, Latitude = src.Coordinates.Y }
            };

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo)!;

    private static CdnImageFile? MapTo(Api.Shared.Services.Models.CdnImageFile? src) =>
        src is null ? null : new CdnImageFile { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };
}
