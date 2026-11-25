using Booking.Api.Services;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;

namespace Booking.Api.UnitTests.Services.MarketplaceBookingSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetArrearsInvoicesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Load_Unauthorized_Invoices_From_Batch(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        IOrganizationArrearsInvoiceRepository organizationArrearsInvoiceRepository,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var subscription = new MarketplaceBookingSubscription
        {
            Id = "subscription-hidden",
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-owner",
                },
            ],
            InvolvedOrganizations = [],
            InvolvedTeams = [],
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository)
            .Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.OrganizationArrearsInvoiceRepository)
            .Returns(organizationArrearsInvoiceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsForGraphQlBatchAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([subscription]);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-viewer");

        var result = await sut.GetArrearsInvoicesAsync([subscription.Id], cancellationToken);

        result[subscription.Id].ShouldBeEmpty();
        A.CallTo(() => organizationArrearsInvoiceRepository.GetByMarketplaceBookingSubscriptionIdsUntrackedAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }
}
