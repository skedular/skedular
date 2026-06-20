# GraphQL Contract: Spaces Pricing and Quota

GraphQL is the primary client-facing contract. Exact names may be adjusted to match existing naming conventions during implementation, but the behavioral contract must remain.

## Queries

### `pricingCatalog(productOfferingCode: PricingCatalogProductOfferingCode): PricingCatalogDetails!`

Returns the active pricing catalog. When `productOfferingCode` is `Spaces`, only Spaces pricing is returned.

**Spaces response requirements**

- `activeVersion.code` = `SPACES_V1` when filtered to Spaces
- `activeVersion.status`
- `productOfferings[].code`
- `productOfferings[].name`
- `productOfferings[].description`
- `productOfferings[].visibility`
- `productOfferings[].plans[].code`
- `productOfferings[].plans[].name`
- `productOfferings[].plans[].description`
- `productOfferings[].plans[].commercialModel`
- `productOfferings[].plans[].features[]`
- `productOfferings[].plans[].limits[]`, including monthly booking-instance quota
- `productOfferings[].plans[].prices[]`
- `productOfferings[].plans[].capacityOptions[]`
- `productOfferings[].plans[].availability`
- `productOfferings[].plans[].recommended`
- `productOfferings[].plans[].displayOrder`

### `organizationSpacesSubscription(organizationId: String!): OrganizationSpacesSubscriptionDetails`

Returns the current Spaces plan assignment for an organization.

**Response shape**

- `id`
- `organizationId`
- `productOfferingCode`
- `planCode`
- `catalogVersionCode` = `SPACES_V1` for Spaces offerings unless a later Spaces version is explicitly introduced
- `monthlyBookingInstanceQuota`
- `status`
- `effectiveFrom`
- `effectiveUntil`

### `organization(customDomain: String).availableOfferings`

Returns available subscription offerings for the owning organization's product family only.

- Private/Teams organizations return Teams offerings only.
- Marketplace/Spaces organizations return Spaces offerings only.
- The active offering is excluded from the returned list.

### `bookingSpacesQuotaStatus(organizationId: String!): BookingSpacesQuotaStatusDetails!`

Returns current billing-period usage and remaining quota from Booking-owned Booking row counts for Spaces booking-instance creation.

**Response shape**

- `organizationId`
- `planCode`
- `periodStartUtc`
- `periodEndUtc`
- `currentUsage`
- `quotaLimit`
- `remainingQuota`
- `upgradePlans[]`

## Mutations

### `updateOrganizationSpacesSubscription(input: UpdateOrganizationSpacesSubscriptionInput!): UpdateOrganizationSpacesSubscriptionPayload!`

Creates or updates the Spaces subscription where existing subscription flows support the requested plan change. Contact Us/Enterprise-style custom capacity remains admin-managed.

**Input fields**

- `organizationId`
- `planCode`
- `customMonthlyBookingInstanceQuota` for admin-managed Contact Us/Enterprise overrides only
- `catalogVersionCode`
- `clientMutationId`

**Behavior**

- Free assigns the catalog Free monthly booking-instance quota.
- Growth and Business assign their catalog monthly booking-instance quota.
- Contact Us requires admin/custom handling and can set negotiated quota.
- Mutation returns clear validation errors when a plan is unavailable or contact is required.

## Booking Mutation Error Contract

Any booking creation mutation or workflow-exposed booking materialization result that is blocked by Spaces quota must return or map to an error payload containing:

- stable quota-exceeded error code
- current usage
- quota limit
- attempted instance count
- attempted current-period instance count
- out-of-period instance count excluded from current-period quota
- remaining quota
- available upgrade/contact plans from the pricing catalog

## Choice Types

Follow the repo pattern for selectable enum-like values:

- shared model/constants and name mapping
- GraphQL `...Details` type with `type` and `name`
- query fields returning available choices for UI controls

Required choices:

- Product offerings, including Spaces
- Spaces subscription plans
- Plan availability states
- Subscription statuses
- Entitlement/quota reason codes

## Generation

Any implementation that changes GraphQL schema must run:

```bash
scripts/generate-graphql.sh
```

Relay artifacts consumed by Spaces or shared web apps must be regenerated and committed; generated files must not be hand-edited.
