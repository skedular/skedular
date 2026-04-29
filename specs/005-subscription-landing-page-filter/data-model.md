# Data Model: Subscription Landing Page Filtering

**Feature**: `005-subscription-landing-page-filter`  
**Date**: 2026-04-27

---

## Existing Entities (no structural changes)

### `MarketplaceBookingSubscription`

| Field                | Type       | Notes                                    |
| -------------------- | ---------- | ---------------------------------------- |
| `Id`                 | `string`   | PK                                       |
| `Status`             | `string`   | Indexed; string constant e.g. `"ACTIVE"` |
| `MarketplaceBooking` | navigation | 1:1, always present                      |

### `MarketplaceBooking`

| Field           | Type     | Notes                                     |
| --------------- | -------- | ----------------------------------------- |
| `Id`            | `string` | PK                                        |
| `PaymentStatus` | `string` | Indexed; string constant e.g. `"PENDING"` |

**No database migrations required** — filtering happens via existing indexed columns.

---

## New / Extended Backend Models

### `MarketplaceBookingSubscriptionSearchCriteria` (extended)

Add two new collection parameters alongside the existing single `Status?`:

```text
+ ICollection<MarketplaceBookingSubscriptionStatus> Statuses   // empty = no restriction
+ ICollection<PaymentStatus>                        PaymentStatuses  // empty = no restriction
```

**Filter semantics:**

- `Statuses` empty → no restriction applied (all subscription statuses)
- `Statuses` non-empty → `WHERE subscription.Status IN (statuses)`
- `PaymentStatuses` empty → no restriction
- `PaymentStatuses` non-empty → `WHERE marketplaceBooking.PaymentStatus IN (paymentStatuses)` (via EF navigation property)
- Both non-empty → AND combination

---

### `MarketplaceBookingSubscriptionWhereInput` (extended)

```text
+ IEnumerable<MarketplaceBookingSubscriptionStatus>? Statuses
+ IEnumerable<PaymentStatus>?                        PaymentStatuses
```

GraphQL names: `statuses`, `paymentStatuses`

---

## New GraphQL Types

### `MarketplaceBookingSubscriptionStatusDetails` (already exists — no change)

```graphql
type MarketplaceBookingSubscriptionStatusDetails {
  type: MarketplaceBookingSubscriptionStatus!
  name: String!
}
```

### `MarketplaceBookingPaymentStatusDetails` (new)

Follows the same `Details` pattern as cancellation mode and subscription status details:

```graphql
type MarketplaceBookingPaymentStatusDetails {
  type: PaymentStatus!
  name: String!
}
```

**C# type**: `MarketplaceBookingPaymentStatusDetails`  
**Location**: `booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/`

---

## New GraphQL Queries

### `marketplaceBookingSubscriptionStatuses`

Returns all valid subscription status choices for the filter combo box.  
Follows the `marketplaceBookingSubscriptionCancellationModes` pattern exactly.

```graphql
marketplaceBookingSubscriptionStatuses: [MarketplaceBookingSubscriptionStatusDetails!]!
```

Returns all 5 values: Active, Cancelled, Expired, RenewalFailed, Paused — each with a display name.

### `marketplaceBookingPaymentStatuses`

Returns all valid payment status choices for the filter combo box.

```graphql
marketplaceBookingPaymentStatuses: [MarketplaceBookingPaymentStatusDetails!]!
```

Returns operator-relevant values only (excludes `RecordNeverCreated`/`NotSet` internal-only values — to be confirmed during implementation by checking what values genuinely appear on subscription marketplace bookings).

---

## Frontend State Model

### Filter State (replaces `statusFilter`/`paymentFilter` strings)

```ts
const [selectedStatuses, setSelectedStatuses] = useState<MarketplaceBookingSubscriptionStatusForFilter[]>([]);
const [selectedPaymentStatuses, setSelectedPaymentStatuses] = useState<MarketplaceBookingPaymentStatusForFilter[]>([]);
```

Where `MarketplaceBookingSubscriptionStatusForFilter` and `MarketplaceBookingPaymentStatusForFilter` are local type helpers following the pattern of `SupportedMarketplaceBookingSubscriptionCancellationMode`.

### URL Encoding

| URL param         | Value                                                 |
| ----------------- | ----------------------------------------------------- |
| `statuses`        | comma-separated type strings, e.g. `ACTIVE,PAUSED`    |
| `paymentStatuses` | comma-separated type strings, e.g. `PENDING,REJECTED` |

Absent param → empty array → no restriction.

---

## Validation Rules

- Unknown status or payment status values arriving in the URL or GraphQL input are silently ignored by the backend (logged as warning per LOG-002); the remaining valid values are applied.
- An empty `Statuses` array is treated identically to a null/absent field: no status restriction.
- An empty `PaymentStatuses` array is treated identically to null/absent: no payment status restriction.
