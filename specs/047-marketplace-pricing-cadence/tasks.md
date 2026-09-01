# Tasks: Marketplace Pricing Cadence Simplification

**Input**: Design documents from `/specs/047-marketplace-pricing-cadence/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/pricing-contracts.md](contracts/pricing-contracts.md), [quickstart.md](quickstart.md)

**Testing discipline**: Prefer unit tests for all isolated behavior and use existing unit coverage wherever possible. Add integration tests only for persistence, migration, event projection, schema wiring, or workflow/external-infrastructure boundaries that unit tests cannot prove. All persistence assertions in integration tests must use repository/query methods only; tests must not access `DbContext` or Entity Framework directly.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the source-first contract and inventory baseline before implementation.

- [X] T001 Record the current `BookingCadence`, removed cadence values, and entitlement default usages with exact source paths in `specs/047-marketplace-pricing-cadence/quickstart.md`
- [X] T002 [P] Document the manual repository-wide `rg` absence check in `specs/047-marketplace-pricing-cadence/quickstart.md` for final execution against active source, generated surfaces, and tests
- [X] T003 [P] Review affected customer/operator documentation references in `src/marketplace/docs/architecture/marketplace-domain-architecture.md` and identify required behavior updates

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Change the shared model and source contracts before story-specific behavior can compile or regenerate.

- [X] T004 Update `ProductPricing` to remove `BookingCadence` and preserve `PurchaseCadence`, min/max duration, renewal, and entitlement fields in `src/shared/Api.Shared.Services/Models/ProductPricing.cs`
- [X] T005 Update `ProductPricingCadence` enum, constants, explicit conversions, display names, invoice names, and provider names to retain `NotSet` plus the ten supported values in `src/shared/Api.Shared.Services/Models/ProductPricingCadence.cs`
- [X] T006 [P] Remove `bookingCadence` and obsolete enum members from the source event contract in `api-definitions/events/skedular/marketplace_v1_value.proto`
- [X] T007 [P] Remove `bookingCadence` and obsolete enum values from marketplace and booking GraphQL source schemas in `src/marketplace/apis/Marketplace.Api/schema.graphqls` and `src/booking/apis/Booking.Api/schema.graphqls`
- [X] T008 Confirm JSON-backed pricing persistence requires no relational migration: `ProductVersion.PricingOptions` remains JSONB and the removed field/value is handled by the updated JSON contract; no direct removal migration or model metadata is required because production contains no sub-day cadence values.
- [X] T009 [P] Update event-to-model mappings to map only `PurchaseCadence` in `src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs` and `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`
- [X] T010 Regenerate event, GraphQL, composed-schema, and affected client artifacts using the consuming `.csproj` builds for protobuf C# types, `scripts/generate-graphql.sh`, and the affected webapp generation scripts
- [X] T011 [P] Regenerate Relay artifacts for affected web apps with `pnpm --dir src/web relay` and verify generated files are not hand-edited in `src/web/apps/webapp/**/__generated__`, `src/web/apps/webapp-host/**/__generated__`, and `src/web/apps/webapp-spaces/**/__generated__`
- [X] T012 Add structured logging for pricing validation, renewal branching, duration rejection, and invalid legacy cadence handling in the owning marketplace and booking services.

**Checkpoint**: Shared contracts compile/regenerate with one cadence field and no unsupported values; story work can proceed.

## Phase 3: User Story 1 - Configure Clear Marketplace Offer Terms (Priority: P1) 🎯 MVP

**Goal**: Make `PurchaseCadence` the sole offer/contract term and ensure auto-renewal and billing/resource slices use the correct independent concepts.

**Independent Test**: Create/read pricing for all ten supported terms, verify removed values and `BookingCadence` are unavailable, verify non-renewing Daily is one term, renewing Daily renews daily, and longer terms retain their purchase term while billing/resource slices follow organization billing settings.

### Tests for User Story 1

- [X] T013 [P] [US1] Update product pricing model/mapping tests for the ten supported cadence values and removed-value rejection in the shared and marketplace unit-test surfaces.
- [X] T014 [P] [US1] Cover product create/edit validation and supported cadence choices with unit tests and synchronized GraphQL schema wiring.
- [X] T015 [P] [US1] Update event-to-model mappings and the affected projection/fixture coverage for the single purchase cadence field.
- [X] T016 [P] [US1] Adjust renewal and billing-slice behavior/tests for non-renewing and renewing day-or-longer terms.

### Implementation for User Story 1

- [X] T017 [US1] Update marketplace product validation and pricing mapping to validate only the supported purchase terms and stop deriving duration steps from `BookingCadence` in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`
- [X] T018 [US1] Update subscription term boundaries and renewal decisions to use `PurchaseCadence` plus auto-renewal only in `src/booking/shared/Booking.Shared/Models/MarketplaceBookingSubscription.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T019 [US1] Preserve organization billing-cycle slicing while removing booking-cadence branches from invoice and arrears planning in `src/booking/shared/Booking.Shared/Services/BookingInvoiceService.cs`, `src/booking/shared/Booking.Shared/Services/OrganizationArrearsBillingPlannerService.cs`, and `src/booking/shared/Booking.Shared/Services/OrganizationArrearsChargeSegmentService.cs`
- [X] T020 [US1] Update recurring invoice and provider schedule mapping to use purchase cadence for term semantics and organization billing cycle for split slices.
- [X] T021 [US1] Remove `BookingCadence` and removed cadence values from marketplace and booking integration fixtures, test builders, refund fixtures, and product-version helpers under `src/booking/**/Fixtures`, `src/booking/**/UnitTests`, and `src/marketplace/**/IntegrationTests`
- [X] T022 [US1] Update host and spaces pricing editors to render one cadence field and the ten supported choices in `src/web/apps/webapp-host/src/components/organization/single-choice-product-pricing-cadence.tsx`, `src/web/apps/webapp-spaces/src/components/organization/single-choice-product-pricing-cadence.tsx`, and related product editor files
- [X] T024 [US1] Add structured logs for purchase-term validation, renewal/no-renewal decisions, and billing-slice branch decisions in the changed marketplace and booking services.

**Checkpoint**: User Story 1 independently supports the complete offer-term lifecycle and is safe to demonstrate as the MVP.

## Phase 4: User Story 2 - Book Any Valid Duration Within Offer Limits (Priority: P1)

**Goal**: Validate individual booking duration from the selected `From`/`Until` interval and min/max limits, without cadence increments.

**Independent Test**: Submit below-minimum, boundary, in-range, above-maximum, reversed, and equal intervals; verify duration validation and then existing opening-hours, availability, and conflict rules.

### Tests for User Story 2

- [X] T025 [P] [US2] Add unit tests for inclusive min/max interval validation, equal/reversed times, and daylight-saving-crossing duration calculations.
- [X] T026 [P] [US2] Update marketplace booking and opening-hours tests to prove cadence no longer controls duration increments.
- [X] T027 [P] [US2] Cover arbitrary duration validation and preserve availability/conflict branches, using existing persistence-boundary coverage where needed.

### Implementation for User Story 2

- [X] T028 [US2] Remove cadence-based duration-step and booking-window branches while preserving min/max validation in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs` and `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`
- [X] T029 [US2] Ensure booking request models, GraphQL inputs, and date-time picker data flow across every affected web app carry `From` and `Until` without any booking cadence field in `src/booking/apis/Booking.Api/schema.graphqls` and all matching booking-flow files under `src/web/apps/` (resolve exact files during implementation)
- [X] T030 [US2] Preserve opening-hours, resource-availability, and conflict validation after duration validation in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingOpeningHoursService.cs`, `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`, and related services
- [X] T031 [US2] Add structured duration-validation logs with pricing context and non-sensitive rejection reasons in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`.

**Checkpoint**: User Story 2 accepts arbitrary valid date-time durations independently of purchase cadence.

## Phase 5: User Story 3 - Purchase Cadence-Free Credit Entitlements (Priority: P1)

**Goal**: Represent entitlements with `NotSet`/null cadence and keep them out of subscription and recurring purchase processing.

**Independent Test**: Create, purchase, use, expire, and inspect an entitlement; verify cadence-free serialization and no renewal/recurring processing while credit, validity, available-day, and duration rules remain active.

### Tests for User Story 3

- [X] T032 [P] [US3] Update entitlement creation, cancellation, expiry, claim, and concurrency tests to assert `NotSet`/null cadence rather than a hardcoded one-time cadence in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements` and `src/booking/domain/Booking.Domain.IntegrationTests/Services`
- [X] T033 [P] [US3] Cover entitlement projection and serialization through the updated shared/event/GraphQL models and affected entitlement test fixtures.
- [X] T034 [P] [US3] Prove entitlement exclusion from renewal and recurring processing with unit tests and explicit renewal guards.

### Implementation for User Story 3

- [X] T035 [US3] Remove entitlement hardcoded cadence defaults and use the project’s `NotSet`/null representation in entitlement creation, mapping, and persistence paths under `src/booking/shared/Booking.Shared/Services/Entitlements`, `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs`, and shared model mappers
- [X] T036 [US3] Exclude entitlement fulfillment from subscription renewal and recurring purchase-cadence branches while preserving credit quantity, validity, available days, and min/max duration rules in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs`, entitlement services, and workflow activities
- [X] T037 [US3] Update entitlement GraphQL, event, and frontend models in every affected web app to omit cadence-based UI/inputs while retaining credit and validity fields.
- [X] T038 [US3] Add structured entitlement logs for cadence-free creation and explicit exclusion from renewal/recurring processing.

**Checkpoint**: User Story 3 independently supports the complete non-cadenced entitlement lifecycle.

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Regenerate, verify, document, and close all cross-surface references.

- [X] T039 [P] Audit the entire repository for `BookingCadence` and removed cadence values, then remove or update every remaining reference across backend source, persistence, migrations, events, GraphQL/OpenAPI contracts, generated artifacts, projections, workflows, fixtures, tests, and all directories under `src/web/apps/`, recording intentional historical/spec references in `specs/047-marketplace-pricing-cadence/quickstart.md`
- [X] T040 [P] Search and update every public, customer-facing, and operator-facing documentation surface that refers to removed cadence values or cadence-based duration, including `src/marketplace/docs`, public-web content, help apps, and webapp documentation directories; if no reference exists in a surface, record that review in `specs/047-marketplace-pricing-cadence/quickstart.md`
- [X] T041 Run all required generators and verify generated GraphQL, protobuf, client, and Relay outputs are synchronized using consuming `.csproj` builds for protobuf C# types, `scripts/generate-graphql.sh`, the affected webapp generation scripts, and `pnpm --dir src/web relay`
- [X] T042 Run `git diff --check`, the full unit-test solution build, affected exact-name .NET tests, and affected frontend tests; record results against `specs/047-marketplace-pricing-cadence/quickstart.md`.
- [X] T043 Run the documented manual repository-wide absence check across backend, contracts, generated outputs, every web app, tests, and public documentation for `BookingCadence`, `ProductPricingCadence.OneTime`, `PerMinute`, `Per15Minutes`, `Per30Minutes`, `PerHour`, and `HalfDay`; review `git status` and generated diffs in `specs/047-marketplace-pricing-cadence/quickstart.md`

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1) has no implementation dependency.
- Foundational (Phase 2) depends on Setup and blocks all user stories.
- User Stories 1, 2, and 3 depend on Foundational; after T010/T011, their tests and implementation can proceed in parallel when files do not overlap.
- Polish (Phase 6) depends on all desired stories and final source-contract changes.

### User Story Dependencies

- **US1**: Foundational only; MVP and term semantics.
- **US2**: Foundational only; duration behavior is independent of US1 except for shared model compilation.
- **US3**: Foundational only; entitlement lifecycle is independent of US2 and uses the shared cadence model from Phase 2.

### Parallel Execution Examples

```text
After Phase 2:
- T013, T014, T015, and T016 can run in parallel as separate test surfaces.
- T022 and T023 can run in parallel as host/spaces and customer frontend work.
- T025, T026, and T027 can run in parallel as duration test surfaces.
- T032, T033, and T034 can run in parallel as entitlement test surfaces.
- T039 and T040 can run in parallel during polish.
```

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational phases.
2. Complete User Story 1, including regenerated contracts and focused tests.
3. Validate the offer-term lifecycle independently.
4. Then deliver User Stories 2 and 3 incrementally.

### Delivery Order

1. Shared model/source contracts and generation.
2. Offer-term and renewal behavior (US1).
3. Arbitrary date-time duration validation (US2).
4. Cadence-free entitlements (US3).
5. Repository-wide cleanup, documentation, regeneration, and verification.

### Notes

- Every task uses the required checklist format with a sequential ID, optional `[P]` marker, required story label in story phases, and an exact file path.
- Generated files must be regenerated from source definitions; do not hand-edit generated outputs.
- Tests must be executed and their result distinguished from build success.
