# Payment Module — Agent Notes

## Purpose

Registers Stripe SDK service interfaces as singletons so they can be injected without
importing the Stripe SDK directly in domain code.

## Registration

```csharp
services.AddStripe(configuration);
```

**Config section key:** `Stripe` — see `Payment/Configurations/StripeConfiguration.cs`.

## What Gets Registered

`AddStripe` sets `Stripe.StripeConfiguration.ApiKey` globally and registers the following Stripe
service interfaces as singletons (implementation: the matching Stripe SDK service class):

| Stripe resource      | Interfaces registered                                                 |
| -------------------- | --------------------------------------------------------------------- |
| Customer             | `ICreatable`, `IUpdatable`, `IDeletable`, `IListable`, `IRetrievable` |
| Account              | `ICreatable`, `IUpdatable`, `IDeletable`, `IRetrievable`              |
| AccountLink          | `ICreatable`                                                          |
| PaymentIntent        | `ICreatable`                                                          |
| SetupIntent          | `ICreatable`, `IRetrievable`                                          |
| PaymentMethod        | `IRetrievable`                                                        |
| Product              | `ICreatable`, `IUpdatable`, `IDeletable`, `IRetrievable`, `IListable` |
| Price                | `ICreatable`, `IUpdatable`, `IRetrievable`                            |
| Session (Checkout)   | `ICreatable`, `IListable`, `IRetrievable`, `IUpdatable`               |
| OAuthToken           | `ICreatable`                                                          |
| PaymentMethodService | Direct singleton                                                      |

## Configuration Reference

```json
{
  "Stripe": {
    "PublishableKey": "pk_...",
    "SecretKey": "sk_...",
    "OrganizationPlatformAccountWebhookKey": "whsec_...",
    "OrganizationConnectAccountWebhookKey": "whsec_...",
    "BookingPlatformAccountWebhookKey": "whsec_...",
    "BookingConnectAccountWebhookKey": "whsec_...",
    "LogStripePlatformAccountWebhookMessages": false,
    "LogStripeConnectAccountWebhookMessages": false,
    "OAuthClientId": "ca_..."
  }
}
```

## Rules

- Do not access `Stripe.StripeConfiguration.ApiKey` directly in domain code — the key is set once
  at startup by `AddStripe`.
- Inject the narrowest Stripe interface your service needs (e.g. `ICreatable<Customer,...>`) rather
  than the concrete `CustomerService`, so tests can mock the Stripe boundary cleanly.
- Webhook signature verification must use the appropriate webhook key per endpoint (platform account
  vs. connect account); do not mix them.
