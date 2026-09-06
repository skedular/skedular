# Tasks: Automatic Marketplace Renewal Charging

**Input**: Design documents from `/specs/049-marketplace-auto-charge/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/), and [quickstart.md](quickstart.md)

**Tests**: Unit, persistence/concurrency, Stripe/webhook, GraphQL contract, and Relay tests are required by the feature specification and plan. Write the listed tests before their corresponding implementation task and prove the stated boundary rather than duplicating unit coverage in integration tests.

**Critical boundary**: `PurchaseRenewalAuthorization` is a Booking-owned, purchase-specific Stripe authorization. It is not Customer's profile `StripePaymentMethod`; no renewal path may query, select, update, or fall back to the customer-profile card.

## Phase 1: Setup

**Purpose**: Establish the implementation seams and test fixtures without changing payment behavior.

- [ ] T001 [P] Document Stripe test PaymentIntent, Checkout, asynchronous, and Connect fixtures in `src/booking/domain/Booking.Domain.IntegrationTests/Stripe/MarketplaceRenewalStripeFixture.cs`.
- [ ] T002 [P] Add renewal-workflow deterministic ID cases to `src/booking/shared/Booking.Shared.UnitTests/Services/WorkflowIdServiceTests/GenerateMarketplaceRenewalPaymentShould.cs`.
- [ ] T003 [P] Add a Stripe catalog characterization test proving Checkout uses an exact persisted Price mapping while a dynamic renewal sends its frozen amount directly to PaymentIntent in `src/booking/shared/Booking.Shared.UnitTests/Services/StripeProductPricingServiceTests/ResolveMarketplaceRenewalCatalogUseShould.cs`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Create Booking-owned durable state, repository boundaries, lifecycle semantics, and workflow identity required by every purchase type.

**Critical**: Complete this phase before changing either reservation or entitlement renewal behavior.

- [ ] T004 [P] Add purchase-source, authorization, renewal-cycle, payment-attempt, fallback, and lifecycle-event shared models in `src/booking/shared/Booking.Shared/Models/MarketplaceRenewalPayment/`.
- [ ] T005 [P] Add `PurchaseRenewalAuthorization`, `MarketplaceRenewalCycle`, `MarketplaceRenewalPaymentAttempt`, `MarketplaceRenewalFallback`, and `MarketplaceRenewalLifecycleEvent` entities with explicit local foreign keys in `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceRenewalPayment/`.
- [ ] T006 Add EF configurations, required indexes, unique constraints, and concurrency tokens for the renewal entities in `src/booking/shared/Booking.Shared/Database/BookingDbContext.cs`.
- [ ] T007 Add repository interfaces and implementations for atomic cycle acquisition, attempt creation, provider-event deduplication, and event append in `src/booking/shared/Booking.Shared/Repositories/MarketplaceRenewalPayment/`.
- [ ] T008 Add forward-only entity mappings, migration, designer, and model snapshot updates in `src/booking/shared/Booking.Shared/Database/Migrations/`.
- [ ] T009 [P] Add lifecycle transition, authorization eligibility, retryability, and event-idempotency unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRenewalPayment/`.
- [ ] T010 Add a persistence/concurrency integration test for renewal-boundary uniqueness, provider-event deduplication, and one-paid-cycle enforcement in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplaceRenewalPaymentRepositoryShould.cs`.
- [ ] T011 Add `IMarketplaceRenewalPaymentService`, purchase-type adapter contracts, and service registrations in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/`.
- [ ] T012 Add deterministic marketplace-renewal workflow ID generation to `src/booking/shared/Booking.Shared/Services/WorkflowIdService.cs`.
- [ ] T013 Add append-only renewal authorization, payment, fallback, materialization, cancellation, refund, and recovery event projection to `src/booking/shared/Booking.Shared/Repositories/MarketplacePurchaseHistoryRepository.cs`.
- [ ] T014 Add MarketplacePurchaseHistory repository tests proving history derives only from persisted renewal events in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistoryRepositoryShould.cs`.

**Checkpoint**: Booking can durably acquire one renewal cycle, append authoritative history, and coordinate a purchase-type adapter without Customer profile-payment data.

---

## Phase 3: User Story 1 - Reservation Offer Automatic Renewal (Priority: P1) MVP

**Goal**: Renew a card-paid AutoRenew reservation subscription with a purchase-specific Stripe authorization, charge only after pricing validation, and create its next reservation cycle only after Stripe confirmation.

**Independent Test**: A reservation subscription with a valid purchase authorization receives one off-session PaymentIntent and one confirmed next cycle; a price change, missing authorization, 3DS requirement, or failed card creates no reservation access and exposes recovery.

### Tests for User Story 1

- [ ] T015 [P] [US1] Add reservation renewal calculation tests for current pricing, currency, tax, billing mode, membership term, and AutoRenew gating in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRenewalPayment/ReservationRenewalCalculationServiceTests/CalculateShould.cs`.
- [ ] T016 [P] [US1] Add off-session attempt tests covering missing, revoked, expired, detached, insufficient-funds, and authentication-required credentials in `src/booking/shared/Booking.Shared.UnitTests/Activities/StripeIntegrationsTests/CreateMarketplaceRenewalPaymentIntentShould.cs`.
- [ ] T017 [US1] Add reservation workflow replay and webhook-before-workflow integration coverage in `src/booking/domain/Booking.Domain.IntegrationTests/Activities/MarketplaceBookingSubscriptionIntegrationsShould.cs`.

### Implementation for User Story 1

- [ ] T018 [US1] Extend reservation initial Checkout and fallback Checkout creation to collect explicit purchase-specific future-off-session consent, persist only safe Stripe references, and never consult Customer `StripePaymentMethod` in `src/booking/shared/Booking.Shared/Activities/StripeIntegrations.cs`.
- [ ] T019 [US1] Implement reservation commercial snapshot calculation from the renewed purchase and current matching pricing in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/ReservationRenewalCalculationService.cs`.
- [ ] T020 [US1] Implement Stripe-account-context validation and idempotent off-session PaymentIntent creation, including direct/destination charge behavior, in `src/booking/shared/Booking.Shared/Activities/StripeIntegrations.cs`.
- [ ] T021 [US1] Implement reservation renewal orchestration that acquires the cycle, blocks changed price/tax from auto-charge, schedules three automatic attempts over three days, and transitions to fallback/manual recovery in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/ReservationRenewalPaymentService.cs`.
- [ ] T022 [US1] Replace hosted-page-only recurring card renewal orchestration with the idempotent reservation payment workflow in `src/booking/shared/Booking.Shared/Workflows/PayRecurringBookingViaCard.cs`.
- [ ] T023 [US1] Gate reservation-cycle/booking-resource materialization on confirmed renewal payment, retaining availability and booking rules as a separate concern, in `src/booking/shared/Booking.Shared/Workflows/BookMarketplaceBookingSubscriptionResources.cs`.
- [ ] T024 [US1] Reconcile Stripe PaymentIntent succeeded, failed, canceled, processing, and authentication-required outcomes idempotently into the reservation renewal state machine in `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs`.
- [ ] T025 [US1] Add structured logs and durable lifecycle events for reservation evaluation, attempt, confirmation, fallback, retry, and webhook conflict in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/ReservationRenewalPaymentService.cs`.

**Checkpoint**: Reservation AutoRenew works without a new checkout on a valid, purchase-specific Stripe authorization and cannot double-charge or create unpaid bookings.

---

## Phase 4: User Story 2 - Credit-Entitlement Automatic Renewal (Priority: P1)

**Goal**: Apply the shared renewal payment model to AutoRenew credit-entitlement purchases, granting the renewed quantity and validity only after confirmed payment.

**Independent Test**: A valid entitlement authorization yields exactly one paid renewal and one entitlement allocation with the current quantity and validity; every unpaid, failed, canceled, or action-required state yields no new credits.

### Tests for User Story 2

- [ ] T026 [P] [US2] Add entitlement renewal calculation tests for credit quantity, validity window, tax, currency, pricing, and AutoRenew semantics in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRenewalPayment/EntitlementRenewalCalculationServiceTests/CalculateShould.cs`.
- [ ] T027 [P] [US2] Add entitlement payment-confirmation tests proving duplicate workflow/webhook delivery cannot allocate credits twice in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRenewalPayment/EntitlementRenewalPaymentServiceTests/ConfirmPaymentShould.cs`.
- [ ] T028 [US2] Add entitlement renewal persistence, webhook replay, expiry, and unused-credit refund-boundary integration coverage in `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementPurchaseRenewalShould.cs`.

### Implementation for User Story 2

- [ ] T029 [US2] Extend entitlement initial and fallback Checkout completion to capture and replace only the entitlement purchase-specific Stripe authorization in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementPurchaseCheckoutService.cs`.
- [ ] T030 [US2] Implement current entitlement commercial snapshot calculation, preserving credit quantity and validity separately from reservation booking cadence, in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/EntitlementRenewalCalculationService.cs`.
- [ ] T031 [US2] Implement entitlement renewal orchestration using the shared idempotent payment-attempt, retry, fallback, and lifecycle-event services in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/EntitlementRenewalPaymentService.cs`.
- [ ] T032 [US2] Add a deterministic entitlement renewal Temporal workflow and invoke it from the entitlement purchase lifecycle in `src/booking/shared/Booking.Shared/Workflows/RenewMarketplaceEntitlementPurchase.cs`.
- [ ] T033 [US2] Gate entitlement and credit-ledger creation on the atomically confirmed paid cycle in `src/booking/shared/Booking.Shared/Services/EntitlementPurchasePaymentService.cs`.
- [ ] T034 [US2] Extend Stripe webhook reconciliation to dispatch entitlement payment outcomes through the shared payment service in `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs`.
- [ ] T035 [US2] Preserve cancellation/refund separation and calculate entitlement refunds only from unused credits under the applicable policy in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementRefundService.cs`.

**Checkpoint**: Credit-entitlement AutoRenew has the same Stripe, retry, recovery, and audit guarantees as reservations without conflating validity with booking cadence.

---

## Phase 5: User Story 3 - Purchase Authorization, Failure Recovery, and Operations (Priority: P1)

**Goal**: Let authorized customers and administrators view a purchase-specific authorization and renewal state, safely refresh or revoke it, complete fallback checkout, retry eligible payment, and perform authorized manual recovery.

**Independent Test**: A profile-card-only purchase is reported as not ready; a customer can establish purchase authorization through Checkout, receive a three-day fallback after failure, and see Relay state update without a reload; admins can safely recover only authorized cases.

### Tests for User Story 3

- [ ] T036 [P] [US3] Add GraphQL authorization and mutation service tests for purchase-specific authorization, fallback, retry, recovery, and profile-method exclusion in `src/booking/apis/Booking.Api.UnitTests/Services/MarketplaceRenewalPaymentServiceTests/`.
- [ ] T037 [P] [US3] Add GraphQL contract scenarios for readiness, fallback, retry, revoke, manual recovery, and permission denial in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/MarketplaceRenewalPaymentContract.graphql`.
- [ ] T038 [US3] Add a GraphQL contract test runner and assertions for the renewal-payment document in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/MarketplaceRenewalPaymentContractShould.cs`.

### Implementation for User Story 3

- [ ] T039 [US3] Add service-backed authorization, fallback, retry, recovery, and cancellation operations in `src/booking/apis/Booking.Api/Services/MarketplaceRenewalPaymentService.cs`.
- [ ] T040 [US3] Add customer/admin renewal-payment query types, inputs, payloads, permissions, and root fields in `src/booking/apis/Booking.Api/GraphQL/MarketplaceRenewalPayment/`.
- [ ] T041 [US3] Add customer and administrator subscription/entitlement renewal status queries and mutations in `src/web/apps/webapp-spaces/src/queries/marketplaceRenewalPayment.ts`.
- [ ] T042 [US3] Build the shared purchase-specific authorization, automatic-payment status, fallback, retry, and recovery panel using shared typography in `src/web/apps/webapp-spaces/src/components/marketplaceRenewalPayment/marketplace-renewal-payment-panel.tsx`.
- [ ] T043 [P] [US3] Add unit tests for renewal payment status, failure copy, available actions, and no-profile-method messaging in `src/web/apps/webapp-spaces/src/components/marketplaceRenewalPayment/marketplace-renewal-payment-panel.test.tsx`.
- [ ] T044 [US3] Render the renewal payment panel and Relay mutations on the reservation purchase page in `src/web/apps/webapp-spaces/src/app/organizations/[organizationCustomDomain]/purchases/[subscriptionId]/page.tsx`.
- [ ] T045 [US3] Render the renewal payment panel and Relay mutations on the entitlement purchase page in `src/web/apps/webapp-spaces/src/app/organizations/[organizationCustomDomain]/purchases/entitlements/[purchaseId]/page.tsx`.
- [ ] T046 [US3] Add the administrator renewal-failure queue, status filter, fallback resend, and manual-recovery actions in `src/web/apps/webapp-spaces/src/rootPages/organizations/organization/subscriptions/page.tsx`.
- [ ] T047 [US3] Regenerate Relay artifacts for renewal-payment operations with `src/web/apps/webapp-spaces/src/queries/__generated__/` as output by running `pnpm --dir src/web relay`.

**Checkpoint**: Customers and administrators can understand and recover marketplace renewal payment state without exposing or using the Customer profile payment method.

---

## Phase 6: User Story 4 - Authoritative Renewal and Entitlement History (Priority: P2)

**Goal**: Present persisted payment, renewal, entitlement, cancellation, refund, and recovery events as the source of truth for customer and administrator history.

**Independent Test**: A historical purchase view renders durable lifecycle events in occurrence order even when current aggregate state has changed; it never fabricates a paid renewal from mutable fields or checkout-link creation.

### Tests for User Story 4

- [ ] T048 [P] [US4] Add GraphQL mapper tests proving lifecycle history maps persisted event data rather than current aggregate fields in `src/booking/apis/Booking.Api.UnitTests/GraphQL/MarketplacePurchaseHistory/MarketplaceRenewalLifecycleEventDetailsShould.cs`.
- [ ] T049 [P] [US4] Add customer/admin history rendering tests for succeeded, pending, failed, fallback, canceled, expired, refund, and manual-recovery events in `src/web/apps/webapp-spaces/src/components/marketplaceRenewalPayment/marketplace-renewal-history-list.test.tsx`.

### Implementation for User Story 4

- [ ] T050 [US4] Extend service-side history reads with renewal-cycle and lifecycle-event projections in `src/booking/apis/Booking.Api/Services/MarketplacePurchaseHistoryService.cs`.
- [ ] T051 [US4] Add GraphQL lifecycle-event details and purchase history fields sourced only from persisted events in `src/booking/apis/Booking.Api/GraphQL/MarketplacePurchaseHistory/MarketplaceRenewalLifecycleEventDetails.cs`.
- [ ] T052 [US4] Add a customer/admin renewal history list with event timestamps, safe monetary data, and no inferred labels in `src/web/apps/webapp-spaces/src/components/marketplaceRenewalPayment/marketplace-renewal-history-list.tsx`.
- [ ] T053 [US4] Integrate the renewal history list into reservation and entitlement purchase pages in `src/web/apps/webapp-spaces/src/components/marketplaceRenewalPayment/index.ts`.
- [ ] T054 [US4] Regenerate Relay artifacts for lifecycle-history operations with `src/web/apps/webapp-spaces/src/queries/__generated__/` as output by running `pnpm --dir src/web relay`.

**Checkpoint**: History UI is driven by authoritative persisted events and remains correct across retries, refunds, cancellation, and aggregate-state changes.

---

## Phase 7: Polish and Cross-Cutting Concerns

**Purpose**: Complete migration/rollout safety, observability, contract generation, and full validation.

- [ ] T055 Add rollout feature flags, legacy-purchase readiness classification, and a no-silent-charge migration path in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/MarketplaceRenewalPaymentRolloutService.cs`.
- [ ] T056 Add operational queries and structured log correlation for source, renewal cycle, attempt, workflow, Stripe account, PaymentIntent, and provider event IDs in `src/booking/shared/Booking.Shared/Services/MarketplaceRenewalPayment/MarketplaceRenewalPaymentObservabilityService.cs`.
- [ ] T057 Add production-safe dashboard/alert queries for stuck processing, exhausted retries, fallback expiry, webhook conflicts, and reconciliation-required Connect ownership failures in `src/booking/shared/Booking.Shared/Repositories/MarketplaceRenewalPayment/MarketplaceRenewalPaymentOperationsRepository.cs`.
- [ ] T058 Add migration and rollout integration tests proving existing AutoRenew purchases without a purchase-specific authorization retain their active term and are routed to checkout instead of automatic charge in `src/booking/domain/Booking.Domain.IntegrationTests/Services/MarketplaceRenewalPaymentRolloutShould.cs`.
- [ ] T059 Add Stripe direct-charge and destination-charge account/credential ownership tests that fail closed with no charge and no grant in `src/booking/domain/Booking.Domain.IntegrationTests/Activities/MarketplaceRenewalStripeConnectShould.cs`.
- [ ] T060 Update generated GraphQL schemas from source by running `scripts/generate-graphql.sh` and retain resulting changes under `api-definitions/graphql/skedular/v1/schema.graphql`.
- [ ] T061 Run `make generate` if API definitions changed and retain only generator-produced artifacts under `api-definitions/` and `src/web/apps/webapp-spaces/src/clients/`.
- [ ] T062 Execute the quickstart test matrix and record actual commands/results in `specs/049-marketplace-auto-charge/quickstart.md`.
- [ ] T063 Run `git diff --check` and review the complete renewal-payment diff, generated artifacts, migration, and test results before requesting review.

---

## Dependencies and Execution Order

### Phase dependencies

- Phase 1 has no implementation dependencies.
- Phase 2 depends on Phase 1 and blocks all user stories.
- US1, US2, and US3 may begin after Phase 2; US1 and US2 use the same shared lifecycle but do not depend on each other's purchase-specific implementation.
- US4 depends on the foundational event projection and can begin after Phase 2, but should land after the story event emitters are present for complete end-to-end verification.
- Phase 7 follows all selected story work.

### User story dependency notes

- **US1 (P1)** is the reservation MVP, but its implementation requires the shared authorization lifecycle from Phase 2; its customer recovery UI is delivered in US3.
- **US2 (P1)** shares the payment infrastructure with US1 but independently calculates/creates entitlement cycles and credits.
- **US3 (P1)** exposes shared state and recovery for both purchase types; it must not use Customer profile payment-method surfaces.
- **US4 (P2)** renders the event record emitted by US1–US3 and existing cancellation/refund flows.

### Parallel opportunities

- T001, T002, T004, T005, T009, T015, T016, T026, T027, T036, T037, T043, T048, and T049 are independently parallelizable as marked.
- After T014, separate developers can implement reservation orchestration (T018–T025), entitlement orchestration (T029–T035), and API/UI recovery (T039–T047).
- Run generator tasks only after their source-contract tasks are complete; do not hand-edit generated GraphQL or Relay artifacts.

## MVP Delivery Strategy

1. Complete T001–T014 to establish durable, idempotent purchase-specific renewal payment state.
2. Complete T015–T025 to deliver reservation renewal charging and webhook-safe payment confirmation.
3. Validate the US1 independent test before rollout: a valid authorization is charged once; changes, failures, 3DS, and missing authorization grant no access and use recovery.
4. Add US2 before enabling entitlement AutoRenew, then US3 before broad customer rollout so fallback/recovery is available.
5. Enable legacy purchases only after T055–T059 confirm that no purchase can be charged from a customer-profile card or without explicit purchase-specific consent.
