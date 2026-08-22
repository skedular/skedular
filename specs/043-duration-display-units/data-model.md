# Data Model: Persisted Duration Display Units

## Duration display unit

| Attribute | Description |
|---|---|
| Value | Optional enum-like value: MINUTES or HOURS |
| Default | HOURS when absent at the editor boundary |
| Ownership | The owning persisted configuration model; Marketplace owns the confirmed initial fields |
| Purpose | Restore editor presentation only |
| Business effect | None; calculations and validation use canonical minutes |

## Marketplace pricing duration fields

Each applicable field keeps its existing canonical value and gains a nullable companion:

| Canonical field | Existing meaning | Companion metadata |
|---|---|---|
| MinDurationMinutes | Optional minimum duration in minutes | MinDurationDisplayUnit |
| MaxDurationMinutes | Optional maximum duration in minutes | MaxDurationDisplayUnit |
| MaxAllowedResourcesLockTimePaidViaCard | Card lock window in minutes | MaxAllowedResourcesLockTimePaidViaCardDisplayUnit |
| MaxAllowedResourcesLockTimePaidViaBankTransfer | Bank-transfer lock window in minutes | MaxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit |
| ProductPricingCancellationRefundRule.MinutesBefore | Refund-rule threshold in minutes | DisplayUnit on the rule |

Names are design-level names; implementation must follow established naming and mapping conventions after confirming source contracts.

## Compatibility and invariants

- Old JSON without companions deserializes successfully.
- Absent companions are exposed as nullable contract fields and interpreted as HOURS by editors.
- Existing canonical minute values are never changed by loading, switching units, autosaving, or submitting.
- Existing conversion and rounding remains unchanged.
- Unknown persisted values follow the owning model’s explicit mapping/error policy.
- Display metadata is not used in booking, refund, lock-window, availability, or workflow calculations.

## Relationships

Product-version pricing owns the duration metadata. Domain projections may carry it only when their existing public contract is an editor-facing projection. Booking snapshots and operational records remain canonical-minute or time-range data.
