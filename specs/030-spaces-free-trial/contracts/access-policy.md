# Contract: Spaces Access Policy

## Purpose

Define one portable, product-scoped decision contract used by Organization, Booking, and Location without coupling domains to each other's persistence.

## Inputs

```text
EvaluateSpacesAccess(
  nowUtc,
  organizationType,
  spacesTrialStartedAt,
  currentOffering,
  action
) -> SpacesAccessDecision
```

`currentOffering` supplies plan code, effective period, and next billing boundary. Callers supply authoritative time. The evaluator performs no I/O.

## Actions

```text
READ
CREATE_OR_MODIFY
CREATE_BOOKING_INSTANCE
PROTECT_EXISTING_COMMITMENT
ACCOUNT_OR_UPGRADE
```

## Status values

```text
TRIAL_ACTIVE
TRIAL_EXPIRING
TRIAL_EXPIRED
COMPLIMENTARY_BRIDGE
PAID_ACTIVE
PAID_INACTIVE
LEGACY_ACTIVE
MISSING_STATE
```

## Decision invariants

- Private/Teams offerings do not enter Spaces trial evaluation.
- Free plan plus null trial anchor fails closed as `MISSING_STATE`.
- Free plan before exact expiry allows create/modify actions; booking actions remain subject to the existing 100-booking-instance monthly quota.
- Free plan at or after exact expiry denies create/modify and booking actions.
- Expired trial continues to allow reads, account/upgrade/export, and protective existing-commitment actions.
- Protective actions must not create replacement bookings, renewals, or other new commitments.
- Paid/legacy offerings take precedence over historical trial expiry.
- Complimentary bridge begins only after explicit paid upgrade and ends at the scheduled first billing boundary.
- The evaluator never mutates state and never trusts client-supplied status or dates.

## Stable reasons

```text
ALLOWED_TRIAL
ALLOWED_PAID
ALLOWED_COMPLIMENTARY_BRIDGE
ALLOWED_PROTECTIVE_ACTION
ALLOWED_READ_OR_RECOVERY
TRIAL_EXPIRED
PAID_INACTIVE
MISSING_TRIAL_STATE
MISSING_OFFERING_STATE
ACTION_NOT_ALLOWED
```

## Error mapping

- Authenticated operator/API denial: machine reason `TRIAL_EXPIRED`, message `Your Skedular Spaces trial has expired. Upgrade to a paid plan to continue.`
- Public customer denial: public availability `TEMPORARILY_UNAVAILABLE`, message `This space is temporarily unavailable for new bookings.`
- Missing state: fail closed and return an operator-oriented retry/support message; do not mislabel as trial expiry.

## Logging contract

Every deny decision logs organization ID, product `Spaces`, status, action, stable reason, evaluation time, trial end when present, and correlation context. Logs exclude customer details, booking payloads, and payment data.
