using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Time;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingOpeningHoursServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingOpeningHoursServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Use_Resource_Opening_Hours_When_Resource_Overrides_Availability(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingOpeningHoursService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var emptyIds = Array.Empty<string>();
        var bookingDay = new DateOnly(2026, 3, 16);
        var pricing = CreatePricing(ProductPricingCadence.Daily);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
            OpeningHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(8, 0), new TimeOnly(17, 0)),
                },
                [],
                []),
        };
        var resource = new Resource
        {
            Id = "res-1",
            Location = location,
            IsAvailableHoursOverridden = true,
            AvailableHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(10, 0), new TimeOnly(14, 0)),
                },
                [],
                []),
            OrganizationTags = [productTag],
        };
        location.Resources = [resource];

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(false, false, emptyIds, cancellationToken)).Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                location.Id,
                bookingDay.ToDateTimeOffset(new TimeSpan(10, 0, 0)),
                bookingDay.ToDateTimeOffset(new TimeSpan(14, 0, 0)),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                Array.Empty<string>(),
                cancellationToken))
            .Returns([resource]);

        var result = await sut.TryResolveDailyPlanAsync(
            null,
            new ProductVersion
            {
                OrganizationTags = [productTag],
            },
            pricing,
            bookingDay,
            1,
            [],
            [],
            null,
            cancellationToken);

        result.ShouldNotBeNull();
        result.From.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(10, 0, 0)));
        result.Until.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(14, 0, 0)));
        result.Resources.Select(item => item.Id).ShouldBe(["res-1"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_Location_Opening_Hours_When_Resource_Does_Not_Override(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingOpeningHoursService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var emptyIds = Array.Empty<string>();
        var bookingDay = new DateOnly(2026, 3, 16);
        var pricing = CreatePricing(ProductPricingCadence.Daily);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
            OpeningHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(8, 0), new TimeOnly(17, 0)),
                },
                [],
                []),
        };
        var resource = new Resource
        {
            Id = "res-1",
            Location = location,
            IsAvailableHoursOverridden = false,
            AvailableHours = null,
            OrganizationTags = [productTag],
        };
        location.Resources = [resource];

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(false, false, emptyIds, cancellationToken)).Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                location.Id,
                bookingDay.ToDateTimeOffset(new TimeSpan(8, 0, 0)),
                bookingDay.ToDateTimeOffset(new TimeSpan(17, 0, 0)),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                Array.Empty<string>(),
                cancellationToken))
            .Returns([resource]);

        var result = await sut.TryResolveDailyPlanAsync(
            null,
            new ProductVersion
            {
                OrganizationTags = [productTag],
            },
            pricing,
            bookingDay,
            1,
            [],
            [],
            null,
            cancellationToken);

        result.ShouldNotBeNull();
        result.From.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(8, 0, 0)));
        result.Until.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(17, 0, 0)));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_Location_Is_Closed_On_The_Booking_Day(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingOpeningHoursService sut,
        ILocationRepository locationRepository,
        CancellationToken cancellationToken)
    {
        var emptyIds = Array.Empty<string>();
        var bookingDay = new DateOnly(2026, 3, 16);
        var dayStart = bookingDay.ToDateTimeOffset(TimeSpan.Zero);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
            OpeningHours = new OpeningHours(
                WeekOpeningHours.Default,
                [dayStart],
                []),
        };
        location.Resources =
        [
            new Resource
            {
                Id = "res-1",
                Location = location,
                OrganizationTags = [productTag],
            },
        ];

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(false, false, emptyIds, cancellationToken)).Returns([location]);

        var result = await sut.TryResolveDailyPlanAsync(
            null,
            new ProductVersion
            {
                OrganizationTags = [productTag],
            },
            CreatePricing(ProductPricingCadence.Daily),
            bookingDay,
            1,
            [],
            [],
            null,
            cancellationToken);

        result.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Daily_Plan_Using_Resources_That_Share_The_Same_Effective_Window(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingOpeningHoursService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var emptyIds = Array.Empty<string>();
        var bookingDay = new DateOnly(2026, 3, 16);
        var pricing = CreatePricing(ProductPricingCadence.Daily, 2);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
            OpeningHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(8, 0), new TimeOnly(17, 0)),
                },
                [],
                []),
        };
        var firstResource = new Resource
        {
            Id = "res-1",
            Location = location,
            IsAvailableHoursOverridden = true,
            AvailableHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(10, 0), new TimeOnly(14, 0)),
                },
                [],
                []),
            OrganizationTags = [productTag],
        };
        var secondResource = new Resource
        {
            Id = "res-2",
            Location = location,
            IsAvailableHoursOverridden = true,
            AvailableHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(10, 0), new TimeOnly(14, 0)),
                },
                [],
                []),
            OrganizationTags = [productTag],
        };
        var thirdResource = new Resource
        {
            Id = "res-3",
            Location = location,
            IsAvailableHoursOverridden = true,
            AvailableHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(12, 0), new TimeOnly(16, 0)),
                },
                [],
                []),
            OrganizationTags = [productTag],
        };
        location.Resources = [firstResource, secondResource, thirdResource];

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(false, false, emptyIds, cancellationToken))
            .Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                location.Id,
                bookingDay.ToDateTimeOffset(new TimeSpan(10, 0, 0)),
                bookingDay.ToDateTimeOffset(new TimeSpan(14, 0, 0)),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                Array.Empty<string>(),
                cancellationToken))
            .Returns([firstResource, secondResource]);

        var result = await sut.TryResolveDailyPlanAsync(
            null,
            new ProductVersion
            {
                OrganizationTags = [productTag],
            },
            pricing,
            bookingDay,
            2,
            [],
            [],
            null,
            cancellationToken);

        result.ShouldNotBeNull();
        result.From.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(10, 0, 0)));
        result.Until.ShouldBe(bookingDay.ToDateTimeOffset(new TimeSpan(14, 0, 0)));
        result.Resources.Select(item => item.Id).ShouldBe(["res-1", "res-2"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Resolve_Location_From_Involved_Location_Then_Resource_Then_Slot(MarketplaceBookingOpeningHoursService sut)
    {
        var location = new Location
        {
            Id = "loc-1",
        };
        var resource = new Resource
        {
            Id = "res-1",
            Location = location,
        };
        var bookingWithLocation = new Database.Entities.Booking
        {
            InvolvedLocations = [location],
        };
        var bookingWithResource = new Database.Entities.Booking
        {
            InvolvedLocations = [],
            InvolvedResources = [resource],
        };
        var bookingWithSlot = new Database.Entities.Booking
        {
            InvolvedLocations = [],
            InvolvedResources = [],
            ResourceBookingSlots =
            [
                new ResourceBookingSlot
                {
                    Resource = resource,
                },
            ],
        };

        sut.ResolveLocation(bookingWithLocation)?.Id.ShouldBe("loc-1");
        sut.ResolveLocation(bookingWithResource)?.Id.ShouldBe("loc-1");
        sut.ResolveLocation(bookingWithSlot)?.Id.ShouldBe("loc-1");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Prefer_Previously_Assigned_Resources_When_They_Are_Still_Available(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        MarketplaceBookingOpeningHoursService sut,
        ILocationRepository locationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var emptyIds = Array.Empty<string>();
        var bookingDay = new DateOnly(2026, 3, 16);
        var pricing = CreatePricing(ProductPricingCadence.Daily);
        var productTag = new OrganizationTag
        {
            Id = "tag-1",
            Type = OrganizationTagTypeConstants.Product,
        };
        var location = new Location
        {
            Id = "loc-1",
            OpeningHours = new OpeningHours(
                WeekOpeningHours.Default with
                {
                    Monday = new OpeningHoursDetails(false, false, new TimeOnly(8, 0), new TimeOnly(17, 0)),
                },
                [],
                []),
        };
        var preferredResource = new Resource
        {
            Id = "res-2",
            Location = location,
            OrganizationTags = [productTag],
        };
        var otherResource = new Resource
        {
            Id = "res-1",
            Location = location,
            OrganizationTags = [productTag],
        };
        location.Resources = [otherResource, preferredResource];

        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => locationRepository.GetAllWithActiveOrganizationAsync(false, false, emptyIds, cancellationToken)).Returns([location]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                location.Id,
                bookingDay.ToDateTimeOffset(new TimeSpan(8, 0, 0)),
                bookingDay.ToDateTimeOffset(new TimeSpan(17, 0, 0)),
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                Array.Empty<string>(),
                cancellationToken))
            .Returns([otherResource, preferredResource]);

        var result = await sut.TryResolveDailyPlanAsync(
            null,
            new ProductVersion
            {
                OrganizationTags = [productTag],
            },
            pricing,
            bookingDay,
            1,
            [],
            ["res-2"],
            null,
            cancellationToken);

        result.ShouldNotBeNull();
        result.Resources.Select(item => item.Id).ShouldBe(["res-2"]);
    }

    private static ProductPricing CreatePricing(ProductPricingCadence purchaseCadence, int numberOfResourcesToBook = 1) =>
        ProductPricing.Empty("pricing-1") with
        {
            PurchaseCadence = purchaseCadence,
            NumberOfResourcesToBook = numberOfResourcesToBook,
        };
}
