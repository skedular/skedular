# Implementation Plan: Weekly Price Day Selection

**Branch**: `035-weekly-day-selection` | **Date**: 2026-07-21 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/035-weekly-day-selection/spec.md`

## Summary

Add an optional, weekly-only exact required-day rule to `ProductPricing`, retaining the existing price-level `AvailableDays` rule as the eligible-day pool. A customer selects a valid fixed weekly pattern in the marketplace customer flow; Booking persists that selection with the purchase/subscription and materializes it through the existing recurring-booking `ByWeekDays` schedule. Resource matching considers only selected days and creates a resource-less booking shell on the selected date when no resource can be assigned. Daily reconciliation repairs untouched shells; an administrator’s edit to an individual booking uses the existing override state so the workflow no longer changes that booking or its fixed pattern. An administrator can cancel and refund an impossible individual booking without ending the subscription. Auto-renewal retains the selected pattern and follows the same shell-and-repair behavior. Host, Spaces, generated GraphQL contracts, and public documentation remain synchronized.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript 6, React 19, Next.js 16
**Primary Dependencies**: HotChocolate/Fusion GraphQL, EF Core/PostgreSQL, Kafka protobuf events, Temporal, Relay, MUI, `@skedular/ui`, `@skedular/shared`
**Storage**: Existing Marketplace product-version pricing JSON; Booking PostgreSQL subscription/recurring-booking data, including resource-less booking shells and existing recurring-instance overrides
**Testing**: xUnit unit and integration tests; Vitest and React Testing Library; GraphQL/Relay generated-contract validation
**Target Platform**: Backend services, Skedular Host, Skedular Spaces, and the public Astro documentation site
**Project Type**: Distributed web application and documentation site
**Performance Goals**: Preserve the existing daily reconciliation cadence; perform constant-size selected-day checks without scanning or allocating unselected weekdays
**Constraints**: Weekly cadence only; exact required selected-day count; available days, required counts, and purchased selections remain separate; UTC calendar semantics; payment stays retained while an untouched shell is eligible for repair
**Scale/Scope**: Additive price fields propagated through Marketplace projections; per-subscription selected-day snapshot, shell repair, and individual-booking override behavior in Booking; Host and Spaces price configuration/operator views, shared marketplace customer selection, and public documentation

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — `ProductPricing`, product-version events, GraphQL types/inputs, and Relay artifacts change. Update `api-definitions/events/skedular/marketplace_v1_value.proto`; run `api-definitions/events/generate.sh`, `scripts/generate-graphql.sh`, and `src/web/apps/webapp/scripts/generate.sh`. Do not hand-edit exported schemas or generated Relay files.
- [x] **II. Domain Boundaries** — Marketplace owns price-definition configuration and republishes it through its existing event. Booking owns purchased selections, selected-day resource matching, resource-less shells, individual-booking overrides, renewal, and refunds. Host/Spaces consume GraphQL contracts; no domain reaches another domain database.
- [x] **III. Testing** — Add unit coverage for weekly-rule and selection validation, selected-day matching, shell repair, individual override behavior, and renewal. Add integration coverage for persistence, workflow/repository behavior, cancellation/refund initiation, and GraphQL mutations using repository-layer assertions. Add Host/Spaces web tests for configuration, selection, status, and individual-booking flows.
- [x] **IV. Frontend** — Collocate Relay selections with Host and Spaces components, regenerate Relay artifacts, use `@skedular/ui` typography wrappers, and use American English in customer and operator copy.
- [x] **V. Pattern Consistency** — Reuse existing `AvailableDays`, `DayOfWeek`, `RecurringBooking.ByWeekDays`, resource-less `Booking` behavior, `HasRecurringInstanceOverrides`, subscription renewal, Temporal workflow-ID, and Marketplace refund patterns. No new intervention aggregate is needed.
- [x] **VI. Logging** — Add structured logs for weekly-rule validation, selected-day candidate evaluation, shell creation/repair, administrator override/cancellation, renewal paths, and refund initiation; include request/workflow correlation and no sensitive customer data.

## Project Structure

### Documentation (this feature)

```text
specs/035-weekly-day-selection/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
api-definitions/events/skedular/marketplace_v1_value.proto
src/shared/Api.Shared.Services/Models/ProductPricing.cs
src/marketplace/{apis,shared,processors}/
src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/
src/booking/apis/Booking.Api/{Mappers,Services}/
src/booking/shared/Booking.Shared/{Models,Database,Repositories,Services,Activities,Workflows}/
src/booking/{shared,apis,domain}/*Tests/
src/web/apps/webapp-host/src/components/{product,marketplaceProductSubscription,booking}/
src/web/apps/webapp-spaces/src/components/{product,marketplaceProductSubscription,booking}/
src/web/apps/webapp/src/components/{marketplaceProductSubscription,marketplaceProductBooking}/
src/web/apps/public-web/src/content/docs/{shared,spaces}/marketplace/
```

**Structure Decision**: Extend the shared product-price contract and Marketplace event projection for weekly configuration. Keep the customer-selected schedule, resource-less shells, repair, individual-booking overrides, renewal, cancellation, and refund workflows in Booking. Expose additive GraphQL fields and reuse individual-booking update/cancel operations in Host and Spaces; update the public documentation. The customer marketplace view in `webapp` must remain compatible where it shares subscription and booking details.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --- | --- | --- |
| None | Existing contract, recurring-schedule, and refund patterns support the feature. | N/A |
