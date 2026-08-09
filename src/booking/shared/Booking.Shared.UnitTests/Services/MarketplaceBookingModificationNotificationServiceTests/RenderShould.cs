using Booking.Shared.Database.Entities;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingModificationNotificationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RenderShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Render_Consistent_Customer_Copy(
        MarketplaceBookingModificationNotificationService sut,
        CancellationToken cancellationToken)
    {
        var modification = new MarketplaceBookingModification
        {
            Id = "modification-1",
            OriginalFrom = new DateTimeOffset(2026, 8, 8, 9, 0, 0, TimeSpan.Zero),
            OriginalUntil = new DateTimeOffset(2026, 8, 8, 10, 0, 0, TimeSpan.Zero),
            ResultFrom = new DateTimeOffset(2026, 8, 9, 9, 0, 0, TimeSpan.Zero),
            ResultUntil = new DateTimeOffset(2026, 8, 9, 10, 0, 0, TimeSpan.Zero),
        };

        var result = await sut.RenderAsync(modification, "Jamie", false, cancellationToken);

        result.Subject.ShouldBe("Your booking was updated");
        result.Text.ShouldContain("Jamie");
        result.Text.ShouldContain("Previous time:");
        result.Text.ShouldContain("New time:");
        result.Html.ShouldContain("Jamie");
        result.Html.ShouldContain("Booking update");
    }
}
