using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Services;
using HotChocolate;
using HotChocolate.Types;
using IMarketplaceBookingSubscriptionService = Booking.Api.Services.IMarketplaceBookingSubscriptionService;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<MarketplaceBookingSubscriptionPayload> CreateMarketplaceBookingSubscriptionAutomaticPaymentRecoveryAsync(
        CreateAutomaticPaymentRecoveryInput input,
        [Service]
        IMarketplaceBookingSubscriptionService marketplaceBookingSubscriptionService,
        [Service]
        IMarketplaceAutomaticPaymentStatusService automaticPaymentStatusService,
        [Service]
        IMarketplaceStripeBillingPortalService billingPortalService,
        CancellationToken cancellationToken)
    {
        try
        {
            var subscription = await marketplaceBookingSubscriptionService.GetByIdAsync(input.SubscriptionId, cancellationToken);
            if (!subscription.AutoRenew || subscription.MarketplaceBooking.PaymentMethod != PaymentMethod.Card)
            {
                return new MarketplaceBookingSubscriptionPayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "Automatic payment recovery is only available for card-backed auto-renewing purchases.",
                };
            }

            var automaticPaymentStatus = await automaticPaymentStatusService.GetForSubscriptionAsync(
                input.SubscriptionId, cancellationToken);
            if (automaticPaymentStatus is null || automaticPaymentStatus.Status is not
                    ("past_due" or "action_required" or "finalization_failed" or "incomplete"))
            {
                return new MarketplaceBookingSubscriptionPayload
                {
                    ClientMutationId = input.ClientMutationId,
                    Error = "Automatic payment recovery is not available for this subscription.",
                };
            }

            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                AutomaticPaymentRecoveryUrl = await billingPortalService.CreateForSubscriptionAsync(
                    input.SubscriptionId,
                    input.ReturnUrl,
                    cancellationToken),
            };
        }
        catch (InvalidOperationException exception)
        {
            return new MarketplaceBookingSubscriptionPayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = exception.Message,
            };
        }
    }

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
