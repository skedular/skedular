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
    public const string DismissSetupDefaultLocation = "DismissSetupDefaultLocation";
    public const string SetAsDefaultLocation = "SetAsDefaultLocation";
    public const string ClearDefaultLocation = "ClearDefaultLocation";
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
    public const string SetAsDefaultTeam = "SetAsDefaultTeam";
    public const string ClearDefaultTeam = "ClearDefaultTeam";
    public const string EditTeam = "EditTeam";
    public const string RemoveTeam = "RemoveTeam";
    public const string ActionsMenu = "Team_ActionsMenu";
    public const string Name = "TeamName";
    public const string About = "TeamAbout";
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
    public const string Name = "ZoneName";
    public const string Description = "ZoneDescription";
}

public static class DeskActionTypes
{
    public const string Desks = "Desks";
    public const string DismissSetupPreferredDesks = "DismissSetupPreferredDesks";
    public const string AddDesk = "AddDesk";
    public const string BulkAddDesks = "BulkAddDesks";
    public const string SetPreferredDesk = "SetPreferredDesk";
    public const string RemovePreferredDesk = "RemovePreferredDesk";
    public const string EditDesk = "EditDesk";
    public const string RemoveDesk = "RemoveDesk";
    public const string ActionsMenu = "Desk_ActionsMenu";
    public const string Name = "ZoneName";
    public const string Deactivated = "ZoneDeactivated";
    public const string RequireBookingApproval = "ZoneRequireBookingApproval";
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
