using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Booking.Api.Mappers;
using Booking.Api.Models;
using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Services.Cache;
using HotChocolate;
using HotChocolate.Types;
using MarketplaceBookingFailureService = Booking.Shared.Services.IMarketplaceBookingFailureService;
using MarketplaceBookingAvailabilityConflict = Booking.Shared.Services.MarketplaceBookingAvailabilityConflict;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<MarketplaceBookingFailureDetails> AcceptPartialMarketplaceBookingAsync(
        ResolvePartialMarketplaceBookingInput input,
        [Service] MarketplaceBookingFailureService failureService,
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var result = await failureService.ResolvePartialAsync(input.Id, MarketplaceBookingFailureResolutionDecisionConstants.Accepted, customerId,
            cancellationToken);
        return graphQlMapper.MapTo(result);
    }

    [UseResolverScope]
    public async Task<MarketplaceBookingFailureDetails> DeclinePartialMarketplaceBookingAsync(
        ResolvePartialMarketplaceBookingInput input,
        [Service] MarketplaceBookingFailureService failureService,
        [Service] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var result = await failureService.ResolvePartialAsync(input.Id, MarketplaceBookingFailureResolutionDecisionConstants.Declined, customerId,
            cancellationToken);
        return graphQlMapper.MapTo(result);
    }

    [UseResolverScope]
    public async Task<BookingPayload> AddPrivateBookingAsync(
        AddPrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var booking = await privateBookingService.AddAsync(graphQlMapper.MapTo(input), input.FullOpeningHoursDate, cancellationToken);
            return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
        }
        catch (SpacesBookingQuotaExceeded exception)
        {
            return ToQuotaErrorPayload(input.ClientMutationId, exception);
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdatePrivateBookingAsync(
        UpdatePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var booking = await privateBookingService.UpdateAsync(
                new PrivateBookingPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                cancellationToken);
            return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeletePrivateBookingAsync(
        DeletePrivateBookingInput input,
        [Service] IPrivateBookingService privateBookingService,
        CancellationToken cancellationToken)
    {
        var booking = await privateBookingService.DeleteAsync(input.Id, cancellationToken);
        return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
    }

    [UseResolverScope]
    public async Task<BookingPayload> AddMarketplaceBookingAsync(
        AddMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await marketplaceBookingService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
            if (result.Failure is not null)
            {
                return new BookingPayload { ClientMutationId = input.ClientMutationId, Failure = graphQlMapper.MapTo(result.Failure) };
            }

            return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(result.Booking!) };
        }
        catch (SpacesBookingQuotaExceeded exception)
        {
            return ToQuotaErrorPayload(input.ClientMutationId, exception);
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
        catch (MarketplaceBookingAvailabilityConflict exception)
        {
            return new BookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                AvailabilityError = new BookingAvailabilityErrorDetails
                {
                    Message = exception.Message, UnavailableResourceIds = exception.UnavailableResourceIds
                }
            };
        }
    }

    [UseResolverScope]
    public async Task<BookingPayload> UpdateMarketplaceBookingAsync(
        UpdateMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var booking = await marketplaceBookingService.UpdateAsync(
                new MarketplaceBookingPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                cancellationToken);
            return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
    }

    [UseResolverScope]
    public async Task<BookingPayload> DeleteMarketplaceBookingAsync(
        DeleteMarketplaceBookingInput input,
        [Service] IMarketplaceBookingService marketplaceBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var booking = await marketplaceBookingService.DeleteAsync(input.Id, input.CancellationOverrideReason, cancellationToken);
            return new BookingPayload { ClientMutationId = input.ClientMutationId, Booking = graphQlMapper.MapTo(booking) };
        }
        catch (MarketplaceBookingCancellationNotAllowed exception)
        {
            return new BookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError = new CancellationErrorDetails { Code = CancellationErrorCode.PolicyRestriction, Message = exception.Message }
            };
        }
        catch (MarketplaceBookingCancellationOverrideReasonRequired exception)
        {
            return new BookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError =
                    new CancellationErrorDetails { Code = CancellationErrorCode.OverrideReasonRequired, Message = exception.Message }
            };
        }
        catch (UnauthorizedAccessException exception)
        {
            return new BookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError = new CancellationErrorDetails
                {
                    Code = CancellationErrorCode.InsufficientManagementPermission, Message = exception.Message
                }
            };
        }
    }

    private static BookingPayload ToQuotaErrorPayload(string? clientMutationId, SpacesBookingQuotaExceeded exception) =>
        new()
        {
            ClientMutationId = clientMutationId,
            QuotaError = new BookingSpacesQuotaErrorDetails
            {
                ErrorCode = exception.ErrorCode,
                ReasonCode =
                    new SpacesQuotaReasonCodeDetails { Type = exception.ReasonCode, Name = exception.ReasonCode.ToSpacesQuotaReasonCodeName() },
                CurrentUsage = exception.CurrentUsage,
                QuotaLimit = exception.QuotaLimit,
                AttemptedCurrentPeriodCount = exception.AttemptedCurrentPeriodCount,
                ExcludedOutOfPeriodCount = exception.ExcludedOutOfPeriodCount,
                TotalAttemptedInstanceCount = exception.TotalAttemptedInstanceCount,
                RemainingQuota = exception.RemainingQuota,
                UpgradePlans = exception.UpgradePlans.Select(upgrade => new UpgradePlanDetails
                {
                    PlanCode = upgrade.PlanCode,
                    Name = upgrade.Name,
                    Availability = upgrade.Availability,
                    PriceDescription = upgrade.PriceDescription
                }).ToList()
            }
        };

    private static BookingPayload ToAccessErrorPayload(string? clientMutationId, SpacesAccessDenied exception) =>
        new()
        {
            ClientMutationId = clientMutationId,
            AccessError = new SpacesAccessErrorDetails
            {
                ErrorCode = exception.ErrorCode,
                Status = exception.Status,
                ReasonCode = exception.ReasonCode,
                UpgradeRequired = exception.UpgradeRequired,
                Message = exception.Message
            }
        };
}
