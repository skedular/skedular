# Data Model: End-to-End Refund Reliability

**Phase**: 1 — Design
**Date**: 2026-07-25
**References**: [spec.md](spec.md) | [research.md](research.md)

---

## Overview

All changes extend the existing `Booking` domain in `src/booking/shared/Booking.Shared/Database/`. No new domain or service boundary is introduced. A new EF migration is required.

---

## 1. `MarketplaceRefund` (Extended)

**Location**: `Booking.Shared/Database/Entities/MarketplaceRefund.cs`

**Existing fields** (unchanged):

- `Id`, `LocalEntityType`, `LocalEntityId`, `Status`, `RequestedAt`, `ReferenceTime`
- `RefundPercentage`, `AppliedRuleMinutesBefore`, `BaseAmount`, `RefundAmount`, `Currency`
- `Reason`, `AccountingProvider`, `ExternalRefundId`, `ExternalRefundNumber`
- `LastProcessedAt`, `LastError`, `PaymentProvider`, `ExternalPaymentRefundId`
- `PaymentRefundStatus`, `PaymentRefundLastProcessedAt`, `PaymentRefundLastError`
- `OrganizationId`, `Organization`, `RequestedByCustomerId`, `RequestedByCustomer`
- `Events` (collection of `MarketplaceRefundEvent`)
- `CreatedAt`, `ModifiedAt` (from `EntityBase`)

**New fields** (added by this feature):

| Field                   | Type              | Nullable           | Constraint            | Purpose                                                                     |
| ----------------------- | ----------------- | ------------------ | --------------------- | --------------------------------------------------------------------------- |
| `IdempotencyKey`        | `string`          | No                 | Unique index, max 128 | Stable key for provider dedup and DB uniqueness guard                       |
| `ApprovedByCustomerId`  | `string?`         | Yes                | FK → Customer         | Actor who approved the refund (bank transfer / discretionary)               |
| `ApprovedAt`            | `DateTimeOffset?` | Yes                | —                     | When the refund was approved                                                |
| `RejectedByCustomerId`  | `string?`         | Yes                | FK → Customer         | Actor who rejected the refund                                               |
| `RejectedAt`            | `DateTimeOffset?` | Yes                | —                     | When the refund was rejected                                                |
| `RejectionReason`       | `string?`         | Yes                | max 1000              | Reason for rejection                                                        |
| `CancelledAt`           | `DateTimeOffset?` | Yes                | —                     | When the refund was cancelled                                               |
| `BankTransferReference` | `string?`         | Yes                | max 256               | Reference number recorded by admin for bank transfer                        |
| `BankTransferSentAt`    | `DateTimeOffset?` | Yes                | —                     | Date admin marked bank transfer as sent                                     |
| `PostPayoutRefund`      | `bool`            | No (default false) | —                     | Whether refund was processed after Stripe payout disbursement               |
| `PolicySnapshotJson`    | `string?`         | Yes                | —                     | JSON snapshot of `ProductPricingCancellationPolicy` at refund creation time |
| `RetryCount`            | `int`             | No (default 0)     | —                     | Number of provider submission retries                                       |
| `ReconciliationStatus`  | `string?`         | Yes                | max 50                | Last reconciliation outcome: Matched, Mismatch, NotFound                    |
| `LastReconciledAt`      | `DateTimeOffset?` | Yes                | —                     | Timestamp of last reconciliation check                                      |
| `StripeRefundPath`       | `string?`         | Yes                | max 64                | Selected path: TransferReversal or PlatformFunded                           |
| `StripeAccountId`        | `string?`         | Yes                | max 128               | Stripe account used for the refund                                          |
| `StripeChargeType`       | `string?`         | Yes                | max 64                | Original charge context: Direct, Destination, or Platform                  |
| `StripeTransferId`       | `string?`         | Yes                | max 128               | Original transfer identifier used for reversal                              |
| `StripeRefundPathSelectedAt` | `DateTimeOffset?` | Yes            | —                     | Timestamp when the provider refund path was selected                       |

**New DB index**:

- Replace the existing unique `(OrganizationId, LocalEntityType, LocalEntityId)` index. It prevents valid multiple partial and modification refunds and MUST be removed by the migration.
- Unique partial index on `(LocalEntityType, LocalEntityId, RefundKind)` where `RefundKind = 'Cancellation'` and `Status NOT IN ('Completed', 'Failed', 'Rejected', 'Cancelled')` — prevents duplicate in-flight cancellation refunds while allowing multiple partial or modification refunds.
- Source-payment allocation rows are locked/versioned while their remaining refundable balance is calculated and reserved, preventing concurrent partial refunds from exceeding a captured payment.

**State machine** (`MarketplaceRefundStatusConstants`):

```
Requested → Processing               (automatic: Stripe)
Requested → UnderReview              (manual review path: bank transfer, discretionary)
UnderReview → Approved
UnderReview → Rejected               (terminal)
Approved → Processing
Processing → ProviderPending         (Stripe pending/requires_action)
Processing → Completed               (immediate success)
Processing → Failed
Processing → ReconciliationRequired  (payout/provider mismatch)
ProviderPending → Completed          (webhook: succeeded)
ProviderPending → Failed             (webhook: failed/canceled)
ProviderPending → ReconciliationRequired
Failed → Processing                  (retry)
Failed → ReconciliationRequired      (payout/provider mismatch)
ReconciliationRequired → Completed   (human confirmed)
ReconciliationRequired → Failed      (human confirmed)
Requested → Cancelled
UnderReview → Cancelled
Approved → Cancelled
```

`Processing` and later provider-submitted states cannot be cancelled locally. They must finish through provider confirmation or reconciliation.

---

## 2. `MarketplaceRefundEvent` (Extended)

**Location**: `Booking.Shared/Database/Entities/MarketplaceRefundEvent.cs`

**Existing fields** (unchanged):

- `Id`, `EventType`, `OccurredAt`, `RefundAmount`, `Reason`
- `AccountingProvider`, `ExternalRefundId`, `ExternalRefundNumber`, `LastError`
- `MarketplaceRefundId`, `MarketplaceRefund`, `ActorCustomerId`, `ActorCustomer`

**New fields**:

| Field            | Type      | Nullable | Purpose                                                    |
| ---------------- | --------- | -------- | ---------------------------------------------------------- |
| `PreviousStatus` | `string?` | Yes      | Status before this transition (enables before/after audit) |
| `NewStatus`      | `string?` | Yes      | Status after this transition                               |
| `CorrelationId`  | `string?` | Yes      | Request/workflow correlation ID for log tracing            |

---

## 3. `MarketplaceRefundStatusConstants` (Extended)

**Location**: `Booking.Shared/Models/MarketplaceRefundStatusConstants.cs`

**Existing** (kept):

```csharp
Requested, UnderReview, Approved, Rejected, Processing, ProviderPending, Completed, Failed, Cancelled, ReconciliationRequired
```

**New constants added**:

```csharp
UnderReview        // Awaiting administrator review
Approved           // Administrator approved; pending provider submission
Rejected           // Administrator rejected; terminal
ProviderPending    // Provider accepted but not yet completed (Stripe pending/requires_action)
Cancelled          // Cancelled before provider submission; terminal
ReconciliationRequired  // Local and provider state mismatch; requires human resolution
```

## 4. `CancellationPolicySnapshot` (New value object)

**Location**: `Booking.Shared/Models/CancellationPolicySnapshot.cs`

Stored as serialized JSON in `MarketplaceRefund.PolicySnapshotJson` and captured from `ProductPricing` at booking confirmation time.

**Fields**:

| Field            | Type                                            | Notes                                                             |
| ---------------- | ----------------------------------------------- | ----------------------------------------------------------------- |
| `PolicyType`     | `string`                                        | Enum string: NoCancellation, FullRefundBeforeCutoff, TieredRefund |
| `Rules`          | `IReadOnlyList<CancellationRefundRuleSnapshot>` | Ordered rules                                                     |
| `CapturedAt`     | `DateTimeOffset`                                | When snapshot was taken                                           |
| `ProductPriceId` | `string`                                        | Which product price version this snapshot came from               |

**`CancellationRefundRuleSnapshot`**:

| Field              | Type  |
| ------------------ | ----- |
| `MinutesBefore`    | `int` |
| `RefundPercentage` | `int` |

---

## 5. `MarketplaceRefundCalculationResult` (New value object)

**Location**: `Booking.Shared/Models/MarketplaceRefundCalculationResult.cs`

Returned by the refund calculation service and persisted as JSON if needed for audit.

| Field                      | Type                         | Notes                                                |
| -------------------------- | ---------------------------- | ---------------------------------------------------- |
| `OriginalGrossAmount`      | `decimal`                    | Total originally paid                                |
| `EligibleRefundAmount`     | `decimal`                    | Amount eligible before deductions                    |
| `CancellationDeduction`    | `decimal`                    | Amount withheld per policy                           |
| `TaxAdjustment`            | `decimal`                    | Tax reversal or retention derived from purchase data |
| `PreviouslyRefundedAmount` | `decimal`                    | Sum of prior completed/pending refunds               |
| `FinalRefundableAmount`    | `decimal`                    | What will be refunded                                |
| `NonRefundableAmount`      | `decimal`                    | What is kept                                         |
| `CalculationReason`        | `string`                     | Human-readable explanation                           |
| `PolicySnapshotUsed`       | `CancellationPolicySnapshot` | Snapshot used in this calculation                    |
| `CalculatedAt`             | `DateTimeOffset`             | When calculation was performed                       |
| `CancellationTime`         | `DateTimeOffset`             | Cancellation request time used as input              |
| `ReferenceTime`            | `DateTimeOffset`             | Booking start / renewal reference time used as input |
| `TimezoneId`               | `string`                     | Purchase-time location or organization timezone      |

## 5a. `MarketplaceRefundPaymentAllocation` (New entity)

**Location**: `Booking.Shared/Database/Entities/MarketplaceRefundPaymentAllocation.cs`

Each refund has one or more allocation rows. A row ties an allocated refund amount to one immutable source payment or invoice and is the accounting boundary for caps and concurrency.

| Field | Type | Constraint | Purpose |
| --- | --- | --- | --- |
| `MarketplaceRefundId` | `string` | FK, required | Parent refund |
| `SourcePaymentProvider` | `string` | required | Stripe, Xero, bank transfer, or another provider |
| `SourcePaymentReference` | `string` | required, indexed | Original payment-intent, charge, invoice, or transfer reference |
| `SourcePaymentAmount` | `decimal` | required | Captured amount available on this source payment |
| `AllocatedRefundAmount` | `decimal` | required | Amount this refund returns from this source payment |
| `Currency` | `string` | required | Source-payment currency |
| `ConcurrencyVersion` | `long` | required | Optimistic concurrency/versioning boundary |

An allocation may have optional child occurrence/tax/fee breakdown records when a refund needs to distinguish delivered and undelivered service.

## 5b. `MarketplaceExternalRefundReconciliation` (New entity)

**Location**: `Booking.Shared/Database/Entities/MarketplaceExternalRefundReconciliation.cs`

Represents a provider refund discovered without a matching local refund. It is operator-visible and must not automatically change local financial state.

| Field | Type | Constraint | Purpose |
| --- | --- | --- | --- |
| `Id` | `string` | Required, unique | Local reconciliation record identifier |
| `Provider` | `string` | Required | Stripe or Xero |
| `ExternalRefundId` | `string` | Required, indexed | Provider refund identifier |
| `Amount` | `decimal?` | Nullable | Provider-reported amount |
| `Currency` | `string?` | Nullable | Provider-reported currency |
| `Status` | `string` | Required | Open, Linked, Dismissed, or Escalated |
| `FirstSeenAt` / `LastSeenAt` | `DateTimeOffset` | Required | Detection timestamps |
| `ResolutionReason` | `string?` | Nullable | Operator decision/audit reason |

---

## 6. `MarketplaceRefundRepository` (Extended)

**Location**: `Booking.Shared/Repositories/MarketplaceRefundRepository.cs`

**New methods**:

| Method                                                                                       | Purpose                                                                                   |
| -------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------- |
| `GetByIdempotencyKeyAsync(string key, CancellationToken)`                                    | Idempotency dedup lookup                                                                  |
| `GetActiveByLocalEntityAsync(string type, string id, CancellationToken)`                     | Returns active (non-terminal) refund for concurrency guard                                |
| `GetPendingBeyondThresholdAsync(DateTimeOffset threshold, CancellationToken)`                | Operational monitoring: find refunds stuck in ProviderPending/Processing beyond threshold |
| `GetByStatusAsync(IReadOnlyList<string> statuses, CancellationToken)`                        | Used by reconciliation batch                                                              |
| `GetSumRefundedAmountBySourcePaymentAsync(string provider, string sourcePaymentReference, CancellationToken)` | Enforces the source-payment cap |
| `ReserveRefundAllocationAsync(...)` | Locks/versions a source-payment allocation while validating and reserving the refundable balance |

---

## 7. New Status Constants

**`MarketplaceRefundEventTypeConstants`** additions to align with new states:

```csharp
UnderReview         = "UNDER_REVIEW"
Approved            = "APPROVED"
Rejected            = "REJECTED"
ProviderPending     = "PROVIDER_PENDING"
Cancelled           = "CANCELLED"
ReconciliationRequired = "RECONCILIATION_REQUIRED"
```

---

## 8. Key Validation Rules

| Rule                                         | Enforcement                                                                                     |
| -------------------------------------------- | ----------------------------------------------------------------------------------------------- |
| Refund amount ≤ remaining refundable balance | Source-payment allocation reservation + `GetSumRefundedAmountBySourcePaymentAsync`              |
| State transitions restricted to allowed set  | State machine guard in `MarketplaceRefundAutomationService` and `MarketplaceRefundAdminService` |
| No refund without confirmed payment          | `HasConfirmedPaymentAsync` check (already exists)                                               |
| Idempotency key uniqueness                   | DB unique index + `GetByIdempotencyKeyAsync` pre-check                                          |
| No in-flight duplicate cancellation          | Cancellation-only partial unique index + active-refund pre-check                                |
| Bank transfer cannot auto-complete           | Application-layer guard: `BankTransferSentAt` required before transition to Completed           |
| All monetary arithmetic uses `decimal`       | Static analysis + code review; no `float`/`double` in calculation paths                         |

---

## 9. EF Migration Summary

Migration: `20260728115003_AddCanonicalRefundReliability`

Changes:

1. Add new nullable/non-nullable columns to `MarketplaceRefund` table (see section 1)
2. Add new nullable columns to `MarketplaceRefundEvent` table (see section 2)
3. Add unique index on `MarketplaceRefund.IdempotencyKey`
4. Create `MarketplaceRefundPaymentAllocation` and `MarketplaceRefundNotificationDelivery`, including indexes and foreign keys for the new empty schema
5. Drop the existing unique local-entity index and add the cancellation-only active-refund index
6. Add `TaxAdjustment` and `TimezoneId` to the persisted calculation snapshot
7. Enforce non-null, unique `IdempotencyKey` values for newly created refund records

Migration location: `src/booking/shared/Booking.Shared/Database/Migrations/`

## 10. Refund Operations and Notification Delivery

### Reconciliation lease

`MarketplaceRefund` must persist a short renewable claim for reconciliation work:

- `ReconciliationLeaseOwner`: worker identity
- `ReconciliationLeaseExpiresAt`: lease expiry timestamp
- `ReconciliationLeaseRenewedAt`: last renewal timestamp

Workers claim individual refunds, renew active claims, and reclaim expired claims. A unique active claim must prevent concurrent processing.

### `MarketplaceRefundNotificationDelivery`

**Location**: `Booking.Shared/Database/Entities/MarketplaceRefundNotificationDelivery.cs`

### Notification delivery

Persist one delivery record per `(MarketplaceRefundId, EventType, RecipientId)` with delivery status, attempt count, last error, and sent timestamp. The tuple must have a database uniqueness constraint so webhook and Temporal retries cannot send duplicate notifications.

### External provider refunds

External provider refunds without a matching local `MarketplaceRefund` remain visible as reconciliation records requiring operator investigation. They must not automatically alter local financial state.

### Recurring-series cancellation ownership

For a partial cancellation of a recurring series, the subscription root
(`MarketplaceBookingSubscription`) owns the single customer-facing `MarketplaceRefund` record.
Canceled child occurrences contribute internal undelivered-period allocations to that refund; they do not
create separate customer-facing refund records or notifications. Replayed cleanup signals must resolve to
the same subscription-root refund through its idempotency key.
