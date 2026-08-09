using Api.Shared.Services.Models;
using Enterprise.Shared;
using HotChocolate.Types.Pagination;
using Location.Shared.Database.Entities;
using static Location.Shared.Models.LocationRestrictedInformationCategoryExtensions;
using DailyDeskCountRecording = Location.Shared.Models.DailyDeskCountRecording;
using DailyRoomCountRecording = Location.Shared.Models.DailyRoomCountRecording;
using Organization = Location.Shared.Database.Entities.Organization;
using Resource = Location.Shared.Database.Entities.Resource;
using LocationPhysicalAddress = Location.Shared.Database.Entities.LocationPhysicalAddress;
using FloorPlan = Location.Shared.Models.FloorPlan;
using LocationRestrictedInformation = Location.Shared.Models.LocationRestrictedInformation;
using PrecomputedLocationProduct = Location.Shared.Models.PrecomputedLocationProduct;
using Product = Location.Shared.Models.Product;

namespace Location.Shared.Mappers;

public interface IEntityMapper
{
    Models.Location MapTo(Database.Entities.Location src);

    Database.Entities.Location MapTo(
        Models.Location src,
        Organization organization,
        IReadOnlyList<OrganizationTag> organizationTags);

    Database.Entities.Location MergeTo(
        Models.Location src,
        Database.Entities.Location dest,
        IReadOnlyList<OrganizationTag> organizationTags);

    Models.Resource MapTo(Resource src);

    Resource MapTo(
        Models.Resource src,
        Database.Entities.Location location,
        IReadOnlyList<OrganizationTag> organizationTags);

    Resource MergeTo(
        Models.Resource src,
        Resource dest,
        Database.Entities.Location location,
        IReadOnlyList<OrganizationTag> organizationTags);

    Models.Resource MapTo(Resource src, Models.Location location);

    IEnumerable<Edge<Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Models.Location location);

    Database.Entities.FloorPlan MapTo(
        FloorPlan src,
        Database.Entities.Location location,
        IReadOnlyList<ResourcePosition>? resourcePositions);

    Database.Entities.FloorPlan MergeTo(
        FloorPlan src,
        Database.Entities.FloorPlan dest,
        Database.Entities.Location location,
        IReadOnlyList<ResourcePosition>? resourcePositions);

    FloorPlan MapTo(Database.Entities.FloorPlan src);

    ResourcePosition MapToEntity(
        Models.ResourcePosition src,
        Resource resource,
        Database.Entities.FloorPlan floorPlan);

    ResourcePosition MergeToEntity(
        Models.ResourcePosition src,
        ResourcePosition dest,
        Resource resource,
        Database.Entities.FloorPlan floorPlan);

    LocationPhysicalAddress MapTo(Models.LocationPhysicalAddress src, Database.Entities.Location location);

    LocationPhysicalAddress MergeTo(
        Models.LocationPhysicalAddress src,
        LocationPhysicalAddress dest,
        Database.Entities.Location location);
}

public class EntityMapper : IEntityMapper
{
    public Models.Location MapTo(Database.Entities.Location src)
    {
        var location = new Models.Location
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            Timezone = src.Timezone,
            Type = src.Type.ToLocationType(),
            ExtraMetadata = src.ExtraMetadata,
            FeatureImages = src.FeatureImages.ToSafeCollection(),
            OpeningHours = src.OpeningHours,
            Organization = MapTo(src.Organization),
            OrganizationTags = [.. MapTo(src.OrganizationTags)],
            UniqueClaimCode = src.UniqueClaimCode,
            ContactedViaEmail = src.ContactedViaEmail,
            ContactedViaSms = src.ContactedViaSms,
            ContactedViaCall = src.ContactedViaCall,
            ContactedViaWhatsapp = src.ContactedViaWhatsapp,
            PrecomputedLocationProducts = [.. MapTo(src.PrecomputedLocationProducts)],
            RestrictedInformation =
            [
                .. src.RestrictedInformation
                    .OrderBy(item => item.SortOrder)
                    .ThenBy(item => item.Title)
                    .Select(MapTo),
            ],
        };

        location.DailyDeskCountRecordings = [.. MapTo(src.DailyDeskCountRecordings, location)];
        location.DailyRoomCountRecordings = [.. MapTo(src.DailyRoomCountRecordings, location)];
        location.Resources = [.. MapTo(src.Resources, location)];
        location.PhysicalAddress = MapTo(src.PhysicalAddress, location);
        location.FloorPlans = [.. src.FloorPlans.Select(MapTo)];

        return location;
    }

    public Database.Entities.Location MapTo(
        Models.Location src,
        Organization organization,
        IReadOnlyList<OrganizationTag> organizationTags) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            ListingMetadata = src.ListingMetadata,
            Timezone = src.Timezone,
            Type = src.Type.ToLocationType(),
            ExtraMetadata = src.ExtraMetadata,
            FeatureImages = [.. src.FeatureImages],
            OpeningHours = src.OpeningHours,
            Organization = organization,
            OrganizationTags = [.. organizationTags],
            UniqueClaimCode = src.UniqueClaimCode,
            ContactedViaEmail = src.ContactedViaEmail,
            ContactedViaSms = src.ContactedViaSms,
            ContactedViaCall = src.ContactedViaCall,
            ContactedViaWhatsapp = src.ContactedViaWhatsapp,
        };

    public Database.Entities.Location MergeTo(
        Models.Location src,
        Database.Entities.Location dest,
        IReadOnlyList<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.ListingMetadata = src.ListingMetadata;
        dest.Timezone = src.Timezone;
        dest.Type = src.Type.ToLocationType();
        dest.ExtraMetadata = src.ExtraMetadata;
        dest.FeatureImages = [.. src.FeatureImages];
        dest.OpeningHours = src.OpeningHours;
        dest.OrganizationTags = [.. organizationTags];
        dest.UniqueClaimCode = src.UniqueClaimCode;
        dest.ContactedViaEmail = src.ContactedViaEmail;
        dest.ContactedViaSms = src.ContactedViaSms;
        dest.ContactedViaCall = src.ContactedViaCall;
        dest.ContactedViaWhatsapp = src.ContactedViaWhatsapp;
        return dest;
    }

    public Models.Resource MapTo(Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden ?? false,
            AvailableHours = src.AvailableHours,
            Tags = [.. MapTo(src.OrganizationTags)],
            ResourcePosition = MapTo(src.ResourcePosition),
        };

    public Resource MapTo(
        Models.Resource src,
        Database.Entities.Location location,
        IReadOnlyList<OrganizationTag> organizationTags) =>
        MergeTo(src, new Resource(), location, organizationTags);

    public Resource MergeTo(
        Models.Resource src,
        Resource dest,
        Database.Entities.Location location,
        IReadOnlyList<OrganizationTag> organizationTags)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Inactive = src.Inactive;
        dest.RequireBookingApproval = src.RequireBookingApproval;
        dest.Color = src.Color;
        dest.Capacity = src.Capacity;
        dest.IsAvailableHoursOverridden = src.IsAvailableHoursOverridden;
        dest.AvailableHours = src.AvailableHours;
        dest.OrganizationTags = [.. organizationTags];
        dest.Location = location;
        return dest;
    }

    public Models.Resource MapTo(Resource src, Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden ?? false,
            AvailableHours = src.AvailableHours,
            Location = location,
            Tags = [.. MapTo(src.OrganizationTags, location.Organization)],
        };

    public IEnumerable<Edge<Models.Resource>> MapTo(IEnumerable<Edge<Resource>> src, Models.Location location) =>
        src.Select(item => MapTo(item, location));

    public Database.Entities.FloorPlan MapTo(
        FloorPlan src,
        Database.Entities.Location location,
        IReadOnlyList<ResourcePosition>? resourcePositions) =>
        MergeTo(src, new Database.Entities.FloorPlan(), location, resourcePositions);

    public Database.Entities.FloorPlan MergeTo(
        FloorPlan src,
        Database.Entities.FloorPlan dest,
        Database.Entities.Location location,
        IReadOnlyList<ResourcePosition>? resourcePositions)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Image = src.Image;
        dest.Location = location;

        if (resourcePositions is not null)
        {
            dest.ResourcePositions = [.. resourcePositions];
        }

        return dest;
    }

    public FloorPlan MapTo(Database.Entities.FloorPlan src)
    {
        var floorPlan = new FloorPlan
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Image = src.Image,
        };

        floorPlan.ResourcePositions = [.. MapTo(src.ResourcePositions, floorPlan)];

        return floorPlan;
    }

    public ResourcePosition MapToEntity(
        Models.ResourcePosition src,
        Resource resource,
        Database.Entities.FloorPlan floorPlan) => MergeToEntity(src, new ResourcePosition(), resource, floorPlan);

    public ResourcePosition MergeToEntity(
        Models.ResourcePosition src,
        ResourcePosition dest,
        Resource resource,
        Database.Entities.FloorPlan floorPlan)
    {
        dest.Id = src.Id;
        dest.X = src.X;
        dest.Y = src.Y;
        dest.Resource = resource;
        dest.FloorPlan = floorPlan;
        return dest;
    }

    public LocationPhysicalAddress MapTo(Models.LocationPhysicalAddress src, Database.Entities.Location location) =>
        MergeTo(src, new LocationPhysicalAddress(), location);

    public LocationPhysicalAddress MergeTo(
        Models.LocationPhysicalAddress src,
        LocationPhysicalAddress dest,
        Database.Entities.Location location)
    {
        dest.Id = src.Id;
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
        dest.Location = location;
        return dest;
    }

    private static Models.OrganizationTag MapTo(OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color,
        };

    private static IEnumerable<Models.OrganizationTag> MapTo(IEnumerable<OrganizationTag> src, Models.Organization? organization) =>
        src.Select(item => MapTo(item, organization));

    private IEnumerable<Models.Resource> MapTo(IEnumerable<Resource> src, Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static IEnumerable<Models.OrganizationTag> MapTo(IEnumerable<OrganizationTag> src) => src.Select(MapTo);

    private static Models.Organization MapTo(Organization src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            CustomDomain = src.CustomDomain,
            Name = src.Name,
            LogoUrl = src.LogoUrl,
            Offering = src.Offering,
            Type = src.Type.ToOrganizationType(),
            IsOwnershipVerified = src.IsOwnershipVerified,
            Tags = [.. MapTo(src.Tags)],
        };

    private static IEnumerable<DailyDeskCountRecording> MapTo(IEnumerable<Database.Entities.DailyDeskCountRecording> src, Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static DailyDeskCountRecording MapTo(Database.Entities.DailyDeskCountRecording src, Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Location = location,
            Date = src.Date,
            Count = src.Count,
        };

    private static IEnumerable<DailyRoomCountRecording> MapTo(
        IEnumerable<Database.Entities.DailyRoomCountRecording> src,
        Models.Location location) =>
        src.Select(item => MapTo(item, location));

    private static DailyRoomCountRecording MapTo(Database.Entities.DailyRoomCountRecording src, Models.Location location) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Location = location,
            Date = src.Date,
            Count = src.Count,
        };

    private static Models.OrganizationTag MapTo(OrganizationTag src, Models.Organization? organization)
    {
        var organizationTag = new Models.OrganizationTag
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name,
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color,
        };

        if (organization is not null)
        {
            organizationTag.Organization = organization;
        }

        return organizationTag;
    }

    private Edge<Models.Resource> MapTo(Edge<Resource> src, Models.Location location)
    {
        var resource = MapTo(src.Node);
        resource.Location = location;
        return new Edge<Models.Resource>(resource, src.Cursor);
    }

    private static IEnumerable<Models.ResourcePosition> MapTo(
        IEnumerable<ResourcePosition> src,
        FloorPlan floorPlan) =>
        src.Select(item => MapTo(item, floorPlan))!;

    private Models.ResourcePosition? MapTo(ResourcePosition? src) =>
        src is null
            ? null
            : new Models.ResourcePosition
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                X = src.X,
                Y = src.Y,
                FloorPlan = MapTo(src.FloorPlan),
            };

    private static Models.ResourcePosition? MapTo(ResourcePosition? src, FloorPlan floorPlan) =>
        src is null
            ? null
            : new Models.ResourcePosition
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                X = src.X,
                Y = src.Y,
                FloorPlan = floorPlan,
                Resource = new Models.Resource
                {
                    Id = src.Resource.Id,
                    CreatedAt = src.CreatedAt,
                    DeletedAt = src.Resource.DeletedAt,
                    ModifiedAt = src.ModifiedAt,
                    Name = src.Resource.Name,
                    Inactive = src.Resource.Inactive,
                    RequireBookingApproval = src.Resource.RequireBookingApproval,
                    Color = src.Resource.Color,
                    Capacity = src.Resource.Capacity,
                    IsAvailableHoursOverridden = src.Resource.IsAvailableHoursOverridden ?? false,
                    AvailableHours = src.Resource.AvailableHours,
                },
            };

    private static Models.LocationPhysicalAddress? MapTo(LocationPhysicalAddress? src, Models.Location location) =>
        src is null
            ? null
            : new Models.LocationPhysicalAddress
            {
                Id = src.Id,
                OsmType = src.OsmType,
                OsmId = src.OsmId,
                PlaceId = src.PlaceId,
                Coordinates = src.Coordinates,
                FormattedAddress = src.FormattedAddress,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country,
                CountryCode = src.CountryCode,
                Location = location,
            };

    private static IEnumerable<PrecomputedLocationProduct> MapTo(
        IEnumerable<Database.Entities.PrecomputedLocationProduct> src) =>
        src.Select(MapTo);

    private static PrecomputedLocationProduct MapTo(Database.Entities.PrecomputedLocationProduct src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Product = MapTo(src.Product),
        };

    private static Product MapTo(Database.Entities.Product src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
        };

    private static LocationRestrictedInformation MapTo(Database.Entities.LocationRestrictedInformation src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Title = src.Title,
            Category = src.Category.ToLocationRestrictedInformationCategory(),
            Content = src.Content,
            Active = src.Active,
            SortOrder = src.SortOrder,
        };
}
