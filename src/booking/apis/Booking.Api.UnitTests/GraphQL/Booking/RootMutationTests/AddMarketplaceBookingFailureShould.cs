using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Services;
using ApiMarketplaceBookingService = Booking.Api.Services.IMarketplaceBookingService;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddMarketplaceBookingFailureShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Return_Failure_Details_When_Availability_Conflict_Is_Finalized(
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] ApiMarketplaceBookingService marketplaceBookingService,
        RootMutation sut,
        AddMarketplaceBookingInput input,
        MarketplaceBookingFailure failure,
        MarketplaceBookingFailureDetails failureDetails,
        CancellationToken cancellationToken)
    {
        var failureModel = (MarketplaceBookingFailureSummary)failure;
        var booking = new Shared.Models.Booking();
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(booking);
        A.CallTo(() => marketplaceBookingService.AddAsync(booking, cancellationToken))
            .Returns(new MarketplaceBookingAddResult(Failure: failureModel));
        A.CallTo(() => graphQlMapper.MapTo(failureModel)).Returns(failureDetails);

        var result = await sut.AddMarketplaceBookingAsync(input, marketplaceBookingService, cancellationToken);

        result.Booking.ShouldBeNull();
        result.AvailabilityError.ShouldBeNull();
        result.Failure.ShouldNotBeNull();
        result.Failure.ShouldBe(failureDetails);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Return_Availability_Error_When_Conflict_Has_No_Finalized_Failure(
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] ApiMarketplaceBookingService marketplaceBookingService,
        RootMutation sut,
        AddMarketplaceBookingInput input,
        IReadOnlyCollection<string> unavailableResourceIds,
        CancellationToken cancellationToken)
    {
        var booking = new Shared.Models.Booking();
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(booking);
        A.CallTo(() => marketplaceBookingService.AddAsync(booking, cancellationToken))
            .ThrowsAsync(new MarketplaceBookingAvailabilityConflict(unavailableResourceIds));

        var result = await sut.AddMarketplaceBookingAsync(input, marketplaceBookingService, cancellationToken);

        result.Booking.ShouldBeNull();
        result.Failure.ShouldBeNull();
        result.AvailabilityError.ShouldNotBeNull();
    }

    [Theory]
    [AutoFakeItEasyData([typeof(BookingFailureQueryFixtureCustomizer)])]
    public async Task Return_Booking_On_Success(
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] ApiMarketplaceBookingService marketplaceBookingService,
        RootMutation sut,
        AddMarketplaceBookingInput input,
        Shared.Models.Booking bookingModel,
        BookingDetails bookingDetails,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(bookingModel);
        A.CallTo(() => marketplaceBookingService.AddAsync(bookingModel, cancellationToken))
            .Returns(new MarketplaceBookingAddResult(bookingModel));
        A.CallTo(() => graphQlMapper.MapTo(bookingModel)).Returns(bookingDetails);

        var result = await sut.AddMarketplaceBookingAsync(input, marketplaceBookingService, cancellationToken);

        result.Failure.ShouldBeNull();
        result.AvailabilityError.ShouldBeNull();
        result.Booking.ShouldNotBeNull();
    }
}
