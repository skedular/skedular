using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Services;

public interface IOrganizationDefaultValuesProvider
{
    ICollection<Tag> GetDefaultTags(Database.Entities.Organization organizationEntity);
}

public class OrganizationDefaultValuesProvider(IRandomHelper randomHelper) : IOrganizationDefaultValuesProvider
{
    public ICollection<Tag> GetDefaultTags(Database.Entities.Organization organizationEntity) =>
        GetDefaultResourceTags(organizationEntity)
            .Concat(GetDefaultLocationSpaceTypeTags(organizationEntity)).ToList();

    private IEnumerable<Tag> GetDefaultResourceTags(Database.Entities.Organization organizationEntity) =>
    [
        new()
        {
            Id = randomHelper.Generate(),
            Name = "Desk",
            Type = OrganizationTagTypeConstants.ResourceDesk,
            Color = "#87CEEB",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = "Room",
            Type = OrganizationTagTypeConstants.ResourceRoom,
            Color = "#98FB98",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = "Parking",
            Type = OrganizationTagTypeConstants.ResourceParking,
            Color = "#20B2AA",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = "Others",
            Type = OrganizationTagTypeConstants.ResourceOthers,
            Color = "#8A2BE2",
            Organization = organizationEntity
        }
    ];

    private IEnumerable<Tag> GetDefaultLocationSpaceTypeTags(Database.Entities.Organization organizationEntity) =>
    [
        new()
        {
            Id = randomHelper.Generate(),
            Name = "Car Park Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
            Color = "#87CEEB",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Event Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
            Color = "#FFD700",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Meeting Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
            Color = "#FF6347",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Office Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
            Color = "#32CD32",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Retail Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
            Color = "#98FB98",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Storage Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
            Color = "#B0E0E6",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Studio Space",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
            Color = "#F5DEB3",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Commercial Kitchen",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
            Color = "#20B2AA",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Shoot Location",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
            Color = "#4682B4",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = "Others",
            Type = OrganizationTagTypeConstants.LocationSpaceTypeOthers,
            Color = "#DAA520",
            Organization = organizationEntity
        }
    ];
}
