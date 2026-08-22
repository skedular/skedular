# Research: Persisted Duration Display Units

## Research scope

This research inventories repository concepts that store a value in minutes and expose it through an editable minutes/hours presentation. It excludes operational TimeSpan values, workflow timeouts, logs, calculations, opening-hours schedules, and read-only customer-derived displays.

## Audit findings

| Occurrence | Domain / persistence boundary | Editor or consumer | Classification | Plan |
|---|---|---|---|---|
| MinDurationMinutes, MaxDurationMinutes | Marketplace ProductPricing inside product-version pricing JSON | Shared duration input; Spaces product editor; Host product/listing and legacy location pricing editors | Persisted editable configuration | Add optional per-field display units |
| MaxAllowedResourcesLockTimePaidViaCard, MaxAllowedResourcesLockTimePaidViaBankTransfer | Marketplace ProductPricing inside product-version pricing JSON | Spaces and Host pricing editors | Persisted editable configuration | Add optional per-field display units |
| ProductPricingCancellationRefundRule.MinutesBefore | Marketplace cancellation/refund rules inside product-version pricing JSON | Spaces and Host pricing editors; policy details | Persisted editable configuration for the editor; derived policy output remains canonical minutes | Add optional rule display unit; do not change policy evaluation |
| Booking TimeSpan/date ranges, Temporal timeouts, reconciliation thresholds, payment retry windows | Booking/shared operational state | Backend workflows/services/tests | Operational/internal duration | Exclude |
| Location opening/available hours and booking slot minutes | Location configuration and availability projection | Schedule/availability editors and backend availability consumers | Time-of-day schedule or operational slot configuration | Exclude |
| Read-only cancellation-policy labels and booking/entitlement duration summaries | Marketplace/customer-facing derived views | Customer-facing components | Read-only derived presentation | Exclude; continue existing formatting |

The audit search covered minute/duration/lock/refund terminology, the shared duration input, Marketplace models and event contracts, web editor paths, and backend calculation paths. No additional persisted editable minutes/hours duration configuration was confirmed outside Marketplace in the current source scan. Planning and implementation tasks must repeat the targeted search after edits and record any newly discovered occurrence here before closing the feature.

## Decisions

### Store display metadata beside each applicable field

- **Decision**: Add nullable display-unit metadata per editable duration/rule field in the existing persisted JSON model.
- **Rationale**: Each field can have a different user preference; a single pricing-level unit would not restore field-level editor state correctly.
- **Alternatives considered**: A separate preference table was rejected because the values already live in product-version JSON.

### Preserve canonical values and existing conversion

- **Decision**: Persist only the selected unit. Do not migrate, normalize, re-round, or rewrite existing minute values. Keep current conversion behavior such as 5 minutes displaying as 0.08 hours.
- **Rationale**: The feature addresses restoration of presentation state, not business-value conversion.
- **Alternatives considered**: Increasing precision or changing rounding was rejected as out of scope and potentially data-changing.

### Backward-compatible optional metadata

- **Decision**: Missing metadata defaults to HOURS in editors; API inputs and persisted records remain valid when fields are absent.
- **Rationale**: Existing records and clients must continue to work without migration.

### Shared UI conversion contract

- **Decision**: Extend the existing shared duration input with canonical minutes, selected unit, initial unit, and controlled unit change behavior while retaining its current display conversion.
- **Rationale**: One conversion contract prevents Spaces, Host, and future editors from drifting.

### Additive contract/event handling

- **Decision**: Update source GraphQL and event definitions only where required by existing projection contracts, then regenerate generated outputs.
- **Rationale**: Metadata follows the owning Marketplace source and remains compatible with old clients.

## Open planning verification

Before implementation is complete, rerun the repository audit, compare results with this inventory, and append any additional in-scope occurrence with its owner, persistence boundary, editor, and test plan.
