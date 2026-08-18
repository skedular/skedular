using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using Location = Booking.Shared.Database.Entities.Location;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.BookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetLocationsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_Locations_Through_The_Repository_Method(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ILocationRepository locationRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["location-1"], []);
        var location = new Location
        {
            Id = "location-1",
            Organization = new Organization
            {
                Id = "org-1",
            },
        };
        var organization = new Organization
        {
            Id = "org-1",
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.LocationRepository).Returns(locationRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => locationRepository.GetActiveByIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "location-1" })),
                cancellationToken))
            .Returns([location]);
        A.CallTo(() => organizationRepository.GetByCustomerIdAsync("customer-1", false, false, cancellationToken)).Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(true);
        A.CallTo(() => bookingRepository.GetPaginatedBookingsUntrackedAsync(
                A<PaginationInputParam>._,
                A<BookingSearchCriteria>._,
                A<IReadOnlyList<BookingOrder>>._,
                A<BookingAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.LocationIds.SequenceEqual(new[] { "location-1" }) &&
                    scope.TeamIds.Count == 0),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false,
            cancellationToken);

        result.Item3.ShouldBe(0);
        A.CallTo(() => locationRepository.GetActiveByIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "location-1" })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static BookingSearchCriteria CreateSearchCriteria(IReadOnlyList<string> locationIds, IReadOnlyList<string> teamIds) =>
        new(
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            [],
            null,
            null,
            null,
            null,
            locationIds,
            teamIds,
            [],
            [],
            null);
}
