# Quickstart: Persisted Duration Display Units

## Prerequisites

- Check out branch 043-duration-display-units.
- Use the repository’s standard .NET and web toolchain.
- Ensure generated artifacts are available or regenerate them through required scripts.

## Validation scenarios

1. Load an existing Marketplace pricing JSON record without display metadata. Confirm it deserializes and each editor defaults to HOURS.
2. Enter 5 minutes, select MINUTES, save, reload, and confirm the editor displays 5 minutes while the stored canonical value remains 5.
3. Select HOURS for the same value, save, reload, and confirm existing conversion displays 0.08 hours without changing the stored value.
4. Switch between units repeatedly without editing the visible value. Confirm the canonical minute value is unchanged.
5. Repeat save/reload checks for minimum duration, maximum duration, card lock window, bank-transfer lock window, and cancellation/refund timing.
6. Repeat editor checks in Spaces, Host unified listing/pricing, and any additional editor identified by the audit.
7. Run backend tests proving booking duration, refund policy, and lock-window calculations are unchanged for identical canonical minutes.
8. Run JSON compatibility, GraphQL contract, and generated Relay/schema validation.

## Expected outcomes

- Existing records and clients remain compatible.
- Only display-unit metadata changes when the user changes the unit.
- No canonical minute value is migrated or rewritten.
- Operational and read-only duration occurrences remain unchanged.

See data-model.md, contracts/duration-display-unit-contract.md, and research.md for design boundaries and audit inventory.
