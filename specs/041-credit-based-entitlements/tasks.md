# Tasks: Credit-Based Booking Entitlements

**Input**: Design documents from `/specs/041-credit-based-entitlements/`
**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Implementation note**: A previous implementation exists for the narrower standalone-purchase version. The first tasks explicitly reconcile it with this clarified scope. Do not create a second entitlement/payment system or a second migration set.

## Phase 1: Setup and Previous-Version Reconciliation

- [x] T001 Audit the current credit/entitlement implementation against `spec.md`, including purchase, grant, booking claim, renewal, refund, operator authorization, Spaces UI, Host UI, and public documentation in `src/booking/`, `src/web/apps/`, and `src/web/apps/public-web/`.
- [x] T002 [P] Classify each existing credit feature change as retain, update, remove, or replace in `specs/041-credit-based-entitlements/research.md`, including the previous standalone-purchase-only workflow and any purchase-time booking behavior.
- [x] T003 [P] Inventory existing reservation-based payment and renewal paths to reuse for token fulfillment in `src/booking/shared/Booking.Shared/Workflows/`, `src/booking/shared/Booking.Shared/Activities/`, `src/booking/apis/Booking.Api/Services/`, and `src/booking/processors/Booking.Processors/`.
- [x] T004 [P] Inventory customer and operator entry points for token booking creation, modification, cancellation, and entitlement administration in `src/web/apps/webapp/`, `src/web/apps/webapp-host/`, and `src/web/apps/webapp-spaces/`.
- [x] T005 [P] Reconcile the implementation scope after the audit by updating previous-version design references that treat automatic renewal as out of scope or model token purchase as a booking in `specs/041-credit-based-entitlements/plan.md`, `data-model.md`, `contracts/`, and `quickstart.md` before implementation begins.

## Phase 2: Foundational Persistence and Contracts

- [x] T006 [P] Update entitlement pricing configuration for fulfillment type, token quantity, validity, restrictions, refund policy, supported payment methods, and auto-renew in the existing Organization pricing model, mapper, GraphQL sources, and replicated Booking pricing state.
- [X] T007 [P] Update purchase and entitlement-cycle domain models with immutable pricing snapshots, renewal references, payment state, and lifecycle state in `src/booking/shared/Booking.Shared/Models/Entitlements/`.
- [x] T008 [P] Move all entity-to-model and model-to-transport conversion for the changed entitlement/purchase types into injected `src/booking/shared/Booking.Shared/Mappers/EntitlementModelMapper.cs` and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`; services must not return entities.
- [X] T009 Update `src/booking/shared/Booking.Shared/Repositories/EntitlementRepository.cs`, `src/booking/shared/Booking.Shared/Repositories/EntitlementPurchaseRepository.cs`, `src/booking/shared/Booking.Shared/Repositories/MarketplaceBookingSubscriptionRepository.cs`, `src/booking/shared/Booking.Shared/Repositories/ResourceRepository.cs`, and `src/booking/shared/Booking.Shared/Repositories/RepositoryFactory.cs` for cycle, renewal, ledger, refund, and booking linkage queries without direct EF access outside repositories.
- [x] T010 Update the existing EF model and generate exactly one migration set for any missing token purchase/renewal/linkage persistence in `src/booking/shared/Booking.Shared/Database/Migrations/`; do not create duplicate migrations for the previous and clarified versions.
- [x] T011 [P] Add explicit persisted-string mappings for fulfillment, payment, renewal, entitlement, ledger, and refund states in `src/booking/shared/Booking.Shared/Models/Entitlements/` and shared constants.
- [x] T012 [P] Add structured logging fields for purchase, grant, renewal, payment, operator action, booking consumption, restoration, forfeiture, expiry, refund, and failure paths in the owning services.

**Checkpoint**: Existing implementation is reconciled; persistence, models, mappers, and contracts support token cycles without duplicating the previous implementation.

## Phase 3: User Story 1 — Purchase Future-Use Tokens (Priority: P1)

**Goal**: Customer purchases a token-based offering through the same Stripe/bank-transfer payment pattern as reservation offerings, with no booking created.

**Independent Test**: Start and confirm a token purchase; verify no booking/resource/quota state exists and exactly one entitlement cycle is granted.

- [x] T013 [P] [US1] Add purchase validation, pricing snapshot, payment deadline, no-booking invariant, and confirmation idempotency unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/EntitlementPurchaseServiceTests/`.
- [x] T014 [P] [US1] Add Stripe payment-action, selected-pricing, amount, currency, and webhook-correlation unit tests in `src/booking/shared/Booking.Shared.UnitTests/Activities/StripeIntegrationsTests/`.
- [x] T015 [US1] Add purchase persistence and no-booking integration tests through repositories in `src/booking/domain/Booking.Domain.IntegrationTests/Services/Entitlements/EntitlementPurchaseShould.cs`.
- [x] T016 [US1] Update existing purchase creation/confirmation service to grant exactly one entitlement cycle only after confirmed payment in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementPurchaseService.cs`.
- [x] T017 [US1] Update Stripe checkout/activity and webhook correlation to resolve purchase identity and selected pricing without assuming a booking in `src/booking/shared/Booking.Shared/Activities/StripeIntegrations.cs` and `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs`.
- [x] T018 [US1] Update bank-transfer invoice creation and authorized manual confirmation to reuse reservation payment behavior without granting on invoice creation in `src/booking/shared/Booking.Shared/Activities/InvoiceIntegrations.cs`, `src/booking/shared/Booking.Shared/Workflows/PayBookingViaBankTransfer.cs`, and `src/booking/apis/Booking.Api/Services/BookingPaymentService.cs`.
- [x] T019 [US1] Remove or update any previous purchase path that creates a booking, schedule, resource allocation, reservation, or quota usage before payment or token use in `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and related workflows.
- [x] T060 [US1] Expire pending entitlement purchases past their payment deadline without granting tokens in `src/booking/shared/Booking.Shared/Repositories/EntitlementPurchaseRepository.cs` and `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementPurchaseService.cs`.
- [x] T020 [US1] Add/adjust authorized purchase GraphQL inputs, payloads, payment actions, status, and history in `src/booking/apis/Booking.Api/GraphQL/EntitlementPurchase/`.
- [x] T021 [US1] Update Skedular Host pricing editor to configure token fulfillment, quantity, validity, restrictions, refund policy, payment methods, and auto-renew in `src/web/apps/webapp-host/src/components/unified-listing-form/HostListingProductSettings.tsx` and its pricing page.
- [x] T022 [P] [US1] Update Skedular Spaces product add/edit editor with the same token fields and validation in `src/web/apps/webapp-spaces/src/components/product/product-editor-form.tsx`, `src/web/apps/webapp-spaces/src/components/product/addProduct/add-product.tsx`, and `src/web/apps/webapp-spaces/src/components/product/editProduct/edit-product.tsx`.

**Checkpoint**: A paid token purchase is visible as a purchase/entitlement, never as a reservation.

## Phase 4: User Story 4 — Renew Token Cycles (Priority: P1)

**Goal**: Auto-renewing token offerings renew at the cycle boundary using current pricing and existing payment behavior.

**Independent Test**: Confirm one renewal and exercise pending/failed/no-compatible-pricing/non-renewing paths.

- [x] T023 [P] [US4] Add renewal cycle, current-pricing re-match, failed-payment, non-renewing, and idempotency unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/EntitlementRenewalServiceTests/`.
- [x] T024 [US4] Add Stripe and bank-transfer renewal payment integration tests, including manual confirmation and no grant before confirmation, in `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementRenewalPaymentShould.cs`.
- [x] T025 [US4] Update the existing marketplace subscription renewal workflow/service to support token cycles without creating a booking/resource reservation in `src/booking/shared/Booking.Shared/Workflows/BookMarketplaceBookingSubscriptionResources.cs`, `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementRenewalService.cs`, and related services.
- [x] T026 [US4] Re-match current active ProductVersion pricing at renewal, fail safely when no compatible token auto-renew pricing exists, and preserve historical snapshots in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` and entitlement services.
- [x] T027 [US4] Enforce cycle-boundary expiry when renewal is pending or failed, with no new tokens before confirmed payment, in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementExpiryService.cs`.
- [x] T028 [US4] Expose renewal state, next renewal, cancel-at-period-end/auto-renew choice, payment action, and failure state through `src/booking/apis/Booking.Api/GraphQL/Entitlement/RootQuery.cs`, `src/booking/apis/Booking.Api/GraphQL/Entitlement/EntitlementDetails.cs`, and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`.
- [x] T029 [US4] Add customer and operator renewal-state/payment-action UI to shared, Host, and Spaces entitlement surfaces under `src/web/apps/webapp/src/components/marketplaceEntitlement/`, `src/web/apps/webapp-host/src/components/marketplaceEntitlement/`, and `src/web/apps/webapp-spaces/src/components/marketplaceEntitlement/`.

**Checkpoint**: Renewal behavior matches reservation-based payment/renewal semantics while granting tokens instead of creating bookings.

## Phase 5: User Story 2 — Book Using Tokens (Priority: P1)

**Goal**: Customers later use an active token to create an eligible ordinary booking.

**Independent Test**: Create a qualifying booking with one token and verify atomic resource allocation and consumption.

- [x] T030 [P] [US2] Extend eligibility tests for product/resource scope, weekday, validity, duration, balance, customer authorization, and operator authorization in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/EntitlementEligibilityServiceTests/`.
- [x] T031 [P] [US2] Add concurrent final-token and atomic rollback integration coverage through repositories in `src/booking/domain/Booking.Domain.IntegrationTests/Services/Entitlements/EntitlementConcurrentClaimShould.cs`.
- [x] T032 [US2] Preserve/update credit-funded booking validation before resource allocation in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs` and `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementBookingService.cs`.
- [x] T033 [US2] Preserve atomic resource claim and ledger mutation through `src/booking/shared/Booking.Shared/Repositories/ResourceRepository.cs`, `src/booking/shared/Booking.Shared/Repositories/EntitlementRepository.cs`, and `src/booking/shared/Booking.Shared/Services/Entitlements/CreditLedgerService.cs`.
- [x] T034 [US2] Expose entitlement and consuming ledger references through `src/booking/apis/Booking.Api/GraphQL/Booking/MarketplaceBookingDetails.cs`, `src/booking/apis/Booking.Api/GraphQL/Booking/AddMarketplaceBookingInput.cs`, `src/booking/apis/Booking.Api/GraphQL/Booking/ModifyMarketplaceBookingInput.cs`, and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`.
- [x] T035 [US2] Add customer token selection and booking creation behavior to shared marketplace booking UI in `src/web/apps/webapp/src/components/marketplaceProductBooking/`.
- [x] T036 [P] [US2] Add equivalent token booking behavior to Host and Spaces booking UI in `src/web/apps/webapp-host/src/components/booking/` and `src/web/apps/webapp-spaces/src/components/booking/`.

**Checkpoint**: Token purchase remains booking-free; later eligible use creates one ordinary booking and consumes one token.

## Phase 6: User Story 3 and User Story 5 — Booking Lifecycle and Operator Actions (Priority: P1)

**Goal**: Customers and authorized Spaces/Host owners/admins create, modify, and cancel token-funded bookings on behalf of the customer.

**Independent Test**: Customer and authorized operator create, modify date/time/resource, and cancel a token-funded booking; unauthorized action is denied.

- [x] T037 [P] [US4] Add modification, cancellation, restoration, forfeiture, operator authorization, actor/customer audit, and idempotency unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/`.
- [x] T038 [US4] Add persistence integration tests for token-funded date/time/resource modification and cancellation outcomes in `src/booking/domain/Booking.Domain.IntegrationTests/Services/Entitlements/EntitlementBookingModificationShould.cs`.
- [x] T039 [US4] Update marketplace booking modification to support token-funded date/time/resource changes atomically while preserving one consumed token in `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and `src/booking/apis/Booking.Api/Services/MarketplaceBookingModificationService.cs`.
- [x] T040 [US4] Update cancellation, restoration, forfeiture, and refund coordination to apply the existing reservation cancellation policy to token-funded bookings in `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementCancellationService.cs`, `src/booking/shared/Booking.Shared/Services/Entitlements/EntitlementModificationService.cs`, and `src/booking/apis/Booking.Api/Services/MarketplaceRefundPreviewService.cs`.
- [x] T041 [US4] Enforce customer and Spaces/Host owner/admin authorization and record operator/customer actors in `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs`, GraphQL authorization sources, and audit models.
- [x] T042 [US4] Expose restore/forfeit outcomes and operator audit details through GraphQL payloads and mappers in `src/booking/apis/Booking.Api/GraphQL/` and `src/booking/apis/Booking.Api/Mappers/`.
- [x] T043 [US4] Update customer booking UI for token-funded modification/cancellation in `src/web/apps/webapp/src/components/marketplaceProductBooking/` and `src/web/apps/webapp/src/components/marketplaceEntitlement/`.
- [x] T044 [P] [US4] Update equivalent Host and Spaces operator/customer UI for token booking creation, date/time/resource modification, cancellation, and restore/forfeit messaging in `src/web/apps/webapp-host/src/components/` and `src/web/apps/webapp-spaces/src/components/`.
- [x] T045 [P] [US4] Add Host and Spaces UI tests covering authorized operator action and restored versus forfeited token messaging in `src/web/apps/webapp-host/src/` and `src/web/apps/webapp-spaces/src/`.

**Checkpoint**: Token-funded bookings are ordinary editable/cancelable bookings for both customers and authorized operators.

## Phase 7: User Story 6 — Monitor Entitlements and Administration (Priority: P2)

**Goal**: Customers and administrators inspect balances, restrictions, renewal, payment, refund, ledger, and linked booking history.

**Independent Test**: Inspect all lifecycle states and verify authorization and complete audit history.

- [x] T046 [P] [US5] Add purchase, entitlement, renewal, authorization, balance, and payment-action read-model unit tests in `src/booking/apis/Booking.Api.UnitTests/Services/`.
- [x] T047 [P] [US5] Add expiry, refund, renewal-failure, manual-settlement, and operator-audit unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/Entitlements/` and `src/booking/shared/Booking.Shared.UnitTests/Services/`.
- [x] T048 [US5] Add expiry/refund/renewal integration tests through repositories in `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementExpiryAndRefundShould.cs` and `src/booking/domain/Booking.Domain.IntegrationTests/Services/EntitlementRenewalPaymentShould.cs`.
- [x] T049 [US5] Add entitlement purchase history, cycle detail, ledger, renewal, refund status, linked bookings, and admin authorization GraphQL sources in `src/booking/apis/Booking.Api/GraphQL/Entitlement/` and `src/booking/apis/Booking.Api/GraphQL/EntitlementPurchase/`.
- [x] T050 [US5] Add customer and operator entitlement balance/history/payment/renewal/refund UI in `src/web/apps/webapp/src/components/marketplaceEntitlement/`, `src/web/apps/webapp-host/src/components/marketplaceEntitlement/`, and `src/web/apps/webapp-spaces/src/components/marketplaceEntitlement/`.
- [x] T051 [US5] Add authorized administrator pricing configuration coverage for token fulfillment, quantity, validity, restrictions, refund policy, payment methods, and auto-renew in `src/booking/domain/Booking.Domain.IntegrationTests/Api/GraphQL/Entitlement/EntitlementPricingConfigurationShould.cs` and both Host/Spaces pricing editor test surfaces.

**Checkpoint**: All token state is visible, auditable, and consistently authorized across customer, Spaces, and Host surfaces.

## Phase 8: Polish and Cross-Cutting Validation

- [x] T052 [P] Regenerate backend GraphQL schemas and composed schema with `scripts/generate-graphql.sh`.
- [x] T053 [P] Regenerate Relay artifacts for `src/web/apps/webapp`, `src/web/apps/webapp-host`, and `src/web/apps/webapp-spaces` using each app’s source-driven Relay command.
- [x] T054 [P] Update Skedular Spaces and Host public documentation for token purchase, no-booking purchase behavior, renewal, later booking, operator actions, modification, cancellation, expiry, refund, and manual settlement in `src/web/apps/public-web/src/content/docs/` and `src/web/apps/public-web/src/data/documentation-source-map.ts`.
- [x] T055 [P] Add public-web documentation validation in `src/web/apps/public-web/tests/credit-entitlements.test.ts`.
- [x] T056 Run affected Booking unit/integration, GraphQL, Relay, Host, Spaces, and public-web validation documented in `specs/041-credit-based-entitlements/quickstart.md`.
- [x] T057 Run `git diff --check`, review generated artifacts, verify exactly one migration set, and prove no token purchase path creates booking/resource/quota state in `specs/041-credit-based-entitlements/quickstart.md`.
- [x] T058 Add explicit regression coverage for reservation-based offerings, ad-hoc bookings, recurring bookings, existing Stripe/bank-transfer payment behavior, and subscription cancellation in the affected Booking integration and API test projects.
- [x] T059 Run `graphify update .` and reconcile final implementation against `spec.md`, `plan.md`, `data-model.md`, `contracts/`, and `quickstart.md`.

## Dependencies and Execution Order

- Phase 1 must complete first because it determines which previous-version implementation is retained, updated, removed, or replaced.
- Phase 2 blocks all user stories and must reuse the existing persistence/payment model rather than introduce parallel state.
- US1 is the MVP and must complete before US2–US5.
- US2 renewal depends on confirmed purchase persistence and existing reservation renewal/payment patterns.
- US3 token booking depends on confirmed entitlement contracts from US1 and may proceed alongside US2 after foundational model stabilization.
- US4 depends on US3 booking references and uses the existing booking modification/cancellation path.
- US5 depends on all lifecycle states from US1–US4.
- Phase 8 depends on all required stories and source contract changes.

## Parallel Opportunities

After Phase 1: T006–T012 can proceed across pricing, models, repositories, migration, mappings, and logging where files do not overlap.

After foundational contracts: T013–T015, T023–T024, and T030–T031 can run in parallel as test work; T021/T022 can proceed independently for Host and Spaces pricing editors.

After US1 persistence stabilizes: Stripe, bank transfer, GraphQL, renewal design, and customer/operator UI work can proceed in parallel.

After US3 booking references stabilize: modification/cancellation services, GraphQL payloads, customer UI, and Host/Spaces operator UI can proceed in parallel.

## Implementation Strategy

1. Reconcile the previous standalone implementation first; remove only obsolete behavior and preserve working shared payment/entitlement code.
2. MVP: deliver token pricing, no-booking purchase, confirmed grant, and both payment methods.
3. Add current-pricing auto-renewal and failure behavior.
4. Complete customer and operator token booking lifecycle.
5. Complete read/admin/UI parity, public documentation, generated outputs, and regression validation.
