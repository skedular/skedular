# GraphQL Contract: Subscription Status Filter Options

**Feature**: `005-subscription-landing-page-filter`  
**API**: Booking domain GraphQL subgraph

---

## New Query: `marketplaceBookingSubscriptionStatuses`

Returns all valid subscription status option values for populating the filter combo box.

### Schema Addition

```graphql
type MarketplaceBookingSubscriptionStatusDetails {
  type: MarketplaceBookingSubscriptionStatus!
  name: String!
}

extend type Query {
  marketplaceBookingSubscriptionStatuses: [MarketplaceBookingSubscriptionStatusDetails!]!
}
```

### Example Response

```json
{
  "data": {
    "marketplaceBookingSubscriptionStatuses": [
      { "type": "ACTIVE", "name": "Active" },
      { "type": "CANCELLED", "name": "Cancelled" },
      { "type": "EXPIRED", "name": "Expired" },
      { "type": "RENEWAL_FAILED", "name": "Renewal failed" },
      { "type": "PAUSED", "name": "Paused" }
    ]
  }
}
```

**Note**: `MarketplaceBookingSubscriptionStatusDetails` type already exists in the schema. No new type registration required.

---

## New Query: `marketplaceBookingPaymentStatuses`

Returns all valid payment status option values for populating the payment status filter combo box.

### Schema Addition

```graphql
type MarketplaceBookingPaymentStatusDetails {
  type: PaymentStatus!
  name: String!
}

extend type Query {
  marketplaceBookingPaymentStatuses: [MarketplaceBookingPaymentStatusDetails!]!
}
```

### Example Response

```json
{
  "data": {
    "marketplaceBookingPaymentStatuses": [
      { "type": "NOT_SET", "name": "Not set" },
      { "type": "PENDING", "name": "Pending" },
      { "type": "REJECTED", "name": "Rejected" },
      { "type": "CONFIRMED", "name": "Confirmed" },
      { "type": "EXPIRED", "name": "Expired" },
      { "type": "NO_PAYMENT_REQUIRED", "name": "No payment required" }
    ]
  }
}
```

---

## Extended Input: `MarketplaceBookingSubscriptionWhereInput`

Adds multi-value status and payment status filter fields.

### Schema Change

```graphql
input MarketplaceBookingSubscriptionWhereInput {
  # existing fields unchanged ...
  status: MarketplaceBookingSubscriptionStatus
  # new fields:
  statuses: [MarketplaceBookingSubscriptionStatus!]
  paymentStatuses: [PaymentStatus!]
}
```

### Filtering Semantics

| `statuses` value   | Behaviour                                           |
| ------------------ | --------------------------------------------------- |
| `null` or `[]`     | No restriction — all subscription statuses returned |
| `[ACTIVE]`         | Only active subscriptions                           |
| `[ACTIVE, PAUSED]` | Active OR paused subscriptions                      |

| `paymentStatuses` value | Behaviour                                                              |
| ----------------------- | ---------------------------------------------------------------------- |
| `null` or `[]`          | No restriction — all payment statuses returned                         |
| `[PENDING]`             | Only subscriptions whose marketplace booking payment status is pending |
| `[PENDING, REJECTED]`   | Pending OR rejected                                                    |

When both `statuses` and `paymentStatuses` are non-empty, both conditions are applied (AND logic).

### Example Query (filtered)

```graphql
query OrganizationSubscriptions(
  $organizationCustomDomain: String!
  $statuses: [MarketplaceBookingSubscriptionStatus!]
  $paymentStatuses: [PaymentStatus!]
) {
  marketplaceBookingSubscriptionStatuses {
    type
    name
  }
  marketplaceBookingPaymentStatuses {
    type
    name
  }
  marketplaceBookingSubscriptions(
    first: 50
    where: { organizationCustomDomain: $organizationCustomDomain, statuses: $statuses, paymentStatuses: $paymentStatuses }
    orderBy: [{ field: NEXT_RENEWAL_AT, direction: ASCENDING }]
  ) {
    edges {
      node {
        id
        status {
          type
          name
        }
        marketplaceBooking {
          paymentStatus {
            type
            name
          }
        }
      }
    }
    totalCount
  }
}
```
