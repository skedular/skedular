# OpenAPI Contract: Pricing Catalog

OpenAPI is secondary for this feature. Use it only if the public website or external consumers need a REST/static-friendly pricing catalog endpoint. If GraphQL is sufficient for all consumers, this contract can remain a planning reference and no OpenAPI route is required.

## `GET /organization/pricing-catalog`

Returns the active pricing catalog for all product offerings or a filtered product offering.

**Query parameters**

- `product`: Optional product offering code, such as `TEAMS` or `SPACES`.
- `version`: Optional catalog version code. Defaults to active version.

**Success response**

- `catalogVersion`
- `productOfferings`
- `plans`
- `features`
- `limits`
- `prices`
- `capacityOptions`
- `availability`
- `displayOrder`

## `GET /organization/{organizationId}/teams-subscription`

Returns the current Teams subscription outcome for an organization.

**Success response**

- `organizationId`
- `productOffering`
- `plan`
- `purchasedCapacity`
- `catalogVersion`
- `status`
- `effectiveFrom`
- `effectiveUntil`
- `activeUserUsage`

## `PATCH /organization/{organizationId}/teams-subscription`

Updates an organization's Teams subscription for self-service plans.

## `PUT /v1/organization/{organizationId}/enterprise-offering`

Skedular-admin Organization workaround REST API for setting negotiated Enterprise terms. This is not customer-facing.

**Request body**

- `unitPrice`: Monthly price per active user in minor currency units for Pay As You Go.
- `fixedPrice`: Fixed monthly price in minor currency units for negotiated Enterprise quota offerings.
- `purchasedCapacity`: Negotiated maximum monthly active users.

**Validation**

- `fixedPrice` must be zero or greater.
- `purchasedCapacity` must be greater than zero.
- The endpoint sets `OrganizationOffering.Code` to Enterprise, `Currency` to `usd`, and stores the negotiated active-user cap.
- Existing subscriptions, including Early Bird, are not changed.

## Generation

If these endpoints are implemented, update the relevant source YAML under `api-definitions/openapi/skedular/organization/` and run:

```bash
api-definitions/openapi/generate.sh
src/web/apps/webapp/scripts/generate.sh
```

Generated controller bases and clients must not be hand-edited.
