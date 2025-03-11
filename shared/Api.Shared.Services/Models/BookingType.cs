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

public static class BookingTypeExtensions
{
    public static BookingType ToBookingType(this string src) =>
        src switch
        {
            BookingTypeConstants.WorkingFromHome => BookingType.WorkingFromHome,
            BookingTypeConstants.WorkingFromOffice => BookingType.WorkingFromOffice,
            BookingTypeConstants.SickLeave => BookingType.SickLeave,
            BookingTypeConstants.AnnualLeave => BookingType.AnnualLeave,
            BookingTypeConstants.WellBeingLeave => BookingType.WellBeingLeave,
            BookingTypeConstants.ClientOffices => BookingType.ClientOffices,
            BookingTypeConstants.Vacation => BookingType.Vacation,
            BookingTypeConstants.TravelingForWork => BookingType.TravelingForWork,
            BookingTypeConstants.NonWorkingDay => BookingType.NonWorkingDay,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static BookingType? ToNullableBookingType(this string? src) =>
        string.IsNullOrWhiteSpace(src)
            ? null
            : src switch
            {
                BookingTypeConstants.WorkingFromHome => BookingType.WorkingFromHome,
                BookingTypeConstants.WorkingFromOffice => BookingType.WorkingFromOffice,
                BookingTypeConstants.SickLeave => BookingType.SickLeave,
                BookingTypeConstants.AnnualLeave => BookingType.AnnualLeave,
                BookingTypeConstants.WellBeingLeave => BookingType.WellBeingLeave,
                BookingTypeConstants.ClientOffices => BookingType.ClientOffices,
                BookingTypeConstants.Vacation => BookingType.Vacation,
                BookingTypeConstants.TravelingForWork => BookingType.TravelingForWork,
                BookingTypeConstants.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };

    public static string ToBookingType(this BookingType src) =>
        src switch
        {
            BookingType.WorkingFromHome => BookingTypeConstants.WorkingFromHome,
            BookingType.WorkingFromOffice => BookingTypeConstants.WorkingFromOffice,
            BookingType.SickLeave => BookingTypeConstants.SickLeave,
            BookingType.AnnualLeave => BookingTypeConstants.AnnualLeave,
            BookingType.WellBeingLeave => BookingTypeConstants.WellBeingLeave,
            BookingType.ClientOffices => BookingTypeConstants.ClientOffices,
            BookingType.Vacation => BookingTypeConstants.Vacation,
            BookingType.TravelingForWork => BookingTypeConstants.TravelingForWork,
            BookingType.NonWorkingDay => BookingTypeConstants.NonWorkingDay,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToNullableBookingType(this BookingType? src) =>
        src is null
            ? string.Empty
            : src switch
            {
                BookingType.WorkingFromHome => BookingTypeConstants.WorkingFromHome,
                BookingType.WorkingFromOffice => BookingTypeConstants.WorkingFromOffice,
                BookingType.SickLeave => BookingTypeConstants.SickLeave,
                BookingType.AnnualLeave => BookingTypeConstants.AnnualLeave,
                BookingType.WellBeingLeave => BookingTypeConstants.WellBeingLeave,
                BookingType.ClientOffices => BookingTypeConstants.ClientOffices,
                BookingType.Vacation => BookingTypeConstants.Vacation,
                BookingType.TravelingForWork => BookingTypeConstants.TravelingForWork,
                BookingType.NonWorkingDay => BookingTypeConstants.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };
}
