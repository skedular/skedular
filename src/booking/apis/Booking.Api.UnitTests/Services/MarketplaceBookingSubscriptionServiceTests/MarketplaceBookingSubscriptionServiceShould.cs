using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Pagination;
using Customer = Booking.Shared.Database.Entities.Customer;
using IMarketplaceBookingSubscriptionService = Booking.Shared.Services.IMarketplaceBookingSubscriptionService;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationMember = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Returns_Subscription_For_Involved_Customer(
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
        [Frozen] IEntityMapper sharedEntityMapper,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1",
            InvolvedCustomers = [new Customer { Id = "customer-1" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var mappedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = subscription.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedMarketplaceBookingSubscriptionService.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);
        A.CallTo(() => sharedEntityMapper.MapTo(subscription)).Returns(mappedSubscription);

        var result = await sut.GetByIdAsync(subscription.Id, cancellationToken);

        result.ShouldBe(mappedSubscription);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetByIdAsync_Allows_Marketplace_Admin_To_View_Other_Customers_Subscription(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedMarketplaceBookingSubscriptionService cachedMarketplaceBookingSubscriptionService,
        [Frozen] IEntityMapper sharedEntityMapper,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1",
            InvolvedCustomers = [new Customer { Id = "customer-2" }],
            InvolvedOrganizations = [new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace }]
        };
        var organization = CreateOrganization("org-1", OrganizationTypeConstants.Marketplace, "customer-1",
            OrganizationMemberRoleConstants.Administrator);
        var mappedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = subscription.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => cachedMarketplaceBookingSubscriptionService.GetByIdAsync(subscription.Id, cancellationToken)).Returns(subscription);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<IReadOnlyList<string>>.That.Matches(ids => ids.SequenceEqual(new[] { "org-1" })),
                null,
                false,
                false,
                cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(true);
        A.CallTo(() => sharedEntityMapper.MapTo(subscription)).Returns(mappedSubscription);

        var result = await sut.GetByIdAsync(subscription.Id, cancellationToken);

        result.ShouldBe(mappedSubscription);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedMarketplaceBookingSubscriptionsAsync_Allows_Requesting_Own_Marketplace_Subscriptions_By_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var searchCriteria = CreateSearchCriteria(["customer-1"], "org-1");
        var organization = new Organization { Id = "org-1", Type = OrganizationTypeConstants.Marketplace };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanViewOtherCustomersBookingsAsync("org-1", "customer-1", cancellationToken))
            .Returns(false);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>.That.Matches(criteria =>
                    criteria.OrganizationId == null &&
                    criteria.OrganizationCustomDomain == null &&
                    criteria.CustomerIds.SequenceEqual(searchCriteria.CustomerIds)),
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<MarketplaceBookingSubscriptionAccessScope>.That.Matches(scope =>
                    scope.OrganizationIds.SequenceEqual(new[] { "org-1" }) &&
                    scope.TeamIds.Count == 0),
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        var result = await sut.GetPaginatedMarketplaceBookingSubscriptionsAsync(
            new PaginationInputParam(null, null, null, null),
            searchCriteria,
            [],
            false,
            cancellationToken);

        result.Item3.ShouldBe(0);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task GetPaginatedMarketplaceBookingSubscriptionsAsync_Throws_When_Marketplace_Member_Requests_Other_Customers_Subscriptions(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        MarketplaceBookingSubscriptionService sut,
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
            sut.GetPaginatedMarketplaceBookingSubscriptionsAsync(
                new PaginationInputParam(null, null, null, null),
                searchCriteria,
                [],
                false,
                cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Forwards_Immediate_Cancellation_Mode_To_Shared_Service(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IContext context,
        [Frozen] IMarketplaceBookingSubscriptionService sharedMarketplaceBookingSubscriptionService,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1" };
        var existingSubscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1", InvolvedCustomers = [customer], InvolvedOrganizations = [], InvolvedTeams = []
        };
        var deletedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => context.GetVerifiableToken()).Returns("token-1");
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(A<string>._, true, cancellationToken)).Returns(customer);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(existingSubscription.Id, cancellationToken))
            .Returns(existingSubscription);
        A.CallTo(() => sharedMarketplaceBookingSubscriptionService.DeleteAsync(
                existingSubscription,
                customer,
                MarketplaceBookingSubscriptionCancellationMode.Immediate,
                false,
                null,
                cancellationToken))
            .Returns(deletedSubscription);

        var result = await sut.DeleteAsync(existingSubscription.Id, MarketplaceBookingSubscriptionCancellationMode.Immediate, null,
            cancellationToken);

        result.ShouldBe(deletedSubscription);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Forwards_At_Period_End_Cancellation_Mode_To_Shared_Service(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] IContext context,
        [Frozen] IMarketplaceBookingSubscriptionService sharedMarketplaceBookingSubscriptionService,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1" };
        var existingSubscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-1", InvolvedCustomers = [customer], InvolvedOrganizations = [], InvolvedTeams = []
        };
        var deletedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => context.GetVerifiableToken()).Returns("token-1");
        A.CallTo(() => customerRepository.GetByVerifiableTokenAsync(A<string>._, true, cancellationToken)).Returns(customer);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(existingSubscription.Id, cancellationToken))
            .Returns(existingSubscription);
        A.CallTo(() => sharedMarketplaceBookingSubscriptionService.DeleteAsync(
                existingSubscription,
                customer,
                MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
                false,
                null,
                cancellationToken))
            .Returns(deletedSubscription);

        var result = await sut.DeleteAsync(existingSubscription.Id, MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd, null,
            cancellationToken);

        result.ShouldBe(deletedSubscription);
    }

    private static MarketplaceBookingSubscriptionSearchCriteria CreateSearchCriteria(
        IReadOnlyList<string> customerIds,
        string? organizationId = null) =>
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
            null,
            null,
            organizationId,
            null,
            [],
            customerIds,
            [],
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
