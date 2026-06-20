using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Offering = Api.Shared.Services.Models.Offering;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SpacesTrialExpiryShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Access_Error_Separate_From_Paid_Quota_Error(
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IPrivateBookingService privateBookingService,
        RootMutation sut,
        SpacesAccessEvaluator evaluator,
        string clientMutationId,
        DateOnly fullOpeningHoursDate,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var input = new AddPrivateBookingInput { ClientMutationId = clientMutationId, FullOpeningHoursDate = fullOpeningHoursDate };
        var booking = new Shared.Models.Booking();
        var offering = new Offering
        {
            Code = OfferingCode.SpacesFreeTierV1, SpacesProductEnabled = true, SpacesTrialStartedAt = now.AddDays(-14), SpacesTrialEndsAt = now
        };
        var decision = evaluator.Evaluate(now, offering, SpacesAccessAction.CreateBookingInstance);
        A.CallTo(() => graphQlMapper.MapTo(input)).Returns(booking);
        A.CallTo(() => privateBookingService.AddAsync(booking, input.FullOpeningHoursDate, cancellationToken))
            .ThrowsAsync(new SpacesAccessDenied(decision));

        var result = await sut.AddPrivateBookingAsync(input, privateBookingService, cancellationToken);

        result.Booking.ShouldBeNull();
        result.QuotaError.ShouldBeNull();
        result.AccessError.ShouldNotBeNull();
        result.AccessError.ErrorCode.ShouldBe(SpacesAccessDenied.Code);
        result.AccessError.Status.ShouldBe(SpacesSubscriptionStatus.TrialExpired);
        result.AccessError.UpgradeRequired.ShouldBeTrue();
    }
}
