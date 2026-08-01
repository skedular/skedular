# Quickstart Validation Guide: End-to-End Refund Reliability

**Feature**: 037-refund-reliability
**References**: [spec.md](spec.md) | [data-model.md](data-model.md) | [contracts/graphql-mutations.md](contracts/graphql-mutations.md)

---

## Prerequisites

1. Skedular running locally with Docker Compose: `docker-compose up` (or `docker-compose-min.yml` for minimal dependencies)
2. Stripe CLI installed and webhook forwarding active: `stripe listen --forward-to localhost:<port>/api/booking/stripe-webhook`
3. Xero sandbox credentials configured in local `appsettings.Development.json`
4. At least one Spaces organization with Stripe Connect configured and at least one active product with a cancellation policy
5. At least one Host organization with a Host booking available
6. `dotnet test` passing on `Booking.Shared.UnitTests` and `Booking.Domain.IntegrationTests`

---

## Scenario 1: Customer Cancels Within the Cancellation Window (Stripe)

**Goal**: Verify that a customer cancellation within the policy window creates a refund record, submits a Stripe refund, receives the webhook, and transitions to `Completed`.

**Steps**:

1. Create a paid Spaces booking (Stripe payment method) with a product that has a cancellation window (e.g., 24-hour window with 100% refund).
2. Cancel the booking via the customer portal before the window expires.
3. Verify:
   - Refund preview shows correct `FinalRefundableAmount` and `NonRefundableAmount`.
   - Confirm cancellation.
   - `MarketplaceRefund` record is created with `Status = Requested`, `IdempotencyKey` set.
   - Refund transitions to `Processing` within seconds.
   - Stripe CLI shows `refund.created` webhook received.
   - Refund transitions to `ProviderPending` or `Completed` depending on Stripe test response.
   - Customer booking page shows "Refund processing" (not "Refund completed") until `refund.updated → succeeded` is received.
   - On webhook `succeeded`: refund transitions to `Completed`, customer view updates.
   - Customer notification email sent: subject must not say "Refund completed" if still `ProviderPending`.

**Expected outcome**: `MarketplaceRefund.Status = Completed`, `ExternalPaymentRefundId` set to Stripe refund ID.

---

## Scenario 2: Customer Cancels Outside the Cancellation Window

**Goal**: Verify that a zero-refund cancellation creates no provider request, correctly records refund as zero, and does not show misleading status.

**Steps**:

1. Create a paid booking with a policy that has a 24-hour window (100% refund within window).
2. Simulate expiry (advance system clock or use a past-start booking).
3. Cancel via customer portal.
4. Verify:
   - Refund preview shows `FinalRefundableAmount = 0`, `NonRefundableAmount = original amount`, policy explanation shown.
   - Confirm cancellation.
   - `MarketplaceRefund` record created with `RefundAmount = 0`, `Status = Completed` (no provider step needed).
   - No Stripe API call made.
   - Customer view shows "Booking cancelled — refund not applicable" with policy reason.

---

## Scenario 3: Bank-Transfer Manual Refund Workflow

**Goal**: Verify the full admin bank-transfer review workflow.

**Steps**:

1. Create a paid booking with bank-transfer payment method.
2. Cancel the booking.
3. Verify `MarketplaceRefund.Status = Requested`.
4. As admin, open the refund queue in `webapp-spaces` — refund appears with full context.
5. Admin approves: GraphQL mutation `approveMarketplaceRefund` → status transitions to `Approved`.
6. Admin records bank transfer: `recordBankTransferRefundSent` with reference number → status transitions to `Processing`.
7. Attempt to call `recordBankTransferRefundSent` again on the same refund — must return error (duplicate prevention).
8. Admin confirms receipt: `confirmBankTransferRefundReceived` → status transitions to `Completed`.
9. Verify `BankTransferReference` and `BankTransferSentAt` are stored.
10. Verify `MarketplaceRefundEvent` has entries for Requested, Approved, Processing (sent), Completed with actor IDs.

**Expected outcome**: 4-step audit trail complete; no auto-completion without reference number.

---

## Scenario 4: Idempotency — Duplicate Cancellation Request

**Goal**: Verify that submitting the same cancellation twice creates exactly one refund record and one Stripe request.

**Steps**:

1. Create a paid booking.
2. Submit cancellation request from two browser tabs simultaneously (or two API calls in rapid succession).
3. Verify:
   - Exactly one `MarketplaceRefund` record exists for the booking.
   - Exactly one Stripe refund ID recorded.
   - No duplicate Stripe API call (check Stripe Dashboard or CLI output).

---

## Scenario 5: Provider Timeout — Refund Stuck in ProviderPending

**Goal**: Verify reconciliation detects and resolves a stuck refund.

**Steps**:

1. Create a paid booking and initiate a refund.
2. In test mode, suppress the `refund.updated` webhook (e.g., stop Stripe CLI forwarding).
3. Refund transitions to `ProviderPending` and stays there.
4. Advance clock beyond the reconciliation threshold (or trigger the reconciliation batch manually).
5. Verify:
   - Reconciliation batch queries Stripe for the refund status.
   - If Stripe reports `succeeded`: refund transitions to `Completed`.
   - If Stripe reports `failed`: refund transitions to `Failed` and appears in operations alert.
   - `ReconciliationStatus` field updated; `LastReconciledAt` set.

---

## Scenario 6: Admin Partial Refund

**Goal**: Verify admin can issue a discretionary partial refund within the refundable balance.

**Steps**:

1. Create a paid booking (amount: $100).
2. As admin in `webapp-spaces`, initiate a partial refund of $30.
3. Verify: `MarketplaceRefund` created for $30, transitions to `Processing`.
4. Attempt to initiate a second partial refund of $80 (total would be $110).
5. Verify: second request rejected with error showing remaining balance = $70.

---

## Scenario 7: Subscription Pro-Rated Cancellation

**Goal**: Verify subscription cancellation calculates the correct pro-rated refund.

**Steps**:

1. Create a Spaces subscription with upfront monthly billing ($120/month, 30 days).
2. Cancel after 10 days.
3. Verify refund preview shows: original amount $120, consumed 10/30 = $40 non-refundable, refund amount $80.
4. Confirm cancellation.
5. Verify `MarketplaceRefund.RefundAmount = 80`, `BaseAmount = 120`.

---

## Running the Test Suite

```bash
# Unit tests (refund policy, state machine, calculation)
dotnet test src/booking/shared/Booking.Shared.UnitTests/ \
  --filter "FullyQualifiedName~MarketplaceRefund"

# Integration tests (database, webhook, Stripe, Xero)
dotnet test src/booking/domain/Booking.Domain.IntegrationTests/ \
  --filter "Category=Refund"

# Frontend component tests
cd src/web/apps/webapp && pnpm test --filter refund
cd src/web/apps/webapp-spaces && pnpm test --filter refund
```

---

## Post-Implementation Smoke Checks

The following are deployment/manual smoke checks, not completion criteria for the implementation. They remain
explicitly deferred until a deployed environment with Stripe CLI, Xero sandbox credentials, and representative
Spaces/Host data is available. Automated unit, repository-boundary, GraphQL, and frontend component tests are the
current validation evidence; these checks must not be reported as completed until they are run in that environment.

- [ ] **Deferred — deployment smoke test:** All 7 validation scenarios pass
- [ ] **Deferred — static check:** No floating-point arithmetic in any refund calculation path (`grep -r "float\|double" src/booking/ --include="*.cs"` returns nothing in refund code)
- [ ] **Deferred — database check:** `IdempotencyKey` is never null on any `MarketplaceRefund` record
- [ ] **Deferred — database check:** `MarketplaceRefundEvent.PreviousStatus` and `NewStatus` populated on every transition
- [ ] **Deferred — deployed webhook check:** Stripe webhook signature validation confirmed active in `BookingStripeWebhookController`
- [ ] **Deferred — deployed documentation check:** Public docs at `/docs/spaces/billing-and-payments/refunds` and `/docs/host/payments-and-refunds` load and contain accurate content
- [ ] **Deferred — deployed UI check:** Admin refund queue in `webapp-spaces` shows refunds filterable by status and external reconciliation records paginated/filterable by provider and status
- [ ] **Deferred — deployed UI check:** Customer booking history shows the correct refund status badge at each stage
