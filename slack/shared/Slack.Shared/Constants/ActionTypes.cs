namespace Slack.Shared.Constants;

public static class Global
{
    public static string OptionLoaderValueSeparator = Environment.NewLine;
}

public static class CommonActionTypes
{
    public const string Back = "Back";
    public const string SendUsFeedback = "SendUsFeedback";
}

public static class HomeActionTypes
{
    public const string Home = "Home";
}

public static class LocationActionTypes
{
    public const string Locations = "Locations";
    public const string AddLocation = "AddLocation";
    public const string DismissSetupPreferredLocation = "DismissSetupPreferredLocation";
    public const string AddAsPreferredLocation = "AddAsPreferredLocation";
    public const string RemovePreferredLocation = "RemovePreferredLocation";
    public const string EditLocation = "EditLocation";
    public const string RemoveLocation = "RemoveLocation";
    public const string ActionsMenu = "Location_ActionsMenu";
    public const string Name = "LocationName";
    public const string About = "LocationAbout";
    public const string SlackUpdateChannel = "SlackUpdateChannel";
}

public static class TeamActionTypes
{
    public const string Teams = "Teams";
    public const string AddTeam = "AddTeam";
    public const string AddAsPreferredTeam = "SetAsPreferredTeam";
    public const string RemovePreferredTeam = "ClearPreferredTeam";
    public const string EditTeam = "EditTeam";
    public const string RemoveTeam = "RemoveTeam";
    public const string ActionsMenu = "Team_ActionsMenu";
    public const string Name = "TeamName";
    public const string About = "TeamAbout";
    public const string PrimaryLocation = "PrimaryLocation";
    public const string SlackUpdateChannel = "SlackUpdateChannel";
}

public static class ZoneActionTypes
{
    public const string Zones = "Zones";
    public const string DismissSetupPreferredZones = "DismissSetupPreferredZones";
    public const string AddZone = "AddZone";
    public const string SetPreferredZone = "SetPreferredZone";
    public const string RemovePreferredZone = "RemovePreferredZone";
    public const string EditZone = "EditZone";
    public const string RemoveZone = "RemoveZone";
    public const string ActionsMenu = "Zone_ActionsMenu";
    public const string Name = "Zone_Name";
    public const string Description = "Zone_Description";
}

public static class ResourceActionTypes
{
    public const string Resources = "Resources";
    public const string AddResource = "AddResource";
    public const string SetPreferredResource = "SetPreferredResource";
    public const string RemovePreferredResource = "RemovePreferredResource";
    public const string EditResource = "EditResource";
    public const string RemoveResource = "RemoveResource";
    public const string ActionsMenu = "Resource_ActionsMenu";
    public const string Name = "Resource_Name";
    public const string Inactive = "Resource_Inactive";
    public const string RequireBookingApproval = "Resource_RequireBookingApproval";
    public const string Capacity = "Resource_Capacity";
}

public static class BookingActionTypes
{
    public const string Bookings = "Bookings";
    public const string AddBooking = "AddBooking";
    public const string InstantAddBooking = "InstantAddBooking";
    public const string CancelBooking = "CancelBooking";
    public const string EditBooking = "EditBooking";
    public const string JoinBooking = "JoinBooking";
}

public static class SettingsActionTypes
{
    public const string Settings = "Settings";
}

public static class BillingActionTypes
{
    public const string Billing = "Billing";
    public const string Email = "Email";
    public const string AddressLine1 = "AddressLine1";
    public const string AddressLine2 = "AddressLine2";
    public const string Suburb = "Suburb";
    public const string City = "City";
    public const string Province = "Province";
    public const string Zipcode = "Zipcode";
}

public static class CustomTagActionTypes
{
    public const string CustomTags = "CustomTags";
    public const string AddCustomTag = "AddCustomTag";
    public const string SetPreferredCustomTag = "SetPreferredCustomTag";
    public const string RemovePreferredCustomTag = "RemovePreferredCustomTag";
    public const string EditCustomTag = "EditCustomTag";
    public const string RemoveCustomTag = "RemoveCustomTag";
    public const string ActionsMenu = "CustomTag_ActionsMenu";
    public const string Name = "CustomTag_Name";
    public const string Description = "CustomTag_Description";
}
