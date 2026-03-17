using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using FakeItEasy;
using Shouldly;
using Testing.Shared;

namespace Booking.Shared.UnitTests.Services.LocationResourceBookingSlotsHelperServiceTests;

public class LocationResourceBookingSlotsHelperServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void GetStartPeriod_Returns_14_Days_Ago_From_Current_Day(LocationResourceBookingSlotsHelperService sut, TimeProvider timeProvider)
    {
        // Arrange
        var currentTime = timeProvider.GetUtcNow();

        // Act
        var result = sut.GetStartPeriod();

        // Assert
        result.ShouldBe(currentTime.StartOfDay().AddDays(-14));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void CreateAllAvailableSlots_Generates_Slots_For_Resource(LocationResourceBookingSlotsHelperService sut)
    {
        // Arrange
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
