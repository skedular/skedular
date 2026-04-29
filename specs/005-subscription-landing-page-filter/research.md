# Research: Subscription Landing Page Filtering

**Feature**: `005-subscription-landing-page-filter`  
**Date**: 2026-04-27  
**Status**: Complete

---

## Current State: Subscription List Page

**File**: `web/apps/webapp/src/rootPages/organizations/organization/subscriptions/page.tsx`

The page **already has** status and payment status filter controls, but they are:

- Single-select only (`TextField select`, `'ALL'` as the empty state)
- Entirely client-side (subscriptions loaded in full, then filtered via `useMemo`)
- Options derived from loaded subscription data (not from dedicated backend queries)

**State:**

```tsx
const [statusFilter, setStatusFilter] = useState("ALL");
const [paymentFilter, setPaymentFilter] = useState("ALL");
```

**Filter logic (currently client-side):**

```tsx
const matchesStatus = statusFilter === "ALL" || lifecycleDisplay.statusLabel === statusFilter;
const matchesPayment = paymentFilter === "ALL" || subscription.marketplaceBooking.paymentStatus.name === paymentFilter;
```

---

## Domain Entities

### `MarketplaceBookingSubscription` entity

- `Status` — `string` column (FK to string constants, e.g. `"ACTIVE"`, `"CANCELLED"`)
- `MarketplaceBooking` — direct 1:1 navigation property (always present, non-nullable)

### `MarketplaceBooking` entity

- `PaymentStatus` — `string` column, indexed (e.g. `"PENDING"`, `"CONFIRMED"`)

### `MarketplaceBookingSubscriptionStatus` enum

Values: `Active`, `Cancelled`, `Expired`, `RenewalFailed`, `Paused`  
String constants: `"ACTIVE"`, `"CANCELLED"`, `"EXPIRED"`, `"RENEWAL_FAILED"`, `"PAUSED"`

### `PaymentStatus` enum

Values: `NotSet`, `Pending`, `Rejected`, `Confirmed`, `Expired`, `RecordNeverCreated`, `NoPaymentRequired`  
String constants: `"NOTSET"`, `"PENDING"`, `"REJECTED"`, `"CONFIRMED"`, `"EXPIRED"`, `"RECORD_NEVER_CREATED"`, `"NO_PAYMENT_REQUIRED"`

---

## Existing Backend Filter Support

### `MarketplaceBookingSubscriptionWhereInput.cs`

- Has `MarketplaceBookingSubscriptionStatus? Status` (single nullable value)
- **Missing**: multi-value `Statuses` and any `PaymentStatuses` filter

### `MarketplaceBookingSubscriptionSearchCriteria`

- Has `MarketplaceBookingSubscriptionStatus? Status` (single)
- **Missing**: `ICollection<MarketplaceBookingSubscriptionStatus> Statuses` and `ICollection<PaymentStatus> PaymentStatuses`

### Repository `AddSearchCriteria`

- Filters by single status: `item.Status == searchCriteria.Status.Value.ToMarketplaceBookingSubscriptionStatus()`
- No payment status filter
- EF Core supports navigation property predicates (e.g. `item.MarketplaceBooking.PaymentStatus`) without explicit Include — generates a JOIN

---

## Backend-Driven Combo Box Pattern (existing)

**Precedent**: `marketplaceBookingSubscriptionCancellationModes` query in `RootQuery.cs`:

```csharp
public IEnumerable<MarketplaceBookingSubscriptionCancellationModeDetails> MarketplaceBookingSubscriptionCancellationModes() => [
    new() { Type = ..., Name = ... },
    ...
];
```

Returns a `Details` type with `type` (enum) and `name` (display string).

**Frontend type helper** (`marketplace-booking-subscription-cancellation-mode.ts`):

```ts
export type SupportedMarketplaceBookingSubscriptionCancellationMode = 'IMMEDIATE' | 'AT_PERIOD_END';
export type SupportedMarketplaceBookingSubscriptionCancellationModeDetails = { type: ...; name: string };
```

**Frontend component** (`multiple-choices-product-pricing-billing-modes.tsx` — directly reusable pattern):

```tsx
<Autocomplete
  name={name}
  multiple={true}
  options={items}
  getOptionValue={(option) => option.type}
  getOptionLabel={(option) => option.name}
  disableCloseOnSelect
  ...
/>
```

Uses `Autocomplete` from `mui-rff`, `multiple={true}`, `disableCloseOnSelect`.

---

## Decisions

| Question                       | Decision                                                       |
| ------------------------------ | -------------------------------------------------------------- |
| Backend API domain             | Booking API                                                    |
| Multi-owner scope              | All spaces the authenticated owner manages                     |
| Query trigger on filter change | Immediate (no debounce) — stale responses discarded            |
| URL/query-string state         | Yes — `useSearchParams` + `useRouter` (already used elsewhere) |
| In-flight loading state        | Skeleton/overlay on list; filter controls remain interactive   |
| Filter option loading          | Once on page load                                              |
| Clear-all affordance           | No — per-filter deselect only                                  |
| Accessibility                  | Follow existing portal baseline                                |

---

## What Must Be Built

### Backend

1. Add `Statuses` (multi-value) and `PaymentStatuses` to `MarketplaceBookingSubscriptionWhereInput`
2. Add `Statuses` and `PaymentStatuses` to `MarketplaceBookingSubscriptionSearchCriteria`
3. Add `MarketplaceBookingSubscriptionStatuses()` query (returns all valid subscription status options)
4. Add `MarketplaceBookingPaymentStatuses()` query (returns all valid payment status options)
5. Add `MarketplaceBookingPaymentStatusDetails` GraphQL type
6. Update `AddNameMapping` extensions for both enum types
7. Update repository `AddSearchCriteria` to apply multi-value filters
8. Update `RootQuery.cs` to wire new inputs through to search criteria
9. Regenerate schema with `scripts/generate-graphql.sh`

### Frontend

1. Add `marketplace-booking-subscription-status.ts` type helper
2. Add `marketplace-booking-payment-status.ts` type helper
3. Add `multiple-choices-marketplace-booking-subscription-statuses.tsx` filter component
4. Add `multiple-choices-marketplace-booking-payment-statuses.tsx` filter component
5. Update subscriptions page root query to fetch status option lists
6. Replace single-select state with array state, add URL sync, re-issue query on change
7. Remove client-side filter logic
8. Add skeleton during in-flight
9. Regenerate Relay artefacts with `web/apps/webapp/scripts/generate.sh`
