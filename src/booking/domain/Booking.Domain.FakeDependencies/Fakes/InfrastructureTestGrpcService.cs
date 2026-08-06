using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Google.Protobuf;
using Grpc.Core;

namespace Booking.Domain.FakeDependencies.Fakes;

public class InfrastructureTestGrpcService(
    FakeCoreGrpcState coreState,
    FakeOrganizationGrpcState organizationState) : InfrastructureTestService.InfrastructureTestServiceBase
{
    public override Task<ResetResponse> Reset(ResetInput request, ServerCallContext context)
    {
        coreState.Reset();
        organizationState.Reset();
        return Task.FromResult(new ResetResponse());
    }

    public override Task<ClearRecordedRequestsResponse> ClearRecordedRequests(
        ClearRecordedRequestsInput request,
        ServerCallContext context)
    {
        coreState.ClearRecordedRequests();
        organizationState.ClearRecordedRequests();
        organizationState.ClearRefreshTokenRequests();
        return Task.FromResult(new ClearRecordedRequestsResponse());
    }

    public override Task<ConfigureScenarioResponse> ConfigureScenario(
        ConfigureScenarioInput request,
        ServerCallContext context)
    {
        if (request.Core?.UploadToPrivateStorage is { } uploadScenario)
        {
            coreState.ConfigureUploadToPrivateStorageResponse(
                uploadScenario.UploadId,
                uploadScenario.Url,
                uploadScenario.ContentType,
                uploadScenario.Width,
                uploadScenario.Height);
        }

        if (request.Organization?.XeroConnection is { } xeroConnectionScenario)
        {
            organizationState.ConfigureXeroConnection(new XeroConnection
            {
                Id = xeroConnectionScenario.Id,
                TenantId = xeroConnectionScenario.TenantId,
                TenantName = xeroConnectionScenario.TenantName,
                BillingMode = xeroConnectionScenario.BillingMode,
                Scopes = xeroConnectionScenario.Scopes,
                IsActive = xeroConnectionScenario.IsActive,
                SendInvoicesViaXero = xeroConnectionScenario.SendInvoicesViaXero,
                AutoReconcilePayments = xeroConnectionScenario.AutoReconcilePayments,
                DefaultSalesAccountCode = xeroConnectionScenario.DefaultSalesAccountCode,
                DefaultReceivablesAccountCode = xeroConnectionScenario.DefaultReceivablesAccountCode,
                DefaultTrackingCategory1 = xeroConnectionScenario.DefaultTrackingCategory1,
                DefaultTrackingCategory2 = xeroConnectionScenario.DefaultTrackingCategory2,
                DefaultBrandingThemeId = xeroConnectionScenario.DefaultBrandingThemeId,
                DefaultReferencePrefix = xeroConnectionScenario.DefaultReferencePrefix,
                LastError = xeroConnectionScenario.LastError,
                HasAccessToken = xeroConnectionScenario.HasAccessToken,
                HasRefreshToken = xeroConnectionScenario.HasRefreshToken,
                AccessTokenEncrypted = xeroConnectionScenario.AccessTokenEncrypted,
                RefreshTokenEncrypted = xeroConnectionScenario.RefreshTokenEncrypted,
            });
        }

        return Task.FromResult(new ConfigureScenarioResponse());
    }

    public override Task<GetRecordedRequestsResponse> GetRecordedRequests(GetRecordedRequestsInput request, ServerCallContext context)
    {
        var coreRequests = coreState.SnapshotRecordedRequests(request.ClearAfterRead);
        var organizationRequests = organizationState.SnapshotRecordedRequests(request.ClearAfterRead);
        var organizationRefreshRequests = organizationState.SnapshotRefreshTokenRequests(request.ClearAfterRead);

        var response = new GetRecordedRequestsResponse();
        response.CoreUploadToPrivateStorageRequests.AddRange(
            coreRequests.Select(item => new CoreUploadToPrivateStorageRequest
            {
                RequestedAtUtc = item.RequestedAtUtc.ToString("O"),
                Extension = item.Extension ?? string.Empty,
                ContentType = item.ContentType ?? string.Empty,
                ContentLength = item.ContentLength,
                Content = ByteString.CopyFrom(item.Content),
            }));
        response.OrganizationGetXeroConnectionRequests.AddRange(
            organizationRequests.Select(item => new OrganizationGetXeroConnectionRequest
            {
                RequestedAtUtc = item.RequestedAtUtc.ToString("O"),
                OrganizationId = item.OrganizationId ?? string.Empty,
                OrganizationCustomDomain = item.OrganizationCustomDomain ?? string.Empty,
            }));
        response.OrganizationRefreshXeroConnectionTokensRequests.AddRange(
            organizationRefreshRequests.Select(item => new OrganizationRefreshXeroConnectionTokensRequest
            {
                RequestedAtUtc = item.RequestedAtUtc.ToString("O"),
                OrganizationId = item.OrganizationId ?? string.Empty,
                HasAccessTokenEncrypted = item.HasAccessTokenEncrypted,
                HasRefreshTokenEncrypted = item.HasRefreshTokenEncrypted,
                AccessTokenExpiresAtUtc = item.AccessTokenExpiresAtUtc?.ToString("O") ?? string.Empty,
                RefreshTokenExpiresAtUtc = item.RefreshTokenExpiresAtUtc?.ToString("O") ?? string.Empty,
            }));
        response.CoreUploadToPrivateStorageRequestCount = coreRequests.Count;
        response.OrganizationGetXeroConnectionRequestCount = organizationRequests.Count;
        response.OrganizationRefreshXeroConnectionTokensRequestCount = organizationRefreshRequests.Count;

        return Task.FromResult(response);
    }
}
