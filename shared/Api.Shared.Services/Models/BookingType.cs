namespace Api.Shared.Services.Models;

public enum BookingType
{
    WorkingFromHome,
    WorkingFromOffice,
    WorkingFromCoworkingSpace,
    SickLeave,
    AnnualLeave,
    WellbeingLeave,
    ClientOffice,
    Vacation,
    TravelingForWork,
    NonWorkingDay
}

public static class BookingTypeConstants
{
    public const string WorkingFromHome = "WORKING_FROM_HOME";
    public const string WorkingFromOffice = "WORKING_FROM_OFFICE";
    public const string WorkingFromCoworkingSpace = "WORKING_FROM_COWORKING_SPACE";
    public const string SickLeave = "SICK_LEAVE";
    public const string AnnualLeave = "ANNUAL_LEAVE";
    public const string WellbeingLeave = "WELLBEING_LEAVE";
    public const string ClientOffice = "CLIENT_OFFICE";
    public const string Vacation = "VACATION";
    public const string TravelingForWork = "TRAVELING_FOR_WORK";
    public const string NonWorkingDay = "NON_WORKING_DAY";
}

public static class BookingTypeExtensions
{
    extension(string src)
    {
        public BookingType ToBookingType() =>
            src switch
            {
                BookingTypeConstants.WorkingFromHome => BookingType.WorkingFromHome,
                BookingTypeConstants.WorkingFromOffice => BookingType.WorkingFromOffice,
                BookingTypeConstants.WorkingFromCoworkingSpace => BookingType.WorkingFromCoworkingSpace,
                BookingTypeConstants.SickLeave => BookingType.SickLeave,
                BookingTypeConstants.AnnualLeave => BookingType.AnnualLeave,
                BookingTypeConstants.WellbeingLeave => BookingType.WellbeingLeave,
                BookingTypeConstants.ClientOffice => BookingType.ClientOffice,
                BookingTypeConstants.Vacation => BookingType.Vacation,
                BookingTypeConstants.TravelingForWork => BookingType.TravelingForWork,
                BookingTypeConstants.NonWorkingDay => BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingTypeName() =>
            src switch
            {
                BookingTypeConstants.WorkingFromHome => "Working from home",
                BookingTypeConstants.WorkingFromOffice => "Working from office",
                BookingTypeConstants.WorkingFromCoworkingSpace => "Working from co-working space",
                BookingTypeConstants.SickLeave => "Sick leave",
                BookingTypeConstants.AnnualLeave => "Annual leave",
                BookingTypeConstants.WellbeingLeave => "Wellbeing leave",
                BookingTypeConstants.ClientOffice => "Client office",
                BookingTypeConstants.Vacation => "Vacation",
                BookingTypeConstants.TravelingForWork => "Traveling for work",
                BookingTypeConstants.NonWorkingDay => "Non working day",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public BookingType? ToNullableBookingType() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    BookingTypeConstants.WorkingFromHome => BookingType.WorkingFromHome,
                    BookingTypeConstants.WorkingFromOffice => BookingType.WorkingFromOffice,
                    BookingTypeConstants.WorkingFromCoworkingSpace => BookingType.WorkingFromCoworkingSpace,
                    BookingTypeConstants.SickLeave => BookingType.SickLeave,
                    BookingTypeConstants.AnnualLeave => BookingType.AnnualLeave,
                    BookingTypeConstants.WellbeingLeave => BookingType.WellbeingLeave,
                    BookingTypeConstants.ClientOffice => BookingType.ClientOffice,
                    BookingTypeConstants.Vacation => BookingType.Vacation,
                    BookingTypeConstants.TravelingForWork => BookingType.TravelingForWork,
                    BookingTypeConstants.NonWorkingDay => BookingType.NonWorkingDay,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(BookingType src)
    {
        public string ToBookingType() =>
            src switch
            {
                BookingType.WorkingFromHome => BookingTypeConstants.WorkingFromHome,
                BookingType.WorkingFromOffice => BookingTypeConstants.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => BookingTypeConstants.WorkingFromCoworkingSpace,
                BookingType.SickLeave => BookingTypeConstants.SickLeave,
                BookingType.AnnualLeave => BookingTypeConstants.AnnualLeave,
                BookingType.WellbeingLeave => BookingTypeConstants.WellbeingLeave,
                BookingType.ClientOffice => BookingTypeConstants.ClientOffice,
                BookingType.Vacation => BookingTypeConstants.Vacation,
                BookingType.TravelingForWork => BookingTypeConstants.TravelingForWork,
                BookingType.NonWorkingDay => BookingTypeConstants.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingTypeName() =>
            src switch
            {
                BookingType.WorkingFromHome => "Working from home",
                BookingType.WorkingFromOffice => "Working from office",
                BookingType.WorkingFromCoworkingSpace => "Working from co-working space",
                BookingType.SickLeave => "Sick leave",
                BookingType.AnnualLeave => "Annual leave",
                BookingType.WellbeingLeave => "Wellbeing leave",
                BookingType.ClientOffice => "Client office",
                BookingType.Vacation => "Vacation",
                BookingType.TravelingForWork => "Traveling for work",
                BookingType.NonWorkingDay => "Non working day",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(BookingType? src)
    {
        public string? ToNullableBookingType() =>
            src is null
                ? null
                : src switch
                {
                    BookingType.WorkingFromHome => BookingTypeConstants.WorkingFromHome,
                    BookingType.WorkingFromOffice => BookingTypeConstants.WorkingFromOffice,
                    BookingType.WorkingFromCoworkingSpace => BookingTypeConstants.WorkingFromCoworkingSpace,
                    BookingType.SickLeave => BookingTypeConstants.SickLeave,
                    BookingType.AnnualLeave => BookingTypeConstants.AnnualLeave,
                    BookingType.WellbeingLeave => BookingTypeConstants.WellbeingLeave,
                    BookingType.ClientOffice => BookingTypeConstants.ClientOffice,
                    BookingType.Vacation => BookingTypeConstants.Vacation,
                    BookingType.TravelingForWork => BookingTypeConstants.TravelingForWork,
                    BookingType.NonWorkingDay => BookingTypeConstants.NonWorkingDay,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
