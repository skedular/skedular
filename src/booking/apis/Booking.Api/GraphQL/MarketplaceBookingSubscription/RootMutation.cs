using Api.Shared.Services;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionPayload> AddMarketplaceBookingSubscriptionAsync(
        AddMarketplaceBookingSubscriptionInput input,
        [Service]
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await marketplaceBookingSubscriptionService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                MarketplaceBookingSubscription = graphQlMapper.MapTo(subscription),
            };
        }
        catch (SpacesAccessDenied exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                AccessError = new SpacesAccessErrorDetails
                {
                    ErrorCode = exception.ErrorCode,
                    Status = exception.Status,
                    ReasonCode = exception.ReasonCode,
                    UpgradeRequired = exception.UpgradeRequired,
                    Message = exception.Message,
                },
            };
        }
    }

    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionPayload> DeleteMarketplaceBookingSubscriptionAsync(
        DeleteMarketplaceBookingSubscriptionInput input,
        [Service]
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await marketplaceBookingSubscriptionService.DeleteAsync(
                input.Id,
                input.CancellationMode,
                input.CancellationOverrideReason,
                cancellationToken);
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                MarketplaceBookingSubscription = graphQlMapper.MapTo(subscription),
            };
        }
        catch (MarketplaceBookingSubscriptionCancellationNotAllowed exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError = new CancellationErrorDetails
                {
                    Code = CancellationErrorCode.PolicyRestriction,
                    Message = exception.Message,
                },
            };
        }
        catch (MarketplaceBookingSubscriptionCancellationOverrideReasonRequired exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError =
                    new CancellationErrorDetails
                    {
                        Code = CancellationErrorCode.OverrideReasonRequired,
                        Message = exception.Message,
                    },
            };
        }
        catch (UnauthorizedAccessException exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError = new CancellationErrorDetails
                {
                    Code = CancellationErrorCode.InsufficientManagementPermission,
                    Message = exception.Message,
                },
            };
        }
        catch (MarketplaceBookingSubscriptionCannotBeUpdated exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                CancellationError =
                    new CancellationErrorDetails
                    {
                        Code = CancellationErrorCode.InvalidTerminalState,
                        Message = exception.Message,
                    },
            };
        }
    }
}
