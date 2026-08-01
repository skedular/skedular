# GraphQL Contracts: Refund Mutations

**Feature**: 037-refund-reliability
**Schema file**: `src/booking/apis/Booking.Api/schema.graphqls`
**Regenerate after changes**: `scripts/generate-graphql.sh`

---

## Contract Scope

Legacy completion, accounting, review, failure, and direct Xero-processing mutations are removed from the new contract. Provider processing is initiated by the refund workflow and resolved through the formal retry/reconciliation mutations.

All elevated mutations require an active Skedular organization Owner or Administrator membership, with organization isolation enforced per mutation.

## Authorization Matrix

| Mutation | Owner | Administrator | Member | Non-member / other organization |
|---|---:|---:|---:|---:|
| `approveMarketplaceRefund` | Allow | Allow | Deny | Deny |
| `rejectMarketplaceRefund` | Allow | Allow | Deny | Deny |
| `cancelMarketplaceRefund` | Allow | Allow | Deny | Deny |
| `retryMarketplaceRefund` | Allow | Allow | Deny | Deny |
| `createPartialMarketplaceRefund` | Allow | Allow | Deny | Deny |
| `recordBankTransferRefundSent` | Allow | Allow | Deny | Deny |
| `confirmBankTransferRefundReceived` | Allow | Allow | Deny | Deny |
| `resolveRefundReconciliationRequired` | Allow | Allow | Deny | Deny |

Authorization is evaluated against the refund's organization before any state transition or provider action. A
Member, non-member, or caller from another organization receives the same denial; mutation responses must not
disclose whether the refund exists.

---

## New Mutations Required

### `approveMarketplaceRefund`

```graphql
mutation approveMarketplaceRefund(input: ApproveMarketplaceRefundInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input ApproveMarketplaceRefundInput {
  id: String!
  reason: String
}
```

**Behaviour**: Transitions refund from `UnderReview` → `Approved`. Records `ApprovedByCustomerId` and `ApprovedAt`. For bank transfer this authorizes, but does not submit, a manual payment; `recordBankTransferRefundSent` remains the separate payment-recording action.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `rejectMarketplaceRefund`

```graphql
mutation rejectMarketplaceRefund(input: RejectMarketplaceRefundInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input RejectMarketplaceRefundInput {
  id: String!
  reason: String!
}
```

**Behaviour**: Transitions refund from `UnderReview` → `Rejected` (terminal). Records `RejectedByCustomerId`, `RejectedAt`, and `RejectionReason`. Sends customer notification.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `retryMarketplaceRefund`

```graphql
mutation retryMarketplaceRefund(input: RetryMarketplaceRefundInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input RetryMarketplaceRefundInput {
  id: String!
}
```

**Behaviour**: Transitions refund from `Failed` → `Processing`. Increments `RetryCount`. Uses same `IdempotencyKey` to prevent duplicate provider requests. Only allowed if retry count is below the configured maximum (default: 3).

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `recordBankTransferRefundSent`

```graphql
mutation recordBankTransferRefundSent(input: RecordBankTransferRefundSentInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input RecordBankTransferRefundSentInput {
  id: String!
  bankTransferReference: String!
  sentAt: DateTime!
}
```

**Behaviour**: Transitions bank-transfer refund from `Approved` → `Processing` (sent). Records `BankTransferReference` and `BankTransferSentAt`. Does NOT mark as Completed — a second explicit confirmation step is required.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `confirmBankTransferRefundReceived`

```graphql
mutation confirmBankTransferRefundReceived(input: ConfirmBankTransferRefundReceivedInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input ConfirmBankTransferRefundReceivedInput {
  id: String!
  reason: String
}
```

**Behaviour**: Transitions bank-transfer refund from `Processing` → `Completed`. Final confirmation step that the money movement has been verified. `BankTransferReference` must be present before this transition is allowed.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `cancelMarketplaceRefund`

```graphql
mutation cancelMarketplaceRefund(input: CancelMarketplaceRefundInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input CancelMarketplaceRefundInput {
  id: String!
  reason: String
}
```

**Behaviour**: Transitions refund from `Requested`, `UnderReview`, or `Approved` → `Cancelled`. It is rejected after provider submission (`Processing` or later); those refunds must resolve through provider confirmation or reconciliation. Records `CancelledAt` and a reason.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

---

### `resolveRefundReconciliationRequired`

```graphql
mutation resolveRefundReconciliationRequired(input: ResolveRefundReconciliationRequiredInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input ResolveRefundReconciliationRequiredInput {
  id: String!
  resolvedStatus: String! # "Completed" or "Failed"
  reason: String!
  providerReference: String
}
```

**Behaviour**: Resolves a refund stuck in `ReconciliationRequired`. Transitions to `Completed` or `Failed` based on human investigation outcome.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

### `createPartialMarketplaceRefund`

```graphql
mutation createPartialMarketplaceRefund(input: CreatePartialMarketplaceRefundInput!): MarketplaceRefundPayload!
```

**Input**:

```graphql
input CreatePartialMarketplaceRefundInput {
  localEntityType: String!
  localEntityId: String!
  sourcePaymentProvider: String!
  sourcePaymentReference: String!
  amount: Decimal!
  reason: String!
}
```

**Behaviour**: Creates a distinct discretionary partial-refund operation only after atomically reserving the remaining balance of the selected source-payment allocation. The amount cannot exceed that source payment's captured amount less completed, pending, or reserved allocations. A new idempotency key is generated for this distinct operation.

**Authorization**: Active organization Owner or Administrator only. Members and non-members are denied.

### Partial-booking resolution mutations

`acceptPartialMarketplaceBooking` and `declinePartialMarketplaceBooking` are customer-authorized mutations. They resolve the durable partial-booking record exactly once: acceptance retains created occurrences and issues the allocated prorated refund; decline cancels created occurrences and issues the full refund. The deadline workflow uses the same idempotent decline outcome.

---

## Updated Types

### `MarketplaceRefundDetails` (Extended)

New fields added to existing type:

```graphql
type MarketplaceRefundDetails {
  # existing fields ...
  idempotencyKey: String!
  approvedByCustomer: CustomerDetails
  approvedAt: DateTime
  rejectedByCustomer: CustomerDetails
  rejectedAt: DateTime
  rejectionReason: String
  cancelledAt: DateTime
  bankTransferReference: String
  bankTransferSentAt: DateTime
  postPayoutRefund: Boolean!
  retryCount: Int!
  reconciliationStatus: String
  lastReconciledAt: DateTime
  calculationBreakdown: RefundCalculationBreakdownDetails
  paymentAllocations: [MarketplaceRefundPaymentAllocationDetails!]!
}
```

### `RefundCalculationBreakdownDetails` (New)

```graphql
type RefundCalculationBreakdownDetails {
  originalGrossAmount: Decimal!
  eligibleRefundAmount: Decimal!
  cancellationDeduction: Decimal!
  taxAdjustment: Decimal!
  previouslyRefundedAmount: Decimal!
  finalRefundableAmount: Decimal!
  nonRefundableAmount: Decimal!
  calculationReason: String!
  calculatedAt: DateTime!
  timezoneId: String!
}

type MarketplaceRefundPaymentAllocationDetails {
  sourcePaymentProvider: String!
  sourcePaymentReference: String!
  sourcePaymentAmount: Decimal!
  allocatedRefundAmount: Decimal!
  currency: String!
}
```

### `MarketplaceRefundPreviewDetails` (Extended)

Additional field for calculation breakdown visibility:

```graphql
type MarketplaceRefundPreviewDetails {
  # existing fields ...
  calculationBreakdown: RefundCalculationBreakdownDetails
  requiresReview: Boolean! # true for bank-transfer or discretionary paths
  reviewReason: String # why review is required
}
```
