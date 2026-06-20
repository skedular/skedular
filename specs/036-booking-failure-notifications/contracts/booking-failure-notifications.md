# Contract Design: Booking Failure Communications

This is the planned source-level contract. GraphQL schema exports and Relay artifacts are generated only after implementing the source resolver/type changes with `scripts/generate-graphql.sh`.

## Read Contract

Expose a `marketplaceBookingFailure` relationship on booking/recurring subscription history and authorized organization/host booking views.

```graphql
type MarketplaceBookingFailureDetails {
  id: ID!
  category: MarketplaceBookingFailureCategoryDetails!
  scope: MarketplaceBookingFailureScopeDetails!
  finalizedAt: DateTime!
  requestedFrom: DateTime
  requestedUntil: DateTime
  customerAction: MarketplaceBookingFailureCustomerActionDetails!
  deliveries: [MarketplaceBookingFailureDeliveryDetails!]!
}
```

The details/choice types follow the repository GraphQL choice-type convention: machine-readable type plus display name. Delivery details are visible only to authorized stakeholders; customer views expose only the customer's own action and notification state.

## Submission Outcome Contract

The existing marketplace booking/subscription mutations keep their successful return shape. A final immediate availability conflict is returned as a typed customer-safe outcome rather than a generic execution error:

```graphql
type MarketplaceBookingSubmissionOutcome {
  booking: Booking
  failure: MarketplaceBookingFailureDetails
  accessError: SpacesAccessErrorDetails
}
```

Exactly one of `booking`, `failure`, or `accessError` is present for a completed immediate submission. Validation and technical execution errors retain their current non-success behavior unless mapped to a safe final category.

## Delivery Semantics

- `Email` is sent only to a verified, authorized, de-duplicated address; both Spaces and Host owner/administrator recipients follow their active authorized organization membership.
- `InApplication` means a retained, queryable failure outcome in the recipient's authorized booking/history surface; it is not a transient toast.
- Duplicate finalization, workflow replay, and delivery retry must preserve the unique failure-recipient-channel delivery record.
- A delivery failure never changes booking allocation or the failure's final category.

## Event/Workflow Boundary

The booking domain persists finalization and delivery rows before scheduling dispatch through its existing transactional Temporal outbox. No new cross-domain public event is required for the first slice; booking history/read models are updated through the existing booking GraphQL/topic patterns. If a cross-domain consumer later needs failures, define a protobuf event under `api-definitions/events/skedular` first and regenerate.
