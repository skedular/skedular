# Security Module — Agent Notes

## Purpose

Provides the security pipeline surface: request token validation middleware, SAML SSO, shared token
contracts, and a gRPC authenticator. Provider implementations and cookie encryption now live in
their own sibling modules and are composed into this pipeline by the root `Enterprise.Shared`
extensions.

## Sub-modules

| Sub-module      | Namespace                          | Entry point                                    |
|-----------------|------------------------------------|------------------------------------------------|
| Core security   | `Enterprise.Shared.Security`       | `services.AddSecurity()` + `app.UseSecurity()` |
| SSO (SAML)      | `Enterprise.Shared.Security.Sso`   | `services.AddSso()` + `app.UseSso()`           |
| Token contracts | `Enterprise.Shared.Security.Token` | Consumed by registered token-provider modules  |

## Registration

### Token providers (called from root `AddIdentityTokenProviders()`)

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

All providers implement `ITokenService`. Root `AddIdentityTokenProviders()` aggregates the
registered ones into `IEnumerable<ITokenService>` for multi-provider validation pipelines.

### Core security middleware

```csharp
services.AddSecurity();
app.UseSecurity();   // adds SecurityContextEnricherMiddleware
```

`SecurityContextEnricherMiddleware` validates the request token against all registered `ITokenService`
instances and enriches the request context with the resolved identity.

### Cookie encryption

```csharp
// Registered separately by the root Extensions.cs helper
builder.AddCookieServices();
```

`ICookieEncryptionService` no longer lives under `Security/`. It is owned by the `Cookie/` module and
uses `IStringEncryptionAlgorithm` from `Encryption/`.

### SAML SSO

```csharp
services.AddSso();
app.UseSso();   // adds SsoContextEnricherMiddleware
```

`ISamlAssertionConsumerService` validates SAML responses and extracts claims.
`ISamlLoginRequestFactory` creates SAML authentication requests.

## Encryption Boundary

- `IStringEncryptionAlgorithm` lives under `Encryption/` and is the low-level cipher shared by cookie
  encryption and Xero token encryption.
- `ICookieEncryptionService` lives under `Cookie/` and remains the cookie-specific wrapper. Do not
  share it with Xero token encryption.
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
- `ITokenService` implementations must be registered before the app starts serving requests; root
  `AddIdentityTokenProviders()` builds the aggregated `IEnumerable<ITokenService>` at startup.
- Do not add cookie-encryption registration logic back into `Security/Extensions.cs`; keep that split in
  the root `Enterprise.Shared/Extensions.cs` composition layer.
- Do not bypass `SecurityContextEnricherMiddleware` by reading tokens manually in controllers.
