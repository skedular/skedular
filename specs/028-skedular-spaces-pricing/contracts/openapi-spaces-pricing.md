# OpenAPI Contract: Spaces Pricing and Admin Support

GraphQL is primary. OpenAPI is secondary and should be added only where existing Organization or Booking REST/workaround surfaces already own the operation.

## Candidate Endpoints

### `GET /organization/v1/pricing-catalog?productOfferingCode=Spaces`

Returns the same active Spaces pricing catalog data exposed through GraphQL for clients or static rendering paths that cannot use GraphQL.

### `GET /organization/v1/organizations/{organizationId}/spaces-subscription`

Returns the current Spaces subscription assignment and effective monthly booking-instance quota for an organization.

### `PUT /organization/v1/organizations/{organizationId}/spaces-subscription`

Admin-supported subscription update for Growth, Business, or Contact Us/Enterprise-style custom quotas where REST workaround/admin surfaces are already used.

**Request fields**

- `planCode`
- `customMonthlyBookingInstanceQuota`
- `catalogVersionCode`

### `PUT /organization/v1/organization/{organizationId}/enterprise-offering`

Existing Organization workaround endpoint used for admin-controlled offering assignment across Teams and Spaces offerings, including negotiated capacity and discounts.

**Request fields relevant to Spaces pricing**

- `offeringCode`: Spaces or Teams offering code to assign.
- `fixedPrice`: Fixed price in minor currency units for negotiated/admin-set offering terms.
- `currency`: Offering currency.
- `purchasedUserCapacity`, `purchasedLocationCapacity`, `purchasedTeamCapacity`: Optional capacity overrides.
- `monthlyBookingInstanceQuota`: Optional Spaces monthly booking-instance quota override.
- `discountPercentage`: Integer 0 through 100. Defaults to 0. Applied to billing charges for this offering and copied to renewed offering periods until changed.

**Validation**

- `discountPercentage` must be 0 through 100.
- A 100% discount produces a zero charge but does not change the offering's plan, price, quota, capacity, or renewal behavior.
- Chargeable non-free offerings still require an organization payment method even when a discount is 100%, because the discount can later be reset while the offering continues to renew.

### `GET /booking/v1/organizations/{organizationId}/spaces-quota`

Returns current Spaces quota status from Booking-owned Booking row counts for the organization and current UTC billing period.

## Error Shape

Booking quota rejection responses must expose:

- `code`: stable quota-exceeded code
- `currentUsage`
- `quotaLimit`
- `attemptedInstanceCount`
- `attemptedCurrentPeriodInstanceCount`
- `excludedOutOfPeriodInstanceCount`
- `remainingQuota`
- `upgradePlans`

## Generation

If OpenAPI YAML changes, update the source contract under `api-definitions/openapi/skedular/*.yaml`, then run:

```bash
api-definitions/openapi/generate.sh
src/web/apps/webapp/scripts/generate.sh
```

Use `make generate` from the repo root when multiple generated surfaces are affected.
