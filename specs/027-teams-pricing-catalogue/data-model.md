# Data Model: Skedular Teams Pricing Catalog Redesign

## PricingCatalog

Organization-owned read model representing the business-owned pricing catalog returned to web and product experiences.

**Fields**

- `id`: Stable catalog identifier.
- `activeVersion`: Current `PricingCatalogVersion`.
- `productOfferings`: Ordered list of product offerings.
- `generatedAt`: Timestamp used for observability/cache validation.

**Validation**

- Must include Skedular Teams.
- May include Skedular Spaces as a framework-level offering even before full Spaces commercial behavior is implemented.
- Display order is catalog-owned.

## PricingCatalogVersion

Organization-owned read model representing a stable version of catalog rules and commercial data.

**Fields**

- `code`: Stable version code, initially extending V1.
- `status`: Draft, active, deprecated, or retired.
- `effectiveFrom`: Start timestamp for version selection.
- `effectiveUntil`: Optional end timestamp.
- `compatibilityNotes`: Explanation of preserved behavior or compatibility constraints.

**Validation**

- Existing subscriptions retain the version and state they already have; this feature does not mutate them.
- V1 extension is preferred unless implementation review proves a new version is required.

## ProductOffering

Organization-owned read model representing a sellable Skedular product area.

**Fields**

- `code`: Teams or Spaces.
- `name`: Display name.
- `description`: Catalog-provided description.
- `plans`: Ordered list of subscription plans.
- `visibility`: Public, hidden, existing-customers-only, or unavailable.

**Validation**

- Teams plans must not encode Spaces-specific assumptions.
- Spaces can be represented without requiring full Spaces subscription behavior in this feature.

## SubscriptionPlan

Organization-owned read model representing a commercial plan within a product offering.

**Fields**

- `code`: Stable plan code.
- `name`: Catalog display name.
- `description`: Catalog description.
- `commercialModel`: Free, usage-based, or capacity-based.
- `features`: Ordered feature list.
- `limits`: Plan limits.
- `prices`: Plan prices.
- `capacityOptions`: Contact Us capacity placeholder for Enterprise Capacity.
- `availability`: Visibility and self-service state.
- `recommended`: Whether the catalog recommends the plan.
- `displayOrder`: Catalog-owned ordering value.

**Validation**

- Teams plans are Free, Pay As You Go, and Enterprise Capacity.
- Historical Enterprise package labels are not separate plan types or public self-service packages.
- Paid Teams plans must not include team or location limits.

## CapacityOption

Organization-owned read model representing contact-only Enterprise capacity behavior in the public catalog. Negotiated Enterprise capacity is stored on the organization offering.

**Fields**

- `userCapacity`: Optional finite user count. Public Enterprise uses null because capacity is negotiated.
- `label`: Catalog-provided label.
- `price`: Optional public price. Public Enterprise uses null because price is negotiated.
- `availability`: Self-service, contact-us, hidden, deprecated, or unavailable.
- `displayOrder`: Catalog-owned ordering.

**Validation**

- Enterprise uses Contact Us public availability.
- Negotiated Enterprise unit price and active-user capacity are stored per organization offering by Skedular admins.
- Capacity options belong to Enterprise Capacity, not separate plan codes.

## OrganizationOffering

Existing durable organization offering/subscription state for an organization. Available offerings are still defined in code under `Api.Shared.Services.Offering`; this record stores the offering currently held by one organization.

**Fields**

- `id`: Organization offering identifier.
- `organizationId`: Owning organization.
- `code`: Existing `OfferingCode` that maps to Free, Pay As You Go, Enterprise Capacity, or legacy Early Bird.
- `unitPrice`: Optional monthly price per active user in minor currency units. Pay As You Go uses this for monthly active-user billing; fixed-quota offerings leave it unset.
- `fixedPrice`: Fixed monthly price in minor currency units for negotiated Enterprise quota offerings. Pay As You Go leaves this unset.
- `currency`: Scheduler pricing currency, currently `usd`.
- `purchasedCapacity`: Active-user capacity stored on the offering row. Free snapshots 10, Pay As You Go snapshots unlimited, and Enterprise stores the negotiated cap.
- `purchasedLocationCapacity`: Location capacity stored on the offering row. Free snapshots 1, paid offerings snapshot unlimited unless later negotiated otherwise.
- `purchasedTeamCapacity`: Team capacity stored on the offering row. Free snapshots 1, paid offerings snapshot unlimited unless later negotiated otherwise.
- `catalogVersionCode`: Optional version used for new catalog decisions.
- `start`: Start timestamp.
- `end`: End timestamp for the monthly billing/offering period.
- `autoRenew`: Whether renewal continues automatically.
- `createdAt` / `updatedAt`: Audit timestamps.

**Validation**

- Every organization has one effective Teams offering/subscription outcome.
- Existing Early Bird subscriptions remain unchanged and honored.
- Existing Free subscriptions retain Free restrictions.
- Enterprise Capacity requires a Skedular-admin REST update with negotiated unit price and purchased active-user capacity.

## OrganizationOfferingActiveMember

Existing organization-owned association that tracks active members for the monthly organization offering period. This is reused for monthly active-user pricing and entitlement counts.

**Fields**

- `organizationOffering`: Monthly offering period that scopes the active-member count.
- `organizationMember`: Qualified organization member/customer for that monthly offering period.

**Validation**

- Active-user counts for billing and entitlement are derived from the existing `OrganizationOfferingActiveMembers` collection.
- Billing remains monthly, aligned with the existing offering period.
- Pending, rejected, or irrelevant actions must not qualify unless added by reviewed rules.

## Entitlement Reason Codes

Organization-owned enum values used by the current GraphQL choice surface. A full entitlement decision read model is deferred until the enforcement slice implements the evaluator against `OrganizationOffering`.

**Validation**

- Reason codes must describe offering-capacity outcomes, not a separate subscription database object.
- Full enforcement must derive limits from `OrganizationOffering.purchasedCapacity`, `OrganizationOffering.purchasedLocationCapacity`, `OrganizationOffering.purchasedTeamCapacity`, and active usage.

## State Transitions

### PricingCatalogVersion

Draft -> Active -> Deprecated -> Retired

### OrganizationOffering

Active monthly offering period -> renewed monthly offering period -> canceled/deleted offering  
Legacy Early Bird remains unchanged unless a future explicit change is approved outside this feature.

## Relationships

- `PricingCatalog` has many `PricingCatalogVersion` entries over time.
- `PricingCatalogVersion` has many `ProductOffering` entries.
- `ProductOffering` has many `SubscriptionPlan` entries.
- `SubscriptionPlan` may have many `CapacityOption` entries.
- `OrganizationOffering` maps to one `ProductOffering`, one `SubscriptionPlan`, and one `PricingCatalogVersion`.
- `OrganizationOfferingActiveMember` entries are counted against capacity fields stored on `OrganizationOffering`.
- Future entitlement decisions must be derived from `OrganizationOffering`, current usage, and attempted action.
