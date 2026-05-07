using Api.Shared.Services.Models;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.Services;
using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace Booking.Api.UnitTests.GraphQL.MarketplaceBookingSubscription.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionsWithUnrecognisedStatusShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Warning_For_Unrecognised_Subscription_Status_Value(
        [Frozen] ILogger<RootQuery> logger,
        RootQuery sut,
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var whereInput = new MarketplaceBookingSubscriptionWhereInput { Statuses = [(MarketplaceBookingSubscriptionStatus)999] };

        A.CallTo(() => marketplaceBookingSubscriptionService.GetPaginatedMarketplaceBookingSubscriptionsAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>._,
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<bool>._,
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        await sut.MarketplaceBookingSubscriptionsAsync(
            null, null, null, null,
            whereInput,
            null,
            marketplaceBookingSubscriptionService,
            cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == "Log"
                           && call.Arguments.Count > 0
                           && (LogLevel)call.Arguments[0]! == LogLevel.Warning)
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Warning_For_Unrecognised_Payment_Status_Value(
        [Frozen] ILogger<RootQuery> logger,
        RootQuery sut,
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        var whereInput = new MarketplaceBookingSubscriptionWhereInput { PaymentStatuses = [(PaymentStatus)999] };

        A.CallTo(() => marketplaceBookingSubscriptionService.GetPaginatedMarketplaceBookingSubscriptionsAsync(
                A<PaginationInputParam>._,
                A<MarketplaceBookingSubscriptionSearchCriteria>._,
                A<IReadOnlyList<MarketplaceBookingSubscriptionOrder>>._,
                A<bool>._,
                cancellationToken))
            .Returns((new PaginatedInfo(false, false, null, null), [], 0));

        await sut.MarketplaceBookingSubscriptionsAsync(
            null, null, null, null,
            whereInput,
            null,
            marketplaceBookingSubscriptionService,
            cancellationToken);

        A.CallTo(logger)
            .Where(call => call.Method.Name == "Log"
                           && call.Arguments.Count > 0
                           && (LogLevel)call.Arguments[0]! == LogLevel.Warning)
            .MustHaveHappenedOnceExactly();
    }
}
