# Contract: Spaces Trial GraphQL Surfaces

## Authenticated organization subscription

Extend the existing Organization query; do not create a second competing subscription source.

```graphql
enum SpacesSubscriptionStatus {
  TRIAL_ACTIVE
  TRIAL_EXPIRING
  TRIAL_EXPIRED
  COMPLIMENTARY_BRIDGE
  PAID_ACTIVE
  PAID_INACTIVE
  LEGACY_ACTIVE
  MISSING_STATE
}

enum SpacesAccessReasonCode {
  ALLOWED_TRIAL
  ALLOWED_PAID
  ALLOWED_COMPLIMENTARY_BRIDGE
  ALLOWED_PROTECTIVE_ACTION
  ALLOWED_READ_OR_RECOVERY
  TRIAL_EXPIRED
  PAID_INACTIVE
  MISSING_TRIAL_STATE
  MISSING_OFFERING_STATE
  ACTION_NOT_ALLOWED
}

type SpacesAccessReasonCodeDetails {
  type: SpacesAccessReasonCode!
  name: String!
}

type OrganizationSpacesSubscriptionDetails {
  # Existing fields retained.
  subscriptionStatus: SpacesSubscriptionStatus!
  trialStartedAt: DateTime
  trialEndsAt: DateTime
  remainingTrialDays: Int!
  canUseProduct: Boolean!
  canAcceptBookings: Boolean!
  canProtectExistingCommitments: Boolean!
  upgradeRequired: Boolean!
  isComplimentaryBridge: Boolean!
  nextBillingAt: DateTime
  accessReason: SpacesAccessReasonCodeDetails!
}

extend type Query {
  organizationSpacesSubscription(organizationId: String!): OrganizationSpacesSubscriptionDetails
  spacesSubscriptionStatuses: [SpacesSubscriptionStatusDetails!]!
  spacesAccessReasonCodes: [SpacesAccessReasonCodeDetails!]!
}
```

Authorization remains organization-member/support scoped.

## Explicit paid upgrade

Keep existing supported offering/subscription mutations but route them through one transition service. The response includes evaluated status and billing boundary so clients can render the bridge accurately.

```graphql
input UpdateOrganizationSpacesSubscriptionInput {
  clientMutationId: String
  organizationId: String!
  planCode: PricingCatalogSubscriptionPlanCode!
  customCapacity: Int
}

type UpdateOrganizationSpacesSubscriptionPayload {
  clientMutationId: String
  organizationSpacesSubscription: OrganizationSpacesSubscriptionDetails!
}
```

Rules:

- Growth/Business self-service upgrades require an attached payment method.
- Trial expiry never auto-invokes this mutation.
- A successful mid-month paid transition reports `COMPLIMENTARY_BRIDGE` and `nextBillingAt` at the next first day.
- Contact Us/admin assignments retain their existing authorization and negotiated-term path.
- Failed/incomplete transition returns an error and leaves expired access blocked.

## Public booking availability

Booking owns this decision and exposes only neutral availability on the public Organization/storefront surface. The field is implemented by the Booking subgraph as an extension of the federated `Organization` type, using Booking's locally replicated Organization offering state.

```graphql
enum SpacesPublicAvailabilityCode {
  AVAILABLE
  TEMPORARILY_UNAVAILABLE
}

type SpacesPublicBookingAvailability {
  canAcceptBookings: Boolean!
  availabilityCode: SpacesPublicAvailabilityCode!
  message: String!
}

extend type Organization {
  spacesPublicBookingAvailability: SpacesPublicBookingAvailability!
}
```

This public type must not include plan code, trial dates, remaining days, payment state, or internal denial reasons.

The Booking resolver MUST NOT synchronously call Organization API or another domain API. Organization remains the source of trial/subscription data and propagates durable inputs through its event/outbox path; Booking processor subscribers update the local projection. This field is a customer UI hint, not an authorization substitute. Every booking mutation re-evaluates access locally using authoritative server time before creating or accepting a booking.

## Booking status and mutation errors

Retain paid quota fields and add access-first fields to the current Booking status.

```graphql
type BookingSpacesQuotaStatusDetails {
  # Existing fields retained for paid quota compatibility.
  quotaApplicable: Boolean!
  subscriptionStatus: SpacesSubscriptionStatus!
  canCreateBooking: Boolean!
  trialEndsAt: DateTime
  remainingTrialDays: Int!
  accessReason: SpacesAccessReasonCodeDetails!
}

type BookingSpacesAccessErrorDetails {
  errorCode: String!
  status: SpacesSubscriptionStatus!
  reasonCode: SpacesAccessReasonCodeDetails!
  trialEndsAt: DateTime
  remainingTrialDays: Int!
  upgradeRequired: Boolean!
}

type BookingPayload {
  # Existing booking and quotaError fields retained.
  accessError: BookingSpacesAccessErrorDetails
}
```

- Free active/expiring trial returns `quotaApplicable = true` and retains the existing 100-booking-instance monthly quota.
- Paid plans keep the existing `quotaError` behavior.
- Expired trial returns `accessError`, never a quota-exceeded error.
- Customer UI maps `accessError` to neutral temporary-unavailability copy.

## Generation

```bash
scripts/generate-graphql.sh
src/web/apps/webapp/scripts/generate.sh
```

Use `make generate` if event/OpenAPI/GraphQL/web generated surfaces are changed together.
