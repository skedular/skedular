using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Customer = Booking.Shared.Database.Entities.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.MarketplaceRefundReadServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetByOrganizationCustomDomainAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Refunds_For_Organization_With_Actor_Names(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen] IMarketplaceRefundEventRepository marketplaceRefundEventRepository,
        [Frozen] ICustomerRepository customerRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMapper mapper,
        [Frozen] IXeroRefundService xeroRefundService,
        MarketplaceRefundReadService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = "org-1" };
        var refund = new MarketplaceRefund { Id = "refund-1", OrganizationId = "org-1", RequestedByCustomerId = "requester-1" };
        var refundEvent = new MarketplaceRefundEvent { Id = "refund-event-1", MarketplaceRefundId = "refund-1", ActorCustomerId = "actor-1" };
        var mappedRefund = new MarketplaceRefundDetails { Id = "refund-1" };
        var mappedEvent = new MarketplaceRefundEventDetails { Id = "refund-event-1" };
        List<Customer> customers =
        [
            new() { Id = "actor-1", GivenName = "Alex", FamilyName = "Operator" },
            new() { Id = "requester-1", GivenName = "Jamie", FamilyName = "Buyer" }
        ];

        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, "acme", false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync("org-1", "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundEventRepository).Returns(marketplaceRefundEventRepository);
        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => marketplaceRefundRepository.GetByOrganizationIdAsync("org-1", A<ICollection<string>?>._, cancellationToken)).Returns([refund]);
        A.CallTo(() => marketplaceRefundEventRepository.GetByMarketplaceRefundIdAsync("refund-1", cancellationToken)).Returns([refundEvent]);
        A.CallTo(() => customerRepository.GetByIdsAsync(
                A<ICollection<string>>.That.Matches(ids => ids.Count == 2 && ids.Contains("actor-1") && ids.Contains("requester-1")), true,
                cancellationToken))
            .Returns(customers);
        A.CallTo(() => mapper.MapTo(refund)).Returns(mappedRefund);
        A.CallTo(() => mapper.MapTo(refundEvent)).Returns(mappedEvent);
        A.CallTo(() => xeroRefundService.GetProcessingAvailabilityAsync(refund, cancellationToken))
            .Returns(new XeroRefundProcessingAvailability(false, "Blocked"));

        var result = await sut.GetByOrganizationCustomDomainAsync("acme", null, cancellationToken);

        result.Count.ShouldBe(1);
        result.Single().RequestedByCustomerName.ShouldBe("Jamie Buyer");
        result.Single().Events.Single().ActorName.ShouldBe("Alex Operator");
        result.Single().CanProcessInXero.ShouldBeFalse();
    }
}
