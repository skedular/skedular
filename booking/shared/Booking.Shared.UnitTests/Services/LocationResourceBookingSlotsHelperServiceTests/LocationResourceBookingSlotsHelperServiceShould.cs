using Booking.Shared.Database.Entities;
using Booking.Shared.Services;
using Enterprise.Shared.Time;

namespace Booking.Shared.UnitTests.Services.LocationResourceBookingSlotsHelperServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LocationResourceBookingSlotsHelperServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void GetStartPeriod_Returns_14_Days_Ago_From_Current_Day(
        [Frozen] TimeProvider timeProvider,
        LocationResourceBookingSlotsHelperService sut)
    {
        // Arrange
        var currentTime = DateTimeOffset.UtcNow;
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(currentTime);

        // Act
        var result = sut.GetStartPeriod();

        // Assert
        result.ShouldBe(currentTime.StartOfDay().AddDays(-14));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void CreateAllAvailableSlots_Generates_Slots_For_Resource(
        [Frozen] TimeProvider timeProvider,
        LocationResourceBookingSlotsHelperService sut)
    {
        // Arrange
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(DateTimeOffset.UtcNow);
        var resource = new Resource { Id = "resource-1", Name = "Test Resource" };

        // Act
        var result = sut.CreateAllAvailableSlots(resource);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.First().Resource.ShouldBe(resource);
        result.First().Available.ShouldBeTrue();
    }
}
