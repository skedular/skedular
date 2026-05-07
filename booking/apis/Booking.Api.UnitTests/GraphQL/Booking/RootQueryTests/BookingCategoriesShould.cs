using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingCategoriesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Booking_Categories(RootQuery sut)
    {
        var result = sut.BookingCategories().ToList();

        result.Count.ShouldBe(10);
        result.ShouldContain(item => item.Category == BookingCategory.WorkingFromHome);
        result.ShouldContain(item => item.Category == BookingCategory.WorkingFromOffice);
        result.ShouldContain(item => item.Category == BookingCategory.WorkingFromCoworkingSpace);
        result.ShouldContain(item => item.Category == BookingCategory.SickLeave);
        result.ShouldContain(item => item.Category == BookingCategory.AnnualLeave);
        result.ShouldContain(item => item.Category == BookingCategory.WellbeingLeave);
        result.ShouldContain(item => item.Category == BookingCategory.ClientOffice);
        result.ShouldContain(item => item.Category == BookingCategory.Vacation);
        result.ShouldContain(item => item.Category == BookingCategory.TravelingForWork);
        result.ShouldContain(item => item.Category == BookingCategory.NonWorkingDay);
    }
}
