using Booking.Api.Mappers;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Booking;

[MutationType]
public class MarketplaceRefundRootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> MarkMarketplaceRefundPendingAccountingAsync(
        MarkMarketplaceRefundPendingAccountingInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.MarkPendingAccountingAsync(
            input.Id,
            input.RefundAmount,
            input.Reason,
            cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> CompleteMarketplaceRefundAsync(
        CompleteMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.CompleteAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> MarkMarketplaceRefundManualRequiredAsync(
        MarkMarketplaceRefundManualRequiredInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.MarkManualRequiredAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> MarkMarketplaceRefundManualCompletedAsync(
        MarkMarketplaceRefundManualCompletedInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.MarkManualCompletedAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> FailMarketplaceRefundAsync(
        FailMarketplaceRefundInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.FailAsync(input.Id, input.Reason, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }

    [UseResolverScope]
    public async Task<MarketplaceRefundPayload> ProcessMarketplaceRefundInXeroAsync(
        ProcessMarketplaceRefundInXeroInput input,
        [Service] IMarketplaceRefundAdminService marketplaceRefundAdminService,
        CancellationToken cancellationToken)
    {
        var refund = await marketplaceRefundAdminService.ProcessInXeroAsync(input.Id, cancellationToken);
        return new MarketplaceRefundPayload { ClientMutationId = input.ClientMutationId, MarketplaceRefund = graphQlMapper.MapTo(refund) };
    }
}
