using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using FakeItEasy;
using Customer = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Api.UnitTests.Services.BookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BookingServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Returns_Booking_For_Involved_Customer(
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] IMapper sharedMapper,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var mappedBooking = new Shared.Models.Booking { Id = booking.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedBookingService.GetByIdAsync(booking.Id, cancellationToken)).Returns(booking);
        A.CallTo(() => sharedMapper.MapTo(booking)).Returns(mappedBooking);

        var result = await sut.GetByIdAsync(booking.Id, cancellationToken);

        result.ShouldBe(mappedBooking);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Allows_Marketplace_Admin_To_View_Other_Customers_Booking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] IMapper sharedMapper,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Database.Entities.Booking
        {
            Id = "booking-1",
            InvolvedCustomers = [new Customer { Id = "customer-2" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Marketplace, "customer-1",
            OrganizationMemberRoleConstants.Administrator);
        var mappedBooking = new Shared.Models.Booking { Id = booking.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedBookingService.GetByIdAsync(booking.Id, cancellationToken)).Returns(booking);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<ICollection<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "org-1" })),
                null,
                false,
                false,
                cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(true);
        A.CallTo(() => sharedMapper.MapTo(booking)).Returns(mappedBooking);

        var result = await sut.GetByIdAsync(booking.Id, cancellationToken);

        result.ShouldBe(mappedBooking);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedBookingsAsync_Allows_Requesting_Own_Marketplace_Bookings_By_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-1"], "org-1");
        var organization = new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(false);
        A.CallTo(() => bookingRepository.GetPaginatedBookingsUntrackedAsync(
                A<PaginationInputParam>._,
                A<BookingSearchCriteria>.That.Matches(criteria =>
                    criteria.OrganizationId == null &&
                    criteria.OrganizationCustomDomain == null &&
                    criteria.CustomerIds.SequenceEqual(searchCriteria.CustomerIds)),
                A<ICollection<BookingOrder>>._,
                A<BookingAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.LocationIds.Count == 0 &&
                    scope.TeamIds.Count == 0),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false,
            cancellationToken);

        result.Item3.ShouldBe(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedBookingsAsync_Throws_When_Marketplace_Member_Requests_Other_Customers_Bookings(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-2"], "org-1");
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Marketplace, "customer-1", OrganizationMemberRoleConstants.Member);

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.GetPaginatedBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedBookingsAsync_Allows_Private_Member_To_Request_Other_Customers_Bookings(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        BookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-2"], "org-1");
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Private, "customer-1", OrganizationMemberRoleConstants.Member);

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(true);
        A.CallTo(() => bookingRepository.GetPaginatedBookingsUntrackedAsync(
                A<PaginationInputParam>._,
                A<BookingSearchCriteria>.That.Matches(criteria =>
                    criteria.OrganizationId == null &&
                    criteria.OrganizationCustomDomain == null &&
                    criteria.CustomerIds.SequenceEqual(searchCriteria.CustomerIds)),
                A<ICollection<BookingOrder>>._,
                A<BookingAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.LocationIds.Count == 0 &&
                    scope.TeamIds.Count == 0),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false,
            cancellationToken);

        result.Item3.ShouldBe(0);
    }

    private static BookingSearchCriteria CreateSearchCriteria(ICollection<string> customerIds, string? organizationId = null) =>
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
            organizationId,
            null,
            [],
            [],
            customerIds,
            []);

    private static Organization CreateOrganization(string id, string type, string customerId, string role) =>
        new()
        {
            Id = id,
            Type = type,
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Id = "membership-1",
                    CustomerId = customerId,
                    Customer = new Customer { Id = customerId },
                    OrganizationId = id,
                    Status = OrganizationMemberStatusConstants.Active,
                    Role = role
                }
            ]
        };
}
