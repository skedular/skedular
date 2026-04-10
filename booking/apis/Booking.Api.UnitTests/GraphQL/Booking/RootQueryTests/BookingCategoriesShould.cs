using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingCategoriesShould
{
    [Fact]
    public void Return_All_Booking_Categories()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

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
