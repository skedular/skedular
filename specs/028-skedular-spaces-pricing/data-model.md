# Data Model: Skedular Spaces Pricing Implementation

## PricingCatalog

Organization-owned read model representing the active product-aware pricing catalog returned to web and product experiences.

**Fields**

- `id`: Stable catalog identifier.
- `activeVersion`: Current `PricingCatalogVersion` for the requested product offering. Spaces uses `SPACES_V1`; Teams uses `TEAMS_V1`.
- `productOfferings`: Ordered list of product offerings, including Spaces.
- `generatedAt`: Timestamp for observability/cache validation.

**Validation**

- Must include a Spaces product offering when callers request Spaces pricing.
- Display order and visibility are catalog-owned.
- Frontend must not hardcode Spaces plan values that are present in this model.

## ProductOffering

Organization-owned read model representing a sellable Skedular product area.

**Fields**

- `code`: Spaces.
- `name`: Display name.
- `description`: Catalog-provided description.
- `plans`: Ordered list of Spaces subscription plans.
- `visibility`: Public, hidden, existing-customers-only, or unavailable.

**Validation**

- Spaces offering must not reuse marketplace product-pricing semantics.
- Spaces offering must coexist with Teams under the same catalog infrastructure while preserving its own independent `SPACES_V1` version.

## SpacesSubscriptionPlan

Catalog plan within the Spaces product offering.

**Fields**

- `code`: Free, Growth, Business, or Contact Us.
- `name`: Catalog display name.
- `description`: Catalog description.
- `monthlyBookingInstanceQuota`: Monthly booking-instance quota; Contact Us may use custom/admin capacity.
- `prices`: Prices and billing cadence for display.
- `features`: Ordered feature list.
- `availability`: Self-service, contact-us, hidden, deprecated, unavailable, or existing-customers-only.
- `recommended`: Whether catalog recommends the plan.
- `displayOrder`: Catalog-owned ordering.

**Validation**

- Free quota is 100 monthly booking instances.
- Growth quota is 500 monthly booking instances.
- Business and Contact Us quotas are catalog/admin-defined.
- No plan may define quotas for locations, resources, desks, rooms, equipment, products, customers, subscriptions, or memberships.

## Organization Spaces Subscription State

Organization-owned current Spaces plan assignment and capacity configuration for an organization, represented on the existing offering model and exposed through Spaces subscription read/update models.

**Fields**

- `id`: Offering/subscription projection identifier.
- `organizationId`: Owning organization.
- `productOfferingCode`: Spaces.
- `planCode`: Free, Growth, Business, or Contact Us.
- `catalogVersionCode`: Product-specific catalog version used for the assignment. Spaces offerings use `SPACES_V1`; Teams offerings use `TEAMS_V1`.
- `monthlyBookingInstanceQuota`: Effective monthly quota, including Enterprise/admin overrides.
- `discountPercentage`: Persisted percentage discount applied when charging this offering. Defaults to 0, must be 0 through 100, and is copied to the renewed offering until changed.
- `status`: Active, scheduled-cancel, canceled, or transition-required if downstream projection requires follow-up.
- `effectiveFrom`: Start timestamp.
- `effectiveUntil`: Optional end timestamp.
- `autoRenew`: Whether the plan renews automatically where applicable.
- `createdAt` / `updatedAt`: Audit timestamps.

**Validation**

- Every organization must have a valid Spaces pricing state within 24 hours of migration completion.
- Organizations with no active Spaces subscription default to Free during migration.
- Admin overrides can set negotiated capacity for Contact Us/Enterprise-style customers.
- Admin/workaround updates can set or reset `discountPercentage` for any offering. A discount changes only the charge calculation; it does not mutate catalog price, negotiated fixed price, unit price, quota, capacity, plan code, or catalog version.
- `discountPercentage` is never null. Omitted admin input and existing rows default to 0.

## Organization Offering Discount

Organization-owned discount value stored directly on the existing offering row.

**Fields**

- `discountPercentage`: Integer percentage from 0 through 100. `0` means no discount. `100` means the calculated charge is zero.

**Billing Behavior**

- The billing amount is calculated from the offering's fixed price, or from unit price multiplied by active billable members, then reduced by `discountPercentage`.
- Discount calculation is applied at charge time for the offering period being billed.
- Discounts do not affect quota calculations, pricing catalog display values, upgrade recommendations, or booking entitlement decisions.
- Discount values are copied to the next renewed offering period and remain active until an admin changes or resets them to 0.

## Spaces Booking Usage

Plan, quota, custom capacity, and billing-period boundaries are stored as properties on the replicated `Organization.Offering` JSONB column in the Booking domain. Current usage is not stored on the offering. It is counted from persisted Booking rows whose scheduled start falls inside the current billing period.

**JSONB Properties**

- `SpacesPlanCode`: Plan code value (1=Free, 5=Growth, 6=Business, 7=ContactUs).
- `SpacesQuotaLimit`: Effective monthly booking instance quota for this plan.
- `SpacesCustomCapacity`: Admin-negotiated custom capacity override for Contact Us.
- `SpacesPeriodStartUtc`: UTC start of the current billing period.
- `SpacesPeriodEndUtc`: UTC end of the current billing period.

**Validation**

- Current usage is queried from Booking-owned persisted booking records and only includes instances scheduled within the period.
- Instances scheduled outside the period are excluded from the period's quota calculation.
- Canceled booking instances remain counted for the period.
- Failed booking creation does not affect usage because no booking row exists to count.
- The quota check is intentionally simple and count-based; minor overage can occur under concurrent creation and is accepted for this feature.

## SpacesQuotaDecision

Shared decision model used by booking workflows and APIs when deciding whether booking instances can be created.

**Fields**

- `allowed`: Whether creation is allowed.
- `reasonCode`: Allowed, subscription-not-found, quota-exceeded, offering-not-effective, or contact-us-required.
- `organizationId`: Organization being checked.
- `planCode`: Effective Spaces plan.
- `periodStartUtc`: Current billing period start.
- `periodEndUtc`: Current billing period end.
- `currentUsage`: Count before the attempted creation.
- `quotaLimit`: Effective limit.
- `attemptedInstanceCount`: Number of new instances requested that are scheduled within the current billing period.
- `excludedOutOfPeriodInstanceCount`: Number of requested instances excluded from current-period quota because they are scheduled outside the current billing period.
- `remainingQuota`: Remaining quota before the attempted creation.
- `upgradePlans`: Catalog-provided upgrade/contact options when denied.
- `userMessage`: User-facing message for API/frontend display.
- `operatorMessage`: Log-oriented message.

**Validation**

- Denied quota responses must include current usage, quota limit, and available upgrade plans.
- Decisions must be based on close-to-real-time persisted Booking rows for the current billing period, not stale cached counts.
- Decisions must not include sensitive booking payloads.

## Booking Instance

Existing Booking-owned record created by the Skedular Spaces booking engine.

**Fields Used By This Feature**

- `id`: Booking identifier.
- `organizationId`: Owning organization.
- `createdAt`: Creation timestamp used for audit/reconciliation.
- recurring/subscription linkage fields where applicable.

**Validation**

- Each distinct stored booking record counts as one monthly booking instance for the billing period that contains its scheduled start.
- Booking instances scheduled outside the current billing period do not count against the current period's quota.
- Updates/rebooking of an existing booking record do not create quota usage.
- Recurring and multi-slot expansion count each stored child instance.

## State Transitions

### Organization Spaces Subscription State

Missing/no active subscription -> Free default -> Growth/Business/Contact Us active -> optional discount applied during billing -> renewed offering copies discount -> discount reset or changed by explicit admin/workaround flow -> canceled or changed by explicit admin/user-supported flow

### Spaces Booking Usage

Organization offering JSONB has current billing period fields -> quota service counts Booking rows scheduled inside those boundaries -> quota checks include only attempted booking instances scheduled inside those boundaries

### SpacesQuotaDecision

Allowed -> booking instances created -> future checks count the new persisted rows  
Denied -> booking creation blocked -> upgrade/contact prompt returned  
Allowed -> booking creation fails -> no booking row exists, so usage is unchanged

## Relationships

- `PricingCatalog` has many `ProductOffering` entries.
- Spaces `ProductOffering` has many `SpacesSubscriptionPlan` entries.
- Organization Spaces subscription state maps the existing offering to the Spaces product offering, one Spaces plan, and one catalog version.
- Organization offering discount is stored with the existing offering state and is copied during renewal independently from Spaces quota state.
- Spaces quota state (plan code, quota, custom capacity, period boundaries) is stored in `Organization.Offering` JSONB; current usage is counted from Booking rows, not stored in JSONB and not stored in a separate usage table.
- Booking creation paths use the replicated `Organization.Offering` JSONB and current-period Booking row counts via `SpacesBookingUsageRepository` to produce `SpacesQuotaDecision`.
- Recurring booking workflows enforce the same `SpacesQuotaDecision` per generated instance.
