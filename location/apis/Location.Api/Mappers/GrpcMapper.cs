using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Api.Shared.Grpc.Skedular.Location.Resources.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using Location.Shared.Models;
using NetTopologySuite.Geometries;
using AreaRange = Api.Shared.Services.Models.AreaRange;
using CdnFile = Api.Shared.Grpc.Skedular.Location.Core.V1.CdnFile;
using CdnImageFile = Api.Shared.Grpc.Skedular.Location.Core.V1.CdnImageFile;
using ContactDetails = Api.Shared.Services.Models.ContactDetails;
using Coordinates = Api.Shared.Grpc.Skedular.Location.Core.V1.Coordinates;
using ListingMetadata = Api.Shared.Services.Models.ListingMetadata;
using LocationType = Api.Shared.Grpc.Skedular.Location.Core.V1.LocationType;
using OpeningHours = Api.Shared.Services.Models.OpeningHours;
using OpeningHoursDetails = Api.Shared.Services.Models.OpeningHoursDetails;
using PeopleCapacity = Api.Shared.Services.Models.PeopleCapacity;
using Permissions = Api.Shared.Grpc.Skedular.Location.Core.V1.Permissions;
using Resource = Api.Shared.Grpc.Skedular.Location.Core.V1.Resource;
using VariedDateOpeningHours = Api.Shared.Grpc.Skedular.Location.Core.V1.VariedDateOpeningHours;
using WeekOpeningHours = Api.Shared.Services.Models.WeekOpeningHours;

namespace Location.Api.Mappers;

public interface IGrpcMapper
{
    global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location MapToGrpcResponse(Shared.Models.Location src);
    Resource MapToGrpcResponse(Shared.Models.Resource src);
    LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src);
    ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src);
    Shared.Models.Location MapTo(Admin_AddInput src);
    Shared.Models.Location MapTo(Admin_UpdateInput src);
    Shared.Models.Location MapTo(AddInput src);
    Shared.Models.Location MapTo(UpdateInput src);
    Shared.Models.Resource MapTo(AddResourceInput src);
    Shared.Models.Resource MapTo(UpdateResourceInput src);
}

public class GrpcMapper : IGrpcMapper
{
    public global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location MapToGrpcResponse(Shared.Models.Location src)
    {
        var location = new global::Api.Shared.Grpc.Skedular.Location.Core.V1.Location
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone.ToSafeString(),
            Type = src.Type switch
            {
                global::Api.Shared.Services.Models.LocationType.Private => LocationType.Private,
                global::Api.Shared.Services.Models.LocationType.Marketplace => LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            OpeningHours = MapToGrpcResponse(src.OpeningHours),
            OrganizationId = src.Organization.Id,
            Permissions =
                new Permissions
                {
                    CanView = src.Permissions.CanView,
                    CanModify = src.Permissions.CanModify,
                    CanDelete = src.Permissions.CanDelete,
                    CanViewAnalytics = src.Permissions.CanViewAnalytics
                },
            ExtraMetadata = MapTo(src.ExtraMetadata),
            PhysicalAddress = MapToGrpcResponse(src.PhysicalAddress),
            UniqueClaimCode = src.UniqueClaimCode.ToSafeString(),
            ContactedViaEmail = src.ContactedViaEmail,
            ContactedViaSms = src.ContactedViaSms,
            ContactedViaCall = src.ContactedViaCall,
            ContactedViaWhatsapp = src.ContactedViaWhatsapp
        };

        location.Resources.AddRange(MapToGrpcResponse(src.Resources));
        location.CustomTagIds.AddRange(src.CustomTags.Select(item => item.Id));
        location.ZoneIds.AddRange(src.Zones.Select(item => item.Id));
        location.SpaceTypeIds.AddRange(src.SpaceTypes.Select(item => item.Id));
        location.FeatureImages.AddRange(MapTo(src.FeatureImages));

        return location;
    }

    public Resource MapToGrpcResponse(Shared.Models.Resource src)
    {
        var resource = new Resource
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden,
            AvailableHours = src.AvailableHours is null ? null : MapToGrpcResponse(src.AvailableHours),
            ResourceTypeId = src.Tags.First(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type)).Id
        };

        resource.CustomTagIds.AddRange(src.Tags.Where(item => item.Type == OrganizationTagType.Custom).Select(item => item.Id));
        resource.ZoneIds.AddRange(src.Tags.Where(item => item.Type == OrganizationTagType.Zone).Select(item => item.Id));
        resource.ProductTagIds.AddRange(src.Tags.Where(item => item.Type == OrganizationTagType.Product).Select(item => item.Id));

        return resource;
    }

    public LocationEdge MapToGrpcResponse(Edge<Shared.Models.Location> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public ResourceEdge MapToGrpcResponse(Edge<Shared.Models.Resource> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public Shared.Models.Location MapTo(Admin_AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Organization { Id = src.OrganizationId },
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            ExtraMetadata = MapTo(src.ExtraMetadata),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            UniqueClaimCode = src.UniqueClaimCode.ToSafeString()
        };

    public Shared.Models.Location MapTo(Admin_UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Organization { Id = src.OrganizationId },
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            ExtraMetadata = MapTo(src.ExtraMetadata),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            UniqueClaimCode = src.UniqueClaimCode.ToSafeString()
        };

    public Shared.Models.Location MapTo(AddInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Organization { Id = src.OrganizationId },
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            ExtraMetadata = MapTo(src.ExtraMetadata),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            UniqueClaimCode = src.UniqueClaimCode.ToSafeString()
        };

    public Shared.Models.Location MapTo(UpdateInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = MapTo(src.ListingMetadata),
            Timezone = src.Timezone,
            Type = src.Type switch
            {
                LocationType.Private => global::Api.Shared.Services.Models.LocationType.Private,
                LocationType.Marketplace => global::Api.Shared.Services.Models.LocationType.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            FeatureImages = MapTo(src.FeatureImages).ToList(),
            Organization = new Organization { Id = src.OrganizationId },
            OrganizationTags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList(),
            ExtraMetadata = MapTo(src.ExtraMetadata),
            PhysicalAddress = MapTo(src.PhysicalAddress),
            UniqueClaimCode = src.UniqueClaimCode.ToSafeString()
        };

    public Shared.Models.Resource MapTo(AddResourceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            Location = new Shared.Models.Location { Id = src.LocationId },
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    public Shared.Models.Resource MapTo(UpdateResourceInput src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color.ToSafeString(),
            Capacity = src.Capacity,
            Tags = src.TagIds.Select(item => new OrganizationTag { Id = item }).ToList()
        };

    private IEnumerable<Resource> MapToGrpcResponse(
        IEnumerable<Shared.Models.Resource> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.OpeningHours MapToGrpcResponse(OpeningHours? src)
    {
        if (src is null)
        {
            return new global::Api.Shared.Grpc.Skedular.Location.Core.V1.OpeningHours
            {
                WeekOpeningHours = new global::Api.Shared.Grpc.Skedular.Location.Core.V1.WeekOpeningHours
                {
                    Monday = MapToGrpcDefault(),
                    Tuesday = MapToGrpcDefault(),
                    Wednesday = MapToGrpcDefault(),
                    Thursday = MapToGrpcDefault(),
                    Friday = MapToGrpcDefault(),
                    Saturday = MapToGrpcDefault(),
                    Sunday = MapToGrpcDefault()
                }
            };
        }

        var openingHours =
            new global::Api.Shared.Grpc.Skedular.Location.Core.V1.OpeningHours { WeekOpeningHours = MapToGrpcResponse(src.WeekOpeningHours) };
        openingHours.ClosedDates.AddRange(src.ClosedDates.Select(item => item.ToTimestamp()));
        openingHours.DatesWithVariedOpeningHours.AddRange(src.DatesWithVariedOpeningHours.Select(item => new VariedDateOpeningHours
        {
            Date = item.Key.ToTimestamp(), OpeningHoursDetails = MapToGrpcResponse(item.Value)
        }));

        return openingHours;
    }

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.WeekOpeningHours MapToGrpcResponse(WeekOpeningHours src) =>
        new()
        {
            Monday = MapToGrpcResponse(src.Monday),
            Tuesday = MapToGrpcResponse(src.Tuesday),
            Wednesday = MapToGrpcResponse(src.Wednesday),
            Thursday = MapToGrpcResponse(src.Thursday),
            Friday = MapToGrpcResponse(src.Friday),
            Saturday = MapToGrpcResponse(src.Saturday),
            Sunday = MapToGrpcResponse(src.Sunday)
        };

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.OpeningHoursDetails MapToGrpcResponse(OpeningHoursDetails src) =>
        new()
        {
            Closed = src.Closed,
            OpenAllDay = src.OpenAllDay,
            From = src.From is null ? string.Empty : $"{src.From.Value.Hour}:{src.From.Value.Minute}",
            Until = src.Until is null ? string.Empty : $"{src.Until.Value.Hour}:{src.Until.Value.Minute}"
        };

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.OpeningHoursDetails MapToGrpcDefault() =>
        new() { Closed = false, OpenAllDay = true, From = string.Empty, Until = string.Empty };

    private static IEnumerable<global::Api.Shared.Services.Models.CdnImageFile> MapTo(IEnumerable<CdnImageFile> src) =>
        src.Select(MapTo);

    private static global::Api.Shared.Services.Models.CdnImageFile MapTo(CdnImageFile src) => new(MapTo(src.Original), MapTo(src.Thumbnail));

    private static global::Api.Shared.Services.Models.CdnFile? MapTo(CdnFile? src) =>
        src is null ? null : new global::Api.Shared.Services.Models.CdnFile(src.Url, src.Height.FromNullInt(), src.Width.FromNullInt());

    private static IEnumerable<CdnImageFile> MapTo(IEnumerable<global::Api.Shared.Services.Models.CdnImageFile> src) =>
        src.Select(MapTo);

    private static CdnImageFile MapTo(global::Api.Shared.Services.Models.CdnImageFile src) =>
        new() { Original = MapTo(src.Original), Thumbnail = MapTo(src.Thumbnail) };

    private static CdnFile? MapTo(global::Api.Shared.Services.Models.CdnFile? src) =>
        src is null ? null : new CdnFile { Url = src.Url.ToSafeString(), Height = src.Height.ToNullInt(), Width = src.Width.ToNullInt() };

    private static LocationExtraMetadata? MapTo(ExtraMetadata? src) =>
        src is null
            ? null
            : new LocationExtraMetadata(
                MapTo(src.ContactDetails),
                MapTo(src.AreaRange),
                MapTo(src.PeopleCapacity),
                src.Website,
                src.RelatedImageLinks,
                src.RelatedVideoLinks,
                src.OtherLinks);

    private static ContactDetails? MapTo(global::Api.Shared.Grpc.Skedular.Location.Core.V1.ContactDetails? src) =>
        src is null ? null : new ContactDetails(src.ContactPeople, src.ContactEmails, src.ContactPhones);

    private static AreaRange? MapTo(global::Api.Shared.Grpc.Skedular.Location.Core.V1.AreaRange? src) =>
        src is null ? null : new AreaRange(src.FromInSqm, src.ToInSqm);

    private static PeopleCapacity? MapTo(global::Api.Shared.Grpc.Skedular.Location.Core.V1.PeopleCapacity? src) =>
        src is null ? null : new PeopleCapacity(src.From, src.To);

    private static ExtraMetadata? MapTo(LocationExtraMetadata? src)
    {
        if (src is null)
        {
            return null;
        }

        var extraMetadata = new ExtraMetadata
        {
            ContactDetails = MapTo(src.ContactDetails),
            AreaRange = MapTo(src.AreaRange),
            PeopleCapacity = MapTo(src.PeopleCapacity),
            Website = src.Website.ToSafeString()
        };

        if (src.RelatedImageLinks is not null)
        {
            extraMetadata.RelatedImageLinks.AddRange(src.RelatedImageLinks);
        }

        if (src.RelatedVideoLinks is not null)
        {
            extraMetadata.RelatedVideoLinks.AddRange(src.RelatedVideoLinks);
        }

        if (src.OtherLinks is not null)
        {
            extraMetadata.OtherLinks.AddRange(src.OtherLinks);
        }

        return extraMetadata;
    }

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.ContactDetails? MapTo(ContactDetails? src)
    {
        if (src is null)
        {
            return null;
        }

        var contactDetails = new global::Api.Shared.Grpc.Skedular.Location.Core.V1.ContactDetails();

        if (src.ContactPeople is not null)
        {
            contactDetails.ContactPeople.AddRange(src.ContactPeople);
        }

        if (src.ContactEmails is not null)
        {
            contactDetails.ContactEmails.AddRange(src.ContactEmails);
        }

        if (src.ContactPhones is not null)
        {
            contactDetails.ContactPhones.AddRange(src.ContactPhones);
        }

        return contactDetails;
    }

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.AreaRange? MapTo(AreaRange? src) =>
        src is null
            ? null
            : new global::Api.Shared.Grpc.Skedular.Location.Core.V1.AreaRange
            {
                FromInSqm = src.FromInSqm.ToSafeString(), ToInSqm = src.ToInSqm.ToSafeString()
            };

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.PeopleCapacity? MapTo(PeopleCapacity? src) =>
        src is null
            ? null
            : new global::Api.Shared.Grpc.Skedular.Location.Core.V1.PeopleCapacity { From = src.From.ToSafeString(), To = src.To.ToSafeString() };

    private static PhysicalAddress? MapToGrpcResponse(LocationPhysicalAddress? src) =>
        src is null
            ? null
            : new PhysicalAddress
            {
                Id = src.Id,
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null ? null : new Coordinates { Longitude = src.Coordinates.X, Latitude = src.Coordinates.Y },
                FormattedAddress = src.FormattedAddress.ToSafeString(),
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString(),
                CountryCode = src.CountryCode.ToSafeString()
            };

    private static LocationPhysicalAddress? MapTo(PhysicalAddress? src) =>
        src is null
            ? null
            : new LocationPhysicalAddress
            {
                Id = src.Id,
                OsmType = src.OsmType.ToSafeString(),
                OsmId = src.OsmId.ToSafeString(),
                PlaceId = src.PlaceId.ToSafeString(),
                Coordinates = src.Coordinates is null
                    ? null
                    : new Point(
                        new Coordinate(src.Coordinates.Longitude, src.Coordinates.Latitude)),
                FormattedAddress = src.FormattedAddress.ToSafeString(),
                AddressLine1 = src.AddressLine1.ToSafeString(),
                AddressLine2 = src.AddressLine2.ToSafeString(),
                Suburb = src.Suburb.ToSafeString(),
                City = src.City.ToSafeString(),
                Province = src.Province.ToSafeString(),
                Zipcode = src.Zipcode.ToSafeString(),
                Country = src.Country.ToSafeString(),
                CountryCode = src.CountryCode.ToSafeString()
            };

    private static ListingMetadata MapTo(global::Api.Shared.Grpc.Skedular.Location.Core.V1.ListingMetadata? src) =>
        src is null
            ? ListingMetadata.Empty
            : new ListingMetadata(src.About.ToSafeString(), src.Title.ToSafeString(), src.SubTitle.ToSafeString(), src.IncludedFeatures);

    private static global::Api.Shared.Grpc.Skedular.Location.Core.V1.ListingMetadata MapTo(ListingMetadata src) =>
        new() { About = src.About.ToSafeString(), Title = src.Title.ToSafeString(), SubTitle = src.SubTitle.ToSafeString() };
}
