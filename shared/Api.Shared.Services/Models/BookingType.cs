namespace Api.Shared.Services.Models;

public enum BookingType
{
    WorkingFromHome,
    WorkingFromOffice,
    SickLeave,
    AnnualLeave,
    WellBeingLeave,
    ClientOffices,
    Vacation,
    TravelingForWork,
    NonWorkingDay
}

public static class BookingTypeConstants
{
    public const string WorkingFromHome = "WORKING_FROM_HOME";
    public const string WorkingFromOffice = "WORKING_FROM_OFFICE";
    public const string SickLeave = "SICK_LEAVE";
    public const string AnnualLeave = "ANNUAL_LEAVE";
    public const string WellBeingLeave = "WELLBEING_LEAVE";
    public const string ClientOffices = "CLIENT_OFFICE";
    public const string Vacation = "VACATION";
    public const string TravelingForWork = "TRAVELING_FOR_WORK";
    public const string NonWorkingDay = "NON_WORKING_DAY";
}
