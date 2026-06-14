# GraphQL Contract: Pricing Catalog and Teams Subscription

GraphQL is the primary client-facing contract. Exact names may be adjusted to match existing naming conventions during implementation, but the behavioral contract must remain.

## Queries

### `pricingCatalog(product: ProductOfferingType): PricingCatalogDetails!`

Returns the active pricing catalog. When `product` is omitted, all product offerings are returned. When set to Teams or Spaces, only that product offering is returned.

**Response shape**

- `version.code`
- `version.status`
- `productOfferings[].code`
- `productOfferings[].name`
- `productOfferings[].description`
- `productOfferings[].visibility`
- `productOfferings[].plans[].code`
- `productOfferings[].plans[].name`
- `productOfferings[].plans[].description`
- `productOfferings[].plans[].commercialModel`
- `productOfferings[].plans[].features[]`
- `productOfferings[].plans[].limits[]`
- `productOfferings[].plans[].prices[]`
- `productOfferings[].plans[].capacityOptions[]`
- `productOfferings[].plans[].availability`
- `productOfferings[].plans[].recommended`
- `productOfferings[].plans[].displayOrder`

### `organizationTeamsSubscription(organizationId: String, organizationCustomDomain: String): OrganizationTeamsSubscriptionDetails!`

Returns the current Teams offering plan outcome for an organization from the existing `OrganizationOffering` row, including legacy Early Bird state when applicable.

**Response shape**

- `id`
- `productOffering.code`
- `plan.code`
- `plan.name`
- `purchasedCapacity`
- `catalogVersion.code`
- `status`
- `effectiveFrom`
- `effectiveUntil`
- `activeUserUsage.current`
- `activeUserUsage.limit`

## Mutations

### `updateOrganizationTeamsSubscription(input: UpdateOrganizationTeamsSubscriptionInput!): OrganizationTeamsSubscriptionPayload!`

Creates or updates the existing Teams organization offering for Free or Pay As You Go. Enterprise Capacity is set through the Skedular-admin Organization workaround REST API. Existing offerings, including Early Bird, are not modified by this feature unless an explicit admin action changes them.

**Input fields**

- `organizationId` or `organizationCustomDomain`
- `planCode`
- no purchased capacity for self-service Free or Pay As You Go changes
- `catalogVersionCode`
- `clientMutationId`

**Behavior**

- Free creates or preserves Free limits.
- Pay As You Go removes team/location limits and tracks monthly active usage.
- Enterprise Capacity requires Skedular-admin negotiated unit price and purchased active-user capacity through the Organization workaround REST API.
- Contact Us capacity cannot be self-service purchased.
- Mutation returns clear offering validation errors.

## Choice Types

Follow the repo pattern for selectable enum-like values:

- Organization-owned constants/name mapping
- GraphQL `...Details` type with `type` and `name`
- query fields returning available choices for UI controls

Required choices:

- Product offerings
- Subscription plan commercial models
- Plan availability states
- Offering plan statuses
- Entitlement reason codes

## Generation

Any implementation that changes GraphQL schema must run:

```bash
scripts/generate-graphql.sh
```

Relay artifacts consumed by web apps must be regenerated and committed; generated files must not be hand-edited.
