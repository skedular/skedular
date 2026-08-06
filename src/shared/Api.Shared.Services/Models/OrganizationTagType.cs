namespace Api.Shared.Services.Models;

public enum OrganizationTagType
{
    Custom,
    Zone,
    Product,

    ResourceDesk,
    ResourceRoom,
    ResourceParking,
    ResourceOthers,
    ResourceEntireLocation,

    LocationSpaceTypeCarParkSpace,
    LocationSpaceTypeEventSpace,
    LocationSpaceTypeMeetingSpace,
    LocationSpaceTypeOfficeSpace,
    LocationSpaceTypeRetailSpace,
    LocationSpaceTypeStorageSpace,
    LocationSpaceTypeStudioSpace,
    LocationSpaceTypeCommercialKitchen,
    LocationSpaceTypeShootLocation,
    LocationSpaceTypeOthers,

    AmenityHighSpeedWifi,
    AmenityDedicatedVlan,
    AmenityWiredEthernet,
    AmenityGuestNetwork,
    AmenityVpnSupport,
    AmenityBackupInternet,
    AmenityNetworkPrinting,
    AmenitySecureSsid,
    AmenityPublicIpOption,
    AmenityStandardPowerOutlets,
    AmenityUsbAPorts,
    AmenityUsbCPorts,
    AmenityWirelessCharging,
    AmenityPowerStrips,
    AmenityUpsBackedPower,
    AmenityMonitorDockingStation,
    AmenityLaptopStandAvailability,
    AmenityAdjustableDesk,
    AmenityStandingDesk,
    AmenityFixedDesk,
    AmenityErgonomicChair,
    AmenityExecutiveChair,
    AmenityDualMonitorSetup,
    AmenityPrivacyDeskDivider,
    AmenityLockablePedestal,
    AmenityWhiteboardAtDesk,
    AmenityTaskLamp,
    AmenityWhiteboard,
    AmenityGlassBoard,
    AmenityFlipChart,
    AmenityMarkersIncluded,
    AmenityProjector,
    AmenityLargeDisplay,
    AmenityHdmiUsbCCasting,
    AmenityVideoConferencingKit,
    AmenitySpeakerphone,
    AmenityRoomCamera,
    AmenityConferenceTable,
    AmenityBreakoutArea,
    AmenityAcousticTreatment,
    AmenitySoundproofRoom,
    AmenityPhoneBooth,
    AmenityQuietZone,
    AmenityFocusRoom,
    AmenityConfidentialMeetingRoom,
    AmenityNoiseCancellingPanels,
    AmenityAirConditioning,
    AmenityHeating,
    AmenityFreshAirVentilation,
    AmenityOperableWindows,
    AmenityNaturalLight,
    AmenityBlackoutBlinds,
    AmenityAmbientLightingControls,
    AmenityTemperatureControls,
    AmenityAirPurifier,
    AmenityHumidityControl,
    AmenityCoffeeMachine,
    AmenityTeaStation,
    AmenityFilteredWater,
    AmenitySparklingWater,
    AmenitySnackBar,
    AmenityKitchenette,
    AmenityFullKitchen,
    AmenityRefrigerator,
    AmenityMicrowave,
    AmenityDishwasher,
    AmenityCateringSupport,
    AmenityWheelchairAccessibleEntrance,
    AmenityElevatorAccess,
    AmenityAccessibleRestroom,
    AmenityAccessibleDeskHeight,
    AmenityBrailleSignage,
    AmenityHearingLoop,
    AmenityStepFreePath,
    AmenityWideDoorways,
    AmenityGenderNeutralRestroom,
    AmenityPrayerRoom,
    AmenityAccess247,
    AmenityBusinessHoursAccess,
    AmenityKeycardEntry,
    AmenityBiometricEntry,
    AmenityReceptionDesk,
    AmenityVisitorManagement,
    AmenityCctv,
    AmenityOnSiteSecurity,
    AmenityLockableRoom,
    AmenityLockerStorage,
    AmenityAlarmSystem,
    AmenityRestroomsNearby,
    AmenityShowerFacilities,
    AmenityEndOfTripFacilities,
    AmenityBikeStorage,
    AmenityLoadingDockAccess,
    AmenityFreightElevator,
    AmenityMailHandling,
    AmenityPackageReceiving,
    AmenityStorageRoom,
    AmenityWasteRecyclingStation,
    AmenityOnSiteParking,
    AmenityReservedParking,
    AmenityEvCharging,
    AmenityStreetParkingNearby,
    AmenityPublicTransitNearby,
    AmenityTaxiRideshareZone,
    AmenityBicycleFriendlyAccess,
    AmenityGymAccess,
    AmenityWellnessRoom,
    AmenityNapPodRoom,
    AmenityOutdoorTerrace,
    AmenityGardenPatio,
    AmenityGameLoungeArea,
    AmenityStandingMeetingZone,
    AmenityMeditationRoom,
    AmenityChildcareNearby,
    AmenityFamilyRoom,
    AmenityLactationRoom,
    AmenityPetFriendly,
    AmenityServiceAnimalReady,
    AmenityItSupportOnSite,
    AmenityFrontDeskSupport,
    AmenityCommunityManager,
    AmenityCleaningService,
    AmenityAfterHoursSupport,
    AmenityFacilitiesHelpdesk,
    AmenityOnDemandSetup,
    AmenityHourlyBookable,
    AmenityDailyPass,
    AmenityMonthlyMembership,
    AmenityMinimumBookingDuration,
    AmenityMaxCapacity,
    AmenityCancellationFlexibility,
    AmenityRefundableBooking,
    AmenityDepositRequired,
    AmenityLeadTimeRequirement,
    AmenityCheckInRequired,
    AmenityHostApprovalRequired,
    AmenityDeskPhone,
    AmenityKeyboardMouseIncluded,
    AmenityMonitorArm,
    AmenityBoardroomLayout,
    AmenityClassroomLayout,
    AmenityHybridReady,
    AmenityRecordingEnabled,
    AmenityStage,
    AmenityPaSystem,
    AmenityLightingRig,
    AmenityMovableSeating,
    AmenityEventStaffSupport,
    AmenitySoundBooth,
    AmenityGreenScreen,
    AmenitySpecializedToolsEquipment,
    AmenitySafetyComplianceKit,
}

public static class OrganizationTagTypeConstants
{
    public const string Custom = "CUSTOM";
    public const string Zone = "ZONE";
    public const string Product = "PRODUCT";

    public const string ResourceDesk = "RESOURCE_DESK";
    public const string ResourceRoom = "RESOURCE_ROOM";
    public const string ResourceParking = "RESOURCE_PARKING";
    public const string ResourceOthers = "RESOURCE_OTHERS";
    public const string ResourceEntireLocation = "RESOURCE_ENTIRE_LOCATION";

    public const string LocationSpaceTypeCarParkSpace = "LOCATION_SPACE_TYPE_CARPARK_SPACE";
    public const string LocationSpaceTypeEventSpace = "LOCATION_SPACE_TYPE_EVENT_SPACE";
    public const string LocationSpaceTypeMeetingSpace = "LOCATION_SPACE_TYPE_MEETING_SPACE";
    public const string LocationSpaceTypeOfficeSpace = "LOCATION_SPACE_TYPE_OFFICE_SPACE";
    public const string LocationSpaceTypeRetailSpace = "LOCATION_SPACE_TYPE_RETAIL_SPACE";
    public const string LocationSpaceTypeStorageSpace = "LOCATION_SPACE_TYPE_STORAGE_SPACE";
    public const string LocationSpaceTypeStudioSpace = "LOCATION_SPACE_TYPE_STUDIO_SPACE";
    public const string LocationSpaceTypeCommercialKitchen = "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN";
    public const string LocationSpaceTypeShootLocation = "LOCATION_SPACE_TYPE_SHOOT_LOCATION";
    public const string LocationSpaceTypeOthers = "LOCATION_SPACE_TYPE_OTHERS";

    public const string AmenityHighSpeedWifi = "AMENITY_HIGH_SPEED_WIFI";
    public const string AmenityDedicatedVlan = "AMENITY_DEDICATED_VLAN";
    public const string AmenityWiredEthernet = "AMENITY_WIRED_ETHERNET";
    public const string AmenityGuestNetwork = "AMENITY_GUEST_NETWORK";
    public const string AmenityVpnSupport = "AMENITY_VPN_SUPPORT";
    public const string AmenityBackupInternet = "AMENITY_BACKUP_INTERNET";
    public const string AmenityNetworkPrinting = "AMENITY_NETWORK_PRINTING";
    public const string AmenitySecureSsid = "AMENITY_SECURE_SSID";
    public const string AmenityPublicIpOption = "AMENITY_PUBLIC_IP_OPTION";
    public const string AmenityStandardPowerOutlets = "AMENITY_STANDARD_POWER_OUTLETS";
    public const string AmenityUsbAPorts = "AMENITY_USB_A_PORTS";
    public const string AmenityUsbCPorts = "AMENITY_USB_C_PORTS";
    public const string AmenityWirelessCharging = "AMENITY_WIRELESS_CHARGING";
    public const string AmenityPowerStrips = "AMENITY_POWER_STRIPS";
    public const string AmenityUpsBackedPower = "AMENITY_UPS_BACKED_POWER";
    public const string AmenityMonitorDockingStation = "AMENITY_MONITOR_DOCKING_STATION";
    public const string AmenityLaptopStandAvailability = "AMENITY_LAPTOP_STAND_AVAILABILITY";
    public const string AmenityAdjustableDesk = "AMENITY_ADJUSTABLE_DESK";
    public const string AmenityStandingDesk = "AMENITY_STANDING_DESK";
    public const string AmenityFixedDesk = "AMENITY_FIXED_DESK";
    public const string AmenityErgonomicChair = "AMENITY_ERGONOMIC_CHAIR";
    public const string AmenityExecutiveChair = "AMENITY_EXECUTIVE_CHAIR";
    public const string AmenityDualMonitorSetup = "AMENITY_DUAL_MONITOR_SETUP";
    public const string AmenityPrivacyDeskDivider = "AMENITY_PRIVACY_DESK_DIVIDER";
    public const string AmenityLockablePedestal = "AMENITY_LOCKABLE_PEDESTAL";
    public const string AmenityWhiteboardAtDesk = "AMENITY_WHITEBOARD_AT_DESK";
    public const string AmenityTaskLamp = "AMENITY_TASK_LAMP";
    public const string AmenityWhiteboard = "AMENITY_WHITEBOARD";
    public const string AmenityGlassBoard = "AMENITY_GLASS_BOARD";
    public const string AmenityFlipChart = "AMENITY_FLIP_CHART";
    public const string AmenityMarkersIncluded = "AMENITY_MARKERS_INCLUDED";
    public const string AmenityProjector = "AMENITY_PROJECTOR";
    public const string AmenityLargeDisplay = "AMENITY_LARGE_DISPLAY";
    public const string AmenityHdmiUsbCCasting = "AMENITY_HDMI_USB_C_CASTING";
    public const string AmenityVideoConferencingKit = "AMENITY_VIDEO_CONFERENCING_KIT";
    public const string AmenitySpeakerphone = "AMENITY_SPEAKERPHONE";
    public const string AmenityRoomCamera = "AMENITY_ROOM_CAMERA";
    public const string AmenityConferenceTable = "AMENITY_CONFERENCE_TABLE";
    public const string AmenityBreakoutArea = "AMENITY_BREAKOUT_AREA";
    public const string AmenityAcousticTreatment = "AMENITY_ACOUSTIC_TREATMENT";
    public const string AmenitySoundproofRoom = "AMENITY_SOUNDPROOF_ROOM";
    public const string AmenityPhoneBooth = "AMENITY_PHONE_BOOTH";
    public const string AmenityQuietZone = "AMENITY_QUIET_ZONE";
    public const string AmenityFocusRoom = "AMENITY_FOCUS_ROOM";
    public const string AmenityConfidentialMeetingRoom = "AMENITY_CONFIDENTIAL_MEETING_ROOM";
    public const string AmenityNoiseCancellingPanels = "AMENITY_NOISE_CANCELLING_PANELS";
    public const string AmenityAirConditioning = "AMENITY_AIR_CONDITIONING";
    public const string AmenityHeating = "AMENITY_HEATING";
    public const string AmenityFreshAirVentilation = "AMENITY_FRESH_AIR_VENTILATION";
    public const string AmenityOperableWindows = "AMENITY_OPERABLE_WINDOWS";
    public const string AmenityNaturalLight = "AMENITY_NATURAL_LIGHT";
    public const string AmenityBlackoutBlinds = "AMENITY_BLACKOUT_BLINDS";
    public const string AmenityAmbientLightingControls = "AMENITY_AMBIENT_LIGHTING_CONTROLS";
    public const string AmenityTemperatureControls = "AMENITY_TEMPERATURE_CONTROLS";
    public const string AmenityAirPurifier = "AMENITY_AIR_PURIFIER";
    public const string AmenityHumidityControl = "AMENITY_HUMIDITY_CONTROL";
    public const string AmenityCoffeeMachine = "AMENITY_COFFEE_MACHINE";
    public const string AmenityTeaStation = "AMENITY_TEA_STATION";
    public const string AmenityFilteredWater = "AMENITY_FILTERED_WATER";
    public const string AmenitySparklingWater = "AMENITY_SPARKLING_WATER";
    public const string AmenitySnackBar = "AMENITY_SNACK_BAR";
    public const string AmenityKitchenette = "AMENITY_KITCHENETTE";
    public const string AmenityFullKitchen = "AMENITY_FULL_KITCHEN";
    public const string AmenityRefrigerator = "AMENITY_REFRIGERATOR";
    public const string AmenityMicrowave = "AMENITY_MICROWAVE";
    public const string AmenityDishwasher = "AMENITY_DISHWASHER";
    public const string AmenityCateringSupport = "AMENITY_CATERING_SUPPORT";
    public const string AmenityWheelchairAccessibleEntrance = "AMENITY_WHEELCHAIR_ACCESSIBLE_ENTRANCE";
    public const string AmenityElevatorAccess = "AMENITY_ELEVATOR_ACCESS";
    public const string AmenityAccessibleRestroom = "AMENITY_ACCESSIBLE_RESTROOM";
    public const string AmenityAccessibleDeskHeight = "AMENITY_ACCESSIBLE_DESK_HEIGHT";
    public const string AmenityBrailleSignage = "AMENITY_BRAILLE_SIGNAGE";
    public const string AmenityHearingLoop = "AMENITY_HEARING_LOOP";
    public const string AmenityStepFreePath = "AMENITY_STEP_FREE_PATH";
    public const string AmenityWideDoorways = "AMENITY_WIDE_DOORWAYS";
    public const string AmenityGenderNeutralRestroom = "AMENITY_GENDER_NEUTRAL_RESTROOM";
    public const string AmenityPrayerRoom = "AMENITY_PRAYER_ROOM";
    public const string AmenityAccess247 = "AMENITY_24_7_ACCESS";
    public const string AmenityBusinessHoursAccess = "AMENITY_BUSINESS_HOURS_ACCESS";
    public const string AmenityKeycardEntry = "AMENITY_KEYCARD_ENTRY";
    public const string AmenityBiometricEntry = "AMENITY_BIOMETRIC_ENTRY";
    public const string AmenityReceptionDesk = "AMENITY_RECEPTION_DESK";
    public const string AmenityVisitorManagement = "AMENITY_VISITOR_MANAGEMENT";
    public const string AmenityCctv = "AMENITY_CCTV";
    public const string AmenityOnSiteSecurity = "AMENITY_ON_SITE_SECURITY";
    public const string AmenityLockableRoom = "AMENITY_LOCKABLE_ROOM";
    public const string AmenityLockerStorage = "AMENITY_LOCKER_STORAGE";
    public const string AmenityAlarmSystem = "AMENITY_ALARM_SYSTEM";
    public const string AmenityRestroomsNearby = "AMENITY_RESTROOMS_NEARBY";
    public const string AmenityShowerFacilities = "AMENITY_SHOWER_FACILITIES";
    public const string AmenityEndOfTripFacilities = "AMENITY_END_OF_TRIP_FACILITIES";
    public const string AmenityBikeStorage = "AMENITY_BIKE_STORAGE";
    public const string AmenityLoadingDockAccess = "AMENITY_LOADING_DOCK_ACCESS";
    public const string AmenityFreightElevator = "AMENITY_FREIGHT_ELEVATOR";
    public const string AmenityMailHandling = "AMENITY_MAIL_HANDLING";
    public const string AmenityPackageReceiving = "AMENITY_PACKAGE_RECEIVING";
    public const string AmenityStorageRoom = "AMENITY_STORAGE_ROOM";
    public const string AmenityWasteRecyclingStation = "AMENITY_WASTE_RECYCLING_STATION";
    public const string AmenityOnSiteParking = "AMENITY_ON_SITE_PARKING";
    public const string AmenityReservedParking = "AMENITY_RESERVED_PARKING";
    public const string AmenityEvCharging = "AMENITY_EV_CHARGING";
    public const string AmenityStreetParkingNearby = "AMENITY_STREET_PARKING_NEARBY";
    public const string AmenityPublicTransitNearby = "AMENITY_PUBLIC_TRANSIT_NEARBY";
    public const string AmenityTaxiRideshareZone = "AMENITY_TAXI_RIDESHARE_ZONE";
    public const string AmenityBicycleFriendlyAccess = "AMENITY_BICYCLE_FRIENDLY_ACCESS";
    public const string AmenityGymAccess = "AMENITY_GYM_ACCESS";
    public const string AmenityWellnessRoom = "AMENITY_WELLNESS_ROOM";
    public const string AmenityNapPodRoom = "AMENITY_NAP_POD_ROOM";
    public const string AmenityOutdoorTerrace = "AMENITY_OUTDOOR_TERRACE";
    public const string AmenityGardenPatio = "AMENITY_GARDEN_PATIO";
    public const string AmenityGameLoungeArea = "AMENITY_GAME_LOUNGE_AREA";
    public const string AmenityStandingMeetingZone = "AMENITY_STANDING_MEETING_ZONE";
    public const string AmenityMeditationRoom = "AMENITY_MEDITATION_ROOM";
    public const string AmenityChildcareNearby = "AMENITY_CHILDCARE_NEARBY";
    public const string AmenityFamilyRoom = "AMENITY_FAMILY_ROOM";
    public const string AmenityLactationRoom = "AMENITY_LACTATION_ROOM";
    public const string AmenityPetFriendly = "AMENITY_PET_FRIENDLY";
    public const string AmenityServiceAnimalReady = "AMENITY_SERVICE_ANIMAL_READY";
    public const string AmenityItSupportOnSite = "AMENITY_IT_SUPPORT_ON_SITE";
    public const string AmenityFrontDeskSupport = "AMENITY_FRONT_DESK_SUPPORT";
    public const string AmenityCommunityManager = "AMENITY_COMMUNITY_MANAGER";
    public const string AmenityCleaningService = "AMENITY_CLEANING_SERVICE";
    public const string AmenityAfterHoursSupport = "AMENITY_AFTER_HOURS_SUPPORT";
    public const string AmenityFacilitiesHelpdesk = "AMENITY_FACILITIES_HELPDESK";
    public const string AmenityOnDemandSetup = "AMENITY_ON_DEMAND_SETUP";
    public const string AmenityHourlyBookable = "AMENITY_HOURLY_BOOKABLE";
    public const string AmenityDailyPass = "AMENITY_DAILY_PASS";
    public const string AmenityMonthlyMembership = "AMENITY_MONTHLY_MEMBERSHIP";
    public const string AmenityMinimumBookingDuration = "AMENITY_MINIMUM_BOOKING_DURATION";
    public const string AmenityMaxCapacity = "AMENITY_MAX_CAPACITY";
    public const string AmenityCancellationFlexibility = "AMENITY_CANCELLATION_FLEXIBILITY";
    public const string AmenityRefundableBooking = "AMENITY_REFUNDABLE_BOOKING";
    public const string AmenityDepositRequired = "AMENITY_DEPOSIT_REQUIRED";
    public const string AmenityLeadTimeRequirement = "AMENITY_LEAD_TIME_REQUIREMENT";
    public const string AmenityCheckInRequired = "AMENITY_CHECK_IN_REQUIRED";
    public const string AmenityHostApprovalRequired = "AMENITY_HOST_APPROVAL_REQUIRED";
    public const string AmenityDeskPhone = "AMENITY_DESK_PHONE";
    public const string AmenityKeyboardMouseIncluded = "AMENITY_KEYBOARD_MOUSE_INCLUDED";
    public const string AmenityMonitorArm = "AMENITY_MONITOR_ARM";
    public const string AmenityBoardroomLayout = "AMENITY_BOARDROOM_LAYOUT";
    public const string AmenityClassroomLayout = "AMENITY_CLASSROOM_LAYOUT";
    public const string AmenityHybridReady = "AMENITY_HYBRID_READY";
    public const string AmenityRecordingEnabled = "AMENITY_RECORDING_ENABLED";
    public const string AmenityStage = "AMENITY_STAGE";
    public const string AmenityPaSystem = "AMENITY_PA_SYSTEM";
    public const string AmenityLightingRig = "AMENITY_LIGHTING_RIG";
    public const string AmenityMovableSeating = "AMENITY_MOVABLE_SEATING";
    public const string AmenityEventStaffSupport = "AMENITY_EVENT_STAFF_SUPPORT";
    public const string AmenitySoundBooth = "AMENITY_SOUND_BOOTH";
    public const string AmenityGreenScreen = "AMENITY_GREEN_SCREEN";
    public const string AmenitySpecializedToolsEquipment = "AMENITY_SPECIALIZED_TOOLS_EQUIPMENT";
    public const string AmenitySafetyComplianceKit = "AMENITY_SAFETY_COMPLIANCE_KIT";

    /// <summary>
    ///     The set of all resource-type tag type constants.
    ///     Use for <c>Contains</c> checks rather than re-declaring the set in each consumer.
    /// </summary>
    public static readonly HashSet<string> ResourceTagTypes =
    [
        ResourceDesk,
        ResourceRoom,
        ResourceParking,
        ResourceOthers,
        ResourceEntireLocation,
    ];

    public static readonly IReadOnlyList<OrganizationTagType> ResourceTypes =
    [
        OrganizationTagType.ResourceDesk,
        OrganizationTagType.ResourceRoom,
        OrganizationTagType.ResourceParking,
        OrganizationTagType.ResourceOthers,
        OrganizationTagType.ResourceEntireLocation,
    ];

    public static readonly IReadOnlyList<OrganizationTagType> LocationSpaceTypes =
    [
        OrganizationTagType.LocationSpaceTypeCarParkSpace,
        OrganizationTagType.LocationSpaceTypeEventSpace,
        OrganizationTagType.LocationSpaceTypeMeetingSpace,
        OrganizationTagType.LocationSpaceTypeOfficeSpace,
        OrganizationTagType.LocationSpaceTypeRetailSpace,
        OrganizationTagType.LocationSpaceTypeStorageSpace,
        OrganizationTagType.LocationSpaceTypeStudioSpace,
        OrganizationTagType.LocationSpaceTypeCommercialKitchen,
        OrganizationTagType.LocationSpaceTypeShootLocation,
        OrganizationTagType.LocationSpaceTypeOthers,
    ];

    public static readonly IReadOnlyList<OrganizationTagType> Amenities =
    [
        OrganizationTagType.AmenityHighSpeedWifi,
        OrganizationTagType.AmenityDedicatedVlan,
        OrganizationTagType.AmenityWiredEthernet,
        OrganizationTagType.AmenityGuestNetwork,
        OrganizationTagType.AmenityVpnSupport,
        OrganizationTagType.AmenityBackupInternet,
        OrganizationTagType.AmenityNetworkPrinting,
        OrganizationTagType.AmenitySecureSsid,
        OrganizationTagType.AmenityPublicIpOption,
        OrganizationTagType.AmenityStandardPowerOutlets,
        OrganizationTagType.AmenityUsbAPorts,
        OrganizationTagType.AmenityUsbCPorts,
        OrganizationTagType.AmenityWirelessCharging,
        OrganizationTagType.AmenityPowerStrips,
        OrganizationTagType.AmenityUpsBackedPower,
        OrganizationTagType.AmenityMonitorDockingStation,
        OrganizationTagType.AmenityLaptopStandAvailability,
        OrganizationTagType.AmenityAdjustableDesk,
        OrganizationTagType.AmenityStandingDesk,
        OrganizationTagType.AmenityFixedDesk,
        OrganizationTagType.AmenityErgonomicChair,
        OrganizationTagType.AmenityExecutiveChair,
        OrganizationTagType.AmenityDualMonitorSetup,
        OrganizationTagType.AmenityPrivacyDeskDivider,
        OrganizationTagType.AmenityLockablePedestal,
        OrganizationTagType.AmenityWhiteboardAtDesk,
        OrganizationTagType.AmenityTaskLamp,
        OrganizationTagType.AmenityWhiteboard,
        OrganizationTagType.AmenityGlassBoard,
        OrganizationTagType.AmenityFlipChart,
        OrganizationTagType.AmenityMarkersIncluded,
        OrganizationTagType.AmenityProjector,
        OrganizationTagType.AmenityLargeDisplay,
        OrganizationTagType.AmenityHdmiUsbCCasting,
        OrganizationTagType.AmenityVideoConferencingKit,
        OrganizationTagType.AmenitySpeakerphone,
        OrganizationTagType.AmenityRoomCamera,
        OrganizationTagType.AmenityConferenceTable,
        OrganizationTagType.AmenityBreakoutArea,
        OrganizationTagType.AmenityAcousticTreatment,
        OrganizationTagType.AmenitySoundproofRoom,
        OrganizationTagType.AmenityPhoneBooth,
        OrganizationTagType.AmenityQuietZone,
        OrganizationTagType.AmenityFocusRoom,
        OrganizationTagType.AmenityConfidentialMeetingRoom,
        OrganizationTagType.AmenityNoiseCancellingPanels,
        OrganizationTagType.AmenityAirConditioning,
        OrganizationTagType.AmenityHeating,
        OrganizationTagType.AmenityFreshAirVentilation,
        OrganizationTagType.AmenityOperableWindows,
        OrganizationTagType.AmenityNaturalLight,
        OrganizationTagType.AmenityBlackoutBlinds,
        OrganizationTagType.AmenityAmbientLightingControls,
        OrganizationTagType.AmenityTemperatureControls,
        OrganizationTagType.AmenityAirPurifier,
        OrganizationTagType.AmenityHumidityControl,
        OrganizationTagType.AmenityCoffeeMachine,
        OrganizationTagType.AmenityTeaStation,
        OrganizationTagType.AmenityFilteredWater,
        OrganizationTagType.AmenitySparklingWater,
        OrganizationTagType.AmenitySnackBar,
        OrganizationTagType.AmenityKitchenette,
        OrganizationTagType.AmenityFullKitchen,
        OrganizationTagType.AmenityRefrigerator,
        OrganizationTagType.AmenityMicrowave,
        OrganizationTagType.AmenityDishwasher,
        OrganizationTagType.AmenityCateringSupport,
        OrganizationTagType.AmenityWheelchairAccessibleEntrance,
        OrganizationTagType.AmenityElevatorAccess,
        OrganizationTagType.AmenityAccessibleRestroom,
        OrganizationTagType.AmenityAccessibleDeskHeight,
        OrganizationTagType.AmenityBrailleSignage,
        OrganizationTagType.AmenityHearingLoop,
        OrganizationTagType.AmenityStepFreePath,
        OrganizationTagType.AmenityWideDoorways,
        OrganizationTagType.AmenityGenderNeutralRestroom,
        OrganizationTagType.AmenityPrayerRoom,
        OrganizationTagType.AmenityAccess247,
        OrganizationTagType.AmenityBusinessHoursAccess,
        OrganizationTagType.AmenityKeycardEntry,
        OrganizationTagType.AmenityBiometricEntry,
        OrganizationTagType.AmenityReceptionDesk,
        OrganizationTagType.AmenityVisitorManagement,
        OrganizationTagType.AmenityCctv,
        OrganizationTagType.AmenityOnSiteSecurity,
        OrganizationTagType.AmenityLockableRoom,
        OrganizationTagType.AmenityLockerStorage,
        OrganizationTagType.AmenityAlarmSystem,
        OrganizationTagType.AmenityRestroomsNearby,
        OrganizationTagType.AmenityShowerFacilities,
        OrganizationTagType.AmenityEndOfTripFacilities,
        OrganizationTagType.AmenityBikeStorage,
        OrganizationTagType.AmenityLoadingDockAccess,
        OrganizationTagType.AmenityFreightElevator,
        OrganizationTagType.AmenityMailHandling,
        OrganizationTagType.AmenityPackageReceiving,
        OrganizationTagType.AmenityStorageRoom,
        OrganizationTagType.AmenityWasteRecyclingStation,
        OrganizationTagType.AmenityOnSiteParking,
        OrganizationTagType.AmenityReservedParking,
        OrganizationTagType.AmenityEvCharging,
        OrganizationTagType.AmenityStreetParkingNearby,
        OrganizationTagType.AmenityPublicTransitNearby,
        OrganizationTagType.AmenityTaxiRideshareZone,
        OrganizationTagType.AmenityBicycleFriendlyAccess,
        OrganizationTagType.AmenityGymAccess,
        OrganizationTagType.AmenityWellnessRoom,
        OrganizationTagType.AmenityNapPodRoom,
        OrganizationTagType.AmenityOutdoorTerrace,
        OrganizationTagType.AmenityGardenPatio,
        OrganizationTagType.AmenityGameLoungeArea,
        OrganizationTagType.AmenityStandingMeetingZone,
        OrganizationTagType.AmenityMeditationRoom,
        OrganizationTagType.AmenityChildcareNearby,
        OrganizationTagType.AmenityFamilyRoom,
        OrganizationTagType.AmenityLactationRoom,
        OrganizationTagType.AmenityPetFriendly,
        OrganizationTagType.AmenityServiceAnimalReady,
        OrganizationTagType.AmenityItSupportOnSite,
        OrganizationTagType.AmenityFrontDeskSupport,
        OrganizationTagType.AmenityCommunityManager,
        OrganizationTagType.AmenityCleaningService,
        OrganizationTagType.AmenityAfterHoursSupport,
        OrganizationTagType.AmenityFacilitiesHelpdesk,
        OrganizationTagType.AmenityOnDemandSetup,
        OrganizationTagType.AmenityHourlyBookable,
        OrganizationTagType.AmenityDailyPass,
        OrganizationTagType.AmenityMonthlyMembership,
        OrganizationTagType.AmenityMinimumBookingDuration,
        OrganizationTagType.AmenityMaxCapacity,
        OrganizationTagType.AmenityCancellationFlexibility,
        OrganizationTagType.AmenityRefundableBooking,
        OrganizationTagType.AmenityDepositRequired,
        OrganizationTagType.AmenityLeadTimeRequirement,
        OrganizationTagType.AmenityCheckInRequired,
        OrganizationTagType.AmenityHostApprovalRequired,
        OrganizationTagType.AmenityDeskPhone,
        OrganizationTagType.AmenityKeyboardMouseIncluded,
        OrganizationTagType.AmenityMonitorArm,
        OrganizationTagType.AmenityBoardroomLayout,
        OrganizationTagType.AmenityClassroomLayout,
        OrganizationTagType.AmenityHybridReady,
        OrganizationTagType.AmenityRecordingEnabled,
        OrganizationTagType.AmenityStage,
        OrganizationTagType.AmenityPaSystem,
        OrganizationTagType.AmenityLightingRig,
        OrganizationTagType.AmenityMovableSeating,
        OrganizationTagType.AmenityEventStaffSupport,
        OrganizationTagType.AmenitySoundBooth,
        OrganizationTagType.AmenityGreenScreen,
        OrganizationTagType.AmenitySpecializedToolsEquipment,
        OrganizationTagType.AmenitySafetyComplianceKit,
    ];
}

public static class OrganizationTagTypeExtensions
{
    extension(string src)
    {
        public OrganizationTagType ToOrganizationTagType() =>
            src switch
            {
                OrganizationTagTypeConstants.Custom => OrganizationTagType.Custom,
                OrganizationTagTypeConstants.Zone => OrganizationTagType.Zone,
                OrganizationTagTypeConstants.Product => OrganizationTagType.Product,

                OrganizationTagTypeConstants.ResourceDesk => OrganizationTagType.ResourceDesk,
                OrganizationTagTypeConstants.ResourceRoom => OrganizationTagType.ResourceRoom,
                OrganizationTagTypeConstants.ResourceParking => OrganizationTagType.ResourceParking,
                OrganizationTagTypeConstants.ResourceOthers => OrganizationTagType.ResourceOthers,
                OrganizationTagTypeConstants.ResourceEntireLocation => OrganizationTagType.ResourceEntireLocation,

                OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace => OrganizationTagType.LocationSpaceTypeCarParkSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeEventSpace => OrganizationTagType.LocationSpaceTypeEventSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace => OrganizationTagType.LocationSpaceTypeMeetingSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace => OrganizationTagType.LocationSpaceTypeOfficeSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace => OrganizationTagType.LocationSpaceTypeRetailSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace => OrganizationTagType.LocationSpaceTypeStorageSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace => OrganizationTagType.LocationSpaceTypeStudioSpace,
                OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen => OrganizationTagType.LocationSpaceTypeCommercialKitchen,
                OrganizationTagTypeConstants.LocationSpaceTypeShootLocation => OrganizationTagType.LocationSpaceTypeShootLocation,
                OrganizationTagTypeConstants.LocationSpaceTypeOthers => OrganizationTagType.LocationSpaceTypeOthers,

                OrganizationTagTypeConstants.AmenityHighSpeedWifi => OrganizationTagType.AmenityHighSpeedWifi,
                OrganizationTagTypeConstants.AmenityDedicatedVlan => OrganizationTagType.AmenityDedicatedVlan,
                OrganizationTagTypeConstants.AmenityWiredEthernet => OrganizationTagType.AmenityWiredEthernet,
                OrganizationTagTypeConstants.AmenityGuestNetwork => OrganizationTagType.AmenityGuestNetwork,
                OrganizationTagTypeConstants.AmenityVpnSupport => OrganizationTagType.AmenityVpnSupport,
                OrganizationTagTypeConstants.AmenityBackupInternet => OrganizationTagType.AmenityBackupInternet,
                OrganizationTagTypeConstants.AmenityNetworkPrinting => OrganizationTagType.AmenityNetworkPrinting,
                OrganizationTagTypeConstants.AmenitySecureSsid => OrganizationTagType.AmenitySecureSsid,
                OrganizationTagTypeConstants.AmenityPublicIpOption => OrganizationTagType.AmenityPublicIpOption,
                OrganizationTagTypeConstants.AmenityStandardPowerOutlets => OrganizationTagType.AmenityStandardPowerOutlets,
                OrganizationTagTypeConstants.AmenityUsbAPorts => OrganizationTagType.AmenityUsbAPorts,
                OrganizationTagTypeConstants.AmenityUsbCPorts => OrganizationTagType.AmenityUsbCPorts,
                OrganizationTagTypeConstants.AmenityWirelessCharging => OrganizationTagType.AmenityWirelessCharging,
                OrganizationTagTypeConstants.AmenityPowerStrips => OrganizationTagType.AmenityPowerStrips,
                OrganizationTagTypeConstants.AmenityUpsBackedPower => OrganizationTagType.AmenityUpsBackedPower,
                OrganizationTagTypeConstants.AmenityMonitorDockingStation => OrganizationTagType.AmenityMonitorDockingStation,
                OrganizationTagTypeConstants.AmenityLaptopStandAvailability => OrganizationTagType.AmenityLaptopStandAvailability,
                OrganizationTagTypeConstants.AmenityAdjustableDesk => OrganizationTagType.AmenityAdjustableDesk,
                OrganizationTagTypeConstants.AmenityStandingDesk => OrganizationTagType.AmenityStandingDesk,
                OrganizationTagTypeConstants.AmenityFixedDesk => OrganizationTagType.AmenityFixedDesk,
                OrganizationTagTypeConstants.AmenityErgonomicChair => OrganizationTagType.AmenityErgonomicChair,
                OrganizationTagTypeConstants.AmenityExecutiveChair => OrganizationTagType.AmenityExecutiveChair,
                OrganizationTagTypeConstants.AmenityDualMonitorSetup => OrganizationTagType.AmenityDualMonitorSetup,
                OrganizationTagTypeConstants.AmenityPrivacyDeskDivider => OrganizationTagType.AmenityPrivacyDeskDivider,
                OrganizationTagTypeConstants.AmenityLockablePedestal => OrganizationTagType.AmenityLockablePedestal,
                OrganizationTagTypeConstants.AmenityWhiteboardAtDesk => OrganizationTagType.AmenityWhiteboardAtDesk,
                OrganizationTagTypeConstants.AmenityTaskLamp => OrganizationTagType.AmenityTaskLamp,
                OrganizationTagTypeConstants.AmenityWhiteboard => OrganizationTagType.AmenityWhiteboard,
                OrganizationTagTypeConstants.AmenityGlassBoard => OrganizationTagType.AmenityGlassBoard,
                OrganizationTagTypeConstants.AmenityFlipChart => OrganizationTagType.AmenityFlipChart,
                OrganizationTagTypeConstants.AmenityMarkersIncluded => OrganizationTagType.AmenityMarkersIncluded,
                OrganizationTagTypeConstants.AmenityProjector => OrganizationTagType.AmenityProjector,
                OrganizationTagTypeConstants.AmenityLargeDisplay => OrganizationTagType.AmenityLargeDisplay,
                OrganizationTagTypeConstants.AmenityHdmiUsbCCasting => OrganizationTagType.AmenityHdmiUsbCCasting,
                OrganizationTagTypeConstants.AmenityVideoConferencingKit => OrganizationTagType.AmenityVideoConferencingKit,
                OrganizationTagTypeConstants.AmenitySpeakerphone => OrganizationTagType.AmenitySpeakerphone,
                OrganizationTagTypeConstants.AmenityRoomCamera => OrganizationTagType.AmenityRoomCamera,
                OrganizationTagTypeConstants.AmenityConferenceTable => OrganizationTagType.AmenityConferenceTable,
                OrganizationTagTypeConstants.AmenityBreakoutArea => OrganizationTagType.AmenityBreakoutArea,
                OrganizationTagTypeConstants.AmenityAcousticTreatment => OrganizationTagType.AmenityAcousticTreatment,
                OrganizationTagTypeConstants.AmenitySoundproofRoom => OrganizationTagType.AmenitySoundproofRoom,
                OrganizationTagTypeConstants.AmenityPhoneBooth => OrganizationTagType.AmenityPhoneBooth,
                OrganizationTagTypeConstants.AmenityQuietZone => OrganizationTagType.AmenityQuietZone,
                OrganizationTagTypeConstants.AmenityFocusRoom => OrganizationTagType.AmenityFocusRoom,
                OrganizationTagTypeConstants.AmenityConfidentialMeetingRoom => OrganizationTagType.AmenityConfidentialMeetingRoom,
                OrganizationTagTypeConstants.AmenityNoiseCancellingPanels => OrganizationTagType.AmenityNoiseCancellingPanels,
                OrganizationTagTypeConstants.AmenityAirConditioning => OrganizationTagType.AmenityAirConditioning,
                OrganizationTagTypeConstants.AmenityHeating => OrganizationTagType.AmenityHeating,
                OrganizationTagTypeConstants.AmenityFreshAirVentilation => OrganizationTagType.AmenityFreshAirVentilation,
                OrganizationTagTypeConstants.AmenityOperableWindows => OrganizationTagType.AmenityOperableWindows,
                OrganizationTagTypeConstants.AmenityNaturalLight => OrganizationTagType.AmenityNaturalLight,
                OrganizationTagTypeConstants.AmenityBlackoutBlinds => OrganizationTagType.AmenityBlackoutBlinds,
                OrganizationTagTypeConstants.AmenityAmbientLightingControls => OrganizationTagType.AmenityAmbientLightingControls,
                OrganizationTagTypeConstants.AmenityTemperatureControls => OrganizationTagType.AmenityTemperatureControls,
                OrganizationTagTypeConstants.AmenityAirPurifier => OrganizationTagType.AmenityAirPurifier,
                OrganizationTagTypeConstants.AmenityHumidityControl => OrganizationTagType.AmenityHumidityControl,
                OrganizationTagTypeConstants.AmenityCoffeeMachine => OrganizationTagType.AmenityCoffeeMachine,
                OrganizationTagTypeConstants.AmenityTeaStation => OrganizationTagType.AmenityTeaStation,
                OrganizationTagTypeConstants.AmenityFilteredWater => OrganizationTagType.AmenityFilteredWater,
                OrganizationTagTypeConstants.AmenitySparklingWater => OrganizationTagType.AmenitySparklingWater,
                OrganizationTagTypeConstants.AmenitySnackBar => OrganizationTagType.AmenitySnackBar,
                OrganizationTagTypeConstants.AmenityKitchenette => OrganizationTagType.AmenityKitchenette,
                OrganizationTagTypeConstants.AmenityFullKitchen => OrganizationTagType.AmenityFullKitchen,
                OrganizationTagTypeConstants.AmenityRefrigerator => OrganizationTagType.AmenityRefrigerator,
                OrganizationTagTypeConstants.AmenityMicrowave => OrganizationTagType.AmenityMicrowave,
                OrganizationTagTypeConstants.AmenityDishwasher => OrganizationTagType.AmenityDishwasher,
                OrganizationTagTypeConstants.AmenityCateringSupport => OrganizationTagType.AmenityCateringSupport,
                OrganizationTagTypeConstants.AmenityWheelchairAccessibleEntrance => OrganizationTagType.AmenityWheelchairAccessibleEntrance,
                OrganizationTagTypeConstants.AmenityElevatorAccess => OrganizationTagType.AmenityElevatorAccess,
                OrganizationTagTypeConstants.AmenityAccessibleRestroom => OrganizationTagType.AmenityAccessibleRestroom,
                OrganizationTagTypeConstants.AmenityAccessibleDeskHeight => OrganizationTagType.AmenityAccessibleDeskHeight,
                OrganizationTagTypeConstants.AmenityBrailleSignage => OrganizationTagType.AmenityBrailleSignage,
                OrganizationTagTypeConstants.AmenityHearingLoop => OrganizationTagType.AmenityHearingLoop,
                OrganizationTagTypeConstants.AmenityStepFreePath => OrganizationTagType.AmenityStepFreePath,
                OrganizationTagTypeConstants.AmenityWideDoorways => OrganizationTagType.AmenityWideDoorways,
                OrganizationTagTypeConstants.AmenityGenderNeutralRestroom => OrganizationTagType.AmenityGenderNeutralRestroom,
                OrganizationTagTypeConstants.AmenityPrayerRoom => OrganizationTagType.AmenityPrayerRoom,
                OrganizationTagTypeConstants.AmenityAccess247 => OrganizationTagType.AmenityAccess247,
                OrganizationTagTypeConstants.AmenityBusinessHoursAccess => OrganizationTagType.AmenityBusinessHoursAccess,
                OrganizationTagTypeConstants.AmenityKeycardEntry => OrganizationTagType.AmenityKeycardEntry,
                OrganizationTagTypeConstants.AmenityBiometricEntry => OrganizationTagType.AmenityBiometricEntry,
                OrganizationTagTypeConstants.AmenityReceptionDesk => OrganizationTagType.AmenityReceptionDesk,
                OrganizationTagTypeConstants.AmenityVisitorManagement => OrganizationTagType.AmenityVisitorManagement,
                OrganizationTagTypeConstants.AmenityCctv => OrganizationTagType.AmenityCctv,
                OrganizationTagTypeConstants.AmenityOnSiteSecurity => OrganizationTagType.AmenityOnSiteSecurity,
                OrganizationTagTypeConstants.AmenityLockableRoom => OrganizationTagType.AmenityLockableRoom,
                OrganizationTagTypeConstants.AmenityLockerStorage => OrganizationTagType.AmenityLockerStorage,
                OrganizationTagTypeConstants.AmenityAlarmSystem => OrganizationTagType.AmenityAlarmSystem,
                OrganizationTagTypeConstants.AmenityRestroomsNearby => OrganizationTagType.AmenityRestroomsNearby,
                OrganizationTagTypeConstants.AmenityShowerFacilities => OrganizationTagType.AmenityShowerFacilities,
                OrganizationTagTypeConstants.AmenityEndOfTripFacilities => OrganizationTagType.AmenityEndOfTripFacilities,
                OrganizationTagTypeConstants.AmenityBikeStorage => OrganizationTagType.AmenityBikeStorage,
                OrganizationTagTypeConstants.AmenityLoadingDockAccess => OrganizationTagType.AmenityLoadingDockAccess,
                OrganizationTagTypeConstants.AmenityFreightElevator => OrganizationTagType.AmenityFreightElevator,
                OrganizationTagTypeConstants.AmenityMailHandling => OrganizationTagType.AmenityMailHandling,
                OrganizationTagTypeConstants.AmenityPackageReceiving => OrganizationTagType.AmenityPackageReceiving,
                OrganizationTagTypeConstants.AmenityStorageRoom => OrganizationTagType.AmenityStorageRoom,
                OrganizationTagTypeConstants.AmenityWasteRecyclingStation => OrganizationTagType.AmenityWasteRecyclingStation,
                OrganizationTagTypeConstants.AmenityOnSiteParking => OrganizationTagType.AmenityOnSiteParking,
                OrganizationTagTypeConstants.AmenityReservedParking => OrganizationTagType.AmenityReservedParking,
                OrganizationTagTypeConstants.AmenityEvCharging => OrganizationTagType.AmenityEvCharging,
                OrganizationTagTypeConstants.AmenityStreetParkingNearby => OrganizationTagType.AmenityStreetParkingNearby,
                OrganizationTagTypeConstants.AmenityPublicTransitNearby => OrganizationTagType.AmenityPublicTransitNearby,
                OrganizationTagTypeConstants.AmenityTaxiRideshareZone => OrganizationTagType.AmenityTaxiRideshareZone,
                OrganizationTagTypeConstants.AmenityBicycleFriendlyAccess => OrganizationTagType.AmenityBicycleFriendlyAccess,
                OrganizationTagTypeConstants.AmenityGymAccess => OrganizationTagType.AmenityGymAccess,
                OrganizationTagTypeConstants.AmenityWellnessRoom => OrganizationTagType.AmenityWellnessRoom,
                OrganizationTagTypeConstants.AmenityNapPodRoom => OrganizationTagType.AmenityNapPodRoom,
                OrganizationTagTypeConstants.AmenityOutdoorTerrace => OrganizationTagType.AmenityOutdoorTerrace,
                OrganizationTagTypeConstants.AmenityGardenPatio => OrganizationTagType.AmenityGardenPatio,
                OrganizationTagTypeConstants.AmenityGameLoungeArea => OrganizationTagType.AmenityGameLoungeArea,
                OrganizationTagTypeConstants.AmenityStandingMeetingZone => OrganizationTagType.AmenityStandingMeetingZone,
                OrganizationTagTypeConstants.AmenityMeditationRoom => OrganizationTagType.AmenityMeditationRoom,
                OrganizationTagTypeConstants.AmenityChildcareNearby => OrganizationTagType.AmenityChildcareNearby,
                OrganizationTagTypeConstants.AmenityFamilyRoom => OrganizationTagType.AmenityFamilyRoom,
                OrganizationTagTypeConstants.AmenityLactationRoom => OrganizationTagType.AmenityLactationRoom,
                OrganizationTagTypeConstants.AmenityPetFriendly => OrganizationTagType.AmenityPetFriendly,
                OrganizationTagTypeConstants.AmenityServiceAnimalReady => OrganizationTagType.AmenityServiceAnimalReady,
                OrganizationTagTypeConstants.AmenityItSupportOnSite => OrganizationTagType.AmenityItSupportOnSite,
                OrganizationTagTypeConstants.AmenityFrontDeskSupport => OrganizationTagType.AmenityFrontDeskSupport,
                OrganizationTagTypeConstants.AmenityCommunityManager => OrganizationTagType.AmenityCommunityManager,
                OrganizationTagTypeConstants.AmenityCleaningService => OrganizationTagType.AmenityCleaningService,
                OrganizationTagTypeConstants.AmenityAfterHoursSupport => OrganizationTagType.AmenityAfterHoursSupport,
                OrganizationTagTypeConstants.AmenityFacilitiesHelpdesk => OrganizationTagType.AmenityFacilitiesHelpdesk,
                OrganizationTagTypeConstants.AmenityOnDemandSetup => OrganizationTagType.AmenityOnDemandSetup,
                OrganizationTagTypeConstants.AmenityHourlyBookable => OrganizationTagType.AmenityHourlyBookable,
                OrganizationTagTypeConstants.AmenityDailyPass => OrganizationTagType.AmenityDailyPass,
                OrganizationTagTypeConstants.AmenityMonthlyMembership => OrganizationTagType.AmenityMonthlyMembership,
                OrganizationTagTypeConstants.AmenityMinimumBookingDuration => OrganizationTagType.AmenityMinimumBookingDuration,
                OrganizationTagTypeConstants.AmenityMaxCapacity => OrganizationTagType.AmenityMaxCapacity,
                OrganizationTagTypeConstants.AmenityCancellationFlexibility => OrganizationTagType.AmenityCancellationFlexibility,
                OrganizationTagTypeConstants.AmenityRefundableBooking => OrganizationTagType.AmenityRefundableBooking,
                OrganizationTagTypeConstants.AmenityDepositRequired => OrganizationTagType.AmenityDepositRequired,
                OrganizationTagTypeConstants.AmenityLeadTimeRequirement => OrganizationTagType.AmenityLeadTimeRequirement,
                OrganizationTagTypeConstants.AmenityCheckInRequired => OrganizationTagType.AmenityCheckInRequired,
                OrganizationTagTypeConstants.AmenityHostApprovalRequired => OrganizationTagType.AmenityHostApprovalRequired,
                OrganizationTagTypeConstants.AmenityDeskPhone => OrganizationTagType.AmenityDeskPhone,
                OrganizationTagTypeConstants.AmenityKeyboardMouseIncluded => OrganizationTagType.AmenityKeyboardMouseIncluded,
                OrganizationTagTypeConstants.AmenityMonitorArm => OrganizationTagType.AmenityMonitorArm,
                OrganizationTagTypeConstants.AmenityBoardroomLayout => OrganizationTagType.AmenityBoardroomLayout,
                OrganizationTagTypeConstants.AmenityClassroomLayout => OrganizationTagType.AmenityClassroomLayout,
                OrganizationTagTypeConstants.AmenityHybridReady => OrganizationTagType.AmenityHybridReady,
                OrganizationTagTypeConstants.AmenityRecordingEnabled => OrganizationTagType.AmenityRecordingEnabled,
                OrganizationTagTypeConstants.AmenityStage => OrganizationTagType.AmenityStage,
                OrganizationTagTypeConstants.AmenityPaSystem => OrganizationTagType.AmenityPaSystem,
                OrganizationTagTypeConstants.AmenityLightingRig => OrganizationTagType.AmenityLightingRig,
                OrganizationTagTypeConstants.AmenityMovableSeating => OrganizationTagType.AmenityMovableSeating,
                OrganizationTagTypeConstants.AmenityEventStaffSupport => OrganizationTagType.AmenityEventStaffSupport,
                OrganizationTagTypeConstants.AmenitySoundBooth => OrganizationTagType.AmenitySoundBooth,
                OrganizationTagTypeConstants.AmenityGreenScreen => OrganizationTagType.AmenityGreenScreen,
                OrganizationTagTypeConstants.AmenitySpecializedToolsEquipment => OrganizationTagType.AmenitySpecializedToolsEquipment,
                OrganizationTagTypeConstants.AmenitySafetyComplianceKit => OrganizationTagType.AmenitySafetyComplianceKit,

                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }

    extension(string? src)
    {
        public OrganizationTagType? ToNullableOrganizationTagType() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src.ToOrganizationTagType();
    }

    extension(OrganizationTagType src)
    {
        public string ToOrganizationTagType() =>
            src switch
            {
                OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
                OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
                OrganizationTagType.Product => OrganizationTagTypeConstants.Product,

                OrganizationTagType.ResourceDesk => OrganizationTagTypeConstants.ResourceDesk,
                OrganizationTagType.ResourceRoom => OrganizationTagTypeConstants.ResourceRoom,
                OrganizationTagType.ResourceParking => OrganizationTagTypeConstants.ResourceParking,
                OrganizationTagType.ResourceOthers => OrganizationTagTypeConstants.ResourceOthers,
                OrganizationTagType.ResourceEntireLocation => OrganizationTagTypeConstants.ResourceEntireLocation,

                OrganizationTagType.LocationSpaceTypeCarParkSpace => OrganizationTagTypeConstants.LocationSpaceTypeCarParkSpace,
                OrganizationTagType.LocationSpaceTypeEventSpace => OrganizationTagTypeConstants.LocationSpaceTypeEventSpace,
                OrganizationTagType.LocationSpaceTypeMeetingSpace => OrganizationTagTypeConstants.LocationSpaceTypeMeetingSpace,
                OrganizationTagType.LocationSpaceTypeOfficeSpace => OrganizationTagTypeConstants.LocationSpaceTypeOfficeSpace,
                OrganizationTagType.LocationSpaceTypeRetailSpace => OrganizationTagTypeConstants.LocationSpaceTypeRetailSpace,
                OrganizationTagType.LocationSpaceTypeStorageSpace => OrganizationTagTypeConstants.LocationSpaceTypeStorageSpace,
                OrganizationTagType.LocationSpaceTypeStudioSpace => OrganizationTagTypeConstants.LocationSpaceTypeStudioSpace,
                OrganizationTagType.LocationSpaceTypeCommercialKitchen => OrganizationTagTypeConstants.LocationSpaceTypeCommercialKitchen,
                OrganizationTagType.LocationSpaceTypeShootLocation => OrganizationTagTypeConstants.LocationSpaceTypeShootLocation,
                OrganizationTagType.LocationSpaceTypeOthers => OrganizationTagTypeConstants.LocationSpaceTypeOthers,

                OrganizationTagType.AmenityHighSpeedWifi => OrganizationTagTypeConstants.AmenityHighSpeedWifi,
                OrganizationTagType.AmenityDedicatedVlan => OrganizationTagTypeConstants.AmenityDedicatedVlan,
                OrganizationTagType.AmenityWiredEthernet => OrganizationTagTypeConstants.AmenityWiredEthernet,
                OrganizationTagType.AmenityGuestNetwork => OrganizationTagTypeConstants.AmenityGuestNetwork,
                OrganizationTagType.AmenityVpnSupport => OrganizationTagTypeConstants.AmenityVpnSupport,
                OrganizationTagType.AmenityBackupInternet => OrganizationTagTypeConstants.AmenityBackupInternet,
                OrganizationTagType.AmenityNetworkPrinting => OrganizationTagTypeConstants.AmenityNetworkPrinting,
                OrganizationTagType.AmenitySecureSsid => OrganizationTagTypeConstants.AmenitySecureSsid,
                OrganizationTagType.AmenityPublicIpOption => OrganizationTagTypeConstants.AmenityPublicIpOption,
                OrganizationTagType.AmenityStandardPowerOutlets => OrganizationTagTypeConstants.AmenityStandardPowerOutlets,
                OrganizationTagType.AmenityUsbAPorts => OrganizationTagTypeConstants.AmenityUsbAPorts,
                OrganizationTagType.AmenityUsbCPorts => OrganizationTagTypeConstants.AmenityUsbCPorts,
                OrganizationTagType.AmenityWirelessCharging => OrganizationTagTypeConstants.AmenityWirelessCharging,
                OrganizationTagType.AmenityPowerStrips => OrganizationTagTypeConstants.AmenityPowerStrips,
                OrganizationTagType.AmenityUpsBackedPower => OrganizationTagTypeConstants.AmenityUpsBackedPower,
                OrganizationTagType.AmenityMonitorDockingStation => OrganizationTagTypeConstants.AmenityMonitorDockingStation,
                OrganizationTagType.AmenityLaptopStandAvailability => OrganizationTagTypeConstants.AmenityLaptopStandAvailability,
                OrganizationTagType.AmenityAdjustableDesk => OrganizationTagTypeConstants.AmenityAdjustableDesk,
                OrganizationTagType.AmenityStandingDesk => OrganizationTagTypeConstants.AmenityStandingDesk,
                OrganizationTagType.AmenityFixedDesk => OrganizationTagTypeConstants.AmenityFixedDesk,
                OrganizationTagType.AmenityErgonomicChair => OrganizationTagTypeConstants.AmenityErgonomicChair,
                OrganizationTagType.AmenityExecutiveChair => OrganizationTagTypeConstants.AmenityExecutiveChair,
                OrganizationTagType.AmenityDualMonitorSetup => OrganizationTagTypeConstants.AmenityDualMonitorSetup,
                OrganizationTagType.AmenityPrivacyDeskDivider => OrganizationTagTypeConstants.AmenityPrivacyDeskDivider,
                OrganizationTagType.AmenityLockablePedestal => OrganizationTagTypeConstants.AmenityLockablePedestal,
                OrganizationTagType.AmenityWhiteboardAtDesk => OrganizationTagTypeConstants.AmenityWhiteboardAtDesk,
                OrganizationTagType.AmenityTaskLamp => OrganizationTagTypeConstants.AmenityTaskLamp,
                OrganizationTagType.AmenityWhiteboard => OrganizationTagTypeConstants.AmenityWhiteboard,
                OrganizationTagType.AmenityGlassBoard => OrganizationTagTypeConstants.AmenityGlassBoard,
                OrganizationTagType.AmenityFlipChart => OrganizationTagTypeConstants.AmenityFlipChart,
                OrganizationTagType.AmenityMarkersIncluded => OrganizationTagTypeConstants.AmenityMarkersIncluded,
                OrganizationTagType.AmenityProjector => OrganizationTagTypeConstants.AmenityProjector,
                OrganizationTagType.AmenityLargeDisplay => OrganizationTagTypeConstants.AmenityLargeDisplay,
                OrganizationTagType.AmenityHdmiUsbCCasting => OrganizationTagTypeConstants.AmenityHdmiUsbCCasting,
                OrganizationTagType.AmenityVideoConferencingKit => OrganizationTagTypeConstants.AmenityVideoConferencingKit,
                OrganizationTagType.AmenitySpeakerphone => OrganizationTagTypeConstants.AmenitySpeakerphone,
                OrganizationTagType.AmenityRoomCamera => OrganizationTagTypeConstants.AmenityRoomCamera,
                OrganizationTagType.AmenityConferenceTable => OrganizationTagTypeConstants.AmenityConferenceTable,
                OrganizationTagType.AmenityBreakoutArea => OrganizationTagTypeConstants.AmenityBreakoutArea,
                OrganizationTagType.AmenityAcousticTreatment => OrganizationTagTypeConstants.AmenityAcousticTreatment,
                OrganizationTagType.AmenitySoundproofRoom => OrganizationTagTypeConstants.AmenitySoundproofRoom,
                OrganizationTagType.AmenityPhoneBooth => OrganizationTagTypeConstants.AmenityPhoneBooth,
                OrganizationTagType.AmenityQuietZone => OrganizationTagTypeConstants.AmenityQuietZone,
                OrganizationTagType.AmenityFocusRoom => OrganizationTagTypeConstants.AmenityFocusRoom,
                OrganizationTagType.AmenityConfidentialMeetingRoom => OrganizationTagTypeConstants.AmenityConfidentialMeetingRoom,
                OrganizationTagType.AmenityNoiseCancellingPanels => OrganizationTagTypeConstants.AmenityNoiseCancellingPanels,
                OrganizationTagType.AmenityAirConditioning => OrganizationTagTypeConstants.AmenityAirConditioning,
                OrganizationTagType.AmenityHeating => OrganizationTagTypeConstants.AmenityHeating,
                OrganizationTagType.AmenityFreshAirVentilation => OrganizationTagTypeConstants.AmenityFreshAirVentilation,
                OrganizationTagType.AmenityOperableWindows => OrganizationTagTypeConstants.AmenityOperableWindows,
                OrganizationTagType.AmenityNaturalLight => OrganizationTagTypeConstants.AmenityNaturalLight,
                OrganizationTagType.AmenityBlackoutBlinds => OrganizationTagTypeConstants.AmenityBlackoutBlinds,
                OrganizationTagType.AmenityAmbientLightingControls => OrganizationTagTypeConstants.AmenityAmbientLightingControls,
                OrganizationTagType.AmenityTemperatureControls => OrganizationTagTypeConstants.AmenityTemperatureControls,
                OrganizationTagType.AmenityAirPurifier => OrganizationTagTypeConstants.AmenityAirPurifier,
                OrganizationTagType.AmenityHumidityControl => OrganizationTagTypeConstants.AmenityHumidityControl,
                OrganizationTagType.AmenityCoffeeMachine => OrganizationTagTypeConstants.AmenityCoffeeMachine,
                OrganizationTagType.AmenityTeaStation => OrganizationTagTypeConstants.AmenityTeaStation,
                OrganizationTagType.AmenityFilteredWater => OrganizationTagTypeConstants.AmenityFilteredWater,
                OrganizationTagType.AmenitySparklingWater => OrganizationTagTypeConstants.AmenitySparklingWater,
                OrganizationTagType.AmenitySnackBar => OrganizationTagTypeConstants.AmenitySnackBar,
                OrganizationTagType.AmenityKitchenette => OrganizationTagTypeConstants.AmenityKitchenette,
                OrganizationTagType.AmenityFullKitchen => OrganizationTagTypeConstants.AmenityFullKitchen,
                OrganizationTagType.AmenityRefrigerator => OrganizationTagTypeConstants.AmenityRefrigerator,
                OrganizationTagType.AmenityMicrowave => OrganizationTagTypeConstants.AmenityMicrowave,
                OrganizationTagType.AmenityDishwasher => OrganizationTagTypeConstants.AmenityDishwasher,
                OrganizationTagType.AmenityCateringSupport => OrganizationTagTypeConstants.AmenityCateringSupport,
                OrganizationTagType.AmenityWheelchairAccessibleEntrance => OrganizationTagTypeConstants.AmenityWheelchairAccessibleEntrance,
                OrganizationTagType.AmenityElevatorAccess => OrganizationTagTypeConstants.AmenityElevatorAccess,
                OrganizationTagType.AmenityAccessibleRestroom => OrganizationTagTypeConstants.AmenityAccessibleRestroom,
                OrganizationTagType.AmenityAccessibleDeskHeight => OrganizationTagTypeConstants.AmenityAccessibleDeskHeight,
                OrganizationTagType.AmenityBrailleSignage => OrganizationTagTypeConstants.AmenityBrailleSignage,
                OrganizationTagType.AmenityHearingLoop => OrganizationTagTypeConstants.AmenityHearingLoop,
                OrganizationTagType.AmenityStepFreePath => OrganizationTagTypeConstants.AmenityStepFreePath,
                OrganizationTagType.AmenityWideDoorways => OrganizationTagTypeConstants.AmenityWideDoorways,
                OrganizationTagType.AmenityGenderNeutralRestroom => OrganizationTagTypeConstants.AmenityGenderNeutralRestroom,
                OrganizationTagType.AmenityPrayerRoom => OrganizationTagTypeConstants.AmenityPrayerRoom,
                OrganizationTagType.AmenityAccess247 => OrganizationTagTypeConstants.AmenityAccess247,
                OrganizationTagType.AmenityBusinessHoursAccess => OrganizationTagTypeConstants.AmenityBusinessHoursAccess,
                OrganizationTagType.AmenityKeycardEntry => OrganizationTagTypeConstants.AmenityKeycardEntry,
                OrganizationTagType.AmenityBiometricEntry => OrganizationTagTypeConstants.AmenityBiometricEntry,
                OrganizationTagType.AmenityReceptionDesk => OrganizationTagTypeConstants.AmenityReceptionDesk,
                OrganizationTagType.AmenityVisitorManagement => OrganizationTagTypeConstants.AmenityVisitorManagement,
                OrganizationTagType.AmenityCctv => OrganizationTagTypeConstants.AmenityCctv,
                OrganizationTagType.AmenityOnSiteSecurity => OrganizationTagTypeConstants.AmenityOnSiteSecurity,
                OrganizationTagType.AmenityLockableRoom => OrganizationTagTypeConstants.AmenityLockableRoom,
                OrganizationTagType.AmenityLockerStorage => OrganizationTagTypeConstants.AmenityLockerStorage,
                OrganizationTagType.AmenityAlarmSystem => OrganizationTagTypeConstants.AmenityAlarmSystem,
                OrganizationTagType.AmenityRestroomsNearby => OrganizationTagTypeConstants.AmenityRestroomsNearby,
                OrganizationTagType.AmenityShowerFacilities => OrganizationTagTypeConstants.AmenityShowerFacilities,
                OrganizationTagType.AmenityEndOfTripFacilities => OrganizationTagTypeConstants.AmenityEndOfTripFacilities,
                OrganizationTagType.AmenityBikeStorage => OrganizationTagTypeConstants.AmenityBikeStorage,
                OrganizationTagType.AmenityLoadingDockAccess => OrganizationTagTypeConstants.AmenityLoadingDockAccess,
                OrganizationTagType.AmenityFreightElevator => OrganizationTagTypeConstants.AmenityFreightElevator,
                OrganizationTagType.AmenityMailHandling => OrganizationTagTypeConstants.AmenityMailHandling,
                OrganizationTagType.AmenityPackageReceiving => OrganizationTagTypeConstants.AmenityPackageReceiving,
                OrganizationTagType.AmenityStorageRoom => OrganizationTagTypeConstants.AmenityStorageRoom,
                OrganizationTagType.AmenityWasteRecyclingStation => OrganizationTagTypeConstants.AmenityWasteRecyclingStation,
                OrganizationTagType.AmenityOnSiteParking => OrganizationTagTypeConstants.AmenityOnSiteParking,
                OrganizationTagType.AmenityReservedParking => OrganizationTagTypeConstants.AmenityReservedParking,
                OrganizationTagType.AmenityEvCharging => OrganizationTagTypeConstants.AmenityEvCharging,
                OrganizationTagType.AmenityStreetParkingNearby => OrganizationTagTypeConstants.AmenityStreetParkingNearby,
                OrganizationTagType.AmenityPublicTransitNearby => OrganizationTagTypeConstants.AmenityPublicTransitNearby,
                OrganizationTagType.AmenityTaxiRideshareZone => OrganizationTagTypeConstants.AmenityTaxiRideshareZone,
                OrganizationTagType.AmenityBicycleFriendlyAccess => OrganizationTagTypeConstants.AmenityBicycleFriendlyAccess,
                OrganizationTagType.AmenityGymAccess => OrganizationTagTypeConstants.AmenityGymAccess,
                OrganizationTagType.AmenityWellnessRoom => OrganizationTagTypeConstants.AmenityWellnessRoom,
                OrganizationTagType.AmenityNapPodRoom => OrganizationTagTypeConstants.AmenityNapPodRoom,
                OrganizationTagType.AmenityOutdoorTerrace => OrganizationTagTypeConstants.AmenityOutdoorTerrace,
                OrganizationTagType.AmenityGardenPatio => OrganizationTagTypeConstants.AmenityGardenPatio,
                OrganizationTagType.AmenityGameLoungeArea => OrganizationTagTypeConstants.AmenityGameLoungeArea,
                OrganizationTagType.AmenityStandingMeetingZone => OrganizationTagTypeConstants.AmenityStandingMeetingZone,
                OrganizationTagType.AmenityMeditationRoom => OrganizationTagTypeConstants.AmenityMeditationRoom,
                OrganizationTagType.AmenityChildcareNearby => OrganizationTagTypeConstants.AmenityChildcareNearby,
                OrganizationTagType.AmenityFamilyRoom => OrganizationTagTypeConstants.AmenityFamilyRoom,
                OrganizationTagType.AmenityLactationRoom => OrganizationTagTypeConstants.AmenityLactationRoom,
                OrganizationTagType.AmenityPetFriendly => OrganizationTagTypeConstants.AmenityPetFriendly,
                OrganizationTagType.AmenityServiceAnimalReady => OrganizationTagTypeConstants.AmenityServiceAnimalReady,
                OrganizationTagType.AmenityItSupportOnSite => OrganizationTagTypeConstants.AmenityItSupportOnSite,
                OrganizationTagType.AmenityFrontDeskSupport => OrganizationTagTypeConstants.AmenityFrontDeskSupport,
                OrganizationTagType.AmenityCommunityManager => OrganizationTagTypeConstants.AmenityCommunityManager,
                OrganizationTagType.AmenityCleaningService => OrganizationTagTypeConstants.AmenityCleaningService,
                OrganizationTagType.AmenityAfterHoursSupport => OrganizationTagTypeConstants.AmenityAfterHoursSupport,
                OrganizationTagType.AmenityFacilitiesHelpdesk => OrganizationTagTypeConstants.AmenityFacilitiesHelpdesk,
                OrganizationTagType.AmenityOnDemandSetup => OrganizationTagTypeConstants.AmenityOnDemandSetup,
                OrganizationTagType.AmenityHourlyBookable => OrganizationTagTypeConstants.AmenityHourlyBookable,
                OrganizationTagType.AmenityDailyPass => OrganizationTagTypeConstants.AmenityDailyPass,
                OrganizationTagType.AmenityMonthlyMembership => OrganizationTagTypeConstants.AmenityMonthlyMembership,
                OrganizationTagType.AmenityMinimumBookingDuration => OrganizationTagTypeConstants.AmenityMinimumBookingDuration,
                OrganizationTagType.AmenityMaxCapacity => OrganizationTagTypeConstants.AmenityMaxCapacity,
                OrganizationTagType.AmenityCancellationFlexibility => OrganizationTagTypeConstants.AmenityCancellationFlexibility,
                OrganizationTagType.AmenityRefundableBooking => OrganizationTagTypeConstants.AmenityRefundableBooking,
                OrganizationTagType.AmenityDepositRequired => OrganizationTagTypeConstants.AmenityDepositRequired,
                OrganizationTagType.AmenityLeadTimeRequirement => OrganizationTagTypeConstants.AmenityLeadTimeRequirement,
                OrganizationTagType.AmenityCheckInRequired => OrganizationTagTypeConstants.AmenityCheckInRequired,
                OrganizationTagType.AmenityHostApprovalRequired => OrganizationTagTypeConstants.AmenityHostApprovalRequired,
                OrganizationTagType.AmenityDeskPhone => OrganizationTagTypeConstants.AmenityDeskPhone,
                OrganizationTagType.AmenityKeyboardMouseIncluded => OrganizationTagTypeConstants.AmenityKeyboardMouseIncluded,
                OrganizationTagType.AmenityMonitorArm => OrganizationTagTypeConstants.AmenityMonitorArm,
                OrganizationTagType.AmenityBoardroomLayout => OrganizationTagTypeConstants.AmenityBoardroomLayout,
                OrganizationTagType.AmenityClassroomLayout => OrganizationTagTypeConstants.AmenityClassroomLayout,
                OrganizationTagType.AmenityHybridReady => OrganizationTagTypeConstants.AmenityHybridReady,
                OrganizationTagType.AmenityRecordingEnabled => OrganizationTagTypeConstants.AmenityRecordingEnabled,
                OrganizationTagType.AmenityStage => OrganizationTagTypeConstants.AmenityStage,
                OrganizationTagType.AmenityPaSystem => OrganizationTagTypeConstants.AmenityPaSystem,
                OrganizationTagType.AmenityLightingRig => OrganizationTagTypeConstants.AmenityLightingRig,
                OrganizationTagType.AmenityMovableSeating => OrganizationTagTypeConstants.AmenityMovableSeating,
                OrganizationTagType.AmenityEventStaffSupport => OrganizationTagTypeConstants.AmenityEventStaffSupport,
                OrganizationTagType.AmenitySoundBooth => OrganizationTagTypeConstants.AmenitySoundBooth,
                OrganizationTagType.AmenityGreenScreen => OrganizationTagTypeConstants.AmenityGreenScreen,
                OrganizationTagType.AmenitySpecializedToolsEquipment => OrganizationTagTypeConstants.AmenitySpecializedToolsEquipment,
                OrganizationTagType.AmenitySafetyComplianceKit => OrganizationTagTypeConstants.AmenitySafetyComplianceKit,

                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToOrganizationTagTypeName() =>
            src switch
            {
                OrganizationTagType.Custom => "Custom",
                OrganizationTagType.Zone => "Zone",
                OrganizationTagType.Product => "Product",

                OrganizationTagType.ResourceDesk => "Desk",
                OrganizationTagType.ResourceRoom => "Room",
                OrganizationTagType.ResourceParking => "Parking",
                OrganizationTagType.ResourceOthers => "Others",
                OrganizationTagType.ResourceEntireLocation => "Entire Location",

                OrganizationTagType.LocationSpaceTypeCarParkSpace => "Car Park Space",
                OrganizationTagType.LocationSpaceTypeEventSpace => "Event Space",
                OrganizationTagType.LocationSpaceTypeMeetingSpace => "Meeting Space",
                OrganizationTagType.LocationSpaceTypeOfficeSpace => "Office Space",
                OrganizationTagType.LocationSpaceTypeRetailSpace => "Retail Space",
                OrganizationTagType.LocationSpaceTypeStorageSpace => "Storage Space",
                OrganizationTagType.LocationSpaceTypeStudioSpace => "Studio Space",
                OrganizationTagType.LocationSpaceTypeCommercialKitchen => "Commercial Kitchen",
                OrganizationTagType.LocationSpaceTypeShootLocation => "Shoot Location",
                OrganizationTagType.LocationSpaceTypeOthers => "Others",

                OrganizationTagType.AmenityHighSpeedWifi => "High Speed WiFi",
                OrganizationTagType.AmenityDedicatedVlan => "Dedicated VLAN",
                OrganizationTagType.AmenityWiredEthernet => "Wired Ethernet",
                OrganizationTagType.AmenityGuestNetwork => "Guest Network",
                OrganizationTagType.AmenityVpnSupport => "VPN Support",
                OrganizationTagType.AmenityBackupInternet => "Backup Internet",
                OrganizationTagType.AmenityNetworkPrinting => "Network Printing",
                OrganizationTagType.AmenitySecureSsid => "Secure SSID",
                OrganizationTagType.AmenityPublicIpOption => "Public IP Option",
                OrganizationTagType.AmenityStandardPowerOutlets => "Standard Power Outlets",
                OrganizationTagType.AmenityUsbAPorts => "USB-A Ports",
                OrganizationTagType.AmenityUsbCPorts => "USB-C Ports",
                OrganizationTagType.AmenityWirelessCharging => "Wireless Charging",
                OrganizationTagType.AmenityPowerStrips => "Power Strips",
                OrganizationTagType.AmenityUpsBackedPower => "UPS Backed Power",
                OrganizationTagType.AmenityMonitorDockingStation => "Monitor Docking Station",
                OrganizationTagType.AmenityLaptopStandAvailability => "Laptop Stand Availability",
                OrganizationTagType.AmenityAdjustableDesk => "Adjustable Desk",
                OrganizationTagType.AmenityStandingDesk => "Standing Desk",
                OrganizationTagType.AmenityFixedDesk => "Fixed Desk",
                OrganizationTagType.AmenityErgonomicChair => "Ergonomic Chair",
                OrganizationTagType.AmenityExecutiveChair => "Executive Chair",
                OrganizationTagType.AmenityDualMonitorSetup => "Dual Monitor Setup",
                OrganizationTagType.AmenityPrivacyDeskDivider => "Privacy Desk Divider",
                OrganizationTagType.AmenityLockablePedestal => "Lockable Pedestal",
                OrganizationTagType.AmenityWhiteboardAtDesk => "Whiteboard at Desk",
                OrganizationTagType.AmenityTaskLamp => "Task Lamp",
                OrganizationTagType.AmenityWhiteboard => "Whiteboard",
                OrganizationTagType.AmenityGlassBoard => "Glass Board",
                OrganizationTagType.AmenityFlipChart => "Flip Chart",
                OrganizationTagType.AmenityMarkersIncluded => "Markers Included",
                OrganizationTagType.AmenityProjector => "Projector",
                OrganizationTagType.AmenityLargeDisplay => "Large Display",
                OrganizationTagType.AmenityHdmiUsbCCasting => "HDMI USB-C Casting",
                OrganizationTagType.AmenityVideoConferencingKit => "Video Conferencing Kit",
                OrganizationTagType.AmenitySpeakerphone => "Speakerphone",
                OrganizationTagType.AmenityRoomCamera => "Room Camera",
                OrganizationTagType.AmenityConferenceTable => "Conference Table",
                OrganizationTagType.AmenityBreakoutArea => "Breakout Area",
                OrganizationTagType.AmenityAcousticTreatment => "Acoustic Treatment",
                OrganizationTagType.AmenitySoundproofRoom => "Soundproof Room",
                OrganizationTagType.AmenityPhoneBooth => "Phone Booth",
                OrganizationTagType.AmenityQuietZone => "Quiet Zone",
                OrganizationTagType.AmenityFocusRoom => "Focus Room",
                OrganizationTagType.AmenityConfidentialMeetingRoom => "Confidential Meeting Room",
                OrganizationTagType.AmenityNoiseCancellingPanels => "Noise Cancelling Panels",
                OrganizationTagType.AmenityAirConditioning => "Air Conditioning",
                OrganizationTagType.AmenityHeating => "Heating",
                OrganizationTagType.AmenityFreshAirVentilation => "Fresh Air Ventilation",
                OrganizationTagType.AmenityOperableWindows => "Operable Windows",
                OrganizationTagType.AmenityNaturalLight => "Natural Light",
                OrganizationTagType.AmenityBlackoutBlinds => "Blackout Blinds",
                OrganizationTagType.AmenityAmbientLightingControls => "Ambient Lighting Controls",
                OrganizationTagType.AmenityTemperatureControls => "Temperature Controls",
                OrganizationTagType.AmenityAirPurifier => "Air Purifier",
                OrganizationTagType.AmenityHumidityControl => "Humidity Control",
                OrganizationTagType.AmenityCoffeeMachine => "Coffee Machine",
                OrganizationTagType.AmenityTeaStation => "Tea Station",
                OrganizationTagType.AmenityFilteredWater => "Filtered Water",
                OrganizationTagType.AmenitySparklingWater => "Sparkling Water",
                OrganizationTagType.AmenitySnackBar => "Snack Bar",
                OrganizationTagType.AmenityKitchenette => "Kitchenette",
                OrganizationTagType.AmenityFullKitchen => "Full Kitchen",
                OrganizationTagType.AmenityRefrigerator => "Refrigerator",
                OrganizationTagType.AmenityMicrowave => "Microwave",
                OrganizationTagType.AmenityDishwasher => "Dishwasher",
                OrganizationTagType.AmenityCateringSupport => "Catering Support",
                OrganizationTagType.AmenityWheelchairAccessibleEntrance => "Wheelchair Accessible Entrance",
                OrganizationTagType.AmenityElevatorAccess => "Elevator Access",
                OrganizationTagType.AmenityAccessibleRestroom => "Accessible Restroom",
                OrganizationTagType.AmenityAccessibleDeskHeight => "Accessible Desk Height",
                OrganizationTagType.AmenityBrailleSignage => "Braille Signage",
                OrganizationTagType.AmenityHearingLoop => "Hearing Loop",
                OrganizationTagType.AmenityStepFreePath => "Step Free Path",
                OrganizationTagType.AmenityWideDoorways => "Wide Doorways",
                OrganizationTagType.AmenityGenderNeutralRestroom => "Gender Neutral Restroom",
                OrganizationTagType.AmenityPrayerRoom => "Prayer Room",
                OrganizationTagType.AmenityAccess247 => "24/7 Access",
                OrganizationTagType.AmenityBusinessHoursAccess => "Business Hours Access",
                OrganizationTagType.AmenityKeycardEntry => "Keycard Entry",
                OrganizationTagType.AmenityBiometricEntry => "Biometric Entry",
                OrganizationTagType.AmenityReceptionDesk => "Reception Desk",
                OrganizationTagType.AmenityVisitorManagement => "Visitor Management",
                OrganizationTagType.AmenityCctv => "CCTV",
                OrganizationTagType.AmenityOnSiteSecurity => "On Site Security",
                OrganizationTagType.AmenityLockableRoom => "Lockable Room",
                OrganizationTagType.AmenityLockerStorage => "Locker Storage",
                OrganizationTagType.AmenityAlarmSystem => "Alarm System",
                OrganizationTagType.AmenityRestroomsNearby => "Restrooms Nearby",
                OrganizationTagType.AmenityShowerFacilities => "Shower Facilities",
                OrganizationTagType.AmenityEndOfTripFacilities => "End of Trip Facilities",
                OrganizationTagType.AmenityBikeStorage => "Bike Storage",
                OrganizationTagType.AmenityLoadingDockAccess => "Loading Dock Access",
                OrganizationTagType.AmenityFreightElevator => "Freight Elevator",
                OrganizationTagType.AmenityMailHandling => "Mail Handling",
                OrganizationTagType.AmenityPackageReceiving => "Package Receiving",
                OrganizationTagType.AmenityStorageRoom => "Storage Room",
                OrganizationTagType.AmenityWasteRecyclingStation => "Waste Recycling Station",
                OrganizationTagType.AmenityOnSiteParking => "On Site Parking",
                OrganizationTagType.AmenityReservedParking => "Reserved Parking",
                OrganizationTagType.AmenityEvCharging => "EV Charging",
                OrganizationTagType.AmenityStreetParkingNearby => "Street Parking Nearby",
                OrganizationTagType.AmenityPublicTransitNearby => "Public Transit Nearby",
                OrganizationTagType.AmenityTaxiRideshareZone => "Taxi Rideshare Zone",
                OrganizationTagType.AmenityBicycleFriendlyAccess => "Bicycle Friendly Access",
                OrganizationTagType.AmenityGymAccess => "Gym Access",
                OrganizationTagType.AmenityWellnessRoom => "Wellness Room",
                OrganizationTagType.AmenityNapPodRoom => "Nap Pod Room",
                OrganizationTagType.AmenityOutdoorTerrace => "Outdoor Terrace",
                OrganizationTagType.AmenityGardenPatio => "Garden Patio",
                OrganizationTagType.AmenityGameLoungeArea => "Game Lounge Area",
                OrganizationTagType.AmenityStandingMeetingZone => "Standing Meeting Zone",
                OrganizationTagType.AmenityMeditationRoom => "Meditation Room",
                OrganizationTagType.AmenityChildcareNearby => "Childcare Nearby",
                OrganizationTagType.AmenityFamilyRoom => "Family Room",
                OrganizationTagType.AmenityLactationRoom => "Lactation Room",
                OrganizationTagType.AmenityPetFriendly => "Pet Friendly",
                OrganizationTagType.AmenityServiceAnimalReady => "Service Animal Ready",
                OrganizationTagType.AmenityItSupportOnSite => "IT Support On Site",
                OrganizationTagType.AmenityFrontDeskSupport => "Front Desk Support",
                OrganizationTagType.AmenityCommunityManager => "Community Manager",
                OrganizationTagType.AmenityCleaningService => "Cleaning Service",
                OrganizationTagType.AmenityAfterHoursSupport => "After Hours Support",
                OrganizationTagType.AmenityFacilitiesHelpdesk => "Facilities Helpdesk",
                OrganizationTagType.AmenityOnDemandSetup => "On Demand Setup",
                OrganizationTagType.AmenityHourlyBookable => "Hourly Bookable",
                OrganizationTagType.AmenityDailyPass => "Daily Pass",
                OrganizationTagType.AmenityMonthlyMembership => "Monthly Membership",
                OrganizationTagType.AmenityMinimumBookingDuration => "Minimum Booking Duration",
                OrganizationTagType.AmenityMaxCapacity => "Max Capacity",
                OrganizationTagType.AmenityCancellationFlexibility => "Cancellation Flexibility",
                OrganizationTagType.AmenityRefundableBooking => "Refundable Booking",
                OrganizationTagType.AmenityDepositRequired => "Deposit Required",
                OrganizationTagType.AmenityLeadTimeRequirement => "Lead Time Requirement",
                OrganizationTagType.AmenityCheckInRequired => "Check In Required",
                OrganizationTagType.AmenityHostApprovalRequired => "Host Approval Required",
                OrganizationTagType.AmenityDeskPhone => "Desk Phone",
                OrganizationTagType.AmenityKeyboardMouseIncluded => "Keyboard Mouse Included",
                OrganizationTagType.AmenityMonitorArm => "Monitor Arm",
                OrganizationTagType.AmenityBoardroomLayout => "Boardroom Layout",
                OrganizationTagType.AmenityClassroomLayout => "Classroom Layout",
                OrganizationTagType.AmenityHybridReady => "Hybrid Ready",
                OrganizationTagType.AmenityRecordingEnabled => "Recording Enabled",
                OrganizationTagType.AmenityStage => "Stage",
                OrganizationTagType.AmenityPaSystem => "PA System",
                OrganizationTagType.AmenityLightingRig => "Lighting Rig",
                OrganizationTagType.AmenityMovableSeating => "Movable Seating",
                OrganizationTagType.AmenityEventStaffSupport => "Event Staff Support",
                OrganizationTagType.AmenitySoundBooth => "Sound Booth",
                OrganizationTagType.AmenityGreenScreen => "Green Screen",
                OrganizationTagType.AmenitySpecializedToolsEquipment => "Specialized Tools Equipment",
                OrganizationTagType.AmenitySafetyComplianceKit => "Safety Compliance Kit",

                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }

    extension(OrganizationTagType? src)
    {
        public string ToNullableOrganizationTagType() =>
            src is null
                ? string.Empty
                : src.Value.ToOrganizationTagType();
    }
}
