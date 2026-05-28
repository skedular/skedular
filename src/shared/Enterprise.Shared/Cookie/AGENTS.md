# Cookie Module — Agent Notes

## Purpose

Owns cookie-specific encryption concerns in `Enterprise.Shared`: cookie configuration binding and
the `ICookieEncryptionService` wrapper used by SSO and other cookie flows.

## Registration

```csharp
builder.AddCookieServices();
```

**Config section key:** `Cookie` — see `Cookie/Configurations/CookieConfiguration.cs`.

## What Lives Here

- `CookieConfiguration` in `Cookie/Configurations/`
- `ICookieEncryptionService`
- `CookieEncryptionService`

## Dependencies

- Uses `IStringEncryptionAlgorithm` from `Encryption/` for the low-level cipher.
- May be consumed by `Security/Sso/`, but it is not owned by `Security/`.

## Rules

- Keep cookie-specific encryption wiring here rather than under `Security/`.
- Do not reuse `ICookieEncryptionService` for Xero token encryption.
- If cookie encryption needs different configuration or lifecycle behaviour, change this module and the
  root `AddCookieServices()` composition point rather than hiding that logic in unrelated modules.
