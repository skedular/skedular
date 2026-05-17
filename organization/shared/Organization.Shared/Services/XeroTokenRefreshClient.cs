using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Xero.NetStandard.OAuth2.Client;
using XeroOAuth2Token = Xero.NetStandard.OAuth2.Token.XeroOAuth2Token;

namespace Organization.Shared.Services;

public interface IXeroTokenRefreshClient
{
    Task<XeroTokenRefreshResult> RefreshAsync(string refreshTokenEncrypted, CancellationToken cancellationToken);
    DateTimeOffset GetNextMaintenanceAt(DateTimeOffset refreshTokenExpiresAt);
    DateTimeOffset GetRetryMaintenanceAt();
}

public class XeroTokenRefreshClient(
    XeroConfiguration xeroConfiguration,
    IXeroSdkClientFactory xeroSdkClientFactory,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    TimeProvider timeProvider) : IXeroTokenRefreshClient
{
    public async Task<XeroTokenRefreshResult> RefreshAsync(string refreshTokenEncrypted, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenEncrypted))
        {
            return new XeroTokenRefreshResult(false, true, null, null, null, null, "Xero refresh token is missing. Reconnect required.");
        }

        if (string.IsNullOrWhiteSpace(xeroConfiguration.ClientId) || string.IsNullOrWhiteSpace(xeroConfiguration.ClientSecret))
        {
            return new XeroTokenRefreshResult(false, false, null, null, null, null, "Xero client credentials are not configured.");
        }

        try
        {
            var client = xeroSdkClientFactory.CreateClient();
            var xeroToken = new XeroOAuth2Token { RefreshToken = xeroTokenEncryptionService.Decrypt(refreshTokenEncrypted) };
            var response = (XeroOAuth2Token)await client.RefreshAccessTokenAsync(xeroToken);

            var now = timeProvider.GetUtcNow();
            var rotatedRefreshToken = string.IsNullOrWhiteSpace(response.RefreshToken) ? xeroToken.RefreshToken : response.RefreshToken;
            var accessTokenExpiresAt = response.ExpiresAtUtc > now ? response.ExpiresAtUtc : now.AddMinutes(30);
            var refreshTokenExpiresAt = now.AddDays(60);

            return new XeroTokenRefreshResult(
                true,
                false,
                xeroTokenEncryptionService.Encrypt(response.AccessToken),
                xeroTokenEncryptionService.Encrypt(rotatedRefreshToken),
                accessTokenExpiresAt,
                refreshTokenExpiresAt,
                null);
        }
        catch (ApiException ex)
        {
            var errorContent = ex.ErrorContent?.ToString() ?? string.Empty;
            var needsReconnect =
                ex.ErrorCode is 400 or 401 &&
                (errorContent.Contains("invalid_grant", StringComparison.InvariantCultureIgnoreCase) ||
                 errorContent.Contains("invalid_token", StringComparison.InvariantCultureIgnoreCase) ||
                 errorContent.Contains("unauthorized_client", StringComparison.InvariantCultureIgnoreCase));

            return new XeroTokenRefreshResult(
                false,
                needsReconnect,
                null,
                null,
                null,
                null,
                $"Xero token refresh failed: {errorContent}");
        }
        catch (Exception ex)
        {
            return new XeroTokenRefreshResult(false, false, null, null, null, null, $"Xero token refresh failed: {ex.Message}");
        }
    }

    public DateTimeOffset GetNextMaintenanceAt(DateTimeOffset refreshTokenExpiresAt)
    {
        var leadTime = TimeSpan.FromDays(Math.Max(1, xeroConfiguration.RefreshBeforeExpiryDays));
        var nextMaintenanceAt = refreshTokenExpiresAt - leadTime;
        var minimumLead = timeProvider.GetUtcNow().AddHours(1);

        return nextMaintenanceAt > minimumLead ? nextMaintenanceAt : minimumLead;
    }

    public DateTimeOffset GetRetryMaintenanceAt() => timeProvider.GetUtcNow().AddHours(1);
}
