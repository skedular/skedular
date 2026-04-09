# Accounting Module — Agent Notes

## Purpose

Provides Xero OAuth2 SDK integration: client factory, token-at-rest encryption, and configuration
binding. This module owns accounting-level Xero concerns that are reusable across domains. Domain-
specific Xero behaviour (e.g. repeating invoice templates) belongs in the owning domain, not here.

## Registration

```csharp
services.AddXeroServices(configuration);
```

**Config section key:** `Xero` — see `Accounting/Configurations/XeroConfiguration.cs`.

## What Gets Registered

| Service                         | Description                                     |
|---------------------------------|-------------------------------------------------|
| `XeroConfiguration` (singleton) | Bound from `appsettings.json` under `"Xero"`    |
| `IXeroSdkClientFactory`         | Creates authenticated Xero API client instances |
| `IXeroTokenEncryptionService`   | Encrypts/decrypts Xero OAuth2 tokens at rest    |

## Encryption Boundary

`IXeroTokenEncryptionService` uses `IStringEncryptionAlgorithm` internally but is configured with its
own key from `XeroConfiguration.EncryptionKey`. Do **not** share keys with `ICookieEncryptionService`.

## Configuration Reference

```json
{
  "Xero": {
    "ClientId": "...",
    "ClientSecret": "...",
    "AuthorizeEndpoint": "https://login.xero.com/identity/connect/authorize",
    "TokenEndpoint": "https://identity.xero.com/connect/token",
    "WebhookKey": "...",
    "LogWebhookMessages": false,
    "Scopes": "openid profile email accounting.transactions",
    "RefreshBeforeExpiryDays": 7,
    "EncryptionKey": { "Key": "...", "Iv": "..." }
  }
}
```

## Rules

- Xero configuration and service registration must go through `AddXeroServices` — do not duplicate
  `XeroConfiguration` binding in individual domain projects.
- Organization owns the org-facing Xero connection state and billing-mode selection.
- Booking owns the downstream invoice-export behaviour (repeating invoices, credit notes).
- Do not infer refund eligibility from Xero invoice state; decide locally first, then mirror to Xero.
- If Xero cancellation fails during a local cancellation, mark the export `TransitionRequired` and
  keep the local state authoritative rather than failing the whole operation.
