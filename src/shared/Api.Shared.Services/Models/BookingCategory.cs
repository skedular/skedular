namespace Api.Shared.Services.Models;

public enum BookingCategory
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

public static class BookingCategoryConstants
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

public static class BookingCategoryExtensions
{
    extension(string src)
    {
        public BookingCategory ToBookingCategory() =>
            src switch
            {
                BookingCategoryConstants.WorkingFromHome => BookingCategory.WorkingFromHome,
                BookingCategoryConstants.WorkingFromOffice => BookingCategory.WorkingFromOffice,
                BookingCategoryConstants.WorkingFromCoworkingSpace => BookingCategory.WorkingFromCoworkingSpace,
                BookingCategoryConstants.SickLeave => BookingCategory.SickLeave,
                BookingCategoryConstants.AnnualLeave => BookingCategory.AnnualLeave,
                BookingCategoryConstants.WellbeingLeave => BookingCategory.WellbeingLeave,
                BookingCategoryConstants.ClientOffice => BookingCategory.ClientOffice,
                BookingCategoryConstants.Vacation => BookingCategory.Vacation,
                BookingCategoryConstants.TravelingForWork => BookingCategory.TravelingForWork,
                BookingCategoryConstants.NonWorkingDay => BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingCategoryName() =>
            src switch
            {
                BookingCategoryConstants.WorkingFromHome => "Working from home",
                BookingCategoryConstants.WorkingFromOffice => "Working from office",
                BookingCategoryConstants.WorkingFromCoworkingSpace => "Working from co-working space",
                BookingCategoryConstants.SickLeave => "Sick leave",
                BookingCategoryConstants.AnnualLeave => "Annual leave",
                BookingCategoryConstants.WellbeingLeave => "Wellbeing leave",
                BookingCategoryConstants.ClientOffice => "Client office",
                BookingCategoryConstants.Vacation => "Vacation",
                BookingCategoryConstants.TravelingForWork => "Traveling for work",
                BookingCategoryConstants.NonWorkingDay => "Non working day",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string? src)
    {
        public BookingCategory? ToNullableBookingCategory() =>
            string.IsNullOrWhiteSpace(src)
                ? null
                : src switch
                {
                    BookingCategoryConstants.WorkingFromHome => BookingCategory.WorkingFromHome,
                    BookingCategoryConstants.WorkingFromOffice => BookingCategory.WorkingFromOffice,
                    BookingCategoryConstants.WorkingFromCoworkingSpace => BookingCategory.WorkingFromCoworkingSpace,
                    BookingCategoryConstants.SickLeave => BookingCategory.SickLeave,
                    BookingCategoryConstants.AnnualLeave => BookingCategory.AnnualLeave,
                    BookingCategoryConstants.WellbeingLeave => BookingCategory.WellbeingLeave,
                    BookingCategoryConstants.ClientOffice => BookingCategory.ClientOffice,
                    BookingCategoryConstants.Vacation => BookingCategory.Vacation,
                    BookingCategoryConstants.TravelingForWork => BookingCategory.TravelingForWork,
                    BookingCategoryConstants.NonWorkingDay => BookingCategory.NonWorkingDay,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(BookingCategory src)
    {
        public string ToBookingCategory() =>
            src switch
            {
                BookingCategory.WorkingFromHome => BookingCategoryConstants.WorkingFromHome,
                BookingCategory.WorkingFromOffice => BookingCategoryConstants.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => BookingCategoryConstants.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => BookingCategoryConstants.SickLeave,
                BookingCategory.AnnualLeave => BookingCategoryConstants.AnnualLeave,
                BookingCategory.WellbeingLeave => BookingCategoryConstants.WellbeingLeave,
                BookingCategory.ClientOffice => BookingCategoryConstants.ClientOffice,
                BookingCategory.Vacation => BookingCategoryConstants.Vacation,
                BookingCategory.TravelingForWork => BookingCategoryConstants.TravelingForWork,
                BookingCategory.NonWorkingDay => BookingCategoryConstants.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToBookingCategoryName() =>
            src switch
            {
                BookingCategory.WorkingFromHome => "Working from home",
                BookingCategory.WorkingFromOffice => "Working from office",
                BookingCategory.WorkingFromCoworkingSpace => "Working from co-working space",
                BookingCategory.SickLeave => "Sick leave",
                BookingCategory.AnnualLeave => "Annual leave",
                BookingCategory.WellbeingLeave => "Wellbeing leave",
                BookingCategory.ClientOffice => "Client office",
                BookingCategory.Vacation => "Vacation",
                BookingCategory.TravelingForWork => "Traveling for work",
                BookingCategory.NonWorkingDay => "Non working day",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(BookingCategory? src)
    {
        public string? ToNullableBookingCategory() =>
            src is null
                ? null
                : src switch
                {
                    BookingCategory.WorkingFromHome => BookingCategoryConstants.WorkingFromHome,
                    BookingCategory.WorkingFromOffice => BookingCategoryConstants.WorkingFromOffice,
                    BookingCategory.WorkingFromCoworkingSpace => BookingCategoryConstants.WorkingFromCoworkingSpace,
                    BookingCategory.SickLeave => BookingCategoryConstants.SickLeave,
                    BookingCategory.AnnualLeave => BookingCategoryConstants.AnnualLeave,
                    BookingCategory.WellbeingLeave => BookingCategoryConstants.WellbeingLeave,
                    BookingCategory.ClientOffice => BookingCategoryConstants.ClientOffice,
                    BookingCategory.Vacation => BookingCategoryConstants.Vacation,
                    BookingCategory.TravelingForWork => BookingCategoryConstants.TravelingForWork,
                    BookingCategory.NonWorkingDay => BookingCategoryConstants.NonWorkingDay,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
