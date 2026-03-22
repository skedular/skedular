using Api.Shared.Services;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using FakeItEasy;
using HotChocolate.Types.Pagination;
using Testing.Shared;
using Customer = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Api.UnitTests.Services.RecurringBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecurringBookingServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Returns_RecurringBooking_For_Involved_Customer(
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedRecurringBookingService cachedRecurringBookingService,
        [Frozen] Booking.Shared.Mappers.IMapper sharedMapper,
        RecurringBookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Booking.Shared.Database.Entities.RecurringBooking
        {
            Id = "recurring-1",
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var mappedBooking = new Booking.Shared.Models.RecurringBooking { Id = booking.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedRecurringBookingService.GetByIdAsync(booking.Id, cancellationToken)).Returns(booking);
        A.CallTo(() => sharedMapper.MapTo(booking)).Returns(mappedBooking);

        var result = await sut.GetByIdAsync(booking.Id, cancellationToken);

        result.ShouldBe(mappedBooking);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Allows_Marketplace_Admin_To_View_Other_Customers_RecurringBooking(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedRecurringBookingService cachedRecurringBookingService,
        [Frozen] Booking.Shared.Mappers.IMapper sharedMapper,
        RecurringBookingService sut,
        CancellationToken cancellationToken)
    {
        var booking = new Booking.Shared.Database.Entities.RecurringBooking
        {
            Id = "recurring-1",
            InvolvedCustomers = [new Customer { Id = "customer-2" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Marketplace, "customer-1", OrganizationMemberRoleConstants.Administrator);
        var mappedBooking = new Booking.Shared.Models.RecurringBooking { Id = booking.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedRecurringBookingService.GetByIdAsync(booking.Id, cancellationToken)).Returns(booking);
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
    public async Task GetPaginatedRecurringBookingsAsync_Allows_Requesting_Own_Marketplace_Bookings_By_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        RecurringBookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-1"], organizationId: "org-1");
        var organization = new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(false);
        A.CallTo(() => recurringBookingRepository.GetPaginatedRecurringBookingsUntrackedAsync(
                A<PaginationInputParam>._,
                A<RecurringBookingSearchCriteria>.That.Matches(criteria =>
                    criteria.OrganizationId == null &&
                    criteria.OrganizationCustomDomain == null &&
                    criteria.CustomerIds.SequenceEqual(searchCriteria.CustomerIds)),
                A<ICollection<RecurringBookingOrder>>._,
                A<RecurringBookingAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.TeamIds.Count == 0),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedRecurringBookingsAsync(
            new PaginationInputParam(null, null, null, null),
            searchCriteria,
            [],
            false,
            cancellationToken);

        result.Item3.ShouldBe(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedRecurringBookingsAsync_Throws_When_Marketplace_Member_Requests_Other_Customers_Bookings(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        RecurringBookingService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-2"], organizationId: "org-1");
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Marketplace, "customer-1", OrganizationMemberRoleConstants.Member);

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.GetPaginatedRecurringBookingsAsync(new PaginationInputParam(null, null, null, null), searchCriteria, [], false, cancellationToken));
    }

    private static RecurringBookingSearchCriteria CreateSearchCriteria(ICollection<string> customerIds, string? organizationId = null) =>
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
            null,
            organizationId,
            null,
            [],
            customerIds);

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
