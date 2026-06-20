using Api.Shared.Services.Models;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Services;

public interface IOrganizationDefaultValuesProvider
{
    IReadOnlyList<Tag> GetDefaultTags(Database.Entities.Organization organizationEntity);
}

public class OrganizationDefaultValuesProvider(IRandomHelper randomHelper) : IOrganizationDefaultValuesProvider
{
    public IReadOnlyList<Tag> GetDefaultTags(Database.Entities.Organization organizationEntity) =>
        GetResourceTags(organizationEntity)
            .Concat(GetLocationSpaceTypeTags(organizationEntity))
            .Concat(GetAmenitiesTags(organizationEntity))
            .ToList();

    private IEnumerable<Tag> GetResourceTags(Database.Entities.Organization organizationEntity) =>
    [
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.ResourceDesk.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.ResourceDesk,
            Color = "#87CEEB",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.ResourceRoom.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.ResourceRoom,
            Color = "#98FB98",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.ResourceParking.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.ResourceParking,
            Color = "#20B2AA",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.ResourceOthers.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.ResourceOthers,
            Color = "#8A2BE2",
            Organization = organizationEntity
        },
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.ResourceEntireLocation.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.ResourceEntireLocation,
            Color = "#4169E1",
            Organization = organizationEntity
        }
    ];

    private IEnumerable<Tag> GetLocationSpaceTypeTags(Database.Entities.Organization organizationEntity) =>
    [
        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeCarParkSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
            Color = "#87CEEB",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeEventSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
            Color = "#FFD700",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeMeetingSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
            Color = "#FF6347",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeOfficeSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
            Color = "#32CD32",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeRetailSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
            Color = "#98FB98",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeStorageSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
            Color = "#B0E0E6",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeStudioSpace.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
            Color = "#F5DEB3",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeCommercialKitchen.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
            Color = "#20B2AA",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeShootLocation.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
            Color = "#4682B4",
            Organization = organizationEntity
        },


        new()
        {
            Id = randomHelper.Generate(),
            Name = OrganizationTagType.LocationSpaceTypeOthers.ToOrganizationTagTypeName(),
            Type = OrganizationTagTypeConstants.LocationSpaceTypeOthers,
            Color = "#DAA520",
            Organization = organizationEntity
        }
    ];

    private IEnumerable<Tag> GetAmenitiesTags(Database.Entities.Organization organizationEntity) =>
        OrganizationTagTypeConstants.Amenities
            .Select(amenity => new Tag
            {
                Id = randomHelper.Generate(),
                Name = amenity.ToOrganizationTagTypeName(),
                Type = amenity.ToOrganizationTagType(),
                Color = GetAmenityColor(amenity),
                Organization = organizationEntity
            });

    private static string GetAmenityColor(OrganizationTagType amenity) =>
        amenity switch
        {
            OrganizationTagType.AmenityHighSpeedWifi or
                OrganizationTagType.AmenityDedicatedVlan or
                OrganizationTagType.AmenityWiredEthernet or
                OrganizationTagType.AmenityGuestNetwork or
                OrganizationTagType.AmenityVpnSupport or
                OrganizationTagType.AmenityBackupInternet or
                OrganizationTagType.AmenityNetworkPrinting or
                OrganizationTagType.AmenitySecureSsid or
                OrganizationTagType.AmenityPublicIpOption
                => "#1E90FF", // connectivity & network

            OrganizationTagType.AmenityStandardPowerOutlets or
                OrganizationTagType.AmenityUsbAPorts or
                OrganizationTagType.AmenityUsbCPorts or
                OrganizationTagType.AmenityWirelessCharging or
                OrganizationTagType.AmenityPowerStrips or
                OrganizationTagType.AmenityUpsBackedPower or
                OrganizationTagType.AmenityMonitorDockingStation or
                OrganizationTagType.AmenityLaptopStandAvailability
                => "#4682B4", // power & hardware

            OrganizationTagType.AmenityAdjustableDesk or
                OrganizationTagType.AmenityStandingDesk or
                OrganizationTagType.AmenityFixedDesk or
                OrganizationTagType.AmenityErgonomicChair or
                OrganizationTagType.AmenityExecutiveChair or
                OrganizationTagType.AmenityDualMonitorSetup or
                OrganizationTagType.AmenityPrivacyDeskDivider or
                OrganizationTagType.AmenityLockablePedestal or
                OrganizationTagType.AmenityWhiteboardAtDesk or
                OrganizationTagType.AmenityTaskLamp or
                OrganizationTagType.AmenityDeskPhone or
                OrganizationTagType.AmenityKeyboardMouseIncluded or
                OrganizationTagType.AmenityMonitorArm
                => "#32CD32", // desk workspace setup

            OrganizationTagType.AmenityWhiteboard or
                OrganizationTagType.AmenityGlassBoard or
                OrganizationTagType.AmenityFlipChart or
                OrganizationTagType.AmenityMarkersIncluded or
                OrganizationTagType.AmenityProjector or
                OrganizationTagType.AmenityLargeDisplay or
                OrganizationTagType.AmenityHdmiUsbCCasting or
                OrganizationTagType.AmenityVideoConferencingKit or
                OrganizationTagType.AmenitySpeakerphone or
                OrganizationTagType.AmenityRoomCamera or
                OrganizationTagType.AmenityConferenceTable or
                OrganizationTagType.AmenityBreakoutArea or
                OrganizationTagType.AmenityBoardroomLayout or
                OrganizationTagType.AmenityClassroomLayout or
                OrganizationTagType.AmenityHybridReady or
                OrganizationTagType.AmenityRecordingEnabled
                => "#8A2BE2", // collaboration features

            OrganizationTagType.AmenityAcousticTreatment or
                OrganizationTagType.AmenitySoundproofRoom or
                OrganizationTagType.AmenityPhoneBooth or
                OrganizationTagType.AmenityQuietZone or
                OrganizationTagType.AmenityFocusRoom or
                OrganizationTagType.AmenityConfidentialMeetingRoom or
                OrganizationTagType.AmenityNoiseCancellingPanels
                => "#6A5ACD", // privacy & acoustics

            OrganizationTagType.AmenityAirConditioning or
                OrganizationTagType.AmenityHeating or
                OrganizationTagType.AmenityFreshAirVentilation or
                OrganizationTagType.AmenityOperableWindows or
                OrganizationTagType.AmenityNaturalLight or
                OrganizationTagType.AmenityBlackoutBlinds or
                OrganizationTagType.AmenityAmbientLightingControls or
                OrganizationTagType.AmenityTemperatureControls or
                OrganizationTagType.AmenityAirPurifier or
                OrganizationTagType.AmenityHumidityControl
                => "#20B2AA", // environment & comfort

            OrganizationTagType.AmenityCoffeeMachine or
                OrganizationTagType.AmenityTeaStation or
                OrganizationTagType.AmenityFilteredWater or
                OrganizationTagType.AmenitySparklingWater or
                OrganizationTagType.AmenitySnackBar or
                OrganizationTagType.AmenityKitchenette or
                OrganizationTagType.AmenityFullKitchen or
                OrganizationTagType.AmenityRefrigerator or
                OrganizationTagType.AmenityMicrowave or
                OrganizationTagType.AmenityDishwasher or
                OrganizationTagType.AmenityCateringSupport
                => "#FF8C00", // hospitality & kitchen

            OrganizationTagType.AmenityWheelchairAccessibleEntrance or
                OrganizationTagType.AmenityElevatorAccess or
                OrganizationTagType.AmenityAccessibleRestroom or
                OrganizationTagType.AmenityAccessibleDeskHeight or
                OrganizationTagType.AmenityBrailleSignage or
                OrganizationTagType.AmenityHearingLoop or
                OrganizationTagType.AmenityStepFreePath or
                OrganizationTagType.AmenityWideDoorways or
                OrganizationTagType.AmenityGenderNeutralRestroom or
                OrganizationTagType.AmenityPrayerRoom
                => "#DA70D6", // accessibility & inclusion

            OrganizationTagType.AmenityAccess247 or
                OrganizationTagType.AmenityBusinessHoursAccess or
                OrganizationTagType.AmenityKeycardEntry or
                OrganizationTagType.AmenityBiometricEntry or
                OrganizationTagType.AmenityReceptionDesk or
                OrganizationTagType.AmenityVisitorManagement or
                OrganizationTagType.AmenityCctv or
                OrganizationTagType.AmenityOnSiteSecurity or
                OrganizationTagType.AmenityLockableRoom or
                OrganizationTagType.AmenityLockerStorage or
                OrganizationTagType.AmenityAlarmSystem
                => "#DC143C", // security & access control

            OrganizationTagType.AmenityRestroomsNearby or
                OrganizationTagType.AmenityShowerFacilities or
                OrganizationTagType.AmenityEndOfTripFacilities or
                OrganizationTagType.AmenityBikeStorage or
                OrganizationTagType.AmenityLoadingDockAccess or
                OrganizationTagType.AmenityFreightElevator or
                OrganizationTagType.AmenityMailHandling or
                OrganizationTagType.AmenityPackageReceiving or
                OrganizationTagType.AmenityStorageRoom or
                OrganizationTagType.AmenityWasteRecyclingStation
                => "#708090", // facilities operations

            OrganizationTagType.AmenityOnSiteParking or
                OrganizationTagType.AmenityReservedParking or
                OrganizationTagType.AmenityEvCharging or
                OrganizationTagType.AmenityStreetParkingNearby or
                OrganizationTagType.AmenityPublicTransitNearby or
                OrganizationTagType.AmenityTaxiRideshareZone or
                OrganizationTagType.AmenityBicycleFriendlyAccess
                => "#2E8B57", // transport & commute

            OrganizationTagType.AmenityGymAccess or
                OrganizationTagType.AmenityWellnessRoom or
                OrganizationTagType.AmenityNapPodRoom or
                OrganizationTagType.AmenityOutdoorTerrace or
                OrganizationTagType.AmenityGardenPatio or
                OrganizationTagType.AmenityGameLoungeArea or
                OrganizationTagType.AmenityStandingMeetingZone or
                OrganizationTagType.AmenityMeditationRoom
                => "#3CB371", // wellness & lifestyle

            OrganizationTagType.AmenityChildcareNearby or
                OrganizationTagType.AmenityFamilyRoom or
                OrganizationTagType.AmenityLactationRoom or
                OrganizationTagType.AmenityPetFriendly or
                OrganizationTagType.AmenityServiceAnimalReady
                => "#FF69B4", // family & pet

            OrganizationTagType.AmenityItSupportOnSite or
                OrganizationTagType.AmenityFrontDeskSupport or
                OrganizationTagType.AmenityCommunityManager or
                OrganizationTagType.AmenityCleaningService or
                OrganizationTagType.AmenityAfterHoursSupport or
                OrganizationTagType.AmenityFacilitiesHelpdesk or
                OrganizationTagType.AmenityOnDemandSetup
                => "#1E90FF", // support services

            OrganizationTagType.AmenityHourlyBookable or
                OrganizationTagType.AmenityDailyPass or
                OrganizationTagType.AmenityMonthlyMembership or
                OrganizationTagType.AmenityMinimumBookingDuration or
                OrganizationTagType.AmenityMaxCapacity or
                OrganizationTagType.AmenityCancellationFlexibility or
                OrganizationTagType.AmenityRefundableBooking or
                OrganizationTagType.AmenityDepositRequired or
                OrganizationTagType.AmenityLeadTimeRequirement or
                OrganizationTagType.AmenityCheckInRequired or
                OrganizationTagType.AmenityHostApprovalRequired
                => "#DAA520", // booking & policy

            OrganizationTagType.AmenityStage or
                OrganizationTagType.AmenityPaSystem or
                OrganizationTagType.AmenityLightingRig or
                OrganizationTagType.AmenityMovableSeating or
                OrganizationTagType.AmenityEventStaffSupport or
                OrganizationTagType.AmenitySoundBooth or
                OrganizationTagType.AmenityGreenScreen or
                OrganizationTagType.AmenitySpecializedToolsEquipment or
                OrganizationTagType.AmenitySafetyComplianceKit
                => "#9370DB", // event/studio/specialized

            _ => "#778899"
        };
}
