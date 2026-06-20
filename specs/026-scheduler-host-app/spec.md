# Feature Specification: Skedular Host

**Feature Branch**: `026-scheduler-host-app`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "scaffold a completely new product called Scheduler Individual, a completely new web app ... for the individual that just wants to become a host and they have a place and they want to rent it."

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Host Onboarding & Organization (Priority: P1)

An individual user who owns a space or resource signs up as a "Host" and creates a Host-type organization. The organization is subject to manual admin verification (same pattern as Spaces — flag to verify ownership before the listing goes live).

**Why this priority**: This is the entry point for the entire product. Without onboarding and a verified Host organization, no locations or products can be listed.

**Independent Test**: Can be fully tested by a new user creating a Host organization, providing basic details, and having an admin verify the organization — resulting in a registered, verified Host.

**Acceptance Scenarios**:

1. **Given** a new user, **When** they sign up via the Host app, **Then** they can create an organization of type "Host" (replacing the former Individual type while preserving its wire value).
2. **Given** a Host organization is created, **When** an admin reviews it, **Then** they can verify the organization via a flag (same UX as Spaces verification).
3. **Given** an unverified Host organization, **When** the host tries to publish a listing, **Then** the listing is blocked until the organization is verified.

---

### User Story 2 - Location & Product Listing (Priority: P1)

A Host can create multiple Locations. Creating a Location automatically provisions one inactive draft Product for that Location together with the hidden Product Tag and Resource. The Host edits the draft's pricing tiers, cancellation policy, and listing details, then explicitly activates it when ready. The Host never chooses a Location while creating a Product and never sees or manages Product Tags or Resources. This reuses the existing Skedular Spaces booking engine and data model while removing marketplace setup from the Host experience.

**Why this priority**: This enables the core business value—making a resource available for rent. The abstraction (Host sees Location+Product, system handles Resource) keeps the Host UX simple while reusing existing booking infrastructure.

**Independent Test**: Can be fully tested by a verified Host creating a Location, editing the automatically provisioned draft Product with location-specific pricing, activating it, and seeing that the hidden Resource and Product Tag were linked behind the scenes.

**Acceptance Scenarios**:

1. **Given** a verified Host, **When** they create a Location, **Then** the Location is saved under their Host organization.
2. **Given** a Host Location, **When** the Location is created, **Then** the system asynchronously and idempotently provisions one hidden Entire Location Resource, one hidden Product Tag, and one inactive draft Product connected through that tag.
3. **Given** a Host with multiple Locations, **When** they manage Products, **Then** each Location's draft Product can have independent pricing tiers and policies without asking the Host to choose or map a Location.
4. **Given** an incomplete draft Product, **When** the Host views product management, **Then** the UI identifies the missing listing, pricing, or policy details and does not allow accidental publication.
5. **Given** a completed draft Product, **When** the Host explicitly activates it, **Then** it becomes eligible for public discovery subject to Host ownership verification.
6. **Given** a Host Location is removed, **When** asynchronous cleanup completes, **Then** its provisioned Product is removed from sale while historical bookings remain auditable.

---

### User Story 3 - Full-Place Booking via Event Type (Priority: P1)

When a guest books a Host's Product, the entire Location is booked — the event type is configured to book the whole place. The Host does not get to define sub-resources or partial booking slots; the booking is for the entire Location.

**Why this priority**: This is the core differentiator from coworking Spaces — Hosts list an entire place (Airbnb-style), not individual desks/rooms. The booking engine must enforce full-place booking.

**Independent Test**: Can be fully tested by a guest booking a Host's Product and verifying that the entire Location becomes unavailable for the booked period.

**Acceptance Scenarios**:

1. **Given** a Host Product, **When** a guest books it, **Then** the entire Location (all auto-created Resources) is marked as unavailable for that time period.
2. **Given** a Location with a booking, **When** another guest tries to book overlapping dates, **Then** the booking is rejected with a conflict indication.
3. **Given** a Product, **When** the event type is configured, **Then** it defaults to "full place" booking (entire Location reserved).

---

### User Story 4 - Host Pricing & Commission (Priority: P1)

Hosts are charged a flat percentage of the booking value as their commission. The Host sets their own price for the Product; the system adds the percentage on top of the guest-facing price (or deducts from host payout). Hosts can use the app for free and only pay when someone makes a booking.

**Why this priority**: This is the business model — free listing, pay-per-booking. Without this, there's no revenue from Hosts.

**Independent Test**: Can be fully tested by completing a booking and verifying the correct commission percentage is calculated and charged.

**Acceptance Scenarios**:

1. **Given** a Host Product with a listed price, **When** a guest completes a booking, **Then** the system calculates a flat percentage commission on the booking value.
2. **Given** a Host with no bookings, **When** they use the app, **Then** they are not charged any fees.
3. **Given** a booking is refunded/cancelled, **When** the commission is reversed, **Then** the commission is adjusted accordingly.

---

### User Story 5 - Map Visibility on Webapp (Priority: P2)

Host listings appear on the same map as Skedualr Spaces on the webapp (public map surface). Host Locations are visible on the first page of the map alongside coworking spaces, differentiated with a distinct icon/badge (Host-type organization badge).

**Why this priority**: Discovery is critical for the product — Hosts need their listings to be found. Reusing the existing map surface avoids building a new discovery surface.

**Independent Test**: Can be fully tested by viewing the webapp map and seeing Host Location pins with a distinct badge alongside coworking space pins.

**Acceptance Scenarios**:

1. **Given** a verified Host with a published Location, **When** a user opens the webapp map, **Then** the Host Location appears on the map with a distinct Host badge/icon.
2. **Given** the map shows both Host Locations and coworking Spaces, **When** a user filters by organization type, **Then** they can filter to show only Hosts or only Spaces.
3. **Given** a Host Location pin, **When** a user clicks it, **Then** they see the Host's listing details (photos, price, availability) consistent with Space listings.

---

### User Story 6 - Host Dashboard & Management (Priority: P2)

A Host can manage their Locations and Products, update pricing, view booking requests, and see their commission history.

**Why this priority**: Necessary for ongoing operations and maintenance of the rental offering.

**Independent Test**: Can be fully tested by a Host updating a Product's price and verifying the change reflects on the map listing.

**Acceptance Scenarios**:

1. **Given** an active listing, **When** the Host changes the price, **Then** all future availability reflects the new price.
2. **Given** a booking request, **When** the Host views their dashboard, **Then** they can see who has rented their Location and when.
3. **Given** a Host with multiple bookings, **When** they view their commission history, **Then** they see a breakdown of commissions charged per booking.

---

### Edge Cases

- What happens when a Host tries to delete a Location while bookings are active?
- How does the system handle a Host who is also a guest (booking another Host's Location)?
- What happens if an admin un-verifies a Host organization with active listings?
- What happens when two Products under the same Location have overlapping event type configurations?
- Can a Host create both daily and monthly Products under the same Location (yes — different Products, different pricing, same Location auto-Resource)?

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: System MUST provide a new web application tailored for "Hosts" (individuals renting out their place).
- **FR-002**: The Host app MUST use a URL following the pattern of existing product apps (e.g., `host-scheduler.app`).
- **FR-003**: System MUST support organization type "Host" in place of the former Individual type, preserving numeric wire compatibility, alongside Private and Marketplace types.
- **FR-004**: Host organizations MUST be subject to manual admin verification (same flag pattern as Spaces) before listings can be published.
- **FR-005**: A Host MUST be able to create multiple Locations under their Host organization.
- **FR-006**: Each Host Location MUST have exactly one system-provisioned draft Product for the MVP. Each Location's Product MUST support independent pricing tiers, cancellation policy, and listing details.
- **FR-007**: The system MUST asynchronously and idempotently provision exactly one hidden Entire Location Resource, one hidden Product Tag, and one inactive draft Product per newly created Host Location. The Product MUST use the provisioned Product Tag. The Host never sees or manages the Resource or Product Tag directly.
- **FR-007a**: Host product management MUST NOT ask the Host to choose a Location, configure marketplace setup, or map Product Tags and Resources. The organization-level Products experience MUST present each Location's provisioned draft for completion and activation.
- **FR-007b**: The provisioned Product MUST remain inactive until the Host supplies all activation-required listing, pricing, payment, and cancellation-policy details and explicitly activates it. Provisioning MUST never publish a listing.
- **FR-007c**: Removing a Host Location MUST asynchronously remove its provisioned Product from sale without deleting or obscuring historical booking records. The deleted Location and its hidden Resource remain soft-deleted historical infrastructure; their system-managed Product Tag MUST NOT make any Product publicly bookable.
- **FR-008**: The booking event type for Host Products MUST default to "full place" — booking the entire Location, not individual Resources.
- **FR-009**: The system MUST reuse the existing Skedualr Spaces booking engine and data model (Locations, Products, Resources, event types, tags).
- **FR-009a**: The system MUST NOT introduce a Location-based booking path, a Host-specific booking engine, or direct Host Location references on Product or Booking entities. For every Host Location, the system MUST provision exactly one hidden Resource and exactly one hidden, system-managed Product Tag. The Product Tag MUST be assigned to the Location and hidden Resource, and every Product belonging to that Location MUST use the same Product Tag. The existing booking engine MUST resolve that Product Tag to the hidden Resource and create all Host bookings against the hidden Resource, never directly against the Location.
- **FR-010**: Hosts MUST be charged a flat 5% commission on booking value — free to use the app, pay only when a booking is made. Commission rate is defined in the Host offering catalog (HostStandardV1) and is versioned per offering.
- **FR-010a**: Host bookings MUST support card payment through Stripe only for the MVP. The booking API and payment workflow MUST reject bank transfer and every other payment method for Host Products. Host payments MUST use Stripe Connect destination charges created on Skedular's platform account, with `transfer_data.destination` routing the Host's proceeds to the connected account and `application_fee_amount` retaining the offering-defined Skedular commission. Bank-transfer payment and recovery of Skedular commission from bank transfers are explicitly out of scope and require a separate future specification.
- **FR-010b**: Booking MUST own cancellation policy, refund eligibility, and the approved refund amount. When an eligible Host booking is cancelled, Skedular MUST initiate the refund against the platform-owned destination charge with transfer reversal and application-fee reversal enabled. Stripe MUST recover the transferred portion from the Host connected-account balance and return the approved amount to the customer; the Host MUST NOT be able to override an approved Booking cancellation refund. Stripe remains responsible for the monetary movement and connected-account balance handling.
- **FR-011**: Host listings MUST appear on the same map surface as Skedualr Spaces on the webapp, differentiated with a distinct icon/badge.
- **FR-012**: System MUST integrate with existing identity and authentication systems so users can switch between Host, Customer, or Space owner roles.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for Host organization creation and verification.
- **LOG-002**: Feature MUST emit structured logs when a Location or Product is created/updated.
- **LOG-003**: Feature MUST emit structured logs when a Resource is auto-created behind the scenes.
- **LOG-004**: Feature MUST emit structured logs for booking completion including commission calculation.
- **LOG-005**: Feature MUST emit actionable warning/error logs if admin verification is rejected or if organization is un-verified with active listings.
- **LOG-006**: Feature logs MUST include correlation context (Request IDs) and avoid sensitive PII leakage.

### Key Entities

- **Host**: An individual user who rents out resources; owns a Host-type organization with one or more Locations.
- **Host Organization**: A new organization type (alongside Private and Marketplace) that represents an individual Host. Subject to admin verification.
- **Location**: The physical or virtual place being rented out (e.g., a house, a garage, an event space). A Host can have many Locations.
- **Product**: The system-provisioned inactive bookable offering for one Host Location. The Host supplies pricing tiers, policies, and listing details, then explicitly activates it. Location association remains indirect through its hidden Product Tag and Resource.
- **Resource**: Auto-created by the system when a Product is created. Linked to a Location. Invisible to the Host — this is the internal wiring to reuse the existing booking engine.
- **Event Type**: Configured per Product. For Host Products, defaults to "full place" — booking the entire Location.
- **Host Offering**: The pricing catalog entry for Host organizations (HostStandardV1). Contains the commission percentage (5%) and is versioned. Commission rate is stored on the `OrganizationOffering` entity, not on the `Organization` entity.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: New hosts can complete onboarding, create a Location, and publish their first Product in under 5 minutes.
- **SC-002**: The Host UI maintains visual consistency with the Spaces app (shared design system) while hiding Resource-level complexity from the Host.
- **SC-003**: The application is accessible via the designated URL without impacting the performance of existing web apps.
- **SC-004**: Host Locations appear on the webapp map with equal visibility to coworking Spaces.
- **SC-005**: Commission is correctly calculated and charged on 100% of Host bookings.

## Assumptions

- **Booking Engine Reuse**: The existing Skedualr Spaces booking engine (Locations, Products, Resources, event types, tags) is reused as-is. The Host product is a presentation-layer abstraction on top of the same data model.
- **Host Listing Auto-Provisioning**: Creating a Host Location starts an idempotent Temporal workflow that creates its Product Tag through Organization, its hidden Entire Location Resource in Location, and one inactive draft Product through Marketplace. Products and Resources remain connected through Product Tags, preserving the existing booking engine. The deterministic system identifiers make retries return or repair the same records instead of creating duplicates.
- **Host Booking Architecture Invariant**: A Host Location is never itself a bookable target. Each Host Location owns one hidden Entire Location Resource and one hidden Product Tag. The tag is attached to the Location, the hidden Resource, and every Product created for that Location. Product-to-Resource resolution therefore remains `Product -> Product Tag -> hidden Resource`, and the existing booking engine books that Resource. Implementations MUST NOT add parallel Location booking logic, a Host-only booking workflow, or fields such as `HostLocationId` to Product or Booking as an alternative association path. The Location can be recovered through the hidden Resource when required.
- **Drafts and publishing**: Unverified Hosts may create and edit draft Locations and Products. Activation and public discovery are rejected until ownership is verified. Un-verification hides active listings but does not cancel existing bookings.
- **Commission settlement**: Host Checkout uses a platform-owned Stripe Connect destination charge. Stripe transfers the net Host proceeds to the connected account and retains the configured Skedular application fee on the platform. Refund adjustments use Booking's durable refund lifecycle; Xero is an accounting projection rather than the refund source of truth.
- **Cancellation and monetary refund boundary**: Skedular Booking decides cancellation and refund outcomes. For an approved refund, Skedular instructs Stripe to refund the platform charge, reverse the corresponding Host transfer, and reverse the corresponding application fee. Stripe performs the monetary movement and manages insufficient connected-account balances. The Host cannot elect to keep money that Booking policy has approved for refund.
- **Host payment scope**: Host bookings accept Stripe card payment only. Bank transfer is not an MVP fallback and MUST NOT be offered for Host Products. Support for bank transfer, including how Skedular reliably collects its commission, is deferred to a separate future specification.
- **Full-Place Booking**: The event type for Host Products is configured to book the entire Location. This is the default and the only mode for MVP.
- **Map Integration**: The existing webapp map surface supports rendering Host Locations alongside coworking Spaces with minimal changes (new icon/badge for Host type).
- **Verification Pattern**: Same admin verification flag pattern as Spaces. No new verification infrastructure required.
- **Commission Model**: Flat 5% of booking value defined in the Host offering catalog (HostStandardV1). Commission rate is hardcoded in the pricing catalog code per offering version, not configured via environment variable. When a new offering version is introduced (e.g., HostStandardV2), existing customers remain on their current offering rate until their next renewal, at which point they receive the new rate. This aligns with the Spaces and Teams offering architecture where pricing and terms are versioned per offering.
- **Offering Catalog Architecture**: Host commission percentage is stored on the `OrganizationOffering` entity (not `Organization`). The offering catalog follows the same pattern as Spaces and Teams:
  - `PricingCatalogProductOfferingCode.Host` enum value added
  - `OfferingCode.HostStandardV1` enum value added with 5% commission
  - `HostPricingCatalogProvider` defines the Host offering in the catalog
  - `PricingCatalogVersionService.GetCurrentHostVersion()` returns the current Host catalog version
  - Commission rate is applied via `OrganizationOfferingPricingExtensions` when creating Host offerings
- **Infrastructure**: New infrastructure (Terraform) will be provided later; for now, the app is scaffolded as a new project in the mono-repo.
- **Auth Reuse**: The existing auth system supports multiple roles per user identity. Host is a new role assignable to the same user identity as Customer or Space owner.
