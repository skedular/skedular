using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Services.Entitlements;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.Entitlement;

[MutationType]
public sealed class RootMutation
{
    [UseResolverScope]
    public async Task<CancelEntitlementPayload> CancelEntitlementAsync(
        CancelEntitlementInput input,
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        IEntitlementCancellationService entitlementCancellationService,
        [Service]
        IGraphQlMapper graphQlMapper,
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        try
        {
            var actorId = await cachedCustomerService.GetIdAsync(cancellationToken);
            await entitlementReadService.GetAuthorizedForCancellationAsync(input.EntitlementId, actorId, cancellationToken);
            var entitlement = await entitlementCancellationService.CancelEntitlementAsync(
                input.EntitlementId,
                input.Reason,
                cancellationToken);
            return new CancelEntitlementPayload
            {
                ClientMutationId = input.ClientMutationId,
                Entitlement = entitlement is null ? null : graphQlMapper.MapTo(entitlement),
                Error = entitlement is null ? "The entitlement was not found." : null,
            };
        }
        catch (Exception exception) when (exception is UnauthorizedAccessException or KeyNotFoundException or InvalidOperationException
                                              or ArgumentException)
        {
            return new CancelEntitlementPayload
            {
                ClientMutationId = input.ClientMutationId,
                Error = exception.Message,
            };
        }
    }
}
