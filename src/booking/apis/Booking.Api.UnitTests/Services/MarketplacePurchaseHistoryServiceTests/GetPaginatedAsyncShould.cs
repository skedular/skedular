using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.Logging;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.MarketplacePurchaseHistoryServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetPaginatedAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_No_Rows_For_Unscoped_Own_Purchases(
        [Frozen] ICachedCustomerService cachedCustomerService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");

        var result = await sut.GetPaginatedAsync(
            new PaginationInputParam(null, 10, null, null),
            null,
            new MarketplacePurchaseHistorySearchCriteria(null, null, null, IncludeMineOnly: true),
            null,
            cancellationToken);

        result.Item3.ShouldBe(0);
        result.Item2.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allow_NonMember_To_Read_Own_Purchases(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
            .Returns(new Organization
            {
                Id = "organization-1",
                CustomDomain = "example.test",
            });
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._,
                A<MarketplacePurchaseHistorySearchCriteria>._,
                A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), Array.Empty<Edge<MarketplacePurchaseHistoryRow>>(), 0));

        await sut.GetPaginatedAsync(
            new PaginationInputParam(null, 10, null, null),
            "example.test",
            new MarketplacePurchaseHistorySearchCriteria("example.test", "another-customer", null,
                IncludeMineOnly: true),
            null,
            cancellationToken);

        A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync(
            A<string>._, A<string>._, cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(
            A<PaginationInputParam>._,
            A<MarketplacePurchaseHistorySearchCriteria>.That.Matches(criteria => criteria.CustomerId == "customer-1"),
            A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._,
            cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Read_only_from_history_repository(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        IMarketplaceBookingService marketplaceBookingService,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        var row = new MarketplacePurchaseHistoryRow(
            "purchase-1",
            MarketplacePurchaseSourceType.Booking,
            TimeProvider.System.GetUtcNow().AddMinutes(-1),
            TimeProvider.System.GetUtcNow(),
            null,
            null,
            PaymentStatusConstants.Confirmed,
            "product-version-1",
            "Hourly desk",
            10,
            "NZD",
            "customer-1",
            "organization-1",
            null,
            null,
            null,
            false,
            false,
            false);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
            .ReturnsLazily(_ => Task.FromResult<Organization?>(new Organization
            {
                Id = "organization-1",
                CustomDomain = "example.test",
            }));
        A.CallTo(() => marketplaceBookingService.GetBookingIdAsync("purchase-1", cancellationToken)).Returns("booking-1");
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync("organization-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(
                A<PaginationInputParam>._,
                A<MarketplacePurchaseHistorySearchCriteria>._,
                A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._,
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, "cursor-1", "cursor-1"),
                [new Edge<MarketplacePurchaseHistoryRow>(row, "cursor-1")], 1));

        var result = await sut.GetPaginatedAsync(
            new PaginationInputParam(null, 10, null, null),
            "example.test",
            new MarketplacePurchaseHistorySearchCriteria("example.test", null, null),
            null,
            cancellationToken);

        result.Item3.ShouldBe(1);
        result.Item2.Single().Node.Id.ShouldBe("purchase-1");
        result.Item2.Single().Node.BookingId.ShouldBe("booking-1");
        A.CallTo(() => marketplaceBookingService.GetBookingIdAsync("purchase-1", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(
            A<PaginationInputParam>._,
            A<MarketplacePurchaseHistorySearchCriteria>._,
            A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._,
            cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_Rows_Outside_Organization_Authorization(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        var row = CreateRow("purchase-1", MarketplacePurchaseSourceType.Booking, false, null);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
            .ReturnsLazily(_ => Task.FromResult<Organization?>(new Organization
            {
                Id = "organization-1",
                CustomDomain = "example.test",
            }));
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync(row.OrganizationId, "customer-1", cancellationToken)).Returns(false);
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._, A<MarketplacePurchaseHistorySearchCriteria>._,
                A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [new Edge<MarketplacePurchaseHistoryRow>(row, "cursor-1")], 1));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.GetPaginatedAsync(new PaginationInputParam(null, 10, null, null),
            "example.test",
            new MarketplacePurchaseHistorySearchCriteria("example.test", null, null), null, cancellationToken));
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._, A<MarketplacePurchaseHistorySearchCriteria>._,
            A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken)).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Pending_Lifecycle_For_Legacy_Subscription_Without_Status(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        var row = CreateRow("subscription-1", MarketplacePurchaseSourceType.Subscription, false, null);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
            .ReturnsLazily(_ => Task.FromResult<Organization?>(new Organization
            {
                Id = "organization-1",
                CustomDomain = "example.test",
            }));
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync(row.OrganizationId, "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._, A<MarketplacePurchaseHistorySearchCriteria>._,
                A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken))
            .Returns((new PaginatedInfo(false, false, "cursor-1", "cursor-1"), [new Edge<MarketplacePurchaseHistoryRow>(row, "cursor-1")], 1));

        var result = await sut.GetPaginatedAsync(new PaginationInputParam(null, 10, null, null), "example.test",
            new MarketplacePurchaseHistorySearchCriteria("example.test", null, null), null, cancellationToken);

        result.Item2.Single().Node.LifecycleState.ShouldBe(MarketplacePurchaseLifecycleState.Pending);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Independent_Lifecycle_And_Refund_State(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        var scenarios = new[]
        {
            (Status: "CANCELLED", Deleted: false, Expected: MarketplacePurchaseLifecycleState.Cancelled),
            (Status: "EXPIRED", Deleted: false, Expected: MarketplacePurchaseLifecycleState.Expired),
            (Status: "RENEWAL_FAILED", Deleted: false, Expected: MarketplacePurchaseLifecycleState.PaymentFailed),
            (Status: null, Deleted: true, Expected: MarketplacePurchaseLifecycleState.Deleted),
        };
        foreach (var scenario in scenarios)
        {
            var row = CreateRow($"purchase-{scenario.Expected}", MarketplacePurchaseSourceType.Subscription, scenario.Deleted, scenario.Status) with
            {
                RefundId = "refund-1",
            };
            A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
            A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
            A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
                .ReturnsLazily(_ => Task.FromResult<Organization?>(new Organization
                {
                    Id = "organization-1",
                    CustomDomain = "example.test",
                }));
            A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
            A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync(row.OrganizationId, "customer-1", cancellationToken))
                .Returns(true);
            A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._, A<MarketplacePurchaseHistorySearchCriteria>._,
                    A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken))
                .Returns((new PaginatedInfo(false, false, "cursor-1", "cursor-1"), [new Edge<MarketplacePurchaseHistoryRow>(row, "cursor-1")], 1));

            var result = await sut.GetPaginatedAsync(new PaginationInputParam(null, 10, null, null), "example.test",
                new MarketplacePurchaseHistorySearchCriteria("example.test", null, null), null, cancellationToken);
            result.Item2.Single().Node.LifecycleState.ShouldBe(scenario.Expected);
            result.Item2.Single().Node.RefundId.ShouldBe("refund-1");
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Warn_And_Return_Pending_For_Unknown_Legacy_Status(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository historyRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService authorizationService,
        [Frozen]
        ILogger<MarketplacePurchaseHistoryService> logger,
        MarketplacePurchaseHistoryService sut,
        CancellationToken cancellationToken)
    {
        var row = CreateRow("subscription-legacy", MarketplacePurchaseSourceType.Subscription, false, "UNKNOWN_STATUS");
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(historyRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "example.test", false, false, cancellationToken))
            .ReturnsLazily(_ => Task.FromResult<Organization?>(new Organization
            {
                Id = "organization-1",
                CustomDomain = "example.test",
            }));
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => authorizationService.CanViewOtherCustomersBookingsAsync(row.OrganizationId, "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => historyRepository.GetPaginatedRowsAsync(A<PaginationInputParam>._, A<MarketplacePurchaseHistorySearchCriteria>._,
                A<IReadOnlyList<MarketplacePurchaseHistoryOrder>?>._, cancellationToken))
            .Returns((new PaginatedInfo(false, false, "cursor-1", "cursor-1"), [new Edge<MarketplacePurchaseHistoryRow>(row, "cursor-1")], 1));

        var result = await sut.GetPaginatedAsync(new PaginationInputParam(null, 10, null, null), "example.test",
            new MarketplacePurchaseHistorySearchCriteria("example.test", null, null), null, cancellationToken);

        result.Item2.Single().Node.LifecycleState.ShouldBe(MarketplacePurchaseLifecycleState.Pending);
        A.CallTo(logger).Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappened();
    }

    private static MarketplacePurchaseHistoryRow CreateRow(string id, MarketplacePurchaseSourceType sourceType, bool isDeleted,
        string? subscriptionStatus) => new(
        id,
        sourceType,
        TimeProvider.System.GetUtcNow().AddMinutes(-1),
        TimeProvider.System.GetUtcNow(),
        null,
        null,
        PaymentStatusConstants.Confirmed,
        "product-version-1",
        "Product",
        10,
        "NZD",
        "customer-1",
        "organization-1",
        null,
        null,
        subscriptionStatus,
        false,
        false,
        isDeleted);
}
