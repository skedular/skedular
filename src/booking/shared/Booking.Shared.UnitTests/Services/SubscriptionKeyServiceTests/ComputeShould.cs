using Booking.Shared.Models;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.SubscriptionKeyServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ComputeShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Be_Deterministic(SubscriptionKeyService sut)
    {
        var filter = CreateFilter(["location-1"], "floor-1", "zone-1", "desk");

        sut.Compute(filter).ShouldBe(sut.Compute(filter));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Ignore_Optional_Null_And_Empty_Values(SubscriptionKeyService sut)
    {
        var withNulls = CreateFilter(["location-1"], null, null, null);
        var withEmptyValues = CreateFilter(["location-1"], "", "", "");

        sut.Compute(withNulls).ShouldBe(sut.Compute(withEmptyValues));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Use_Org_Wide_Key_For_Multiple_Locations(SubscriptionKeyService sut)
    {
        var oneLocation = sut.Compute(CreateFilter(["location-1"], null, null, null));
        var multipleLocations = sut.Compute(CreateFilter(["location-1", "location-2"], null, null, null));
        var orgWide = sut.Compute(CreateFilter([], null, null, null));

        oneLocation.ShouldNotBe(multipleLocations);
        multipleLocations.ShouldBe(orgWide);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Produce_Eight_Affected_Keys_For_A_Specific_Location(SubscriptionKeyService sut)
    {
        var keys = sut.AffectedKeys("org.example", "location-1", "floor-1", "zone-1", "desk", new DateOnly(2026, 8, 16)).ToList();

        keys.Count.ShouldBe(16);
        keys.Distinct().Count().ShouldBe(16);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Change_When_The_Date_Changes(SubscriptionKeyService sut)
    {
        var first = CreateFilter(["location-1"], null, null, null);
        var second = first with
        {
            Date = first.Date.AddDays(1),
        };

        sut.Compute(first).ShouldNotBe(sut.Compute(second));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Change_When_Organization_Changes(SubscriptionKeyService sut)
    {
        var first = CreateFilter(["location-1"], null, null, null);
        var second = first with
        {
            OrganizationCustomDomain = "other.example",
        };

        sut.Compute(first).ShouldNotBe(sut.Compute(second));
    }

    private static ResourceAvailabilityDayFilter CreateFilter(IReadOnlyList<string> locationIds, string? floorId, string? zoneId,
        string? resourceType) => new()
    {
        Date = new DateOnly(2026, 8, 16),
        OrganizationCustomDomain = "org.example",
        LocationIds = locationIds,
        FloorId = floorId,
        ZoneId = zoneId,
        ResourceType = resourceType,
    };
}
