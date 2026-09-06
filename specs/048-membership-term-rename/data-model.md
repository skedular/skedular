# Data Model: MembershipTerm Terminology Rename

## Product pricing

The existing pricing aggregate remains canonical. Its contractual-period property is renamed from `PurchaseCadence` to `MembershipTerm` across domain and persistence.

| Concept | Phase-one rule |
|---|---|
| `MembershipTerm` | Same values and meaning as the former property; the customer’s contractual signup period. |
| Persistence field | Renamed to `MembershipTerm`/`membershipTerm` by the forward migration; existing values are preserved. |
| Renewal | Uses the membership term only when the offer is configured to renew. |
| Billing cycle | Independent organization-owned period that may split invoicing or materialization. |
| Booking duration | Independent selected interval governed by existing duration rules. |
| Entitlement cadence-free state | Retains existing null/`NotSet` semantics. |

## Validation rules

- Preserve supported values, nullability, unset behavior, and invalid-value handling.
- Do not map billing cycles, invoice schedules, booking durations, or generic time units to `MembershipTerm`.
- Verify existing and newly written records round-trip without loss or reinterpretation.

## Out of scope

- Renaming the persisted storage field.
- Backfilling or transforming existing product/offering records.
- Changing term values, renewal, billing, booking duration, entitlement, cancellation, refund, or financial behavior.
