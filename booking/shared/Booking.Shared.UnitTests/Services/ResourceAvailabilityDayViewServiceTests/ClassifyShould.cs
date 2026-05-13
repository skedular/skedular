using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.ResourceAvailabilityDayViewServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ClassifyShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_Blocked_When_Resource_Is_Inactive(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(true, false, false, 480, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Blocked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Blocked_Even_When_Location_Is_Closed(ResourceAvailabilityClassifier sut)
    {
        // Blocked takes precedence over Unavailable
        var result = sut.Classify(true, true, false, 480, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Blocked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Unavailable_When_Location_Is_Closed(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, true, false, 480, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Unavailable);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Unavailable_When_Day_Is_Closed(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, true, 0, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Unavailable);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Available_When_No_Bookings(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, false, 480, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Available);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_FullyBooked_When_BookedMinutes_Equals_TotalOpeningMinutes(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, false, 480, 480);

        result.ShouldBe(ResourceAvailabilityClassification.FullyBooked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_FullyBooked_When_BookedMinutes_Exceeds_TotalOpeningMinutes(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, false, 480, 600);

        result.ShouldBe(ResourceAvailabilityClassification.FullyBooked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_PartiallyBooked_When_Some_Minutes_Are_Booked(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, false, 480, 120);

        result.ShouldBe(ResourceAvailabilityClassification.PartiallyBooked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Unavailable_When_Booked_But_No_Opening_Hours(ResourceAvailabilityClassifier sut)
    {
        // No opening hours configured (totalOpeningMinutes = 0): resource is unavailable regardless of bookings
        var result = sut.Classify(false, false, false, 0, 60);

        result.ShouldBe(ResourceAvailabilityClassification.Unavailable);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Return_Unavailable_When_No_Opening_Hours_And_No_Bookings(ResourceAvailabilityClassifier sut)
    {
        var result = sut.Classify(false, false, false, 0, 0);

        result.ShouldBe(ResourceAvailabilityClassification.Unavailable);
    }
}
