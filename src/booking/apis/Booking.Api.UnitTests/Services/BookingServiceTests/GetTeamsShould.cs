using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Api.UnitTests.Services.BookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetTeamsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_Teams_Through_The_Repository_Method(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ITeamRepository teamRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["team-1"]);
        var team = new Team
        {
            Id = "team-1",
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
        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => teamRepository.GetActiveByIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "team-1" })),
                cancellationToken))
            .Returns([team]);
        A.CallTo(() => organizationRepository.GetByCustomerIdAsync("customer-1", false, false, cancellationToken)).Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(true);
        A.CallTo(() => bookingRepository.GetPaginatedBookingsUntrackedAsync(
                A<PaginationInputParam>._,
                A<BookingSearchCriteria>._,
                A<IReadOnlyList<BookingOrder>>._,
                A<BookingAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.LocationIds.Count == 0 &&
                    scope.TeamIds.SequenceEqual(new[] { "team-1" })),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false,
            cancellationToken);

        result.Item3.ShouldBe(0);
        A.CallTo(() => teamRepository.GetActiveByIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "team-1" })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static BookingSearchCriteria CreateSearchCriteria(IReadOnlyList<string> teamIds) =>
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
            [],
            teamIds,
            [],
            [],
            null);
}
