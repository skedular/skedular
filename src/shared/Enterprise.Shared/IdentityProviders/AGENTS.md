# IdentityProviders Module — Agent Notes

## Purpose

Owns provider-specific token validation implementations and identity-provider configuration for external auth systems
such as WorkOS, Cognito, Google, and Azure Entra.

## Registration

```csharp
builder.AddIdentityTokenProviders();
```

Provider services are registered only when their matching config sections are present.

## What Lives Here

- Provider-specific token services:
    - `Azure/` → `IAzureEntraTokenService`
    - `Cognito/` → `ICognitoTokenService`
    - `Google/` → `IGoogleTokenService`
    - `WorkOS/` → `IWorkOSTokenService`
- Shared identity-provider configuration in `Configurations/`
- WorkOS-specific support models such as `WorkOS/Profile`

## Relationship To Security

- Each provider implements `Enterprise.Shared.Security.Token.ITokenService`.
- `Security/` consumes these services via `IEnumerable<ITokenService>` but does not own the provider implementations
  anymore.

## Rules

- Keep provider implementation code here, not under `Security/`.
- Provider registration conditions belong in root `Enterprise.Shared/Extensions.cs`.
- If a provider needs extra configuration guards, add them in `AddIdentityTokenProviders()` and keep the guard aligned
  with the provider's actual runtime requirements.
- Never populate request identity from a parsed JWT until signature, issuer, lifetime, and applicable audience
  validation
  succeeds. `JsonWebTokenHandler.ValidateTokenAsync` returns a result; an awaited call alone does not mean validation
  passed.
- WorkOS AuthKit session JWTs do not contain `aud`. Validate them against the client-specific JWKS and configured issuer
  allowlist, including custom auth domains. Cognito and Azure tokens still require their configured audience.
- Log validation failures without credentials or token payloads, and propagate request cancellation.
