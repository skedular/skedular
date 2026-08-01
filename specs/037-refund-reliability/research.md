# Research: End-to-End Refund Reliability

**Phase**: 0 — Pre-design research
**Date**: 2026-07-27 (refreshed against the current working tree)
**Feature**: [spec.md](spec.md) | [plan.md](plan.md)

---

## 1. Current Codebase Inventory

### Existing Refund Implementation (Confirmed by Code Audit)

| Component                              | Location                                                                          | Purpose                                                                                                                                                                                                         |
| -------------------------------------- | --------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `MarketplaceRefund` entity             | `Booking.Shared/Database/Entities/MarketplaceRefund.cs`                           | Primary refund record (EF entity)                                                                                                                                                                               |
| `MarketplaceRefundEvent` entity        | `Booking.Shared/Database/Entities/MarketplaceRefundEvent.cs`                      | Immutable audit trail event per state change                                                                                                                                                                    |
| `MarketplaceRefundStatusConstants`     | `Booking.Shared/Models/MarketplaceRefundStatusConstants.cs`                       | Canonical ten-state model: Requested, UnderReview, Approved, Rejected, Processing, ProviderPending, Completed, Failed, Cancelled, ReconciliationRequired |
| `MarketplaceRefundEventTypeConstants`  | `Booking.Shared/Models/MarketplaceRefundEventTypeConstants.cs`                    | Event type strings aligned to status transitions                                                                                                                                                                |
| `MarketplaceRefundEntityTypeConstants` | `Booking.Shared/Models/MarketplaceRefundEntityTypeConstants.cs`                   | Entity types: MarketplaceBooking, MarketplaceBookingSubscription                                                                                                                                                |
| `MarketplaceRefundPreview`             | `Booking.Shared/Models/MarketplaceRefundPreview.cs`                               | Preview DTO returned before cancellation confirmation                                                                                                                                                           |
| `MarketplaceRefundQuote`               | `Booking.Shared/Models/MarketplaceRefundQuote.cs`                                 | Policy evaluation result with `CalculateRefundAmount()`                                                                                                                                                         |
| `MarketplaceRefundPolicyService`       | `Booking.Shared/Services/MarketplaceRefundPolicyService.cs`                       | Evaluates cancellation policy rules and supports persisted calculation/policy snapshots |
| `MarketplaceRefundService`             | `Booking.Shared/Services/MarketplaceRefundService.cs`                             | Creates refund records; handles booking and subscription cancellation                                                                                                                                           |
| `MarketplaceRefundAutomationService`   | `Booking.Shared/Services/MarketplaceRefundAutomationService.cs`                   | Routes refund to Stripe (Host) or Xero after creation                                                                                                                                                           |
| `StripeHostRefundService`              | `Booking.Shared/Services/StripeHostRefundService.cs`                              | Stripe refund for Host card payments; uses `RefundApplicationFee=true`                                                                                                                                          |
| `XeroRefundService`                    | `Booking.Shared/Services/XeroRefundService.cs`                                    | Xero credit-note projection; persists the credit-note reference before later settlement work |
| `MarketplaceRefundEventService`        | `Booking.Shared/Services/MarketplaceRefundEventService.cs`                        | Creates `MarketplaceRefundEvent` audit records                                                                                                                                                                  |
| `MarketplaceRefundNotificationService` | `Booking.Shared/Services/MarketplaceRefundNotificationService.cs`                 | Sends email notifications on status changes                                                                                                                                                                     |
| `MarketplaceRefundRepository`          | `Booking.Shared/Repositories/MarketplaceRefundRepository.cs`                      | EF repository with idempotency, allocation reservation, threshold queries, leases, durable notification delivery, and unmatched-provider records |
| `MarketplaceRefundEventRepository`     | `Booking.Shared/Repositories/MarketplaceRefundEventRepository.cs`                 | EF repository for audit events                                                                                                                                                                                  |
| `MarketplaceRefundAdminService`        | `Booking.Api/Services/MarketplaceRefundAdminService.cs`                           | Admin approval, rejection, cancellation, bank-transfer, retry, partial-refund, and reconciliation operations                                                                                                      |
| `MarketplaceRefundReadService`         | `Booking.Api/Services/MarketplaceRefundReadService.cs`                            | Read-side queries for API layer                                                                                                                                                                                 |
| `MarketplaceRefundPreviewService`      | `Booking.Api/Services/MarketplaceRefundPreviewService.cs`                         | Preview endpoint service                                                                                                                                                                                        |
| `BookingStripeWebhookController`       | `Booking.Api/Controllers/BookingStripeWebhookController.cs`                       | Validates and publishes Stripe refund, charge, checkout, and payout webhooks                                                                                                                                    |
| `BookingInternalSubscriber`            | `Booking.Processors/Subscribers/BookingInternalSubscriber.cs`                     | Dispatches Stripe events, persists charge/transfer context, delegates payout reconciliation, reconciles refund events, and raises customer/operator notifications                                                  |
| GraphQL schema — queries               | `Booking.Api/schema.graphqls`                                                     | `marketplaceBookingRefundPreview`, `marketplaceBookingSubscriptionRefundPreview`, `marketplaceRefund`, `marketplaceRefunds`, `marketplaceRefundStatuses`, `marketplaceExternalRefundReconciliations`       |
| GraphQL schema — mutations             | `Booking.Api/schema.graphqls`                                                     | Canonical approval/rejection, cancellation, bank-transfer, partial-refund, retry, local reconciliation-resolution, and external reconciliation-resolution mutations; generated schema outputs are checked in |
| Public docs — Spaces                   | `public-web/src/content/docs/spaces/bookings/refunds.md`                          | Spaces refund documentation (exists)                                                                                                                                                                            |
| Public docs — Host                     | `public-web/src/content/docs/host/bookings/payments-cancellations-and-refunds.md` | Host refund documentation exists and describes cancellation, Stripe timing, reconciliation, and bank-transfer follow-up |

### Current State Gaps (Confirmed)

> The original rows below are retained as the pre-implementation audit baseline. They are historical findings, not the current state. The current implementation status is recorded in the refreshed assessment below.

### Refreshed Current-State Assessment (2026-07-27)

| Gap | Severity | Current evidence |
| --- | --- | --- |
| Unmatched-provider operations exposure | High | Organization-scoped, paginated external reconciliation query and resolution mutation are implemented; records without a resolvable organization remain retained for platform retry. |
| Temporal retry-policy evidence | High | Provider failures are persisted and surfaced as retryable activity failures with a three-attempt Temporal policy; exhausted workflows are finalized durably. |
| Xero scheduled reconciliation detail | High | Credit-note and payment verification use persisted identifiers and scheduled `LastReconciledAt` reconciliation. |
| Partial recurring-booking acceptance coverage | Medium | Acceptance, decline, expiry, replay, and concurrency behavior is covered by workflow tests; deployed-environment validation remains outside this change. |

| Gap                                                                              | Severity     | Detail                                                                                                                                                                                             |
| -------------------------------------------------------------------------------- | ------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| State machine: 6 states vs. 10 required                                          | Critical     | Missing: UnderReview, Approved, Rejected, Cancelled, ReconciliationRequired                                                                                                                        |
| No idempotency key on `MarketplaceRefund`                                        | Critical     | Concurrent or replayed requests can create duplicate records                                                                                                                                       |
| No concurrency guard                                                             | Critical     | Two simultaneous cancellations of the same booking can create two refund records                                                                                                                   |
| No subscription Stripe refund path                                               | High         | `StripeHostRefundService.IsHostRefundAsync` checks for Host booking; Spaces subscription Stripe path missing                                                                                       |
| Xero only supports `MarketplaceBooking` entity                                   | High         | `XeroRefundService.ProcessAsync` fails with `LocalEntityType = MarketplaceBookingSubscription`                                                                                                     |
| Policy not snapshotted at purchase                                               | High         | `MarketplaceRefundPolicyService` reads from `ProductPricing` at evaluation time; product price changes retroactively affect refund eligibility                                                     |
| No reconciliation batch process (historical baseline)                            | High         | Addressed by the scheduled reconciliation hosted service; remaining gaps are listed in the refreshed assessment above. |
| Admin service bypasses state machine                                             | High         | `MarketplaceRefundAdminService` accepts arbitrary status strings; invalid transitions not rejected                                                                                                 |
| Stripe status mapping (historical baseline)                                      | Medium       | Addressed: `pending` and `requires_action` now map to `ProviderPending`. |
| Missing bank-transfer manual workflow states                                     | Medium       | No `Approved`/`Rejected` path for bank-transfer refunds                                                                                                                                            |
| Partial-booking acceptance timeout (historical baseline)                         | Medium       | The workflow exists; remaining integration coverage is listed in the refreshed assessment above. |
| Subscription pro-rated calculation missing                                       | Medium       | `CreateImmediateSubscriptionCancellationRefundAsync` exists but pro-rate logic for partially consumed cycles needs audit                                                                           |
| `MarketplaceRefundEvent.ActorCustomerId` is the only actor field                 | Low          | No `ApprovedByCustomerId`, no operator/admin actor recording                                                                                                                                       |
| Spaces refund documentation incomplete                                           | Low          | Host refund page is referenced but content not verified as complete                                                                                                                                |
| **Terminal refund can be reopened and reprocessed**                              | **Critical** | `MarketplaceRefundService` upsert (~line 181) unconditionally resets an existing refund to `Requested` regardless of current status, including `Completed` — enabling double-refund to provider    |
| **Admin mutations do not gate on confirmed payment**                             | **High**     | `MarketplaceRefundAdminService.UpdateStatusAsync` (~line 84) has no `HasConfirmedPaymentAsync` check; only `ProcessInXeroAsync` does; admins can settle unpaid bookings                            |
| **Stripe webhook does not notify or raise GraphQL events on completion**         | **High**     | Fixed: the subscriber reconciles the refund, projects accounting, raises the owner GraphQL change, and sends the status notification |
| **Xero credit-note ID not persisted if allocation/payment step fails**           | **Medium**   | Fixed: the credit-note identifier is persisted immediately after credit-note creation, before later settlement/allocation work |

---

## 2. Stripe Refund Research

**Source**: https://docs.stripe.com/refunds (verified 2026-07-25)

### Decision: Stripe refund behaviour for destination charges

- **Decision**: For destination charges (used by Skedular Spaces/Host via Stripe Connect), Stripe debits the **platform account** for refunds, not the connected operator account directly.
- **Rationale**: Confirmed by official docs: "Stripe debits your platform for refunds to destination charge or separate charge and transfer payments. Reverse the transfers associated with these charge types to recover the refund amount from your connected accounts."
- **Implication**: `StripeHostRefundService.ProcessAsync` uses the persisted original charge context. For a disbursed destination-charge payout it attempts transfer reversal first; when Stripe returns a supported structured reversal error code it retries with a platform-funded refund. Unknown errors route to reconciliation. Direct charges are refunded on their connected account without transfer reversal.
- **Alternatives considered**: Direct charge (operator account pays directly) — not applicable because Skedular is the platform.

### Decision: Stripe partial refunds

- **Decision**: Multiple partial refunds against a single payment intent are supported. Total cannot exceed original charge.
- **Rationale**: Confirmed by Stripe API docs.
- **Implementation note**: Skedular must enforce the cap locally (`MarketplaceRefundRepository.GetByLocalEntityAsync` + sum of existing refund amounts) before calling the provider.

### Decision: Stripe idempotency keys

- **Decision**: Use Stripe idempotency keys on all refund creation requests.
- **Rationale**: Stripe idempotency keys prevent duplicate charge/refund creation. The key should be derived from the `MarketplaceRefund.Id` (or `IdempotencyKey` field). Current `StripeHostRefundService` passes `refund.Id` as the idempotency key — correct pattern; must be made consistent.

### Decision: Stripe refund statuses

- **Decision**: Map Stripe statuses as follows to local refund states:
  - `pending` → `ProviderPending`
  - `requires_action` → `ProviderPending` (customer bank details needed — Stripe handles notification)
  - `succeeded` → `Completed`
  - `failed` → `Failed`
  - `canceled` → `Cancelled` (new state)
- **Alternatives considered**: Keeping a separate accounting-pending state for Stripe refunds — rejected because provider processing and Xero accounting projection are different boundaries.

### Decision: Stripe webhook events

- **Decision**: Handle `refund.created`, `refund.updated`, `refund.failed`. Verify webhook signature using Stripe's `ConstructEvent` before processing.
- **Current state**: `BookingStripeWebhookController` handles `refund.created`, `refund.updated`, and `refund.failed`; both endpoints use `ConstructEvent` with configured signing secrets before publishing events.
- **Rationale**: `refund.failed` event is critical and must update local status to `Failed` and trigger operational alert.

### Decision: Stripe Connect — Host vs. Spaces charge type

- **Decision**: Persist and use the charge type recorded on the original payment context. Direct, Destination, and unknown contexts are routed from that persisted context rather than inferred during refund processing.
- **Rationale**: Refund processing path differs by charge type, so the original payment context is authoritative.

### Decision: Stripe processing fees

- **Decision**: Stripe processing fees from the original charge are not returned on refunds.
- **Rationale**: Confirmed by Stripe docs: "Stripe's processing fees from the original transaction aren't returned."
- **Implication**: The refund amount calculation must not include processing fees as refundable. This aligns with the spec's "non-refundable amount" component.

---

## 3. Xero Refund Research

**Source**: https://developer.xero.com/documentation/api/accounting/creditnotes and /payments (verified 2026-07-25)

### Decision: Xero credit note vs. cash refund distinction

- **Decision**: A Xero credit note (ACCRECCREDIT) is an accounting adjustment. To return money, a separate payment must be applied to the credit note via the Xero Payments endpoint referencing the credit note and a bank account. Skedular must never treat credit-note creation as proof of money movement.
- **Rationale**: Xero API docs state: "To refund credit notes, use the payments endpoint." Creating a credit note alone does not move money.
- **Implication**: Current `XeroRefundService.ProcessAsync` must be reviewed to confirm it creates a payment against the credit note (not just the credit note). If it only creates the credit note, the `Completed` status is premature.

### Decision: Xero credit note lifecycle

- **Decision**: Credit notes must be `AUTHORISED` before allocation. Creation and allocation are two separate API calls.
- **Rationale**: Confirmed by Xero API docs.
- **Implication**: `XeroRefundService` must handle the two-step process: (1) create/authorize credit note, (2) apply payment against credit note.

### Decision: Xero refund idempotency

- **Decision**: Use the `ExternalRefundId` (Xero CreditNoteID) as the local correlation key. On retry, check `GetByExternalPaymentRefundIdAsync` before creating a new Xero credit note.
- **Rationale**: Xero does not have native request idempotency keys like Stripe. The correlation must be enforced locally.

### Decision: Xero daily batch reconciliation

- **Decision**: Implement a daily scheduled batch that queries Xero for credit notes and payments modified since the last reconciliation timestamp and compares them to local `MarketplaceRefund` records.
- **Rationale**: Xero webhooks are available but less reliable than Stripe. A daily batch is the hybrid approach agreed in the spec clarifications.

---

## 4. Bank-Transfer Manual Refund Workflow Research

### Decision: Bank-transfer state machine path

- **Decision**: Bank-transfer refunds follow: `Requested → UnderReview → Approved → Processing → Completed`, with the processing and confirmation actions performed explicitly by an operator.
  - `Requested`: Refund record created, administrator notified
  - `UnderReview`: Administrator opens the refund queue
  - `Approved`: Administrator approves with payment details recorded
  - `Processing`: Marked as "bank transfer sent" with reference number and date
  - `Completed`: Confirmed — must require explicit confirmation step, not automatic
- **Rationale**: Spec FR-042 requires explicit manual steps. The system must never auto-complete a bank-transfer refund.
- **Alternatives considered**: No `UnderReview` state (go directly Requested → Approved) — rejected because the review step is required to prevent accidental approval.

### Decision: Duplicate prevention for bank-transfer refunds

- **Decision**: A unique constraint on `(OrganizationId, LocalEntityType, LocalEntityId)` where status is not `Rejected` or `Cancelled` prevents duplicate refund records for the same booking. At the application layer, `GetByLocalEntityAsync` is called before `Add`.
- **Rationale**: Idempotency guard must exist at both DB and application layer.

---

## 5. Refund State Machine Design

### Decision: Shared 10-state model across all payment methods

- **Decision**: All payment methods use the same `MarketplaceRefundStatusConstants`. Payment-method-specific routing is handled by service logic, not separate state sets.
- **Rationale**: Agreed in spec clarification session. Simplifies UI rendering, reporting, and audit trail.

**State transition table**:

| From                   | To                     | Trigger                                                           |
| ---------------------- | ---------------------- | ----------------------------------------------------------------- |
| Requested              | Processing             | Automatic path (Stripe)                                           |
| Requested              | UnderReview            | Manual-review path (bank transfer, admin-initiated discretionary) |
| UnderReview            | Approved               | Administrator approves                                            |
| UnderReview            | Rejected               | Administrator rejects                                             |
| Approved               | Processing             | System or administrator triggers provider submission              |
| Processing             | ProviderPending        | Provider confirms receipt but not yet complete (Stripe `pending`) |
| Processing             | Completed              | Provider confirms success immediately                             |
| Processing             | Failed                 | Provider returns error                                            |
| ProviderPending        | Completed              | Provider webhook: `refund.updated` → `succeeded`                  |
| ProviderPending        | Failed                 | Provider webhook: `refund.updated` → `failed` or `refund.failed`  |
| ProviderPending        | ReconciliationRequired | Reconciliation detects mismatch                                   |
| Failed                 | Processing             | Retry triggered by administrator or scheduled retry               |
| ReconciliationRequired | Completed              | Human confirms provider completed                                 |
| ReconciliationRequired | Failed                 | Human confirms provider failed                                    |
| Requested              | Cancelled              | Explicit cancellation before provider submission                  |
| UnderReview            | Cancelled              | Explicit cancellation before provider submission                  |
| Approved               | Cancelled              | Explicit cancellation before provider submission                  |

**Terminal states**: Completed, Failed (after exhausted retries), Rejected, and Cancelled. ReconciliationRequired is non-terminal and resolves only to Completed or Failed.

---

## 6. Concurrency and Idempotency

### Decision: Idempotency key scheme

- **Decision**: Add `IdempotencyKey` (string, max 128 chars, unique, not null) to `MarketplaceRefund`. Value is generated by `IRandomHelper` at record creation time and stored permanently. Used as Stripe idempotency key and Xero correlation key.
- **Rationale**: Stable idempotency key survives retries. DB unique constraint on `IdempotencyKey` prevents duplicate records.

### Decision: Concurrency guard

- **Decision**: Replace the existing unique local-entity index with a cancellation-only active-refund index. Each refund allocates to one or more source payments; allocation rows are locked/versioned while remaining balance is reserved.
- **Rationale**: Prevents duplicate cancellations while allowing valid multiple partial/modification refunds and enforcing the cap per original payment.
- **Alternatives considered**: A single booking-level unique index — rejected because it prevents split tenders and multiple partial refunds.

---

## 7. Cancellation Policy Snapshot

### Decision: Persist policy snapshot at booking purchase time

- **Decision**: Add a `CancellationPolicySnapshot` JSON field to `MarketplaceBooking` (the core booking entity, not the refund entity), capturing the policy type and refund rules at the time of purchase.
- **Rationale**: `MarketplaceRefundPolicyService` currently reads from the live `ProductPricing` object. If the product price changes, the refund calculation changes retroactively, violating FR-020. The snapshot must be taken when the booking is confirmed.
- **Alternatives considered**: Snapshot on the `MarketplaceRefund` record — rejected because the snapshot must be available for preview before a refund is created.

---

## 8. Post-Payout Stripe Refund

### Decision: Post-payout refund handling

- **Decision**: After a Stripe payout has been disbursed, the platform must fund the refund from platform balance and separately reverse the transfer to recover from the operator. Add a `PostPayoutRefund` boolean field to `MarketplaceRefund` to record which path was used.
- **Rationale**: Stripe docs confirm destination charge refunds are debited from the platform. Transfer reversal is the correct recovery mechanism.
- **Current implementation**: Payout disbursement is recorded from `payout.paid` webhook context. `PostPayoutRefund` is set only when that state is persisted; unknown payout state is routed to reconciliation rather than inferred from a transfer ID.

---

## 9. Web Application Coverage

### Decision: webapp and webapp-spaces UI components required

- **Decision**: Two new component groups required:
  1. `webapp` — `RefundStatusBadge` (inline status indicator), `RefundPreviewPanel` (pre-cancellation confirmation modal showing calculation breakdown), `RefundHistoryTimeline` (refund event audit display in booking history)
  2. `webapp-spaces` — `RefundQueue` (admin list of pending/under-review refunds), `BankTransferRefundWorkflow` (step-by-step manual approval form), `PartialRefundForm` (discretionary partial refund), `ReconciliationAlertBanner` (operational mismatch alert)
- **Rationale**: FR-110 requires UI in both apps. Relay fragments must be collocated; status labels must use American spelling.

### Decision: Public documentation

- **Decision**: Update `spaces/bookings/refunds.md` and create or update `host/bookings/payments-cancellations-and-refunds.md` to reflect the new state model, bank-transfer manual process, and per-method timelines.
- **Rationale**: FR-111; docs must ship alongside feature changes.

---

## 10. Unresolved Items for Planning Phase

| Item                                                            | Notes                                                                                                            |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------- |
| Historical refund-data migration                                 | Not applicable: this feature starts with an empty refund schema and requires no backward compatibility            |
| Confirm `XeroRefundService` creates payment against credit note | Confirmed: payment is created in `outstandingAmount <= 0m` branch; allocation used for outstanding invoice path  |
| Verify `BookingStripeWebhookController` signature validation    | Confirm `ConstructEvent` with signing secret is implemented                                                      |
| Audit subscription pro-rate calculation                         | Verify `CreateImmediateSubscriptionCancellationRefundAsync` correctly handles partially consumed billing windows |
| Confirm `reverse_transfer` usage on destination charge refunds  | Confirmed: persisted post-payout destination-charge context selects transfer reversal first, with structured-code fallback and reconciliation for unknown outcomes |

---

## 11. Code-Level Security and Correctness Findings (External Audit, 2026-07-25)

The findings in this section are the original pre-implementation audit baseline. The four findings were subsequently addressed in code and covered by focused tests; they remain here for traceability only.

An independent code review rated the existing implementation **7/10** and identified four specific risks:

### Finding 1 — Terminal Refund Can Be Reopened (Critical)

**File**: `src/booking/shared/Booking.Shared/Services/MarketplaceRefundService.cs` ~line 181
**Confirmed**: Yes. The upsert branch for an existing refund unconditionally sets `existingRefund.Status = Requested` regardless of current status. A `Completed` refund can be reset and reprocessed, issuing a double provider refund.
**Fix implemented**: Guard upsert: if existing refund is in a terminal state (`Completed`, `Rejected`, `Cancelled`) return it unchanged without mutation; legacy manual/accounting states are not part of the canonical model.

### Finding 2 — Admin Operations Do Not Gate on Confirmed Payment (High)

**File**: `src/booking/apis/Booking.Api/Services/MarketplaceRefundAdminService.cs` ~line 84
**Confirmed**: Yes. `UpdateStatusAsync` applies state transitions without `HasConfirmedPaymentAsync`. Only `ProcessInXeroAsync` has this guard. Admins can mark a refund `Completed` against an unpaid booking.
**Fix implemented**: Add `HasConfirmedPaymentAsync` to administrative transitions before approval, processing, completion, rejection, cancellation, reconciliation resolution, or retry.

### Finding 3 — Stripe Webhook Completion Is Silent (High, historical)

**File**: `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs` ~line 55
**Historical finding**: This was true during the initial audit. It is fixed in the current implementation: after reconciliation, the subscriber raises the owner GraphQL change and sends the refund status notification.

### Finding 4 — Xero Credit-Note ID Not Persisted on Partial Failure (Medium, historical)

**File**: `src/booking/shared/Booking.Shared/Services/XeroRefundService.cs` ~line 139
**Historical finding**: This was true during the initial audit. It is fixed in the current implementation: the Xero credit-note identifier is assigned and persisted immediately after creation, before allocation/payment settlement work.

## 12. Stripe Charge-Model Audit Result

- Spaces checkout sessions use Stripe Connect destination charges: `HostStripeApplicationFeeService.CreateDestinationCharge`
  supplies `PaymentIntentData.TransferData.Destination` and an application fee.
- The checkout activity creates the session on the platform account for that destination-charge path, while non-host
  flows use the connected account directly.
- Refund processing uses the captured PaymentIntent with `RefundApplicationFee = true` and selects `ReverseTransfer`
  from persisted charge and payout context.
- The recorded path may be `TransferReversal` or `PlatformFunded`; unsupported or ambiguous provider outcomes are
  routed to reconciliation rather than selected from exception text.
