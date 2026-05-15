using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Grpc.Core;
using OrganizationService = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService;

namespace Booking.Domain.FakeDependencies.Fakes;

public class FakeOrganizationGrpcService : OrganizationService.OrganizationServiceBase
{
}

public class FakeOrganizationBillingGrpcService(FakeOrganizationGrpcState state, TimeProvider timeProvider)
    : OrganizationBillingService.OrganizationBillingServiceBase
{
    public override Task<XeroConnection> Admin_GetXeroConnection(Admin_GetXeroConnectionInput request, ServerCallContext context)
    {
        state.GetXeroConnectionRequests.Enqueue(
            new RecordedGetXeroConnectionRequest(
                timeProvider.GetUtcNow(),
                request.OrganizationId,
                request.OrganizationCustomDomain));
        return Task.FromResult(state.XeroConnectionResponse);
    }

    public override Task<XeroConnection> Admin_RefreshXeroConnectionTokens(
        Admin_RefreshXeroConnectionTokensInput request,
        ServerCallContext context)
    {
        state.RefreshXeroConnectionTokens(
            request.OrganizationId,
            request.AccessTokenEncrypted,
            request.RefreshTokenEncrypted,
            request.AccessTokenExpiresAt,
            request.RefreshTokenExpiresAt);

        return Task.FromResult(state.XeroConnectionResponse);
    }
}
