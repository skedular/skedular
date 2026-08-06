using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

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
        [Frozen]
        IMarketplaceBookingService marketplaceBookingService,
        MarketplaceBookingDetails marketplaceBookingDetails,
        BookingEntity booking,
        MarketplaceBookingFailure failure,
        MarketplaceBookingFailureDetails failureDetails,
        CancellationToken cancellationToken)
    {
        var failureModel = (MarketplaceBookingFailureSummary)failure;
        A.CallTo(() => marketplaceBookingService.GetBookingIdAsync(marketplaceBookingDetails.Id, cancellationToken)).Returns(booking.Id);
        A.CallTo(() => failureReadService.GetByBookingIdAsync(booking.Id, cancellationToken)).Returns(failureModel);
        A.CallTo(() => graphQlMapper.MapTo(failureModel)).Returns(failureDetails);

        var result = await MarketplaceBookingDetailsType.GetFailure(
            marketplaceBookingDetails,
            failureReadService,
            marketplaceBookingService,
            graphQlMapper,
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
        A.CallTo(() => failureReadService.GetByBookingIdAsync(bookingDetails.Id, cancellationToken)).Returns(failureModel);
        A.CallTo(() => graphQlMapper.MapTo(failureModel)).Returns(failureDetails);

        var result = await BookingDetailsType.GetFailureAsync(graphQlMapper, failureReadService, bookingDetails, cancellationToken);

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
        A.CallTo(() => failureReadService.GetByBookingIdAsync(bookingDetails.Id, cancellationToken))
            .Returns((MarketplaceBookingFailureSummary?)null);

        var result = await BookingDetailsType.GetFailureAsync(graphQlMapper, failureReadService, bookingDetails, cancellationToken);

        result.ShouldBeNull();
        A.CallTo(() => graphQlMapper.MapTo(A<MarketplaceBookingFailureSummary>._)).MustNotHaveHappened();
    }
}
