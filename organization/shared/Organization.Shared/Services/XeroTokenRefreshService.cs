using Enterprise.Shared.Accounting;
using Organization.Shared.Database.Entities;

namespace Organization.Shared.Services;

public interface IXeroTokenRefreshService
{
    Task<XeroTokenRefreshResult> RefreshAsync(OrganizationXeroConnection connection, CancellationToken cancellationToken);
    DateTimeOffset GetNextMaintenanceAt(DateTimeOffset refreshTokenExpiresAt);
    DateTimeOffset GetRetryMaintenanceAt();
}

public class XeroTokenRefreshService(IXeroTokenRefreshClient xeroTokenRefreshClient) : IXeroTokenRefreshService
{
    public Task<XeroTokenRefreshResult> RefreshAsync(OrganizationXeroConnection connection, CancellationToken cancellationToken) =>
        xeroTokenRefreshClient.RefreshAsync(connection.RefreshTokenEncrypted ?? string.Empty, cancellationToken);

    public DateTimeOffset GetNextMaintenanceAt(DateTimeOffset refreshTokenExpiresAt) =>
        xeroTokenRefreshClient.GetNextMaintenanceAt(refreshTokenExpiresAt);

    public DateTimeOffset GetRetryMaintenanceAt() => xeroTokenRefreshClient.GetRetryMaintenanceAt();
}
