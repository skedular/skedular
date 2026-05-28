using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.ResourceAvailabilityDayViewServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ApplyShould
{
    private static readonly ResourceDayView s_sampleView = new()
    {
        ResourceId = "res-1",
        ResourceName = "Desk 1",
        ResourceType = OrganizationTagTypeConstants.ResourceDesk,
        LocationId = "loc-1",
        LocationName = "HQ",
        FloorId = null,
        FloorName = null,
        ZoneId = null,
        ZoneName = null,
        Date = new DateOnly(2025, 6, 1),
        Status = ResourceAvailabilityClassification.PartiallyBooked,
        OpeningFrom = new TimeOnly(9, 0),
        OpeningUntil = new TimeOnly(17, 0),
        TotalOpeningMinutes = 480,
        BookedMinutes = 120,
        BookingWindows =
        [
            new BookingWindow
            {
                BookingId = "b-1",
                From = new DateTimeOffset(2025, 6, 1, 9, 0, 0, TimeSpan.Zero),
                Until = new DateTimeOffset(2025, 6, 1, 11, 0, 0, TimeSpan.Zero),
                IsRecurring = false,
                IsCheckedIn = false,
                BookedByName = "Alice Smith",
                BookedByUserId = "u-1",
                Notes = "Team meeting"
            }
        ]
    };

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_All_Detail_Fields_For_Private_Organization(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Private, []);

        var window = result[0].BookingWindows[0];
        window.BookedByName.ShouldBe("Alice Smith");
        window.BookedByUserId.ShouldBe("u-1");
        window.Notes.ShouldBe("Team meeting");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Redact_Detail_Fields_For_Marketplace_Regular_Member(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Marketplace, []);

        var window = result[0].BookingWindows[0];
        window.BookedByName.ShouldBeNull();
        window.BookedByUserId.ShouldBeNull();
        window.Notes.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Detail_Fields_For_Marketplace_Admin(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Marketplace, ["ADMIN"]);

        var window = result[0].BookingWindows[0];
        window.BookedByName.ShouldBe("Alice Smith");
        window.BookedByUserId.ShouldBe("u-1");
        window.Notes.ShouldBe("Team meeting");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Detail_Fields_For_Marketplace_Owner(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Marketplace, ["OWNER"]);

        var window = result[0].BookingWindows[0];
        window.BookedByName.ShouldBe("Alice Smith");
        window.BookedByUserId.ShouldBe("u-1");
        window.Notes.ShouldBe("Team meeting");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Redact_Detail_Fields_For_Individual_Regular_Member(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Individual, ["MEMBER"]);

        var window = result[0].BookingWindows[0];
        window.BookedByName.ShouldBeNull();
        window.BookedByUserId.ShouldBeNull();
        window.Notes.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Preserve_Non_Detail_Fields_When_Redacting(ResourceDayViewBookingVisibilityFilter sut)
    {
        var result = sut.Apply([s_sampleView], OrganizationTypeConstants.Marketplace, []);

        var window = result[0].BookingWindows[0];
        window.BookingId.ShouldBe("b-1");
        window.IsRecurring.ShouldBeFalse();
        window.IsCheckedIn.ShouldBeFalse();
        window.From.ShouldBe(new DateTimeOffset(2025, 6, 1, 9, 0, 0, TimeSpan.Zero));
        window.Until.ShouldBe(new DateTimeOffset(2025, 6, 1, 11, 0, 0, TimeSpan.Zero));
    }
}
