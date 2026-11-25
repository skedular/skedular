using Booking.Shared.Repositories;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class ArrearsInvoiceBatchQueriesShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Translate_Batch_Queries_With_Missing_Ids(
        string bookingId,
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var byBooking = await repositoryFactory.OrganizationArrearsInvoiceRepository
            .GetByBookingIdsUntrackedAsync([bookingId], cancellationToken);
        var bySubscription = await repositoryFactory.OrganizationArrearsInvoiceRepository
            .GetByMarketplaceBookingSubscriptionIdsUntrackedAsync([subscriptionId], cancellationToken);

        byBooking.ShouldBeEmpty();
        bySubscription.ShouldBeEmpty();
    }
}
