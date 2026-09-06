# Data Model: Automatic Marketplace Renewal Charging

## Rules

- Booking owns all local renewal/payment/fallback records and their lifecycle events.
- Stripe identifiers are references only. No card number, CVC, client secret, or full provider payload is persisted.
- A purchase-specific authorization is not the Customer domain’s profile payment method.
- Amount, currency, tax, billing mode, contractual term, and purchase-specific values are frozen when an attempt starts.
- A renewal is paid only after provider confirmation has been reconciled.
- Local entity references use explicit EF Core foreign keys; migrations are forward-only.

## PurchaseRenewalAuthorization

One current authorization per purchase and Stripe account context, with historical replacements retained through lifecycle events.

| Field | Required | Meaning |
|---|---:|---|
| Id | Yes | Local authorization ID. |
| SourceType / SourceId | Yes | Extensible AutoRenew purchase source. |
| StripeAccountId | Conditional | Account context for credential and charge. |
| StripeCustomerId | Conditional | Technical Stripe customer reference, never a link to profile payment-method selection. |
| StripePaymentCredentialId | Yes | Stripe-side credential reference scoped to this purchase. |
| ConsentStatus | Yes | Active, Revoked, Missing, or Invalid. |
| ConsentCapturedAt / ConsentTextVersion | Conditional | Evidence of explicit consent. |
| UsabilityStatus | Yes | Usable, Expired, Detached, Unusable, or Unknown. |
| RevokedAt | Conditional | Authorization invalidation. |
| CreatedAt / UpdatedAt | Yes | Audit times. |

Uniqueness: one active `(SourceType, SourceId, StripeAccountId)` authorization. A profile-card-only record cannot satisfy this constraint.

## MarketplaceRenewalCycle

Immutable commercial instance of a single renewal boundary.

| Field | Required | Meaning |
|---|---:|---|
| Id | Yes | Renewal ID and idempotency root. |
| SourceType / SourceId | Yes | Purchase/subscription to renew. |
| ProductVersionId / PricingId | Yes | Pricing selected at renewal evaluation. |
| RenewalAt / From / Until | Yes | Renewal boundary and contractual window. |
| Amount / Currency / TaxAmount / TaxBehavior | Yes* | Frozen commercial values. |
| BillingMode | Yes | Upfront, arrears, or supported mode. |
| MembershipTerm | Conditional | Reservation/subscription term. |
| EntitlementQuantity / CreditValidityFrom / CreditValidityUntil | Conditional | Entitlement-specific renewal values. |
| Status | Yes | Scheduled, PaymentPending, ActionRequired, Paid, Failed, FallbackRequired, ManuallyRecovered, Cancelled, or Expired. |
| Transition timestamps | Conditional | PaidAt, FailedAt, CancelledAt, ExpiredAt. |

`*` Tax amount can be zero; tax behavior remains explicit.
Uniqueness: `(SourceType, SourceId, RenewalAt)` is unique; only one cycle can transition to Paid for that boundary.

## MarketplaceRenewalPaymentAttempt

| Field | Required | Meaning |
|---|---:|---|
| Id / RenewalCycleId | Yes | Local attempt and owning cycle. |
| AttemptNumber | Yes | Monotonic; automatic attempts 1–3 only. |
| AttemptKind | Yes | AutomaticOffSession or FallbackCheckout. |
| IdempotencyKey | Yes | Stable provider request identity. |
| Amount / Currency | Yes | Exact attempted values. |
| StripePaymentIntentId / StripeCheckoutSessionId | Conditional | Provider references. |
| StripeAccountId | Conditional | Provider account context. |
| Status | Yes | Created, Processing, Succeeded, Failed, Cancelled, ActionRequired, or Unknown. |
| FailureCategory / NextAction | Conditional | Safe user/operator recovery information. |
| RetryAt / ConfirmedAt | Conditional | Scheduling and confirmed outcome. |

Uniqueness: `(RenewalCycleId, AttemptNumber)`, idempotency key, and provider identifier/account pairs. Retries reuse the same attempt identity where Stripe supports it.

## MarketplaceRenewalFallback

| Field | Required | Meaning |
|---|---:|---|
| Id / RenewalCycleId / PaymentAttemptId | Yes | Recovery identity and ownership. |
| CheckoutUrl | Yes | Hosted action; URL alone is never paid proof. |
| CreatedAt / ExpiresAt | Yes | Link expires after three days. |
| NotificationStatus | Yes | Pending, Sent, Failed, or NotRequired. |
| CompletedAt | Conditional | Confirmed fallback completion. |

## MarketplaceRenewalLifecycleEvent

Append-only history source for UI and recovery.

| Field | Required | Meaning |
|---|---:|---|
| EventId / OccurredAt | Yes | Event identity and actual occurrence time. |
| SourceType / SourceId | Yes | Purchase source. |
| RenewalCycleId / PaymentAttemptId | Conditional | Related state. |
| EventType | Yes | Authorization, renewal, payment, fallback, materialization, refund, or recovery event. |
| Amount / Currency | Conditional | Historical values. |
| ProviderEventId / IdempotencyKey | Conditional / Yes | Reconciliation and deduplication. |
| Metadata | Conditional | Non-sensitive diagnostics. |

Event families: AuthorizationCaptured/Revoked; RenewalScheduled/Paid/Failed/Cancelled/Expired; PaymentAttempted/Processing/Succeeded/Failed/Cancelled/AuthenticationRequired; FallbackCreated/Expired; ReservationCycleCreated; EntitlementCreated; RefundRequested/Completed/Failed; ManualRecovery.

## State transitions

```text
Scheduled -> PaymentPending -> Processing -> Paid
PaymentPending -> ActionRequired -> FallbackRequired -> Paid
PaymentPending -> Failed -> PaymentPending  (at most three automatic attempts)
Failed -> FallbackRequired -> ManuallyRecovered or Paid
any unpaid state -> Cancelled or Expired
```

- `Paid` atomically gates exactly-one reservation materialization or entitlement/credit allocation.
- `ActionRequired`, `Failed`, `FallbackRequired`, and `Expired` never grant new access/credits.
- AutoRenew disabled prevents future cycles only; it does not rewrite completed history.
- Cancellation and refund are distinct state machines.
