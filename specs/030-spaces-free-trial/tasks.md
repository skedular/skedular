# Tasks: Skedular Spaces 14-Day Free Trial

**Input**: Design documents from `/specs/030-spaces-free-trial/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Tests**: Unit, integration, contract, and frontend tests are required by the feature specification and project constitution. Write each story's tests first and confirm they fail for the intended reason before implementation.

**Organization**: Tasks are grouped by user story so each increment can be implemented and validated independently after the shared foundation is complete.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it changes different files and has no incomplete dependency.
- **[Story]**: Maps the task to a user story in `spec.md`.
- Every task names the exact file or directory it changes.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Record the generated and hand-written surfaces before implementation begins.

- [X] T001 Create the implementation, generation, and enforcement-boundary inventory in `specs/030-spaces-free-trial/generated-surface-inventory.md`, mapping each contract source to its generated outputs/command and every administrator, customer, marketplace, recurring, import, automation/job, and direct-service booking creation path to its owning gate and test
- [X] T002 [P] Add deterministic trial-clock and organization fixture conventions to `specs/030-spaces-free-trial/quickstart.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the durable trial state, portable access policy, and cross-domain projection required by every story.

**CRITICAL**: Complete this phase before starting any user-story implementation.

- [X] T003 [P] Add failing evaluator boundary/action-matrix tests and a focused less-than-5-ms-p95 performance test in `src/shared/Api.Shared.Services.UnitTests/Offering/SpacesAccessEvaluatorTests/EvaluateShould.cs` and `src/shared/Api.Shared.Services.UnitTests/Offering/SpacesAccessEvaluatorTests/EvaluatePerformanceShould.cs`
- [X] T004 [P] Add failing status and reason-name mapping unit tests in `src/shared/Api.Shared.Services.UnitTests/Offering/SpacesSubscriptionStatusExtensionsTests/GetNameShould.cs` and `src/shared/Api.Shared.Services.UnitTests/Offering/SpacesAccessReasonCodeExtensionsTests/GetNameShould.cs`
- [X] T005 Implement the shared access contracts in `src/shared/Api.Shared.Services/Offering/SpacesAccessAction.cs`, `src/shared/Api.Shared.Services/Offering/SpacesAccessDecision.cs`, `src/shared/Api.Shared.Services/Offering/SpacesAccessReasonCode.cs`, and `src/shared/Api.Shared.Services/Offering/SpacesSubscriptionStatus.cs`
- [X] T006 Implement the action-aware, caller-clocked `SpacesAccessEvaluator` in `src/shared/Api.Shared.Services/Offering/SpacesAccessEvaluator.cs` without adding APIs unavailable to `netstandard2.0`
- [X] T007 Add trial and billing-boundary fields to the Organization persistence entities in `src/organization/shared/Organization.Shared/Database/Entities/Organization.cs` and `src/organization/shared/Organization.Shared/Database/Entities/OrganizationOffering.cs`
- [X] T008 [P] Add `SpacesTrialStartedAt`, `SpacesBillingStartsAt`, and derived trial dates to domain models in `src/organization/shared/Organization.Shared/Models/Organization.cs`, `src/organization/shared/Organization.Shared/Models/OrganizationOffering.cs`, and `src/shared/Api.Shared.Services/Models/Offering.cs`
- [X] T009 Update Organization entity/model and offering event mappings in `src/organization/shared/Organization.Shared/Mappers/EntityMapper.cs` and `src/organization/shared/Organization.Shared/Mappers/EventMapper.cs`
- [X] T010 Create the schema-only `src/organization/shared/Organization.Shared/Database/Migrations/AddSpacesTrialState.cs` and update `src/organization/shared/Organization.Shared/Database/Migrations/OrganizationDbContextModelSnapshot.cs` for immutable trial start and nullable bridge billing start without performing data backfill in the migration
- [X] T011 [P] Add repository-backed schema persistence tests for Free Spaces, paid Spaces, and Teams-only organizations in `src/organization/domain/Organization.Domain.IntegrationTests/Persistence/SpacesTrialStatePersistenceShould.cs`
- [X] T012 Extend the offering payload in `api-definitions/events/skedular/organization_v1_value.proto` with nullable Spaces trial start/end, Spaces enabled, and next billing date fields, then immediately run `api-definitions/events/generate.sh` before changing consumer code
- [X] T013 [P] Add failing Organization event-mapping tests for trial and billing fields in `src/organization/shared/Organization.Shared.UnitTests/Mappers/EventMapperTests/MapOrganizationOfferingShould.cs`
- [X] T014 [P] Add failing Booking and Location projection mapping tests in `src/booking/processors/Booking.Processors.UnitTests/Mappers/EventMapperTests/MapOrganizationShould.cs` and `src/location/processors/Location.Processors.UnitTests/Mappers/EventMapperTests/MapOrganizationShould.cs`
- [X] T015 Extend the shared JSON offering projection consumed by `src/booking/shared/Booking.Shared/Database/Entities/Organization.cs` and `src/location/shared/Location.Shared/Database/Entities/Organization.cs` with durable trial inputs in `src/shared/Api.Shared.Services/Models/Offering.cs`
- [X] T016 Project the durable trial and billing fields in `src/booking/processors/Booking.Processors/Mappers/EventMapper.cs`, `src/booking/processors/Booking.Processors/Subscribers/OrganizationSubscriber.cs`, `src/location/processors/Location.Processors/Mappers/EventMapper.cs`, and `src/location/processors/Location.Processors/Subscribers/OrganizationSubscriber.cs`
- [X] T017 Verify JSONB projection compatibility through mapper round-trip tests in `src/booking/processors/Booking.Processors.UnitTests/Mappers/EventMapperTests/MapOrganizationShould.cs` and `src/location/processors/Location.Processors.UnitTests/Mappers/EventMapperTests/MapOrganizationShould.cs`; no Booking or Location migration is required because the offering projection remains schema-less JSONB
- [X] T018 Register the shared Spaces access evaluator in `src/shared/Api.Shared.Services/Extensions.cs`
- [X] T019 Define structured log event IDs and stable properties for initialization, creation-date fallback, access decisions, expiry, projection, upgrade, bridge, billing, and Teams bypass in `src/organization/shared/Organization.Shared/Logging/SpacesTrialLogEvents.cs`, `src/booking/shared/Booking.Shared/Logging/SpacesTrialLogEvents.cs`, and `src/location/shared/Location.Shared/Logging/SpacesTrialLogEvents.cs`

**Checkpoint**: Durable state and local policy evaluation are available in Organization, Booking, and Location without projecting a time-sensitive status.

---

## Phase 3: User Story 1 - Use Spaces During the Trial (Priority: P1) MVP

**Goal**: A new or existing Spaces organization can use all current Free capabilities for exactly 14 days while retaining the existing 100-booking-instance monthly limit.

**Independent Test**: Create a Spaces Free organization, advance a fixed clock within day 14, verify bookings are allowed through instance 100 and rejected above the existing monthly limit; repeat for a Teams-only organization and verify no trial behavior is applied.

### Tests for User Story 1

- [X] T020 [P] [US1] Add failing Free catalog tests proving the plan is a 14-day trial with the existing 100-booking-instance monthly cap in `src/organization/shared/Organization.Shared.UnitTests/Services/Pricing/SpacesPricingCatalogTests/GetCatalogShould.cs`
- [X] T021 [P] [US1] Add failing trial-initialization tests for new Spaces organizations and first Spaces enablement, asserting structured initialization/outbox logs and sensitive-data exclusion, in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationSpacesSubscriptionServiceTests/UpdateAsyncShould.cs`
- [X] T022 [P] [US1] Add failing access-first quota tests for active and expiring trials, asserting the existing 100-booking monthly limit and safe decision logs, in `src/booking/shared/Booking.Shared.UnitTests/Services/SpacesBookingQuotaServiceTests/CanCreateBookingInstancesShould.cs`
- [X] T023 [P] [US1] Cover active-trial private, marketplace, and recurring booking creation through the service/quota and recurring-activity suites under `src/booking/shared/Booking.Shared.UnitTests`
- [X] T024 [P] [US1] Cover immutable trial persistence, creation-date fallback, outbox mapping, unchanged `SpacesFreeTierV1`, and Teams exclusion across Organization repository integration and service/event unit suites; no separate Organization.Api integration project exists
- [X] T025 [US1] Add repository-backed Booking integration coverage proving the active trial accepts instance 100, rejects instance 101, and leaves trial dates unchanged in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesRecurringBookingQuotaShould.cs`

### Implementation for User Story 1

- [X] T026 [P] [US1] Keep the Spaces Free offering at 100 booking instances per month and add trial-specific product copy in `src/shared/Api.Shared.Services/Models/Offering.cs`
- [X] T027 [US1] Update the Organization Spaces pricing catalog to expose the 14-day trial and nullable booking cap in `src/organization/shared/Organization.Shared/Services/Pricing/SpacesPricingCatalogProvider.cs`
- [X] T028 [US1] Set `SpacesTrialStartedAt` once during Marketplace/Spaces organization creation in `src/organization/apis/Organization.Api/GraphQL/Organization/RootMutation.cs` and its backing Organization creation service
- [X] T029 [US1] Set an absent trial start once when Spaces is first enabled, while preserving it across plan changes and re-enablement, in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [X] T030 [US1] Implement creation-date fallback for existing `SpacesFreeTierV1` organizations in Organization event/status mapping without a hosted backfill, new offering version, subscription migration, or synchronous cross-domain API call
- [X] T031 [US1] Make `SpacesBookingQuotaService` evaluate access before quota, retain usage counts for active/expiring Free trials, and bypass usage only when already expired in `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`
- [X] T032 [US1] Wire active-trial decisions through private, marketplace, and recurring creation boundaries in `src/booking/shared/Booking.Shared/Services/PrivateBookingService.cs`, `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`, and `src/booking/shared/Booking.Shared/Activities/PrivateRecurringBookingIntegrations.cs`
- [X] T033 [US1] Emit structured initialization, creation-date-fallback, active-trial quota, and product-scoped bypass logs in the owning Organization, Booking, Location, and Marketplace services

**Checkpoint**: The 14-day trial is usable, immutable, retains the existing monthly booking limit, and is isolated from Teams.

---

## Phase 4: User Story 2 - Understand Trial State and Time Remaining (Priority: P1)

**Goal**: Every authenticated Spaces client receives a clear status and remaining days, and operators see persistent status, warnings, and upgrade guidance.

**Independent Test**: Query organizations at 14 days, 4 days, 3 days, less than 1 day, and the exact expiry boundary; verify API status/remaining-day values and the matching Spaces shell banner/card without relying on booking usage.

### Tests for User Story 2

- [X] T034 [P] [US2] Add failing Organization GraphQL unit tests for subscription status/reason choices, trial fields, member/support authorization, read-only trial-date immutability, and safe structured logs in `src/organization/apis/Organization.Api.UnitTests/GraphQL/Pricing/RootQueryTests/PricingCatalogChoicesShould.cs` and `src/organization/apis/Organization.Api.UnitTests/GraphQL/Pricing/RootQueryTests/OrganizationSpacesSubscriptionShould.cs`
- [X] T035 [P] [US2] Cover boundary timestamps, ceiling-based remaining days, authorization, and read-only visibility through evaluator and Organization GraphQL/service tests because no separate Organization.Api integration project exists
- [X] T036 [P] [US2] Add tested root-shell presentation policy for active, 3-day warning, exact expiry, inaccessible, paid, bridge, legacy, and singular-day copy in `src/web/apps/webapp-spaces/src/components/rootShell/spaces-subscription-presentation.test.ts`
- [X] T037 [P] [US2] Add trial status-card tests for active, expired, paid-inactive, and missing-state responses in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-spaces-quota-status.test.tsx`; paid/bridge/legacy shell mappings are covered by T036

### Implementation for User Story 2

- [X] T038 [P] [US2] Add GraphQL details types for Spaces subscription status and access reason choices in `src/organization/apis/Organization.Api/GraphQL/Pricing/SpacesSubscriptionStatusDetails.cs` and `src/organization/apis/Organization.Api/GraphQL/Pricing/SpacesAccessReasonDetails.cs`
- [X] T039 [US2] Extend `organizationSpacesSubscription` with status, trial timestamps, remaining days, access booleans, bridge state, next billing date, and reason in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`
- [X] T040 [US2] Expose queryable Spaces status and reason choices from `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`, then run `scripts/generate-graphql.sh` and `src/web/apps/webapp/scripts/generate.sh` before implementing operator frontend consumers
- [X] T041 [US2] Add the shared Spaces subscription query/context consumed by the operator shell in `src/web/apps/webapp-spaces/src/components/rootShell/spaces-subscription-context.tsx`
- [X] T042 [US2] Render a persistent trial/expiry banner with remaining days and upgrade CTA in `src/web/apps/webapp-spaces/src/components/rootShell/root-shell.tsx`
- [X] T043 [US2] Replace Free booking-quota messaging with trial status and exact expiry messaging in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-spaces-quota-status.tsx`
- [X] T044 [US2] Add subscription-page warning and billing-state copy in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-admin-subscriptions-section.tsx`
- [X] T045 [US2] Add structured warning-threshold, expiry-observation, and missing-state logs to the status resolution path in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs`

**Checkpoint**: API and operator UI present one consistent, clock-derived subscription status and warning experience.

---

## Phase 5: User Story 3 - Enforce Expiration Without Losing Data (Priority: P1)

**Goal**: At expiry, new commitments and product mutations are blocked while reads, exports, account/upgrade actions, and protective cancellation/refund/closure actions remain available; all data and public listings are preserved.

**Independent Test**: Seed an organization with listings, configuration, future bookings, and recurring commitments; advance to the exact expiry instant; verify all new booking and mutation paths fail with an access error, protective actions and reads still work, listings remain visible but unavailable, and persisted data is unchanged.

### Tests for User Story 3

- [X] T046 [P] [US3] Add failing Booking API tests distinguishing expired-access errors from paid-plan quota errors and asserting safe structured denial logs in `src/booking/apis/Booking.Api.UnitTests/GraphQL/Booking/BookingPayloadTests/SpacesAccessErrorShould.cs` and `src/booking/apis/Booking.Api.UnitTests/GraphQL/Booking/RootMutationTests/SpacesTrialExpiryShould.cs`
- [X] T047 [P] [US3] Cover exact expiry denial plus protective private/marketplace cancellation and refund behavior in the Booking quota/service suites
- [X] T048 [P] [US3] Cover recurring and automation suppression at the shared creation boundary, including workflow survival and cleanup, in `src/booking/shared/Booking.Shared.UnitTests/Activities/BookingIntegrationsTests/AdjustRequiredResourcesForPrivateRecurringBookingAsyncShould.cs` and the marketplace subscription activity suite
- [X] T049 [P] [US3] Add failing Location authorization tests for expired create/modify versus read access in `src/location/apis/Location.Api.UnitTests/Services/Authorization/OrganizationOfferingServiceTests/CanCreateLocationShould.cs`
- [X] T050 [P] [US3] Reconciled with T059: Organization owns account/billing/upgrade operations, while Booking, Location, and Marketplace own and test operational authorization
- [X] T051 [P] [US3] Cover projected expiry enforcement and preservation through repository-backed Booking integration plus Location authorization/projection tests; protective actions remain covered in owning service suites
- [X] T052 [P] [US3] Cover visible-but-unavailable customer storefront behavior and paid-access restoration through public availability resolver and customer component/form tests
- [X] T053 [P] [US3] Cover operator blocked controls, preserved subscription access, warnings, and customer stale-denial behavior with component tests; full-stack browser execution is recorded separately in quickstart because the stack is not available in this validation environment

### Implementation for User Story 3

- [X] T054 [P] [US3] Add a typed Spaces access error model and GraphQL details type in `src/booking/apis/Booking.Api/GraphQL/Booking/SpacesAccessErrorDetails.cs`
- [X] T055 [US3] Add `accessError` to `BookingPayload` while retaining `quotaError` for paid quotas in `src/booking/apis/Booking.Api/GraphQL/Booking/BookingPayload.cs`
- [X] T056 [US3] Enforce action-aware expiry at every administrator, customer, marketplace, import, automation/job, direct-service, and new-instance Booking boundary inventoried by T001, centered on `src/booking/apis/Booking.Api/Services/PrivateBookingService.cs`, `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs`, and `src/booking/shared/Booking.Shared/Services/SpacesBookingQuotaService.cs`
- [X] T057 [US3] Prevent expired recurring renewals and replacement commitments while preserving cancellation/payment/refund cleanup in `src/booking/shared/Booking.Shared/Activities/PrivateRecurringBookingIntegrations.cs`
- [X] T058 [US3] Enforce create/modify expiry decisions while retaining reads in `src/location/apis/Location.Api/Services/Authorization/OrganizationOfferingService.cs`, `src/location/apis/Location.Api/GraphQL/Location/RootMutation.cs`, `src/location/apis/Location.Api/GraphQL/Resource/RootMutation.cs`, and `src/location/apis/Location.Api/GraphQL/FloorPlan/RootMutation.cs`
- [X] T059 [US3] Verify Organization mutations in scope are account, billing, and upgrade operations that remain available; enforce Spaces operational mutations in their owning Booking and Location domains rather than introducing a redundant Organization authorization API
- [X] T060 [US3] Extend the federated `Organization` type with neutral public `spacesPublicBookingAvailability` in the Booking subgraph under `src/booking/apis/Booking.Api/GraphQL/Organization/`, resolving only from Booking's local replicated state with no synchronous cross-domain API call; then run `scripts/generate-graphql.sh` and `src/web/apps/webapp/scripts/generate.sh` before implementing customer frontend consumers
- [X] T061 [US3] Query public booking availability and disable booking/subscription CTAs with neutral copy in `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-location-card.tsx`, `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-hero.tsx`, and `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscribe-hero.tsx`
- [X] T062 [US3] Handle server-side expiry errors and prevent stale-client submission in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-form.tsx` and `src/web/apps/webapp/src/components/marketplaceProductSubscription/marketplace-product-subscribe-form.tsx`
- [X] T063 [US3] Disable operator create/modify controls after expiry while preserving navigation, exports, cancellation, refunds, and upgrade actions through the root blocked state and create controls in `src/web/apps/webapp-spaces`
- [X] T064 [US3] Emit structured allow/deny decisions, reason codes, recurring suppression, projection, and protective-action logs in the owning Organization, Booking, and Location status/enforcement services
- [X] T065 [US3] Add repository-backed exact-expiry preservation coverage proving existing booking rows remain queryable and unchanged in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/SpacesRecurringBookingQuotaShould.cs`; configuration/listing preservation follows the no-write evaluator boundary

**Checkpoint**: Expiry is enforced by the backend across operator and customer paths without deleting or hiding organization data.

---

## Phase 6: User Story 4 - Upgrade and Resume Operations (Priority: P2)

**Goal**: An expired organization can explicitly upgrade with a payment method, resume immediately in a complimentary bridge, and receive its first full calendar-month charge on the next first day of the month.

**Independent Test**: Upgrade an expired organization midmonth, verify immediate access and no bridge charge, run renewal on the next first day and verify the upcoming full-month offering is charged once, then cover cancellation before that charge and confirm return to expired trial without retroactive billing.

### Tests for User Story 4

- [X] T066 [P] [US4] Add upgrade validation tests for explicit plan selection, payment method, paid-update idempotency, preserved trial history, and retry-safe failure behavior in `src/organization/apis/Organization.Api.UnitTests/Services/Pricing/OrganizationSpacesSubscriptionServiceTests/UpdateAsyncShould.cs`
- [X] T067 [P] [US4] Verify Spaces-specific and generic offering paths reuse the same repository, outbox, payment, workflow-id, and renewal primitives without forcing unlike transition semantics into one service
- [X] T068 [P] [US4] Cover bridge-row non-charging, upcoming full-month charging, idempotency, and safe lifecycle logs across Organization activity, workflow, and payment tests
- [X] T069 [P] [US4] Add activity coverage proving the bridge renews into the next full calendar month and returns the new offering id for charging in `src/organization/shared/Organization.Shared.UnitTests/Activities/OrganizationOfferingsTests/RenewAutoRenewableOrganizationOfferingAsyncShould.cs`
- [X] T070 [P] [US4] Cover midmonth bridge persistence, first-of-month full-period renewal, duplicate/idempotent behavior, cancellation, retry, and local-projection restoration across repository integration and focused Organization service/activity tests
- [X] T071 [P] [US4] Cover operator upgrade availability, payment-method requirement, bridge/next-billing presentation, retry, and resumed access through subscription component and shared status-presentation tests

### Implementation for User Story 4

- [X] T072 [US4] Retain `OrganizationSpacesSubscriptionService` as the product-specific transition coordinator while reusing generic repository, outbox, payment, workflow-id, and renewal primitives; direct delegation was rejected because the complimentary bridge has different persistence and billing semantics
- [X] T073 [US4] Require explicit paid selection and a valid payment method before trial activation, leaving failed/incomplete upgrades blocked and safely retryable, in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [X] T074 [US4] Persist an immediate complimentary bridge through month-end with `SpacesBillingStartsAt` set to the next first day in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [X] T075 [US4] Add the Spaces bridge-boundary workflow branch that creates and charges the upcoming full-month offering rather than the expiring bridge row in `src/organization/shared/Organization.Shared/Workflows/ScheduleRenewOrganizationOffering.cs`
- [X] T076 [US4] Make bridge activation, boundary charging, retry, and duplicate signals idempotent in `src/organization/shared/Organization.Shared/Activities/OrganizationOfferings.cs`
- [X] T077 [US4] Return cancellation-before-first-charge organizations to expired trial without retroactive charging in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationSpacesSubscriptionService.cs`
- [X] T078 [US4] Map bridge and next-billing state into subscription status resolution and Organization events in `src/organization/apis/Organization.Api/GraphQL/Pricing/RootQuery.cs` and `src/organization/shared/Organization.Shared/Mappers/EventMapper.cs`
- [X] T079 [US4] Implement upgrade, payment-method, complimentary-period, and next-charge messaging in `src/web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-admin-subscriptions-section.tsx`
- [X] T080 [US4] Emit structured upgrade, bridge, charge, retry, cancellation, and billing-boundary logs in `src/organization/apis/Organization.Api/Services/Pricing/OrganizationSpacesSubscriptionService.cs`, `src/organization/shared/Organization.Shared/Workflows/ScheduleRenewOrganizationOffering.cs`, and `src/organization/shared/Organization.Shared/Activities/OrganizationOfferings.cs`

**Checkpoint**: Paid conversion restores access immediately and aligns the first charge to the next calendar-month cycle without changing existing paid renewals.

---

## Phase 7: User Story 5 - See Accurate Public Pricing (Priority: P2)

**Goal**: Public Spaces pricing consistently describes a 14-day trial and never implies a permanent Free tier, while Teams pricing remains unchanged.

**Independent Test**: Render all public pricing entry points and machine-readable content; verify Spaces uses trial language with the existing 100-booking monthly limit and snapshot/negative tests prove Teams copy and prices are unchanged.

### Tests for User Story 5

- [X] T081 [P] [US5] Add failing Spaces trial and Teams regression assertions in `src/web/apps/public-web/tests/pricing-catalog-rendering.test.ts`
- [X] T082 [P] [US5] Add failing product chooser and machine-readable copy assertions in `src/web/apps/public-web/tests/pricing-product-chooser.test.ts` and `src/web/apps/public-web/tests/llms-content.test.ts`

### Implementation for User Story 5

- [X] T083 [US5] Replace permanent-Free and booking-limit Spaces copy with 14-day trial messaging in `src/web/apps/public-web/src/data/pricing-catalog/pricing-catalog.ts` and `src/web/apps/public-web/src/data/pricing.ts`
- [X] T084 [US5] Update Spaces pricing page, FAQ, SEO metadata, and upgrade calls to action in `src/web/apps/public-web/src/pages/pricing/[product].astro` and related Spaces content components under `src/web/apps/public-web/src/`
- [X] T085 [P] [US5] Update machine-readable Spaces plan descriptions in `src/web/apps/public-web/src/pages/llms.txt.ts` and `src/web/apps/public-web/src/pages/llms-full.txt.ts`
- [X] T086 [US5] Audit and remove remaining permanent-Free or 100-booking Spaces claims while preserving Teams content under `src/web/apps/public-web/`

**Checkpoint**: Human-facing and machine-readable public content accurately presents Spaces as a 14-day trial and leaves Teams untouched.

---

## Phase 8: Polish & Cross-Cutting Validation

**Purpose**: Regenerate contracts, prove unaffected behavior, and validate the complete feature.

- [X] T087 Re-run `api-definitions/events/generate.sh` as a final drift check and verify it produces no unexpected changes beyond the outputs already generated immediately after `api-definitions/events/skedular/organization_v1_value.proto` changed
- [X] T088 Re-run `scripts/generate-graphql.sh` as a final drift check and review the per-API, composed, gateway, and integration GraphQL schemas under `api-definitions/graphql/` and affected API projects
- [X] T089 Re-run `src/web/apps/webapp/scripts/generate.sh` as a final drift check and verify checked-in Relay artifacts under `src/web/apps/webapp/src/queries/__generated__/` and `src/web/apps/webapp-spaces/src/queries/__generated__/` are current
- [X] T090 [P] Add explicit paid-plan and legacy-plan regression coverage in the Booking quota and Organization renewal/activity suites
- [X] T091 [P] Add explicit Teams creation, pricing, and mutation regression coverage in Organization tests and `src/web/apps/public-web/tests/pricing-catalog-rendering.test.ts`; no Organization.Api integration project exists
- [X] T092 Run the focused backend unit, performance, integration, system, and logging-verification test commands and record results in `specs/030-spaces-free-trial/quickstart.md`
- [X] T093 Run the focused `webapp-spaces`, customer `webapp`, and `public-web` lint/test/build commands and record results in `specs/030-spaces-free-trial/quickstart.md`; browser-level validation remains tracked separately when the full local stack is available
- [X] T094 Run `make generate`, inspect for stale or hand-edited generated outputs, and record the clean regeneration result in `specs/030-spaces-free-trial/quickstart.md`
- [X] T095 Run `graphify update .` and verify the updated code relationships in `graphify-out/`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 — Setup**: No dependencies.
- **Phase 2 — Foundational**: Depends on Phase 1 and blocks all stories.
- **Phase 3 — US1**: Depends on Phase 2; this is the recommended MVP increment.
- **Phase 4 — US2**: Depends on Phase 2 and consumes the evaluator/state established by US1, but its status contract can be tested independently with seeded state.
- **Phase 5 — US3**: Depends on Phase 2; final UI integration uses the status contract from US2.
- **Phase 6 — US4**: Depends on US2 status fields and US3 expired-state enforcement.
- **Phase 7 — US5**: Depends only on Phase 2 and may proceed alongside US1–US4 after the product language is settled.
- **Phase 8 — Polish**: Depends on all selected stories and must run after all contract source changes.

### User Story Dependency Graph

```text
Setup -> Foundation -> US1 (active trial)
                    -> US2 (status) -> US3 (expiry UX) -> US4 (upgrade/bridge)
                    -> US5 (public pricing)
```

### Within Each User Story

1. Add the listed tests and confirm they fail for the expected missing behavior.
2. Implement models/contracts before services and resolvers.
3. Implement backend enforcement before enabling or disabling client controls.
4. Complete integration tests and the story checkpoint before moving to the next dependent story.
5. Regenerate each contract immediately after its source changes and before downstream consumers; Phase 8 only verifies that regeneration is clean.

### Parallel Opportunities

- T001 and T002 can run concurrently.
- T003/T004, T011/T013/T014, and model work on separate domains can run concurrently within Foundation when their direct prerequisites are satisfied.
- All test tasks marked `[P]` within a story can be authored concurrently.
- US5 can run in parallel with backend-heavy US1–US4 after Foundation.
- Organization, Booking, Location, operator app, customer app, and public website implementation tasks can be split across owners where they do not edit the same files.
- T090 and T091 can run concurrently before the final generation and validation pass.

---

## Parallel Examples

### User Story 1

```text
T020 Free catalog tests
T021 Organization initialization tests
T022 Booking access/quota tests
T023 Booking service boundary tests
T024 Organization integration tests
```

### User Story 3

```text
T046 Booking GraphQL contract tests
T049 Location authorization tests
T050 Organization authorization tests
T052 Customer storefront tests
T053 Operator blocked-control tests
```

### User Story 5

```text
T081 Public pricing rendering tests
T082 Product chooser and machine-readable tests
T085 Machine-readable content implementation
```

---

## Implementation Strategy

### MVP First

1. Complete Setup and Foundation.
2. Complete US1 and its checkpoint to deliver a 14-day active trial with the existing Free booking limit.
3. Do not deploy the product change until US2 and US3 are also complete, because status visibility and expiry enforcement are release-critical even though US1 is independently testable.

### Incremental Delivery

1. **Foundation**: Durable state, portable policy, and projections.
2. **US1**: Active trial and removal of Free booking limits.
3. **US2**: Consistent API/operator status and warnings.
4. **US3**: Expiry enforcement, preservation, and public unavailability.
5. **US4**: Explicit paid conversion and calendar-month bridge billing.
6. **US5**: Public pricing accuracy and Teams regression protection.
7. **Polish**: Regeneration, full regression suite, and graph refresh.

## Notes

- `[P]` means different files and no incomplete prerequisite, not merely “could be assigned to someone else.”
- Existing paid subscriptions and all Teams subscription/pricing paths require regression coverage before release.
- Never hand-edit protobuf-generated event classes, exported GraphQL schemas, or Relay artifacts.
- Use repository-layer assertions in integration tests; do not query EF `DbContext` directly.
- Use American English for all user- and operator-facing copy.
- Commit after each task or coherent task group and stop at any checkpoint for independent validation.
