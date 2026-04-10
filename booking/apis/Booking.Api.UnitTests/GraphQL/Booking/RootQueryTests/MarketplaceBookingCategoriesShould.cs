using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingCategoriesShould
{
    [Fact]
    public void Return_All_Marketplace_Booking_Categories()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

        var result = sut.MarketplaceBookingCategories().ToList();

        result.Count.ShouldBe(3);
        result.ShouldContain(item =>
            item.Category == BookingCategory.WorkingFromOffice && item.Name == BookingCategory.WorkingFromOffice.ToBookingCategoryName());
        result.ShouldContain(item =>
            item.Category == BookingCategory.WorkingFromCoworkingSpace &&
            item.Name == BookingCategory.WorkingFromCoworkingSpace.ToBookingCategoryName());
        result.ShouldContain(item =>
            item.Category == BookingCategory.ClientOffice && item.Name == BookingCategory.ClientOffice.ToBookingCategoryName());
    }
}
