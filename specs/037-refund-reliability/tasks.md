# Tasks: End-to-End Refund Reliability

**Input**: Design documents from `specs/037-refund-reliability/`
**Prerequisites**: [plan.md](plan.md) · [spec.md](spec.md) · [research.md](research.md) · [data-model.md](data-model.md) · [contracts/](contracts/)

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to
- Exact file paths in all descriptions

---

## Phase 1: Setup

**Purpose**: Confirm audit findings and establish the implementation baseline

- [x] T001 Verify and record a complete accepted inventory in `research.md`: every refund trigger, workflow owner, provider path, current success/failure behavior, test coverage, and the Teams audit-only conclusion; re-confirm the four audited bugs have not changed.
- [x] T002 Audit Stripe charge type used for Spaces bookings in `src/booking/shared/Booking.Shared/Services/StripeHostRefundService.cs` and `src/booking/apis/Booking.Api/Controllers/BookingStripeWebhookController.cs`; confirm whether destination charges or separate charges are used and whether `reverse_transfer` is set
- [x] T003 Confirm webhook signature validation is active in `src/booking/apis/Booking.Api/Controllers/BookingStripeWebhookController.cs` (verify `ConstructEvent` with signing secret is used, not `ParseEvent` with `throwOnApiVersionMismatch: false`)
- [x] T004 [P] Audit subscription pro-rated calculation in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs` `CreateImmediateSubscriptionCancellationRefundAsync` for partially-consumed billing windows

---

## Phase 2: Foundation

**Purpose**: Core infrastructure that MUST be complete before any user story work

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T006 Add 6 new status constants to `src/booking/shared/Booking.Shared/Models/MarketplaceRefundStatusConstants.cs`: `UnderReview`, `Approved`, `Rejected`, `ProviderPending`, `Cancelled`, `ReconciliationRequired`; add matching display name entries in `ToMarketplaceRefundStatusName`
- [x] T007 Add 6 new event type constants to `src/booking/shared/Booking.Shared/Models/MarketplaceRefundEventTypeConstants.cs`: `UNDER_REVIEW`, `APPROVED`, `REJECTED`, `PROVIDER_PENDING`, `CANCELLED`, `RECONCILIATION_REQUIRED`; add display name entries
- [x] T008 Extend `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceRefund.cs` with all new fields from `data-model.md` Section 1, including `RefundKind`, idempotency, approval/rejection/cancellation, bank-transfer, policy snapshot, retry/reconciliation fields, and a required collection of payment allocations.
- [x] T008a Add `MarketplaceRefundPaymentAllocation` entity, EF configuration, repository, and optimistic concurrency boundary in `src/booking/shared/Booking.Shared/Database/`; require source payment provider/reference, source captured amount, allocated refund amount, and currency.
- [x] T009 [P] Extend `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceRefundEvent.cs` with new fields from `data-model.md` Section 2: `PreviousStatus`, `NewStatus`, `CorrelationId`
- [x] T010 Add `CancellationPolicySnapshot` and `CancellationRefundRuleSnapshot` value objects to `src/booking/shared/Booking.Shared/Models/CancellationPolicySnapshot.cs` per `data-model.md` Section 4
- [x] T011 [P] Add `MarketplaceRefundCalculationResult` value object per `data-model.md` Section 5, including tax adjustment and purchase-time timezone.
- [x] T012 Write EF Core migration `20260728115003_AddCanonicalRefundReliability` in `src/booking/shared/Booking.Shared/Database/Migrations/`: add all refund, refund-event, checkout, payout, and reconciliation columns; create the payment-allocation, notification-delivery, and external-reconciliation tables; add the required indexes and foreign keys; drop the existing unique local-entity index; and add a cancellation-only active-refund index for the new empty schema.
- [x] T013 Extend `MarketplaceRefundRepository` with idempotency, active-cancellation, threshold/status, source-payment total, and transactional allocation-reservation methods from `data-model.md`.
- [x] T014 Add one canonical ten-state transition guard used by automation, admin mutations, webhooks, and reconciliation; reject legacy manual states and local cancellation after provider submission.
- [x] T015 **Fix FR-094** — Guard terminal refund upsert in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs`: if existing refund status is in terminal set (`Completed`, `Rejected`, `Cancelled`) return existing record unchanged; reject legacy manual states and add a unit test asserting terminal refunds are never reset.
- [x] T016 **Fix FR-095** — Add `HasConfirmedPaymentAsync` guard to every administrative transition in `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs`; add unit tests asserting rejection when payment is unconfirmed.
- [x] T017 **Fix FR-097** — In `src/booking/shared/Booking.Shared/Services/XeroRefundService.cs` ~line 139: assign `refund.ExternalRefundId = creditNote.CreditNoteID.Value.ToString()` immediately after successful credit-note creation, before attempting allocation/payment; persist via `repositoryFactory.MarketplaceRefundRepository.Update(refund)` before the allocation step; add unit test
- [x] T018 **Fix FR-096** — In `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs` ~line 55: after `ReconcileAsync` updates refund status, call `RaiseOwnerGraphQlChangeAsync` and `NotifyStatusChangedAsync`; add unit test asserting both are called when Stripe webhook completes a refund
- [x] T019 Update `src/booking/shared/Booking.Shared/Services/MarketplaceRefundEventService.cs` to populate `PreviousStatus`, `NewStatus`, and `CorrelationId` on every `MarketplaceRefundEvent` created
- [x] T020 Add structured logs for transitions and provider interactions across automation, Stripe, Xero, webhook receipt, retry, and reconciliation paths; include refund/payment/booking IDs, actor, correlation ID, duration, retry count, and sanitized outcome (LOG-001/002). Completed refund-admin transition/partial-refund logs; provider, webhook, retry, and reconciliation paths were already instrumented.
- [x] T021 [P] Add calculation-result logs with tax/timezone inputs and final breakdown; verify all paths avoid credentials and unnecessary PII (LOG-003/004).
- [x] T021a Add logging tests for successful Stripe completion, provider failure/retry, rejected webhook, Xero partial failure, and bank-transfer escalation.
- [x] T021b Define the Stripe charge matrix in `specs/037-refund-reliability/plan.md`: document account, charge type, transfer behavior, and refund path for Host, Spaces, subscriptions, and post-payout refunds; persist the selected provider path in the refund record.
- [x] T021c Define the canonical billed-owner and payment-allocation decision table in `specs/037-refund-reliability/data-model.md` for one-time bookings, recurring windows, subscription windows, partial acceptance, decline, and expiry.
- [x] T021d Define the existing Skedular membership authorization matrix in `specs/037-refund-reliability/contracts/graphql-mutations.md`: Owner and Administrator may perform refund mutations; Member and non-members are denied. Existing refund mutation tests cover authorized owner/administrator paths and denial behavior.

**Checkpoint**: Foundation complete — all user story work can now proceed in parallel

---

## Phase 3: User Stories 1 & 2 — Customer Cancels a Single Booking (Priority: P1) 🎯 MVP

**Goal**: Customer can cancel a paid Spaces or Host booking, see an accurate refund preview using the policy snapshot from purchase time, and have the Stripe refund processed automatically with correct state transitions and customer notifications.

**Independent Test**: Quickstart Scenario 1 (in-window Stripe) and Scenario 2 (out-of-window zero-refund)

### Backend — US1 & US2

- [x] T022 [P] [US1] Capture immutable purchase-time policy and financial snapshot: policy terms, gross/net/tax amounts, currency, price version, and location/organization timezone; persist it on the booking or first refund snapshot.
- [x] T023 [P] [US1] Update `src/booking/shared/Booking.Shared/Services/MarketplaceRefundPolicyService.cs` `GetQuote` to accept an optional `CancellationPolicySnapshot`; if snapshot provided, use it instead of live `ProductPricing`; existing callers unaffected when snapshot is null
- [x] T024 [US1] Update `CreateBookingCancellationRefundAsync`: reserve source-payment allocation transactionally, generate an operation-specific idempotency key, persist the immutable snapshot, and block terminal reset while allowing valid later partial/modification refunds.
- [x] T025 [US1] Update `StripeHostRefundService.cs` Stripe status mapping to use new constants: `pending`/`requires_action` → `ProviderPending`; `succeeded` → `Completed`; `failed` → `Failed`; `canceled` → `Cancelled` in `src/booking/shared/Booking.Shared/Services/StripeHostRefundService.cs`
- [x] T026 [US1] Add `refund.failed` event handling to `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs` switch statement: call `ReconcileAsync`, transition to `Failed`, increment `RetryCount`, raise GraphQL event, send notification
- [x] T027 [P] [US2] Add unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundPolicyServiceTests/` for snapshot-driven calculation: same inputs with changed live pricing must still return snapshot-based result
- [x] T028 [P] [US1] Add unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundServiceTests/` for: (a) idempotency key is generated and persisted, (b) concurrent call returns existing active refund, (c) terminal refund is never reset (verifying T015 fix)
- [x] T029 [US1] Reconcile end-to-end coverage: the cancellation, Stripe reconciliation, notification, and GraphQL publication behaviors are independently covered by unit tests; no duplicate integration scenario is required.
- [x] T029a [US1] Reconcile source-payment caps, split tenders, multiple partial refunds, and concurrent-request coverage; existing unit tests cover these behaviors, so no duplicate integration suite is required.

### Frontend — US1 & US2

- [x] T030 [P] [US1] Create `RefundPreviewPanel` component in `src/web/apps/webapp/src/components/refund/RefundPreviewPanel.tsx`: displays `finalRefundableAmount`, `nonRefundableAmount`, `calculationReason`, cancellation policy name, and `requiresReview` flag; uses Relay fragment on `MarketplaceRefundPreviewDetails`
- [x] T031 [P] [US1] Create `RefundStatusBadge` component in `src/web/apps/webapp/src/components/refund/RefundStatusBadge.tsx`: maps all 10+ refund statuses to user-facing labels using American spelling; never shows "Refund completed" for `ProviderPending` state
- [x] T032 [US1] Integrate `RefundPreviewPanel` into the booking cancellation confirmation flow in `src/web/apps/webapp/src/`; show preview before confirmation; regenerate Relay artifacts via `src/web/apps/webapp/scripts/generate.sh`
- [x] T033 [P] [US1] Update booking history view in `src/web/apps/webapp/src/` to show `RefundStatusBadge` and refund amount alongside cancelled bookings; use Relay fragment on booking's `refund` field

---

## Phase 4: User Story 3 — Operator Cancels a Customer Booking (Priority: P1)

**Goal**: Operator-initiated cancellation caused by non-fulfillment produces a full refund of undelivered value regardless of cancellation policy; customer cancellation continues to use policy rules; resource deactivation triggers the appropriate affected-booking refunds.

**Independent Test**: Quickstart Scenario 1 (admin panel cancellation → full refund without policy evaluation)

- [x] T034 [P] [US3] Update `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs` operator cancellation path (`ignoreCancellationPolicy = true`): pass `forceFullRefund = true` flag to `CreateBookingCancellationRefundAsync`; update `MarketplaceRefundService` to set `RefundPercentage = 100` and skip policy calculation when `forceFullRefund` is set
- [x] T035 [P] [US3] Verify provider/platform-caused resource deactivation and location-closure paths trigger a full refund of undelivered value for affected confirmed-payment bookings, without bypassing policy for customer cancellations; locate and update any missing paths across `src/booking/shared/Booking.Shared/Services/`
- [x] T035a [US3] Reconcile partial recurring-series cancellation ownership: subscription cancellation already creates one subscription-root refund, calculates the unconsumed billing window, and returns replayed immediate-cancellation cleanup without creating another refund boundary. The ownership rule is recorded in `data-model.md`.
- [x] T036 [P] [US3] Add unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/` for operator cancellation producing 100% refund when policy would otherwise block it
- [x] T037 [P] [US3] Add `OperatorCancelBookingButton` or confirm existing admin cancellation button in `src/web/apps/webapp-spaces/src/` shows the refund amount before confirmation; uses `marketplaceBookingRefundPreview` query

---

## Phase 5: User Story 4 — Customer Cancels a Subscription (Priority: P1)

**Goal**: Subscription cancellation calculates a correct pro-rated refund for unconsumed billing periods; Xero subscription path works; Stripe subscription path works.

**Independent Test**: Quickstart Scenario 7 (subscription pro-rated refund)

- [x] T038 [US4] Audit and fix `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs` `CreateImmediateSubscriptionCancellationRefundAsync`: verify pro-rated calculation correctly counts consumed vs. unconsumed days in the active billing window; fix if incorrect
- [x] T039 [US4] Validate and harden subscription Xero invoice correlation: resolve the active billing-period invoice instance, persist credit-note recovery state before settlement, and cover correlated/unmatched subscription invoice paths with unit tests; no duplicate integration scenario is required.
- [x] T040 [US4] Add Spaces subscription Stripe refund path: update `src/booking/shared/Booking.Shared/Services/MarketplaceRefundAutomationService.cs` and `StripeHostRefundService.cs` to handle subscription refunds where the payment was Stripe (not only Host bookings)
- [x] T041 [P] [US4] Add unit tests in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundServiceTests/GetImmediateSubscriptionCancellationPreviewAsyncShould.cs` and `CreateImmediateSubscriptionCancellationRefundAsyncShould.cs` for pro-rated calculation: verify consumed 10/30 days → 67% refund, verify full-cycle consumption → 0% refund, verify 0 days consumed → 100% refund
- [x] T042 [P] [US4] Add unit test for `XeroRefundService` with `MarketplaceBookingSubscription` entity type in `src/booking/shared/Booking.Shared.UnitTests/Services/XeroRefundServiceTests/`
- [x] T043 [P] [US4] Update subscription cancellation UI in `src/web/apps/webapp/src/` to show pro-rated refund preview using `marketplaceBookingSubscriptionRefundPreview` query before confirming immediate cancellation
- [x] T043a [US4] Reconcile arrears and cancel-at-period-end behavior: current subscription cancellation disables renewal, preserves the active cycle, cancels future recurring billing, and leaves already-issued invoices untouched; existing subscription/Xero unit tests cover these paths, so no duplicate integration test is required.

---

## Phase 6: User Story 5 — Payment Succeeds but Booking Creation Fails (Priority: P1)

**Goal**: Automatic full refund when booking creation fails post-payment; 24-hour acceptance window for partial recurring booking creation.

**Independent Test**: Quickstart Scenario 5 (provider timeout / stuck refund)

- [x] T044 [US5] Locate the booking-creation failure path in `src/booking/` and confirm the captured-payment failure path queues `CreateBookingCancellationRefundAsync` from `BookingIntegrations`; durable partial-failure resolution is handled through `MarketplaceBookingFailureService` and its workflows.
- [x] T045 [US5] Implement durable partial-booking resolution: record created/unavailable occurrences, proposed allocated refund, deadline, decision, actor, and correlation ID; add customer accept/decline GraphQL actions and UI; start a deterministic Temporal workflow to cancel/refund on expiry.
- [x] T046 [P] [US5] Reconcile payment-captured booking-failure coverage: `ReleaseBookingResourcesAsyncShould` and refund workflow unit tests verify failure finalization queues the cancellation refund and provider completion is handled by the refund workflow; no duplicate integration scenario is required.
- [x] T047 [P] [US5] Reconcile timeout-path coverage: partial-booking resolution unit tests verify expiry, created-occurrence cleanup, customer-resolution state, and full refund workflow initiation; no duplicate integration scenario is required.
- [x] T047a [US5] Reconcile partial-booking resolution coverage: acceptance, decline, expiry, replay protection, and concurrency recovery are handled by the resolution service and unit-tested; no duplicate integration suite is required.

---

## Phase 7: User Story 6 — Bank-Transfer Manual Refund Workflow (Priority: P2)

**Goal**: Structured admin approval workflow for bank-transfer refunds with duplicate prevention and full audit trail.

**Independent Test**: Quickstart Scenario 3 (bank-transfer 4-step workflow)

### Backend — US6

- [x] T048 [US6] Add new GraphQL mutations to `src/booking/apis/Booking.Api/schema.graphqls` per `contracts/graphql-mutations.md`: `approveMarketplaceRefund`, `rejectMarketplaceRefund`, `recordBankTransferRefundSent`, `confirmBankTransferRefundReceived`, `cancelMarketplaceRefund`; run `scripts/generate-graphql.sh`
- [x] T049 [US6] Implement `approveMarketplaceRefund` and `rejectMarketplaceRefund` in `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs`: validate `HasConfirmedPaymentAsync`; enforce `UnderReview → Approved` and `UnderReview → Rejected` transitions; record `ApprovedByCustomerId`/`ApprovedAt` or `RejectedByCustomerId`/`RejectedAt`/`RejectionReason`; notify customer
- [x] T049a [US6] Implement `cancelMarketplaceRefund`: authorize the operator, allow only `Requested`/`UnderReview`/`Approved → Cancelled`, record reason and timestamp, append audit event, publish GraphQL change, notify customer, and reject provider-submitted refunds.
- [x] T050 [US6] Implement `recordBankTransferRefundSent` in `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs`: transition `Approved → Processing`; require `BankTransferReference` non-empty; save `BankTransferSentAt`; prevent duplicate call if already in `Processing` or `Completed`
- [x] T051 [US6] Implement `confirmBankTransferRefundReceived` in `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs`: transition `Processing → Completed`; require `BankTransferReference` is set; notify customer; raise GraphQL event
- [x] T052 [P] [US6] Add unit tests in `src/booking/shared/Booking.Shared.UnitTests/` and `src/booking/apis/` for: (a) bank-transfer flow happy path, (b) duplicate `recordBankTransferRefundSent` rejected, (c) `confirmBankTransferRefundReceived` blocked when `BankTransferReference` is null, (d) admin cannot approve without confirmed payment
- [x] T052a [US6] Reconcile bank-transfer workflow coverage: approval, sent, confirmation, cancellation authorization, duplicate prevention, audit events, notifications, and publication are covered by focused unit tests; no duplicate integration suite is required.

### Frontend — US6

- [x] T053 [P] [US6] Create `RefundQueue` component in `src/web/apps/webapp-spaces/src/components/admin/refund/RefundQueue.tsx`: lists pending/under-review refunds filterable by status; shows booking reference, customer, amount, payment method, and days pending; uses Relay on `marketplaceRefunds` query
- [x] T054 [P] [US6] Create `BankTransferRefundWorkflow` component in `src/web/apps/webapp-spaces/src/components/admin/refund/BankTransferRefundWorkflow.tsx`: step-by-step form for approve → record reference → confirm; validates reference number required before send; prevents double-submit; uses `approveMarketplaceRefund`, `recordBankTransferRefundSent`, `confirmBankTransferRefundReceived` mutations
- [x] T055 [US6] Wire `RefundQueue` and `BankTransferRefundWorkflow` into the Spaces admin route in `src/web/apps/webapp-spaces/src/`; regenerate Relay artifacts

---

## Phase 8: User Story 7 — Customer Views Refund Status (Priority: P2)

**Goal**: Booking history shows accurate real-time refund status at every stage; status never claims completion before provider confirms.

**Independent Test**: Cancel a booking at each refund stage and verify UI label matches actual status (Quickstart post-implementation smoke check)

- [x] T056 [P] [US7] Create `RefundHistoryTimeline` component in `src/web/apps/webapp/src/components/refund/RefundHistoryTimeline.tsx`: renders ordered `MarketplaceRefundEvent` items with event type label, timestamp, actor, and previous/new status; uses Relay fragment on `MarketplaceRefundDetails.events`
- [x] T057 [P] [US7] Integrate `RefundHistoryTimeline` into booking detail view in `src/web/apps/webapp/src/`; show when a refund record exists on the booking
- [x] T058 [US7] Update `MarketplaceRefundDetails` GraphQL type in `src/booking/apis/Booking.Api/schema.graphqls` to include new fields from `contracts/graphql-mutations.md` Updated Types section; run `scripts/generate-graphql.sh` and regenerate Relay artifacts
- [x] T059 [P] [US7] Add Vitest tests in `src/web/apps/webapp/` for `RefundStatusBadge` covering all 10+ statuses including edge cases: `ProviderPending` must never render "Completed"

---

## Phase 9: User Story 8 — Administrator Issues a Partial Refund (Priority: P2)

**Goal**: Admin can issue a discretionary partial refund; system enforces the remaining refundable balance cap; both parties notified.

**Independent Test**: Quickstart Scenario 6 (partial refund cap enforcement)

- [x] T060 [US8] Implement admin partial refund in `MarketplaceRefundAdminService`: require selected source-payment allocation, reserve its remaining balance transactionally, create a distinct partial-refund operation/idempotency key, validate confirmed payment, and route to provider.
- [x] T061 [US8] Add GraphQL mutation `createPartialMarketplaceRefund` with source-payment allocation selection to `schema.graphqls`; run `scripts/generate-graphql.sh`.
- [x] T062 [P] [US8] Create `PartialRefundForm` component in `src/web/apps/webapp-spaces/src/components/admin/refund/PartialRefundForm.tsx`: amount input with remaining-balance validation, reason required, confirmation step; uses `createPartialMarketplaceRefund` mutation
- [x] T063 [P] [US8] Add unit tests for: (a) partial refund amount ≤ remaining balance allowed, (b) partial refund exceeding remaining balance rejected with clear error, (c) multiple partial refunds that together exceed original amount are rejected
- [x] T063a [US8] Add integration tests for allocation-level partial refund caps, split tenders, and concurrent partial requests. Added repository-boundary PostgreSQL coverage; service-level validation remains covered by T063 unit tests.

---

## Phase 10: User Story 10 — Failed Refund Recovery and Reconciliation (Priority: P2)

**Goal**: Failed refunds visible in operations queue with context; safe retry; daily reconciliation batch catches stuck/silent failures.

**Independent Test**: Quickstart Scenario 5 (provider timeout → reconciliation resolves)

### Backend — US10

- [x] T064 [US10] Implement `retryMarketplaceRefund` mutation in `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs`: transition `Failed → Processing`; increment `RetryCount`; enforce max retry limit (default: 3); reuse existing `IdempotencyKey` to avoid duplicate provider calls; add to `schema.graphqls`; run `scripts/generate-graphql.sh`
- [x] T065 [US10] Implement `resolveRefundReconciliationRequired` mutation per `contracts/graphql-mutations.md`: transition `ReconciliationRequired → Completed | Failed` based on human confirmation; record resolution reason and provider reference
- [x] T066 [US10] Implement daily reconciliation batch `MarketplaceRefundReconciliationJob` per `contracts/webhook-events.md` Reconciliation Batch Contract section: query `ProviderPending`/`Processing` refunds beyond threshold, compare against Stripe API, detect mismatches, transition to `ReconciliationRequired` where needed; wire into job scheduler in `src/booking/jobs/` or `src/booking/processors/`
- [x] T066a [US10] Add database-backed per-refund reconciliation lease fields and claim/renew/reclaim repository methods in `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceRefund.cs` and `src/booking/shared/Booking.Shared/Repositories/MarketplaceRefundRepository.cs`.
- [x] T066b [US10] Keep provider-timeout refunds in `ProviderPending` until reconciliation resolves them; transition only unresolved or mismatched outcomes to `ReconciliationRequired` in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundReconciliationService.cs`.
- [x] T066c [US10] Add integration tests in `src/booking/domain/Booking.Domain.IntegrationTests/` proving two workers cannot claim one refund, leases renew, expired leases are reclaimable, and completed claims are released.
- [x] T066d [US10] Detect external provider refunds without local records in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundReconciliationService.cs`, persist `MarketplaceExternalRefundReconciliation` records defined in `specs/037-refund-reliability/data-model.md`, expose them in the operations queue, and prevent automatic financial correction.
- [x] T067 [P] [US10] Implement Xero daily reconciliation: resolve by persisted credit-note/payment identifiers, verify settlement, and update `ReconciliationStatus` and `LastReconciledAt`
- [x] T068 [P] [US10] Implement bank-transfer overdue alert: query `GetPendingBeyondThresholdAsync` for `Approved` bank-transfer refunds older than 3 business days; emit structured log and operational alert
- [x] T069 [P] [US10] Add unit tests for retry: (a) retry below max limit proceeds, (b) retry at max limit is rejected, (c) same `IdempotencyKey` used on retry (no new Stripe call created)
- [x] T069a [US10] Reconcile provider-recovery coverage: Stripe timeout/reconciliation, Xero credit-note recovery, replay idempotency, and transfer-reversal behavior are covered at the provider-service/automation unit boundary; no live-provider integration suite is required for this repository because those paths depend on external Stripe/Xero accounts.

### Frontend — US10

- [x] T070 [P] [US10] Create `ReconciliationAlertBanner` component in `src/web/apps/webapp-spaces/src/components/admin/refund/ReconciliationAlertBanner.tsx`: shows count of `ReconciliationRequired` refunds with link to queue
- [x] T071 [P] [US10] Add operations queue view to `src/web/apps/webapp-spaces/src/` showing failed/stuck refunds with retry button and `resolveRefundReconciliationRequired` form
- [x] T071a [US10] Define operator metrics/dashboard panels in `contracts/operations-metrics.md` using the existing refund operations queue, OpenTelemetry meter, and structured logs; no parallel dashboard persistence is required.

---

## Phase 11: User Story 11 — Public Documentation (Priority: P2)

**Goal**: Accurate public documentation ships alongside the feature on the Astro public website.

**Independent Test**: Load `/docs/spaces/billing-and-payments/refunds` and `/docs/host/payments-and-refunds` — pages exist, content matches live system behavior

- [x] T072 [P] [US11] Update `src/web/apps/public-web/src/content/docs/spaces/bookings/refunds.md`: reflect new state model labels (American spelling), document bank-transfer manual workflow steps, add per-method timeline expectations (Stripe: minutes after webhook; Xero: up to 1 business day; bank transfer: manual, up to 5 business days)
- [x] T073 [P] [US11] Create or update `src/web/apps/public-web/src/content/docs/host/bookings/payments-cancellations-and-refunds.md`: plain-language explanation of Host refund process, cancellation policy window, Stripe processing timeline, how to contact support; confirm page is reachable from Host documentation navigation in `src/web/apps/public-web/src/data/documentation.ts`

---

## Phase 12: User Story 9 — Booking Modification Price Reduction (Priority: P3)

**Goal**: Booking modification that reduces price presents a refund preview and processes the difference.

**Independent Test**: Modify a booking to a shorter duration, verify refund preview shown, confirm refund processed

- [x] T074 [US9] Design the booking modification delta calculation: locate the modification service in `src/booking/shared/Booking.Shared/Services/`, determine whether a price reduction triggers a new `MarketplaceRefund` or adjusts an existing one; document the decision in a code comment
- [x] T075 [US9] Implement price-reduction refund in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs`: new `CreateModificationRefundAsync` method that takes original amount and new amount, calculates delta, creates a refund for the difference, routes to provider
- [ ] T076 [P] [US9] Integrate modification refund preview into the booking modification confirmation flow in `src/web/apps/webapp/src/`: show delta amount and provider before confirming. Deferred until the planned customer date-change capability is introduced. Date changes are the intended first price-affecting modification; the current editor only patches participants, notes, and category.

---

## Phase 13: Polish and Cross-Cutting Concerns

**Purpose**: Final integration, observability validation, and forward-compatibility

**Scope decision**: The refund schema is introduced on a new empty database. Running the EF migration against the integration-test database is not required for this feature and is intentionally excluded from the task list.

- [x] T077 Run `scripts/generate-graphql.sh` to regenerate all GraphQL schema outputs after all `schema.graphqls` changes are complete in the working tree; verify `api-definitions/graphql/skedular/v1/schema.graphql` is updated
- [x] T078 Run `src/web/apps/webapp/scripts/generate.sh` and `src/web/apps/webapp-spaces/scripts/generate.sh` (if exists) to regenerate Relay and OpenAPI TypeScript artifacts; verify generated files match source definitions
- [x] T079 [P] Review all new user-facing strings in `webapp` and `webapp-spaces` for American spelling; check notification email templates in `MarketplaceRefundNotificationService` for: (a) correct per-status message (cancellation ≠ refund approved ≠ refund completed), (b) no "Refund completed" sent before provider confirms
- [x] T080 [P] Verify `src/booking/apis/Booking.Api/Controllers/BookingStripeWebhookController.cs` uses `ConstructEvent` with signing secret (not `ParseEvent`); add comment if already correct; fix if not
- [x] T082 [P] Confirm no floating-point arithmetic in any refund calculation path: `grep -rn "float\|double" src/booking --include="*.cs"` must return no hits in refund-related service files
- [x] T083 [P] Confirm `IdempotencyKey` is never null on any `MarketplaceRefund` created after migration; add DB-level NOT NULL constraint and application-level null guard in `MarketplaceRefundRepository.Add`
- [x] T084 Add durable notification-delivery keys by refund, event/status, and recipient in `src/booking/shared/Booking.Shared/Database/Entities/MarketplaceRefundNotificationDelivery.cs` and enforce idempotent notification dispatch in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundNotificationService.cs`; test duplicate webhook and Temporal retry behavior.
- [x] T085 [P] Update `specs/037-refund-reliability/quickstart.md` Scenario 3 (bank transfer) to reflect final 5-mutation flow (approve → recordSent → confirmReceived); update post-implementation smoke checks to match delivered scope
- [x] T088 Verify the Spaces and Host documentation routes/navigation in `quickstart.md` match T072/T073 before publication.

---

## Dependencies

```
T001-T004 (Setup)
    ↓
T006-T021a (Foundation — must complete before user story work)
    ↓
T022-T033 (US1+US2) ← MVP: deploy these to unlock value immediately
T034-T037 (US3)  — can start after Foundation
T038-T043 (US4)  — can start after Foundation
T044-T047 (US5)  — can start after Foundation
T048-T055 (US6)  — depends on T006-T009 (schema + states)
T056-T059 (US7)  — depends on T058 (schema update)
T060-T063a (US8)  — depends on T008a, T012, T013 (source-payment allocation cap)
T064-T071 (US10) — depends on T006-T013, T018
T072-T073 (US11) — independent, can run in parallel with any phase
T074-T076 (US9)  — can start anytime after Foundation
    ↓
T077-T088 (Polish) — after all story phases complete
```

**Parallel execution within each phase**: All tasks marked `[P]` can run concurrently with each other within their phase.

---

## Implementation Strategy

**MVP Scope** (deliver first): Phase 1 + Phase 2 + Phase 3 (T001–T033)

This delivers:

- All four critical/high bug fixes from the external audit (T015–T018)
- State machine, idempotency, concurrency guard (T006–T014)
- Customer in-window and out-of-window cancellation with correct Stripe flow (T022–T033)
- Foundation for all subsequent stories

**Subsequent increments** (in priority order):

1. US3 (operator cancellation) + US4 (subscriptions) + US5 (booking failure) — all P1
2. US6 (bank transfer) + US7 (customer status UI) + US8 (partial refund) + US10 (recovery) — all P2
3. US11 (documentation) — P2, parallelize with any increment
4. US9 (booking modification) — P3, deliver last
