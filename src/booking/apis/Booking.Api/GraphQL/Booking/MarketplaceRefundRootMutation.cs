using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class MarketplaceRefundRootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> ApproveMarketplaceRefundAsync(
        ApproveMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.ApproveAsync(input.Id, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> RejectMarketplaceRefundAsync(
        RejectMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.RejectAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> RecordBankTransferRefundSentAsync(
        RecordBankTransferRefundSentInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.RecordBankTransferSentAsync(input.Id, input.BankTransferReference, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> ConfirmBankTransferRefundReceivedAsync(
        ConfirmBankTransferRefundReceivedInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.ConfirmBankTransferReceivedAsync(input.Id, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> RetryMarketplaceRefundAsync(
        RetryMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.RetryAsync(input.Id, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> ResolveRefundReconciliationRequiredAsync(
        ResolveRefundReconciliationRequiredInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.ResolveReconciliationAsync(input.Id, input.Completed, input.Reason, input.ProviderReference, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> CancelMarketplaceRefundAsync(
        CancelMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.CancelAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> CreatePartialMarketplaceRefundAsync(
        CreatePartialMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService service,
        CancellationToken cancellationToken)
    {
        var refund = await service.CreatePartialAsync(input.AllocationId, input.Amount, input.Reason, input.IdempotencyKey, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceExternalRefundReconciliationPayload> ResolveMarketplaceExternalRefundReconciliationAsync(
        ResolveMarketplaceExternalRefundReconciliationInput input,
        [Service] IMarketplaceRefundOperationsService service,
        [Service] IOrganizationAuthorizationService organizationAuthorizationService,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IContext context,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(input.OrganizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var reconciliation = await service.ResolveExternalRefundAsync(
            input.Provider,
            input.ExternalRefundId,
            input.Status,
            input.Reason,
            input.OrganizationId,
            customerId,
            context.GetCorrelationId(),
            cancellationToken);
        return new MarketplaceExternalRefundReconciliationPayload
        {
            ClientMutationId = input.ClientMutationId,
            Reconciliation = new MarketplaceExternalRefundReconciliationDetails
            {
                Id = reconciliation.Id,
                Provider = reconciliation.Provider.ToString(),
                ExternalRefundId = reconciliation.ExternalRefundId,
                Amount = reconciliation.Amount,
                Currency = reconciliation.Currency?.ToString().ToLowerInvariant(),
                Status = reconciliation.Status.ToString(),
                FirstSeenAt = reconciliation.FirstSeenAt,
                LastSeenAt = reconciliation.LastSeenAt,
                ResolutionReason = reconciliation.ResolutionReason,
                ResolutionActorCustomerId = reconciliation.ResolutionActorCustomerId,
                ResolutionCorrelationId = reconciliation.ResolutionCorrelationId
            }
        };
    }

    [UseResolverScope]
    public async Task<MarketplaceExternalRefundReconciliationPayload> ResolveUnassignedMarketplaceExternalRefundReconciliationAsync(
        ResolveUnassignedMarketplaceExternalRefundReconciliationInput input,
        [Service] IMarketplaceRefundOperationsService service,
        [Service] IPlatformOperationsAuthorizationService authorizationService,
        [Service] ICachedCustomerService cachedCustomerService,
        [Service] IContext context,
        CancellationToken cancellationToken)
    {
        if (!authorizationService.IsAuthorized())
        {
            throw new UnauthorizedAccessException();
        }

        var reconciliation = await service.ResolveUnassignedExternalRefundAsync(
            input.Provider,
            input.ExternalRefundId,
            input.Status,
            input.Reason,
            await cachedCustomerService.GetIdAsync(cancellationToken),
            context.GetCorrelationId(),
            cancellationToken);
        return new MarketplaceExternalRefundReconciliationPayload
        {
            ClientMutationId = input.ClientMutationId,
            Reconciliation = new MarketplaceExternalRefundReconciliationDetails
            {
                Id = reconciliation.Id,
                Provider = reconciliation.Provider.ToString(),
                ExternalRefundId = reconciliation.ExternalRefundId,
                Amount = reconciliation.Amount,
                Currency = reconciliation.Currency?.ToString().ToLowerInvariant(),
                Status = reconciliation.Status.ToString(),
                FirstSeenAt = reconciliation.FirstSeenAt,
                LastSeenAt = reconciliation.LastSeenAt,
                ResolutionReason = reconciliation.ResolutionReason,
                ResolutionActorCustomerId = reconciliation.ResolutionActorCustomerId,
                ResolutionCorrelationId = reconciliation.ResolutionCorrelationId
            }
        };
    }
}
