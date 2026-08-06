using System.Collections.Concurrent;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Google.Protobuf.WellKnownTypes;

namespace Booking.Domain.FakeDependencies.Fakes;

public class FakeOrganizationGrpcState
{
    public ConcurrentQueue<RecordedGetXeroConnectionRequest> GetXeroConnectionRequests { get; } = new();
    public ConcurrentQueue<RecordedRefreshXeroConnectionTokensRequest> RefreshXeroConnectionTokensRequests { get; } = new();

    public XeroConnection XeroConnectionResponse { get; private set; } = new();

    public void ConfigureXeroConnection(XeroConnection response) =>
        XeroConnectionResponse = response;

    public void RefreshXeroConnectionTokens(
        string? organizationId,
        string? accessTokenEncrypted,
        string? refreshTokenEncrypted,
        Timestamp? accessTokenExpiresAt,
        Timestamp? refreshTokenExpiresAt)
    {
        RefreshXeroConnectionTokensRequests.Enqueue(
            new RecordedRefreshXeroConnectionTokensRequest(
                TimeProvider.System.GetUtcNow(),
                organizationId,
                !string.IsNullOrWhiteSpace(accessTokenEncrypted),
                !string.IsNullOrWhiteSpace(refreshTokenEncrypted),
                accessTokenExpiresAt?.ToDateTimeOffset(),
                refreshTokenExpiresAt?.ToDateTimeOffset()));

        XeroConnectionResponse = new XeroConnection
        {
            Id = XeroConnectionResponse.Id,
            TenantId = XeroConnectionResponse.TenantId,
            TenantName = XeroConnectionResponse.TenantName,
            BillingMode = XeroConnectionResponse.BillingMode,
            Scopes = XeroConnectionResponse.Scopes,
            IsActive = XeroConnectionResponse.IsActive,
            SendInvoicesViaXero = XeroConnectionResponse.SendInvoicesViaXero,
            AutoReconcilePayments = XeroConnectionResponse.AutoReconcilePayments,
            DefaultSalesAccountCode = XeroConnectionResponse.DefaultSalesAccountCode,
            DefaultReceivablesAccountCode = XeroConnectionResponse.DefaultReceivablesAccountCode,
            DefaultTrackingCategory1 = XeroConnectionResponse.DefaultTrackingCategory1,
            DefaultTrackingCategory2 = XeroConnectionResponse.DefaultTrackingCategory2,
            DefaultBrandingThemeId = XeroConnectionResponse.DefaultBrandingThemeId,
            DefaultReferencePrefix = XeroConnectionResponse.DefaultReferencePrefix,
            LastError = XeroConnectionResponse.LastError,
            HasAccessToken = !string.IsNullOrWhiteSpace(accessTokenEncrypted),
            HasRefreshToken = !string.IsNullOrWhiteSpace(refreshTokenEncrypted),
            AccessTokenEncrypted = accessTokenEncrypted ?? string.Empty,
            RefreshTokenEncrypted = refreshTokenEncrypted ?? string.Empty,
            AccessTokenExpiresAt = accessTokenExpiresAt ?? XeroConnectionResponse.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = refreshTokenExpiresAt ?? XeroConnectionResponse.RefreshTokenExpiresAt,
            LastSuccessfulSyncAt = XeroConnectionResponse.LastSuccessfulSyncAt,
        };
    }

    public IReadOnlyCollection<RecordedGetXeroConnectionRequest> SnapshotRecordedRequests(bool clearAfterRead)
    {
        var items = GetXeroConnectionRequests.ToArray();
        if (clearAfterRead)
        {
            ClearRecordedRequests();
        }

        return items;
    }

    public IReadOnlyCollection<RecordedRefreshXeroConnectionTokensRequest> SnapshotRefreshTokenRequests(bool clearAfterRead)
    {
        var items = RefreshXeroConnectionTokensRequests.ToArray();
        if (clearAfterRead)
        {
            ClearRefreshTokenRequests();
        }

        return items;
    }

    public void ClearRecordedRequests()
    {
        while (GetXeroConnectionRequests.TryDequeue(out _))
        {
        }
    }

    public void ClearRefreshTokenRequests()
    {
        while (RefreshXeroConnectionTokensRequests.TryDequeue(out _))
        {
        }
    }

    public void Reset()
    {
        ClearRecordedRequests();
        ClearRefreshTokenRequests();
        XeroConnectionResponse = new XeroConnection();
    }
}

public record RecordedGetXeroConnectionRequest(
    DateTimeOffset RequestedAtUtc,
    string? OrganizationId,
    string? OrganizationCustomDomain);

public record RecordedRefreshXeroConnectionTokensRequest(
    DateTimeOffset RequestedAtUtc,
    string? OrganizationId,
    bool HasAccessTokenEncrypted,
    bool HasRefreshTokenEncrypted,
    DateTimeOffset? AccessTokenExpiresAtUtc,
    DateTimeOffset? RefreshTokenExpiresAtUtc);
