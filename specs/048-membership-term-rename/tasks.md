# Tasks: MembershipTerm Terminology Rename

**Input**: Design documents from `/specs/048-membership-term-rename/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

## Phase 1: Setup

- [X] T001 Inventory every active former `MembershipTerm` reference with `rg` across backend, contracts, generated outputs, web apps, tests, fixtures, logs, and documentation, recording persistence-boundary exceptions in `specs/048-membership-term-rename/quickstart.md`
- [X] T002 Identify authoritative source files and generated outputs for each inventory group in `specs/048-membership-term-rename/research.md`

## Phase 2: Foundational Contract and Model Changes

- [X] T003 [P] Rename the shared domain pricing property and related model terminology in `src/shared/Api.Shared.Services/Models/ProductPricing.cs` and adjacent shared pricing models
- [X] T004 [P] Rename marketplace pricing source definitions and explicit mappings in `src/marketplace/shared/Marketplace.Shared/` while preserving existing values and unset semantics
- [X] T005 [P] Rename booking pricing consumers and event mappings in `src/booking/shared/Booking.Shared/` and `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`
- [X] T006 [P] Update authoritative protobuf/event definitions under `api-definitions/events/skedular/` to expose the membership-term field while preserving field numbering and values
- [X] T007 [P] Update authoritative GraphQL and OpenAPI definitions under `api-definitions/graphql/` and `api-definitions/openapi/` to expose `membershipTerm`
- [X] T008 Add explicit persistence serialization mapping that reads and writes the renamed storage field while exposing `MembershipTerm` in `src/marketplace/shared/` and `src/booking/shared/`
- [X] T009 Add unit coverage for renamed model conversion, null/unset/cadence-free values, invalid values, and storage compatibility in `src/marketplace/apis/Marketplace.Api.UnitTests/` and `src/booking/shared/Booking.Shared.UnitTests/`

## Phase 3: User Story 1 - Understand and Choose a Membership Term (Priority: P1) 🎯 MVP

**Goal**: Customers and hosts see the contractual signup period consistently as “membership term.”

**Independent Test**: Review and test each offer, pricing, subscription, and booking flow in every web app; all active labels, help text, validation, and displayed values use the new term and distinguish billing cycle and booking duration.

- [X] T010 [P] [US1] Update customer webapp pricing editors, forms, validation, and labels under `src/web/apps/webapp/` to use `membershipTerm`
- [X] T011 [P] [US1] Update Host pricing and offer editors under `src/web/apps/host/` to use the membership-term terminology
- [X] T012 [P] [US1] Update Spaces pricing and legacy editor surfaces under `src/web/apps/spaces/` to use the membership-term terminology
- [X] T013 [P] [US1] Update public-web offer, product, help, and documentation surfaces under `src/web/apps/public-web/` and repository documentation
- [X] T014 [US1] Update customer-facing and operator-facing copy, validation messages, comments, and logs associated with the former concept while preserving American English in affected backend and web files
- [X] T015 [US1] Add or update frontend tests and fixtures for membership-term labels, choices, validation, and distinction from billing cycle and booking duration under `src/web/apps/`

## Phase 4: User Story 2 - Preserve Domain Behavior (Priority: P1)

**Goal**: The rename does not alter pricing, renewal, entitlement, booking, billing, invoice, cancellation, refund, or authorization outcomes.

**Independent Test**: Run representative existing and newly created pricing through term, renewal, entitlement, booking, billing, invoice, cancellation, and refund scenarios and compare outcomes with the pre-rename behavior.

- [X] T016 [P] [US2] Rename subscription term and renewal consumers in `src/booking/shared/Models/MarketplaceBookingSubscription.cs` and `src/booking/shared/Services/MarketplaceBookingSubscriptionService.cs`
- [X] T017 [P] [US2] Rename booking-generation, opening-hours, duration, and product-version consumers under `src/booking/shared/Services/`
- [X] T018 [P] [US2] Rename billing, arrears, invoice, Xero, refund, and accounting schedule consumers under `src/booking/shared/Services/` and `src/booking/shared/Models/`
- [X] T019 [P] [US2] Update marketplace validators and product services in `src/marketplace/apis/Marketplace.Api/Services/ProductService.cs` and related validation files
- [X] T020 [US2] Update domain unit tests and fixtures for renewal, billing slices, entitlements, booking generation, invoices, cancellation, and refunds under `src/booking/shared/Booking.Shared.UnitTests/`
- [X] T021 [US2] Update persistence and event integration tests under `src/booking/domain/Booking.Domain.IntegrationTests/` to verify existing storage-field compatibility and unchanged projected behavior

## Phase 5: User Story 3 - Maintain Repository-Wide Contract Consistency (Priority: P1)

**Goal**: All contracts and generated artifacts use the new terminology without stale active names.

**Independent Test**: Regenerate all affected outputs, build affected consumers, and run the repository-wide search allowing only documented persistence-compatibility or historical references.

- [X] T022 [US3] Run the event-generation portion of `api-definitions/generate.sh` and verify generated event outputs match the updated protobuf sources
- [X] T023 [US3] Run `scripts/generate-graphql.sh` and verify composed, per-API, and system GraphQL schemas expose `membershipTerm`
- [X] T024 [US3] Run `src/web/apps/webapp/scripts/generate.sh` and regenerate Relay artifacts with `pnpm --dir src/web relay`
- [X] T025 [US3] Regenerate affected OpenAPI clients with `api-definitions/openapi/generate.sh` and update generated client consumers
- [X] T026 [US3] Update generated-surface inventories and active specifications under `specs/` to use `MembershipTerm`, preserving only explicitly labeled historical evidence
- [X] T027 [US3] Build affected backend and web consumers and fix all stale identifier, serialization, schema, and generated-artifact references in their authoritative sources

## Phase 6: Polish and Cross-Cutting Validation

- [X] T028 [P] Update structured log assertions and operator diagnostics under `src/booking/` and `src/marketplace/` to use membership-term terminology with correlation context
- [X] T029 [P] Update repository comments, fixtures, test names, help content, and product documentation outside story-specific files using the inventory from T001
- [X] T030 Run focused unit tests, integration tests, frontend tests, and exact discovered test names documented in `specs/048-membership-term-rename/quickstart.md`
- [X] T031 Run the repository-wide absence check for active former `MembershipTerm` references and classify any remaining matches as persistence compatibility or historical evidence
- [X] T032 Run `git diff --check`, inspect generated diffs and `git status`, and record verification results in `specs/048-membership-term-rename/quickstart.md`
- [X] T033 Confirm no database migration, storage-field rename, backfill, or existing-record transformation was introduced in this changeset
- [X] T034 [US3] Build and validate the customer webapp, Host, Spaces, public web, and legacy editor packages after regeneration, recording each result in `specs/048-membership-term-rename/quickstart.md`
- [X] T035 [US3] Build or type-check every generated API client, GraphQL consumer, event consumer, and Relay operation affected by the renamed contract, recording unavailable or zero-test results in `specs/048-membership-term-rename/quickstart.md`

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 is independent setup.
- Phase 2 depends on the inventory and blocks all story work.
- US1, US2, and US3 depend on Phase 2 and can proceed in parallel where files do not overlap.
- Phase 6 depends on all desired story phases and regeneration completion.

### User Story Dependencies

- US1, US2, and US3 are independently testable after Phase 2.
- US1 is the MVP and can be validated before the behavior-preservation and full-repository cleanup phases.

## Parallel Opportunities

- T003–T007 can run in parallel by ownership area.
- T010–T013 can run in parallel by web application.
- T016–T019 can run in parallel by service area.
- T022–T025 can run in parallel only after all authoritative source changes are complete.
- T028–T029 can run in parallel with final focused testing.

## Implementation Strategy

1. Complete inventory and authoritative model/contract changes.
2. Deliver US1 as the MVP, including affected web copy and tests.
3. Complete US2 behavior-preservation coverage.
4. Regenerate and validate US3 across all consumers.
5. Finish repository-wide documentation/log cleanup and verification.
6. Apply and verify the forward database migration.
