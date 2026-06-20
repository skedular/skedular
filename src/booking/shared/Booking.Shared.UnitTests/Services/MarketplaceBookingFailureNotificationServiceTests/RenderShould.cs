using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureNotificationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RenderShould
{
    [Theory]
    [InlineData(MarketplaceBookingFailureCategoryConstants.AvailabilityConflict, "no longer available")]
    [InlineData(MarketplaceBookingFailureCategoryConstants.PaymentFailed, "could not complete")]
    [InlineData(MarketplaceBookingFailureCategoryConstants.PaymentExpired, "payment expired")]
    public async Task Render_Customer_Safe_Category_Copy(string category, string expectedText)
    {
        var sut = new MarketplaceBookingFailureNotificationService(A.Fake<IRepositoryFactory>());
        var failure = new MarketplaceBookingFailure { Category = category, CustomerAction = MarketplaceBookingFailureCustomerActionConstants.Rebook };

        var message = await sut.RenderAsync(failure, false, "Jamie", CancellationToken.None);

        message.Subject.ShouldBe("Your booking could not be completed");
        message.Text.ShouldContain("Jamie");
        message.Text.ShouldContain(expectedText);
        message.Html.ShouldContain(expectedText);
    }
}
