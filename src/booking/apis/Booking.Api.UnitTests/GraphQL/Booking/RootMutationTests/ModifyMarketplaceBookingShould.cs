using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ModifyMarketplaceBookingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_An_Eligibility_Error_For_A_Missing_Operator_Reason(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IMarketplaceBookingModificationService marketplaceBookingModificationService,
        RootMutation sut,
        ModifyMarketplaceBookingInput input,
        MarketplaceBookingModificationCommand command,
        CancellationToken cancellationToken)
    {
        var error = new MarketplaceBookingModificationError(
            MarketplaceBookingModificationErrorCode.OperatorReasonRequired,
            "A reason is required when changing a booking for a customer.");
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(command);
        A.CallTo(() => marketplaceBookingModificationService.ModifyAsync(command, cancellationToken))
            .Returns(new MarketplaceBookingModificationResult(null, null, error));

        var result = await sut.ModifyMarketplaceBookingAsync(input, marketplaceBookingModificationService, cancellationToken);

        result.EligibilityError.ShouldNotBeNull();
        result.EligibilityError.Code.ShouldBe(MarketplaceBookingModificationErrorCode.OperatorReasonRequired);
        result.AvailabilityError.ShouldBeNull();
        result.ConflictError.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_A_Conflict_Error_For_A_Stale_Version(
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IMarketplaceBookingModificationService marketplaceBookingModificationService,
        RootMutation sut,
        ModifyMarketplaceBookingInput input,
        MarketplaceBookingModificationCommand command,
        CancellationToken cancellationToken)
    {
        var error = new MarketplaceBookingModificationError(
            MarketplaceBookingModificationErrorCode.StaleVersion,
            "Reload and try again.");
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(command);
        A.CallTo(() => marketplaceBookingModificationService.ModifyAsync(command, cancellationToken))
            .Returns(new MarketplaceBookingModificationResult(null, null, error));

        var result = await sut.ModifyMarketplaceBookingAsync(input, marketplaceBookingModificationService, cancellationToken);

        result.ConflictError.ShouldNotBeNull();
        result.ConflictError.Code.ShouldBe(MarketplaceBookingModificationErrorCode.StaleVersion);
        result.EligibilityError.ShouldBeNull();
    }
}
