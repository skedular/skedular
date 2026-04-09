# Security Module — Agent Notes

## Purpose

Provides multi-provider JWT/SAML token validation, cookie encryption, SAML SSO, and a gRPC
authenticator. Each identity provider is registered only when its configuration section is present,
so hosts that do not use a given provider pay no runtime cost.

## Sub-modules

| Sub-module      | Namespace                          | Entry point                                               |
|-----------------|------------------------------------|-----------------------------------------------------------|
| Core security   | `Enterprise.Shared.Security`       | `services.AddSecurity()` + `app.UseSecurity()`            |
| SSO (SAML)      | `Enterprise.Shared.Security.Sso`   | `services.AddSso()` + `app.UseSso()`                      |
| Token providers | `Enterprise.Shared.Security.Token` | Registered automatically by `AddIdentityTokenProviders()` |

## Registration

### Token providers (called from `AddIdentityTokenProviders()`)

```csharp
// In root Extensions.cs — each provider registered only when its config section exists
builder.AddIdentityTokenProviders();
```

Supported providers and their config keys:

| Provider       | Config section key          | Service registered                                      |
|----------------|-----------------------------|---------------------------------------------------------|
| WorkOS         | `IdentityProviders:WorkOS`  | `IWorkOSTokenService`                                   |
| Cognito        | `IdentityProviders:Cognito` | `ICognitoTokenService`                                  |
| Google         | `IdentityProviders:Google`  | `IGoogleTokenService`                                   |
| Azure Entra ID | `Azure:Entra`               | `IAzureEntraTokenService`, `IGraphServiceClientFactory` |

All providers implement `ITokenService`. `AddSecurity()` aggregates the registered ones into
`IEnumerable<ITokenService>` for multi-provider validation pipelines.

### Core security middleware

```csharp
services.AddSecurity();
app.UseSecurity();   // adds SecurityContextEnricherMiddleware
```

`SecurityContextEnricherMiddleware` validates the request token against all registered `ITokenService`
instances and enriches the request context with the resolved identity.

### Cookie encryption

```csharp
// Registered automatically when CookieConfiguration section is present
// Config section key: "Cookie"
```

`ICookieEncryptionService` uses `IStringEncryptionAlgorithm` internally. The encryption key comes from
`CookieConfiguration.EncryptionKey`.

### SAML SSO

```csharp
services.AddSso();
app.UseSso();   // adds SsoContextEnricherMiddleware
```

`ISamlAssertionConsumerService` validates SAML responses and extracts claims.
`ISamlLoginRequestFactory` creates SAML authentication requests.

## Encryption Boundary

- `IStringEncryptionAlgorithm` — low-level cipher shared by cookie encryption and Xero token encryption.
- `ICookieEncryptionService` — cookie-specific wrapper. Do not share with Xero token encryption.
- `IXeroTokenEncryptionService` lives in `Accounting/` with its own key configuration.

## gRPC Authentication

`IGrpcAuthenticator` (registered by `AddSecurity()`) verifies API key or bearer token metadata on
incoming gRPC calls. Use `GrpcExtensions.CreateMetadata(...)` to attach credentials on the client side.

## Configuration Reference

```json
{
  "IdentityProviders": {
    "WorkOS": { "ApiKey": "..." },
    "Cognito": { "JwksUri": "...", "Issuer": "..." },
    "Google": { "Issuer": "https://accounts.google.com", "ClientId": "..." }
  },
  "Azure": {
    "Entra": { "TenantId": "...", "Issuer": "..." }
  },
  "Cookie": {
    "EncryptionKey": { "Key": "...", "Iv": "..." }
  }
}
```

## Rules

- Do not reuse `ICookieEncryptionService` for Xero token encryption — they must stay separate.
- `ITokenService` implementations must be registered before `AddSecurity()` is called; the aggregation
  snapshot is built at startup.
- Do not bypass `SecurityContextEnricherMiddleware` by reading tokens manually in controllers.
