# GraphQL Contract: Marketplace Purchase Lifecycle History

The Booking GraphQL schema adds a history connection to eligible subscription and entitlement purchase details. Names below are normative; generated schema and Relay artifacts are outputs and must be regenerated from the source contract.

```graphql
enum MarketplacePurchaseHistoryEventType {
  PURCHASE_CREATED
  SUBSCRIPTION_STARTED
  SUBSCRIPTION_RENEWED
  CANCELLATION_SCHEDULED
  CANCELLATION_COMPLETED
  ENTITLEMENT_CREATED
  ENTITLEMENT_EXPIRED
  CREDITS_CONSUMED
  PAYMENT_STATE_CHANGED
  REFUND_STATE_CHANGED
}

enum MarketplacePurchaseHistorySourceType {
  SUBSCRIPTION
  ENTITLEMENT
}

type MarketplacePurchaseHistoryEventDetails {
  id: String!
  sourceId: String!
  sourceType: MarketplacePurchaseHistorySourceType!
  type: MarketplacePurchaseHistoryEventType!
  name: String!
  occurredAt: DateTime!
  recordedAt: DateTime!
  cancellationRequestedAt: DateTime
  cancellationEffectiveAt: DateTime
  paymentStatus: PaymentStatus
  refundId: String
  refundStatus: MarketplaceRefundStatus
  creditQuantity: Int
  remainingCreditQuantity: Int
  amount: Decimal
  currency: Currency
  reason: String
}

type MarketplacePurchaseHistoryEventEdge {
  node: MarketplacePurchaseHistoryEventDetails!
  cursor: String!
}

type MarketplacePurchaseHistoryEventConnection {
  pageInfo: PageInfo!
  edges: [MarketplacePurchaseHistoryEventEdge!]!
  totalCount: Int!
}
```

Eligible purchase details expose `history(first, after, last, before)`; standalone booking details do not expose or query it. The query is authorized using the same organization/customer scope as the purchase list. Empty history is a successful empty connection.
