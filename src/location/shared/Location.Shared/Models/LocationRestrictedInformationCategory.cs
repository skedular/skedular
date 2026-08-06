namespace Location.Shared.Models;

public enum LocationRestrictedInformationCategory
{
    Other,
    Wifi,
    Access,
    Parking,
    CheckIn,
    CheckOut,
    AfterHours,
    Deliveries,
    Guests,
    Equipment,
    Kitchen,
    MeetingRooms,
    Noise,
    Maintenance,
    Accessibility,
    Storage,
    Cleaning,
    Waste,
    Security,
    Evacuation,
    Pets,
    Smoking,
    HouseRules,
}

public static class LocationRestrictedInformationCategoryConstants
{
    public const string Other = "OTHER";
    public const string Wifi = "WIFI";
    public const string Access = "ACCESS";
    public const string Parking = "PARKING";
    public const string CheckIn = "CHECK_IN";
    public const string CheckOut = "CHECK_OUT";
    public const string AfterHours = "AFTER_HOURS";
    public const string Deliveries = "DELIVERIES";
    public const string Guests = "GUESTS";
    public const string Equipment = "EQUIPMENT";
    public const string Kitchen = "KITCHEN";
    public const string MeetingRooms = "MEETING_ROOMS";
    public const string Noise = "NOISE";
    public const string Maintenance = "MAINTENANCE";
    public const string Accessibility = "ACCESSIBILITY";
    public const string Storage = "STORAGE";
    public const string Cleaning = "CLEANING";
    public const string Waste = "WASTE";
    public const string Security = "SECURITY";
    public const string Evacuation = "EVACUATION";
    public const string Pets = "PETS";
    public const string Smoking = "SMOKING";
    public const string HouseRules = "HOUSE_RULES";
}

public static class LocationRestrictedInformationCategoryExtensions
{
    extension(LocationRestrictedInformationCategory src)
    {
        public string ToLocationRestrictedInformationCategory() =>
            src switch
            {
                LocationRestrictedInformationCategory.Other => LocationRestrictedInformationCategoryConstants.Other,
                LocationRestrictedInformationCategory.Wifi => LocationRestrictedInformationCategoryConstants.Wifi,
                LocationRestrictedInformationCategory.Access => LocationRestrictedInformationCategoryConstants.Access,
                LocationRestrictedInformationCategory.Parking => LocationRestrictedInformationCategoryConstants.Parking,
                LocationRestrictedInformationCategory.CheckIn => LocationRestrictedInformationCategoryConstants.CheckIn,
                LocationRestrictedInformationCategory.CheckOut => LocationRestrictedInformationCategoryConstants.CheckOut,
                LocationRestrictedInformationCategory.AfterHours => LocationRestrictedInformationCategoryConstants.AfterHours,
                LocationRestrictedInformationCategory.Deliveries => LocationRestrictedInformationCategoryConstants.Deliveries,
                LocationRestrictedInformationCategory.Guests => LocationRestrictedInformationCategoryConstants.Guests,
                LocationRestrictedInformationCategory.Equipment => LocationRestrictedInformationCategoryConstants.Equipment,
                LocationRestrictedInformationCategory.Kitchen => LocationRestrictedInformationCategoryConstants.Kitchen,
                LocationRestrictedInformationCategory.MeetingRooms => LocationRestrictedInformationCategoryConstants.MeetingRooms,
                LocationRestrictedInformationCategory.Noise => LocationRestrictedInformationCategoryConstants.Noise,
                LocationRestrictedInformationCategory.Maintenance => LocationRestrictedInformationCategoryConstants.Maintenance,
                LocationRestrictedInformationCategory.Accessibility => LocationRestrictedInformationCategoryConstants.Accessibility,
                LocationRestrictedInformationCategory.Storage => LocationRestrictedInformationCategoryConstants.Storage,
                LocationRestrictedInformationCategory.Cleaning => LocationRestrictedInformationCategoryConstants.Cleaning,
                LocationRestrictedInformationCategory.Waste => LocationRestrictedInformationCategoryConstants.Waste,
                LocationRestrictedInformationCategory.Security => LocationRestrictedInformationCategoryConstants.Security,
                LocationRestrictedInformationCategory.Evacuation => LocationRestrictedInformationCategoryConstants.Evacuation,
                LocationRestrictedInformationCategory.Pets => LocationRestrictedInformationCategoryConstants.Pets,
                LocationRestrictedInformationCategory.Smoking => LocationRestrictedInformationCategoryConstants.Smoking,
                LocationRestrictedInformationCategory.HouseRules => LocationRestrictedInformationCategoryConstants.HouseRules,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };

        public string ToLocationRestrictedInformationCategoryName() =>
            src switch
            {
                LocationRestrictedInformationCategory.Other => "Other",
                LocationRestrictedInformationCategory.Wifi => "Wi-Fi",
                LocationRestrictedInformationCategory.Access => "Access",
                LocationRestrictedInformationCategory.Parking => "Parking",
                LocationRestrictedInformationCategory.CheckIn => "Check-in",
                LocationRestrictedInformationCategory.CheckOut => "Check-out",
                LocationRestrictedInformationCategory.AfterHours => "After hours",
                LocationRestrictedInformationCategory.Deliveries => "Deliveries",
                LocationRestrictedInformationCategory.Guests => "Guests",
                LocationRestrictedInformationCategory.Equipment => "Equipment",
                LocationRestrictedInformationCategory.Kitchen => "Kitchen",
                LocationRestrictedInformationCategory.MeetingRooms => "Meeting rooms",
                LocationRestrictedInformationCategory.Noise => "Noise",
                LocationRestrictedInformationCategory.Maintenance => "Maintenance",
                LocationRestrictedInformationCategory.Accessibility => "Accessibility",
                LocationRestrictedInformationCategory.Storage => "Storage",
                LocationRestrictedInformationCategory.Cleaning => "Cleaning",
                LocationRestrictedInformationCategory.Waste => "Waste",
                LocationRestrictedInformationCategory.Security => "Security",
                LocationRestrictedInformationCategory.Evacuation => "Evacuation",
                LocationRestrictedInformationCategory.Pets => "Pets",
                LocationRestrictedInformationCategory.Smoking => "Smoking",
                LocationRestrictedInformationCategory.HouseRules => "House rules",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }

    extension(string src)
    {
        public LocationRestrictedInformationCategory ToLocationRestrictedInformationCategory() =>
            src switch
            {
                LocationRestrictedInformationCategoryConstants.Other => LocationRestrictedInformationCategory.Other,
                LocationRestrictedInformationCategoryConstants.Wifi => LocationRestrictedInformationCategory.Wifi,
                LocationRestrictedInformationCategoryConstants.Access => LocationRestrictedInformationCategory.Access,
                LocationRestrictedInformationCategoryConstants.Parking => LocationRestrictedInformationCategory.Parking,
                LocationRestrictedInformationCategoryConstants.CheckIn => LocationRestrictedInformationCategory.CheckIn,
                LocationRestrictedInformationCategoryConstants.CheckOut => LocationRestrictedInformationCategory.CheckOut,
                LocationRestrictedInformationCategoryConstants.AfterHours => LocationRestrictedInformationCategory.AfterHours,
                LocationRestrictedInformationCategoryConstants.Deliveries => LocationRestrictedInformationCategory.Deliveries,
                LocationRestrictedInformationCategoryConstants.Guests => LocationRestrictedInformationCategory.Guests,
                LocationRestrictedInformationCategoryConstants.Equipment => LocationRestrictedInformationCategory.Equipment,
                LocationRestrictedInformationCategoryConstants.Kitchen => LocationRestrictedInformationCategory.Kitchen,
                LocationRestrictedInformationCategoryConstants.MeetingRooms => LocationRestrictedInformationCategory.MeetingRooms,
                LocationRestrictedInformationCategoryConstants.Noise => LocationRestrictedInformationCategory.Noise,
                LocationRestrictedInformationCategoryConstants.Maintenance => LocationRestrictedInformationCategory.Maintenance,
                LocationRestrictedInformationCategoryConstants.Accessibility => LocationRestrictedInformationCategory.Accessibility,
                LocationRestrictedInformationCategoryConstants.Storage => LocationRestrictedInformationCategory.Storage,
                LocationRestrictedInformationCategoryConstants.Cleaning => LocationRestrictedInformationCategory.Cleaning,
                LocationRestrictedInformationCategoryConstants.Waste => LocationRestrictedInformationCategory.Waste,
                LocationRestrictedInformationCategoryConstants.Security => LocationRestrictedInformationCategory.Security,
                LocationRestrictedInformationCategoryConstants.Evacuation => LocationRestrictedInformationCategory.Evacuation,
                LocationRestrictedInformationCategoryConstants.Pets => LocationRestrictedInformationCategory.Pets,
                LocationRestrictedInformationCategoryConstants.Smoking => LocationRestrictedInformationCategory.Smoking,
                LocationRestrictedInformationCategoryConstants.HouseRules => LocationRestrictedInformationCategory.HouseRules,
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }
}
