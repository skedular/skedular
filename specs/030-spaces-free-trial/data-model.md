# Data Model: Skedular Spaces Free Trial

## Organization Trial State

Organization-owned durable state establishing whether and when the organization received its one Spaces trial.

**Fields**

- `organizationId`: Existing organization identifier.
- `organizationType`: Existing Private, Marketplace, or Individual type used to scope Spaces behavior.
- `createdAt`: Existing immutable organization creation timestamp.
- `spacesTrialStartedAt`: Nullable UTC timestamp. Once set, it is immutable.
- `spacesTrialEndsAt`: Derived as `spacesTrialStartedAt + 14 days`; not stored in Organization persistence.

**Initialization rules**

- New Marketplace/Spaces organization: `spacesTrialStartedAt = createdAt`.
- Existing Marketplace/Spaces Free organization after rollout: effective trial start is `spacesTrialStartedAt ?? createdAt`; do not rewrite the record solely to populate the fallback.
- Offering identity remains `SpacesFreeTierV1`; no V2 offering or subscription-code migration is introduced.
- Existing Private/Teams-only organization: remains null.
- Older Teams-only organization enabling Spaces for the first time: set to enablement time in the same transaction as Spaces offering assignment.
- Disabling/re-enabling Spaces, upgrading, canceling, downgrading, deleting an offering, or payment failure never resets the value.

**Indexes and validation**

- Nullable index supports migration/support queries for Spaces organizations missing initialization.
- Timestamp must not be later than the transaction's authoritative current time.
- Updates attempting to replace a non-null value are rejected or ignored with a warning log.

## Organization Spaces Offering

Existing Organization-owned offering/subscription row representing the current Spaces plan and calendar-month billing period.

**Existing fields retained**

- `id`, `organizationId`, `code`, `start`, `end`, `autoRenew`
- `unitPrice`, `fixedPrice`, `currency`, `discountPercentage`
- purchased capacity fields, `catalogVersion`
- payment-intent relationship and audit/deletion timestamps

**Fields added for the initial paid transition**

- `spacesBillingStartsAt`: Nullable timestamp set to the next first day of the month for a successful mid-month upgrade. Null for Free, legacy, and ordinary renewed paid offerings.
- `isComplimentaryBridge`: Derived from a paid Spaces offering whose `spacesBillingStartsAt` is after its access start and has not yet been consumed by the first paid-period transition.

**Derived lifecycle meaning**

- `SpacesFreeTierV1`: time access comes from the trial anchor, while the existing 100-booking-instance monthly quota continues to apply during the trial.
- Growth/Business/Contact Us: existing paid quota/capacity and renewal behavior remains.
- Paid offering with a mid-month `start`, next-first-of-month `end`, and matching `spacesBillingStartsAt`: complimentary bridge; the bridge is never the billed offering.
- At `spacesBillingStartsAt`, the transition charges and persists the upcoming full-month offering `[billingStartsAt, billingStartsAt + 1 month)` using an idempotent upcoming-offering/payment key.
- Renewed paid offering starting exactly at the calendar boundary: paid active period.
- Deleted/canceled paid offering: original trial anchor remains; Free fallback evaluates against that original trial.

**Validation**

- Chargeable offering activation requires an attached payment method.
- Free trial retains the existing 100-booking-instance monthly quota.
- Existing paid plan prices, quotas, negotiated terms, and discounts are not rewritten by migration.
- Only one non-deleted current offering is used for the organization under the existing repository invariant.

## Spaces Subscription Status

Portable enum returned by the evaluator and exposed to authenticated clients.

| Status | Condition | New operations | New bookings | Protective existing-commitment actions |
|---|---|---:|---:|---:|
| `TRIAL_ACTIVE` | Free plan, more than 3 whole days remain | Allowed | Allowed subject to the existing monthly quota | Allowed |
| `TRIAL_EXPIRING` | Free plan, more than 0 and at most 3 whole days remain | Allowed | Allowed subject to the existing monthly quota | Allowed |
| `TRIAL_EXPIRED` | Free plan at/after trial end | Denied | Denied | Allowed |
| `COMPLIMENTARY_BRIDGE` | Explicit paid upgrade active before first month-boundary charge | Allowed | Allowed under paid-plan quota | Allowed |
| `PAID_ACTIVE` | Current effective paid offering outside bridge | Allowed | Existing paid quota rules | Allowed |
| `PAID_INACTIVE` | Paid offering missing/not effective after its permitted period | Denied | Denied | Allowed |
| `LEGACY_ACTIVE` | Existing supported legacy Spaces offering | Existing behavior | Existing behavior | Allowed |
| `MISSING_STATE` | Spaces organization lacks required trial/offering state | Fail closed | Fail closed | Only account/support recovery |

**Rules**

- Status is evaluated from current state and caller-supplied authoritative UTC time; it is not persisted as the authorization source.
- Paid/legacy plan codes take precedence over historical trial expiry.
- `remainingTrialDays = max(0, ceiling((trialEndsAt - now) / 24 hours))`.
- At the exact trial end instant, status is `TRIAL_EXPIRED` and remaining days is `0`.

## Spaces Access Action

Portable action category supplied by each owning domain.

| Action | Examples |
|---|---|
| `READ` | View organization data, bookings, configuration, reporting, or public listing |
| `CREATE_OR_MODIFY` | Create/update locations, resources, products, schedules, listings, or other operational configuration |
| `CREATE_BOOKING_INSTANCE` | One-off, marketplace, recurring, subscription-generated, imported, or administrator-created booking instance |
| `PROTECT_EXISTING_COMMITMENT` | Cancel, refund, or close an existing booking/subscription without replacement or renewal |
| `ACCOUNT_OR_UPGRADE` | Authentication, account administration, payment method, billing, plan upgrade, data export, support recovery |

## Spaces Access Decision

Shared result consumed by services and API mappers.

**Fields**

- `allowed`: Whether the requested action may proceed.
- `status`: Current `SpacesSubscriptionStatus`.
- `reasonCode`: Stable `SpacesAccessReasonCode`.
- `action`: Evaluated action.
- `planCode`: Current Spaces plan when available.
- `trialStartedAt`, `trialEndsAt`, `remainingTrialDays`: Trial timing.
- `canUseProduct`, `canAcceptBookings`, `canProtectExistingCommitments`: Client-friendly access flags.
- `upgradeRequired`: True for expired/inactive states recoverable by paid upgrade.
- `nextBillingAt`: First/next calendar-month charge time when applicable.
- `isComplimentaryBridge`: Explicit bridge indicator.

**Reason codes**

- `ALLOWED_TRIAL`, `ALLOWED_PAID`, `ALLOWED_COMPLIMENTARY_BRIDGE`, `ALLOWED_PROTECTIVE_ACTION`, `ALLOWED_READ_OR_RECOVERY`
- `TRIAL_EXPIRED`, `PAID_INACTIVE`, `MISSING_TRIAL_STATE`, `MISSING_OFFERING_STATE`, `ACTION_NOT_ALLOWED`

## Authenticated Organization Spaces Subscription

Organization API read model extending the current `OrganizationSpacesSubscription`.

**Fields**

- Existing plan, commercial model, catalog version, period, capacity, and audit fields.
- `subscriptionStatus`: Current Spaces-specific status.
- `trialStartedAt`, `trialEndsAt`, `remainingTrialDays`.
- `canUseProduct`, `canAcceptBookings`, `canProtectExistingCommitments`, `upgradeRequired`.
- `nextBillingAt`, `isComplimentaryBridge`.
- `accessReason`: Stable reason details (`type`, `name`).

**Visibility**

- Available only through authenticated organization/support authorization.
- Not embedded in public storefront responses.

## Public Spaces Booking Availability

Minimal client-facing projection for marketplace/storefront browsing.

**Fields**

- `canAcceptBookings`: Boolean.
- `availabilityCode`: `AVAILABLE` or `TEMPORARILY_UNAVAILABLE`.
- `message`: Neutral copy; no plan, trial, billing, payment, or internal reason details.

**Rules**

- Published listings remain queryable when unavailable.
- Booking/subscribe controls are disabled when false.
- Booking mutation enforcement remains authoritative if availability becomes stale.

## Replicated Offering Trial State

Existing JSON-backed Organization offering projection in Booking and Location.

**Fields added**

- `spacesTrialStartedAt`
- `spacesTrialEndsAt`
- `spacesProductEnabled`
- `spacesNextBillingAt` where the current offering has a scheduled boundary

**Fields retained**

- `spacesPlanCode`, paid quota/custom capacity, paid period boundaries, generic offering data.

**Rules**

- Dates are source inputs; consumers derive current status locally.
- Consumers update these values only from ordered Organization events.
- Trial projection never affects Private/Teams offering evaluation.

## Booking Access and Quota Decision

Evolution of existing `SpacesQuotaDecision` into access-first evaluation while preserving paid quota data.

**Fields added**

- `accessDecision`: Current Spaces access status/reason/timing summary.
- `quotaApplicable`: True for the Free trial and paid plans that retain quotas.

**Existing fields retained for paid plans**

- plan code, current usage, quota, attempted/excluded counts, remaining quota, current period, upgrade plans.

**Rules**

- Active/expiring Free trial: access is allowed, then the existing 100-booking-instance monthly quota and usage query are applied.
- Expired Free trial: denied with `TRIAL_EXPIRED`, independent of usage.
- Paid/legacy offering: existing quota evaluation and reason codes remain.
- Missing projected state: fail closed and log as missing state, not cache miss.

## State Transitions

```text
Teams-only, Spaces never enabled
  -- first Spaces enablement --> TrialActive

New Spaces organization
  -- organization creation --> TrialActive

TrialActive
  -- <= 3 whole days remain --> TrialExpiring
  -- explicit paid upgrade --> ComplimentaryBridge or PaidActive

TrialExpiring
  -- exact trial end --> TrialExpired
  -- explicit paid upgrade --> ComplimentaryBridge or PaidActive

TrialExpired
  -- explicit paid upgrade + payment method --> ComplimentaryBridge or PaidActive

ComplimentaryBridge
  -- first-of-month charge succeeds + renewal --> PaidActive
  -- canceled before first charge --> TrialExpired (no retroactive charge)
  -- upgrade/transition incomplete --> TrialExpired

PaidActive
  -- normal renewal --> PaidActive
  -- cancel/downgrade to Free --> TrialExpired when original trial has elapsed
  -- offering ceases to be effective --> PaidInactive
```

## Relationships

- Organization has one immutable optional Spaces trial anchor and one current offering under existing invariants.
- Subscription status combines Organization trial state, current offering, and current time.
- Organization publishes stable dates/plan inputs to Booking and Location through the existing Organization event.
- Booking and Location use the shared evaluator but own their domain mutation and authorization decisions.
- Public availability is derived from the same Organization access decision but intentionally omits commercial details.
- Paid upgrade continues using the existing payment method, offering, Temporal workflow, payment intent, and renewal relationships.

## Migration and Preservation

- Add nullable Organization column and model mappings.
- Do not backfill trial anchors or migrate customer plans. Existing Marketplace Free organizations derive their effective anchor from Organization `CreatedAt` when the stored anchor is absent.
- Do not add a hosted startup migration path. New Spaces organizations and first enablement persist the anchor through normal write flows.
- Publish changed Organization projections through the outbox.
- Do not update or delete Booking, RecurringBooking, MarketplaceBooking, subscription, customer, location, resource, product, listing, configuration, invoice, or accounting rows.
