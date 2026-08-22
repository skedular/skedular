# Duration Display Unit Contract

## Shared editor contract

The shared duration input accepts a canonical value in minutes, an optional initial display unit, a selected display unit, a unit-change callback, and a canonical-minute change callback.

If no unit is supplied, the editor uses HOURS. Unit changes preserve the canonical minute value and reuse the existing visible conversion and rounding behavior.

## Persisted API contract

Applicable read and write fields are additive and nullable. A client may omit them. A response may omit them for legacy records. When present, the value is MINUTES or HOURS.

The contract must return enough fields for Relay normalization and editor rendering. Mutation success must not trigger a browser reload.

## Source and generated artifacts

Update owning C# models and source GraphQL/event definitions first. Regenerate affected outputs with repository-required GraphQL, event, and web Relay scripts. Generated schemas and client artifacts are never hand-edited.
