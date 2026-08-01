# Webhook Event Contracts: Stripe and Provider Webhooks

**Feature**: 037-refund-reliability
**Webhook controller**: `src/booking/apis/Booking.Api/Controllers/BookingStripeWebhookController.cs`
**Internal subscriber**: `src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs`

---

## Stripe Webhook Events

### `refund.created`

Received when Stripe creates a refund. Used to confirm submission was accepted.

**Handling**:

1. Verify Stripe-Signature header using `ConstructEvent` with signing secret.
2. Extract `refund.id` and `refund.metadata.marketplace_refund_id`.
3. Call `StripeHostRefundService.ReconcileAsync(stripeRefund)`.
4. If local refund is in `Processing`, transition to `ProviderPending` (status `pending`) or `Completed` (status `succeeded`).

**Current implementation**: `refund.created` is signature-validated by `BookingStripeWebhookController`, published to the internal booking event stream, and reconciled by `StripeHostRefundService.ReconcileAsync`.

---

### `refund.updated`

Received when Stripe updates a refund (status change, ARN available, etc.).

**Handling**:

1. Verify signature.
2. Extract `refund.status`.
3. Map Stripe status to local state:
   - `succeeded` → `Completed`
   - `failed` or `canceled` → `Failed` (then Cancelled)
   - `pending` or `requires_action` → `ProviderPending`
4. If status changed, update `MarketplaceRefund`, add `MarketplaceRefundEvent`, save changes.
5. If transition to `Failed`: increment `RetryCount`, trigger retry or escalate to operations.

**Accounting boundary**: A Stripe webhook only reconciles the Stripe payment refund. It MUST NOT create a Xero
credit note or invoke a Stripe-to-Xero projection method. Xero credit-note processing is limited to refunds whose
payment flow is owned by the accounting/bank-transfer path.

---

### `refund.failed`

Received when a Stripe refund definitively fails.

**Handling**:

1. Verify signature.
2. Transition local refund to `Failed`.
3. Record `failure_reason` in `LastError`.
4. Add audit event with `CorrelationId`.
5. If `RetryCount` < max: schedule retry with backoff.
6. If `RetryCount` >= max: transition to failed terminal state and add to operations alert queue.
7. Send customer notification: refund failed.

**Current implementation**: `refund.failed` is signature-validated by `BookingStripeWebhookController`, published to the internal booking event stream, and handled by `BookingInternalSubscriber` through the same Stripe reconciliation path as the other refund events.

---

## Webhook Security Requirements

| Requirement                        | Implementation                                                                                                             |
| ---------------------------------- | -------------------------------------------------------------------------------------------------------------------------- |
| Stripe-Signature header validation | `ConstructEvent(payload, signature, webhookSecret)` — must confirm this is implemented in `BookingStripeWebhookController` |
| Replay protection                  | Stripe includes `created` timestamp in event; reject events older than 5 minutes                                           |
| Idempotent processing              | Before processing: look up refund by `ExternalPaymentRefundId`; skip if already in target state                            |
| No sensitive data in logs          | Log only: refund ID, event type, new status, correlation ID                                                                |

---

## Reconciliation Batch Contract

**Component**: New `MarketplaceRefundReconciliationJob` (Hangfire or Temporal activity)
**Schedule**: Daily (time configured per environment, default: 02:00 UTC)

### Stripe Reconciliation

1. Query `MarketplaceRefundRepository.GetRefundsForReconciliationAsync(threshold, maxCount)` for a bounded scheduled reconciliation batch.
2. For each: call Stripe Refunds API `GET /v1/refunds/{external_id}`.
3. Compare `status` with local `PaymentRefundStatus`.
4. If mismatch: update local record and add audit event.
5. If provider refund not found: transition to `ReconciliationRequired` and alert operations.
6. Emit structured log: `MarketplaceRefundReconciliationResult` with counts: matched, mismatched, not-found.

### Xero Reconciliation

1. Process only refunds that have an accounting-owned payment flow and a persisted Xero `ExternalRefundId`.
2. Resolve the credit note by persisted `ExternalRefundId`; use `LastReconciledAt` only as an optimization for historical searches.
3. Verify the credit-note status, allocation, and payment settlement by persisted identifiers.
4. If status mismatch (e.g., credit note voided externally): update `ReconciliationStatus = Mismatch`, add audit event.
5. Update `LastReconciledAt`.

### Bank-Transfer Reconciliation

1. Query `MarketplaceRefundRepository.GetPendingBeyondThresholdAsync(now - 3 business days)` where `PaymentProvider = null` and status = `Approved`.
2. For each: emit operational alert (bank transfer approved but not sent).
