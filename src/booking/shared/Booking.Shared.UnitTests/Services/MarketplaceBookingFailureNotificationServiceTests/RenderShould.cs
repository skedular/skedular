using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingFailureNotificationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RenderShould
{
    [Theory]
    [InlineAutoFakeItEasyData([], MarketplaceBookingFailureCategoryConstants.AvailabilityConflict, "no longer available")]
    [InlineAutoFakeItEasyData([], MarketplaceBookingFailureCategoryConstants.PaymentFailed, "could not complete")]
    [InlineAutoFakeItEasyData([], MarketplaceBookingFailureCategoryConstants.PaymentExpired, "payment expired")]
    public async Task Render_Customer_Safe_Category_Copy(
        string category,
        string expectedText,
        MarketplaceBookingFailureNotificationService sut,
        CancellationToken cancellationToken)
    {
        var failure = new MarketplaceBookingFailure
        {
            Category = category,
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.Rebook,
        };
        var message = await sut.RenderAsync(failure, false, "Jamie", cancellationToken);

        message.Subject.ShouldBe("Your booking could not be completed");
        message.Text.ShouldContain("Jamie");
        message.Text.ShouldContain(expectedText);
        message.Html.ShouldContain(expectedText);
    }
}
