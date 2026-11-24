using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using GreenDonut;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingFailureQueryShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Resolve_Marketplace_Booking_Failure_Through_The_Owning_Booking(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IMarketplaceBookingFailureReadService failureReadService,
        MarketplaceBookingDetails marketplaceBookingDetails,
        MarketplaceBookingFailure failure,
        MarketplaceBookingFailureDetails failureDetails,
        CancellationToken cancellationToken)
    {
        var failureModel = (MarketplaceBookingFailureSummary)failure;
        A.CallTo(() => failureReadService.GetByMarketplaceBookingIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(new Dictionary<string, MarketplaceBookingFailureSummary>
            {
                [marketplaceBookingDetails.Id] = failureModel,
            });
        A.CallTo(() => graphQlMapper.MapTo(failureModel)).Returns(failureDetails);
        var dataLoader = new MarketplaceBookingFailureDataLoader(
            AutoBatchScheduler.Default,
            new DataLoaderOptions(),
            failureReadService,
            graphQlMapper);

        var result = await MarketplaceBookingDetailsType.GetFailure(
            marketplaceBookingDetails,
            dataLoader,
            cancellationToken);

        result.ShouldBe(failureDetails);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Return_Failure_When_Booking_Has_A_Finalized_Failure(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IMarketplaceBookingFailureReadService failureReadService,
        BookingDetails bookingDetails,
        MarketplaceBookingFailure failure,
        MarketplaceBookingFailureDetails failureDetails,
        CancellationToken cancellationToken)
    {
        var failureModel = (MarketplaceBookingFailureSummary)failure;
        A.CallTo(() => failureReadService.GetByBookingIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(new Dictionary<string, MarketplaceBookingFailureSummary>
            {
                [bookingDetails.Id] = failureModel,
            });
        A.CallTo(() => graphQlMapper.MapTo(failureModel)).Returns(failureDetails);
        var dataLoader = new BookingFailureDataLoader(
            AutoBatchScheduler.Default,
            new DataLoaderOptions(),
            failureReadService,
            graphQlMapper);

        var result = await BookingDetailsType.GetFailureAsync(dataLoader, bookingDetails, cancellationToken);

        result.ShouldNotBeNull();
        result.ShouldBe(failureDetails);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Return_Null_When_No_Failure_Exists_For_Booking(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IMarketplaceBookingFailureReadService failureReadService,
        BookingDetails bookingDetails,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => failureReadService.GetByBookingIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(new Dictionary<string, MarketplaceBookingFailureSummary>());
        var dataLoader = new BookingFailureDataLoader(
            AutoBatchScheduler.Default,
            new DataLoaderOptions(),
            failureReadService,
            graphQlMapper);

        var result = await BookingDetailsType.GetFailureAsync(dataLoader, bookingDetails, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => graphQlMapper.MapTo(A<MarketplaceBookingFailureSummary>._)).MustNotHaveHappened();
    }
}
