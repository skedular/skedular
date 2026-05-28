namespace Enterprise.Shared.Accounting;

public record XeroTokenRefreshResult(
    bool IsSuccessful,
    bool NeedsReconnect,
    string? AccessTokenEncrypted,
    string? RefreshTokenEncrypted,
    DateTimeOffset? AccessTokenExpiresAt,
    DateTimeOffset? RefreshTokenExpiresAt,
    string? Error);
