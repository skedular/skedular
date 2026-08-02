# Data Model: Admin Cancellation Policy Override

## Cancellation Actor

Represents the authenticated source of a cancellation request.

| Field | Description | Rules |
|---|---|---|
| Category | Customer, owner, or administrator | Server-derived; never trusted from client input |
| Customer identity | Authenticated customer when applicable | Required for customer requests; retained for audit |
| Organization identity | Product-owning organization | Required for owner/admin requests |
| Permission basis | Existing booking/subscription management authority | Required for owner/admin override |

## Cancellation Request

Represents the operation to cancel a marketplace booking or subscription.

| Field | Description | Rules |
|---|---|---|
| Target | Booking or subscription identifier | Must be active or safely idempotent |
| Mode | Immediate or at period end | Period-end applies to subscriptions; booking behavior remains existing |
| Actor | Cancellation Actor | Resolved server-side |
| Override applied | Whether customer policy was bypassed | True only after authorization succeeds and policy would otherwise reject |
| Override reason | Short operator-provided explanation | Required when override is applied; rejected when absent/blank |
| Outcome | Cancelled, scheduled, rejected, or already terminal | Must preserve existing lifecycle semantics |

## Cancellation Audit Event

Durable history for explaining cancellation decisions.

- Target type and identifier
- Actor category and actor identity when available
- Product-owning organization
- Requested cancellation mode
- Customer policy result
- Whether an override was applied
- Operator reason when overridden
- Cancellation outcome and timestamp
- Correlation identifier for related refund/payment processing

## Refund Relationship

Cancellation may create or update a separate refund aggregate; it does not make refund state part of cancellation authorization.

- **Stripe**: eligible request proceeds through automatic processing.
- **Bank transfer**: refund remains pending until owner/admin approval and transfer confirmation.
- **Xero**: refund remains subject to owner/admin approval, Xero processing availability, and reconciliation.

## State Rules

1. Customer request + policy fails → cancellation rejected; no override.
2. Authorized operator + policy fails + reason present → cancellation proceeds; override recorded.
3. Authorized operator + refund created → provider-specific refund workflow starts or waits for approval.
4. Replayed request → existing terminal/scheduled state returned; no duplicate refund boundary or provider action.
