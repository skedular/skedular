using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.SpacesBookingUsageRolloverServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RolloverCurrentPeriodsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Zero_Because_Usage_Is_Count_Based(
        SpacesBookingUsageRolloverService sut,
        CancellationToken cancellationToken)
    {
        var result = await sut.RolloverCurrentPeriodsAsync(cancellationToken);

        result.ShouldBe(0);
    }
}
