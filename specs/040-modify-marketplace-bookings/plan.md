# Implementation Plan: Modify Marketplace Bookings

**Branch**: `040-modify-marketplace-bookings` | **Date**: 2026-08-07 | **Spec**: [spec.md](spec.md)
**Input**: Allow eligible customers and authorized operators to change the date/time and, where supported, selected resources of confirmed marketplace bookings without changing the purchase.

## Summary

Introduce a dedicated, explicit marketplace-booking modification command rather than extending the current autosave patch. The command validates the proposed window and selected resources atomically against the original purchased entitlement; preserves all commercial/payment state; marks modified subscription occurrences as overrides; records durable audit/notification delivery state; and exposes one consistent flow to the customer, Spaces operator, and Host operator experiences. The plan updates the GraphQL contract and Relay artifacts, the Booking domain and Temporal-safe reconciliation behavior, durable change notification/recovery, product-specific UI, and public documentation.

## Technical Context

**Language/Version**: C# / .NET 10 backend; TypeScript 6, React 19, Next.js 16 web clients
**Primary Dependencies**: HotChocolate/Fusion GraphQL, EF Core/PostgreSQL, Temporal, Relay 21, MUI 9, `@skedular/ui`, `@skedular/shared`
**Storage**: Booking-owned PostgreSQL; new Booking-domain audit and notification-delivery persistence, accessed only through Booking repositories
**Testing**: xUnit/FakeItEasy unit tests; focused Booking-domain integration tests for persistence/concurrency/Temporal boundaries; Vitest and React Testing Library for UI
**Target Platform**: Booking API/jobs services and the `webapp`, `webapp-spaces`, `webapp-host`, and public-web applications
**Project Type**: Distributed web application with GraphQL service and background workflows
**Performance Goals**: An eligible user can load change options and submit a successful change within the spec's three-minute journey; the command claims resources atomically and returns an actionable result without a partial booking
**Constraints**: Confirmed or no-payment-required future bookings only; retain original product/version/price/quantity/payment; no refund/invoice/payment-workflow restart; subscription changes stay in the current cycle; Host is date/time only; resource selection is Spaces-only when the product exposes alternatives
**Scale/Scope**: One booking occurrence per command, including individual Spaces subscription occurrences; all three web clients consume generated GraphQL artifacts; no subscription-calendar editor or Host subscription enablement

## Constitution Check

_GATE: Passed before Phase 0 research; re-checked after Phase 1 design._

- [x] **I. Contract-First** — The Booking GraphQL source contract, schema, and Relay artifacts change. Update source definitions first, run `scripts/generate-graphql.sh`, then run the Relay generation commands for `webapp`, `webapp-spaces`, and `webapp-host`; do not edit generated output.
- [x] **II. Domain Boundaries** — Booking owns command validation, persistence, audit, notification delivery, and Temporal coordination. Product/resource entitlement is read through existing Booking-domain repositories/services; no cross-domain database access is introduced. New persisted enum-like values use owning model constants and explicit mappings.
- [x] **III. Testing** — Unit tests lead for eligibility, authorization, atomic resource claim, subscription override, notification state, and logs. Add focused integration coverage only for migration/repository persistence, serializable conflict, GraphQL wiring, and Temporal outbox/activity boundaries.
- [x] **IV. Frontend** — Relay queries/fragments remain collocated, generated artifacts are regenerated, UI uses `@skedular/ui` typography and American English. Update Spaces, Host, shared marketplace, booking/resource/availability, and subscription public documentation.
- [x] **V. Pattern Consistency** — The dedicated command avoids overloading the existing generic autosave patch. Audit/delivery follow the existing marketplace refund/failure durable-delivery and Temporal-outbox patterns; this is a reuse, not a parallel infrastructure pattern.
- [x] **VI. Logging** — Add structured start/completion/rejection/conflict, override, audit-delivery, and recovery logs with booking/change/actor correlation; never log sensitive customer content.

## Project Structure

### Documentation (this feature)

```text
specs/040-modify-marketplace-bookings/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── modify-marketplace-booking.graphql
```

### Source Code

```text
src/booking/
├── apis/Booking.Api/
│   ├── GraphQL/Booking/
│   ├── Mappers/
│   ├── Services/
│   └── schema.graphqls
├── shared/Booking.Shared/
│   ├── Database/{Entities,Migrations}/
│   ├── Repositories/
│   ├── Services/
│   ├── Activities/
│   └── Workflows/
└── apis/Booking.Api.UnitTests/ and shared/Booking.Shared.UnitTests/

src/web/apps/
├── webapp/src/components/{marketplaceProductBooking,booking}/
├── webapp-spaces/src/components/booking/editMarketplaceBooking/
├── webapp-host/src/components/booking/editMarketplaceBooking/
└── public-web/src/content/docs/{shared,spaces,host}/
```

**Structure Decision**: Keep all business rules, audit data, and delivery orchestration in Booking. The shared customer product uses `webapp`; operator experiences remain in their intentional Spaces/Host component trees, with Host never exposing an individual-resource picker.

## Design Decisions

1. Add `modifyMarketplaceBooking` as a dedicated command. `updateMarketplaceBooking` remains a limited participant/notes/category autosave patch, preventing accidental slot changes while a form is edited.
2. Authorize against the persisted booking: the purchasing/involved customer may self-modify; only the persisted product-owner organization owner/admin may act for a customer. Never trust caller-supplied organization/team ids to establish authority.
3. Use the persisted `EntityFrameworkVersion` and a serializable resource-claim transaction. Validate before slot release; claim the proposed complete resource set before committing the replacement. Return a typed stale/eligibility/availability outcome and never leave a partial change.
4. Treat `resourceIds: null` as retain/automatic allocation and non-empty ids as an explicit Spaces selection. Reject empty/over-limit/ineligible selections. Host ignores resource selection and remains full-place/date-time-only.
5. Create durable modification/audit and notification-delivery records. Dispatch operator-change customer notification through the existing Temporal outbox/activity delivery pattern; expose a recoverable in-app record if delivery fails.
6. Set `HasRecurringInstanceOverrides` only after a successful subscription occurrence modification. Daily reconciliation must preserve it and must not use its resources as next-cycle preferred resources.

## Implementation Sequence

1. Define Booking shared models, repository interfaces/entities/migration, service command/result/error models, and durable notification/audit delivery state.
2. Implement validation, persisted-booking authorization, serializable proposed-window/resource claim, same-cycle subscription guard, audit persistence, GraphQL event refresh, and structured logs.
3. Add notification Temporal outbox/activity/template and recovery state, with no payment, invoice, refund, or deletion workflow side effects.
4. Add GraphQL command/query fields and mappings; regenerate composed schema and all Relay artifacts.
5. Implement shared customer booking detail/hub flow, Spaces operator date/resource flow, and Host operator date-only flow; add client telemetry and change-history/result UI.
6. Add focused unit/integration/UI tests, update public-web documentation, and run generation/build/test validation from [quickstart.md](quickstart.md).

## Complexity Tracking

No constitution violations requiring justification.
