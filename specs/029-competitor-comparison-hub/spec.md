# Feature Specification: Skedular Competitor Comparison Hub

**Feature Branch**: `029-competitor-comparison-hub`  
**Created**: 2026-06-20  
**Status**: Draft  
**Input**: User description: "Project: Skedular Competitor Comparison Hub. Build a complete competitor comparison system for the Skedular public web app. Before generating any comparison content, analyze the existing Skedular specifications, website content, pricing model, integrations, booking capabilities, coworking functionality, marketplace functionality, billing capabilities, and white-label features. The implementation must not hardcode comparison content. Instead, create a structured competitor dataset and a normalized feature matrix that can generate all comparison pages automatically. The final result must include a Comparison Hub Page, Individual Comparison Pages, Alternative Pages, Shared Comparison Components, Shared Feature Matrix, SEO Metadata, FAQ Schema, Structured Data, and all pages generated from a single source of truth."

## Clarifications

### Session 2026-06-20

- Q: Where should supporting SEO pages live? → A: Under `/compare`
- Q: What evidence level is required for competitor claims? → A: Seed data plus evidence/review status
- Q: How should the existing Skedda-only comparison URL be handled? → A: Remove with no redirect or alias
- Q: When should comparison pages be published? → A: Publish only when all required pages are complete

## User Scenarios & Testing _(mandatory)_

### User Story 0 - Replace Existing One-Off Skedda Page (Priority: P0)

A product owner needs the current one-off Skedda comparison page in the public web app removed before the new comparison system is introduced, so the site does not keep a hardcoded legacy comparison page beside the data-driven comparison hub.

**Why this priority**: The existing Skedda-only page is the exact pattern this feature replaces. Removing it first prevents duplicate routes, conflicting content, and stale comparison claims.

**Independent Test**: Can be tested by confirming the old one-off Skedda comparison implementation is removed without redirect or alias behavior, `/compare` exists, and the Skedda comparison is reachable only through the new generated comparison system.

**Acceptance Scenarios**:

1. **Given** the public web app contains an existing Skedda-only comparison page, **When** the feature is implemented, **Then** that one-off page implementation is removed rather than updated in place.
2. **Given** the new `/compare` page exists, **When** a visitor opens it, **Then** the page lists all published comparison pages, including Skedular vs Skedda.
3. **Given** a visitor clicks any listed comparison from `/compare`, **When** navigation completes, **Then** the visitor lands on the corresponding generated comparison page.
4. **Given** a visitor opens the previous Skedda-only comparison URL, **When** the old route has been removed, **Then** the system does not redirect, alias, or preserve that URL.

---

### User Story 1 - Establish Evidence-Based Comparison Data (Priority: P1)

A product or marketing owner needs Skedular's competitor comparison pages to be generated from a structured source of truth, with Skedular capabilities based only on current specifications, help content, product pages, pricing data, routes, and implemented behavior, so published comparisons are accurate and maintainable.

**Why this priority**: The comparison pages cannot be trusted if Skedular features are inferred from outdated drafts or hand-written per-page claims.

**Independent Test**: Can be tested by reviewing the comparison data inventory and confirming every Skedular capability used in generated pages has a current evidence reference, and that no page contains duplicated hardcoded comparison claims outside the shared data source.

**Acceptance Scenarios**:

1. **Given** the comparison data source is reviewed, **When** a Skedular capability appears in the matrix, **Then** it includes a current evidence reference from existing specifications, help content, product/pricing content, routes, or implemented product surfaces.
2. **Given** a capability exists only in outdated or speculative content, **When** comparison data is prepared, **Then** it is excluded or marked unpublished until supported by current evidence.
3. **Given** a competitor comparison page is rendered, **When** its overview, feature matrix, pricing, integrations, best-for copy, limitations, FAQs, and CTA are inspected, **Then** the content comes from the shared comparison dataset and normalized feature matrix.

---

### User Story 2 - Compare Skedular Against a Specific Competitor (Priority: P1)

A visitor evaluating Skedular against a named competitor can open a dedicated comparison page and understand where each product fits, which feature areas differ, how pricing and integrations compare, who each product is best for, and why teams or operators choose Skedular.

**Why this priority**: Individual comparison pages are the main conversion path for high-intent searches such as "Skedular vs Skedda" or "OfficeRnD alternatives."

**Independent Test**: Can be tested by opening each required comparison URL and confirming it includes the required sections, accurate competitor identity, shared feature matrix rows, pricing and integration comparisons, FAQs, structured data, internal links, and CTA.

**Acceptance Scenarios**:

1. **Given** a visitor opens `/compare/skedular-vs-skedda`, **When** the page loads, **Then** it presents the Skedda comparison using the same layout and data model as every other individual comparison page.
2. **Given** any required competitor page is opened, **When** the visitor scans the feature matrix, **Then** feature rows are grouped consistently across Workspace Management, Coworking Management, Marketplace, Payments, Integrations, Administration, Analytics, and Developer categories.
3. **Given** a competitor does not support a Skedular-supported capability, **When** the page explains the gap, **Then** the limitation is phrased as a factual comparison rather than unsupported disparagement.

---

### User Story 3 - Browse the Comparison Hub (Priority: P2)

A visitor who is still choosing what to compare can open the comparison hub and discover all competitor, alternative, and best-software pages grouped by category, use case, and buyer intent.

**Why this priority**: The hub creates internal linking, helps undecided visitors navigate, and gives search engines a clear comparison index.

**Independent Test**: Can be tested by opening `/compare` and confirming every generated comparison and supporting page is linked with meaningful category context, short summaries, and clear navigation to Skedular calls to action.

**Acceptance Scenarios**:

1. **Given** a visitor opens `/compare`, **When** the page loads, **Then** it links to every individual comparison page, every alternatives page, and every best-software supporting page.
2. **Given** competitors belong to different categories, **When** the hub groups them, **Then** categories include workplace management, coworking management, hybrid workplace, workplace operations, and marketplace-oriented platforms where applicable.
3. **Given** a future competitor is added to the shared dataset, **When** the hub is regenerated, **Then** the competitor appears without creating bespoke page content.

---

### User Story 4 - Discover Alternative and Best-Software Pages (Priority: P2)

A visitor searching broader terms such as "best coworking software" or "Skedda alternatives" can land on a supporting SEO page that summarizes relevant options and links into detailed Skedular comparison pages.

**Why this priority**: Broader intent pages capture evaluation traffic before a visitor has committed to a single competitor comparison.

**Independent Test**: Can be tested by opening each supporting page and confirming it is generated from the same competitor and feature data, includes internal links to detailed comparisons, and contains SEO metadata, FAQ schema, structured data, and CTA.

**Acceptance Scenarios**:

1. **Given** a visitor opens "Best Coworking Software," **When** the page loads, **Then** it ranks or groups coworking-relevant products using the normalized feature matrix and clearly explains where Skedular fits.
2. **Given** a visitor opens "Best Skedda Alternatives," **When** the page loads, **Then** it highlights alternatives relevant to Skedda buyers and links to the Skedular vs Skedda comparison.
3. **Given** a competitor's data changes, **When** alternative and best-software pages are regenerated, **Then** updated facts appear consistently across all affected pages.

---

### User Story 5 - Maintain and Extend Competitor Data (Priority: P3)

A content maintainer can add, remove, or update a competitor, feature, category, FAQ, or page-targeting entry in one place and have every generated comparison page stay consistent.

**Why this priority**: The system must remain useful as competitor positioning, Skedular capabilities, pricing, and SEO targets change.

**Independent Test**: Can be tested by adding a sample future competitor to the dataset and verifying the hub, competitor page, relevant alternatives pages, feature matrix, metadata, structured data, and internal links update without page-specific duplicated copy.

**Acceptance Scenarios**:

1. **Given** a new competitor is added with category, capabilities, strengths, limitations, pricing notes, integrations, FAQs, and target pages, **When** pages are regenerated, **Then** the new competitor appears in all relevant generated surfaces.
2. **Given** a normalized feature label changes, **When** the matrix is regenerated, **Then** every page uses the new label consistently.
3. **Given** a maintainer removes a competitor from published status, **When** pages are regenerated, **Then** the hub and supporting pages stop linking to unpublished comparison content.

### Edge Cases

- A Skedular capability appears in an older draft but is absent from current specs, help content, product routes, pricing data, or implemented surfaces.
- A visitor or search crawler requests the previous one-off comparison URL after removal.
- A competitor capability is listed in the initial dataset but lacks enough detail to compare pricing or integration depth.
- A feature is supported by both products but with different scope, audience, or maturity.
- A feature category has no supported competitor features for a given comparison.
- A competitor changes branding or product naming after the initial dataset is created.
- A page target conflicts with another SEO page, duplicate canonical path, or repeated metadata title.
- A future competitor has multiple product lines that do not map cleanly to one comparison page.
- FAQ answers risk making unsupported claims about a competitor or Skedular.
- Structured data would include content that is not visible on the rendered page.
- No eligible competitors exist for a supporting alternatives page after unpublished entries are filtered.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST remove the existing one-off Skedda comparison page from the public web app before publishing the new comparison hub and generated comparison pages.
- **FR-001a**: The removed Skedda comparison MUST be replaced by the generated `/compare/skedular-vs-skedda` page from the shared comparison dataset.
- **FR-001b**: The `/compare` hub MUST be the primary index for all comparison pages and MUST list every published individual comparison page.
- **FR-001c**: Clicking any comparison entry on `/compare` MUST take visitors to that competitor's generated comparison page.
- **FR-001d**: The previous one-off Skedda comparison URL MUST NOT redirect, alias, or render preserved legacy comparison content after removal.
- **FR-001e**: Generated comparison pages MUST be built cleanly from the new shared data model in the order listed in this specification.
- **FR-001f**: The comparison hub and generated comparison pages MUST NOT be published until `/compare`, all listed individual comparison pages, and all listed supporting pages are complete.
- **FR-002**: The system MUST start with a current Skedular capability audit before publishing comparison copy.
- **FR-003**: The Skedular capability audit MUST review current specifications, help content, product/pricing content, public app routes, split product app routes, integrations, booking behavior, coworking/operator functionality, marketplace functionality, billing behavior, and branding/domain capabilities.
- **FR-004**: The Skedular capability audit MUST record evidence for each included capability and MUST exclude or mark unpublished any capability that is not supported by current evidence.
- **FR-005**: The comparison system MUST use one structured source of truth for competitors, Skedular capabilities, normalized features, page targets, SEO metadata, FAQs, structured data inputs, and internal links.
- **FR-005a**: Individual comparison pages, alternative pages, and the comparison hub MUST NOT contain page-specific hardcoded comparison claims that duplicate or override the shared dataset.
- **FR-006**: The initial competitor dataset MUST include Skedda, OfficeRnD, Nexudus, Gable, Robin, Officely, Envoy, Kadence, Archie, and deskbird.
- **FR-007**: Each competitor record MUST include name, slug, category, core capabilities, strengths, limitations, integration notes, pricing comparison notes, best-fit buyer profile, Skedular positioning, FAQ entries, and publication status.
- **FR-007a**: Each publishable competitor claim MUST include an evidence note or explicit review status; seed data alone is not enough for publication.
- **FR-008**: The normalized feature matrix MUST include these categories: Workspace Management, Coworking Management, Marketplace, Payments, Integrations, Administration, Analytics, and Developer.
- **FR-009**: Workspace Management features MUST include Desk Booking, Room Booking, Parking Booking, Event Booking, Custom Resources, Floor Plans, Interactive Maps, Booking Rules, and Resource Permissions.
- **FR-010**: Coworking Management features MUST include Member Management, Membership Plans, Recurring Memberships, Billing, Invoicing, Tax Handling, Subscription Management, and Community Features.
- **FR-011**: Marketplace features MUST include Public Listings, Workspace Discovery, Marketplace Inventory, Host Onboarding, and Public Booking Pages.
- **FR-012**: Payments features MUST include Stripe, Stripe Connect, Xero, Manual Invoicing, Weekly Billing, Fortnightly Billing, and Monthly Billing.
- **FR-013**: Integrations features MUST include Slack, Teams, SSO, WorkOS, Calendar Integrations, and Access Control.
- **FR-014**: Administration features MUST include Multi Location, Multi Team, Custom Branding, Custom Domains, and White Label.
- **FR-015**: Analytics features MUST include Occupancy Reporting, Utilization Reporting, Revenue Reporting, and Booking Analytics.
- **FR-016**: Developer features MUST include API and Webhooks.
- **FR-017**: Feature matrix entries MUST distinguish at least supported, partially supported, not supported, unknown, and not applicable states.
- **FR-018**: Feature matrix entries MUST support notes explaining meaningful scope differences without requiring custom page sections.
- **FR-019**: The system MUST generate a comparison hub at `/compare`.
- **FR-020**: The system MUST generate these individual comparison pages: `/compare/skedular-vs-skedda`, `/compare/skedular-vs-officernd`, `/compare/skedular-vs-nexudus`, `/compare/skedular-vs-gable`, `/compare/skedular-vs-robin`, `/compare/skedular-vs-officely`, `/compare/skedular-vs-envoy`, `/compare/skedular-vs-kadence`, `/compare/skedular-vs-archie`, and `/compare/skedular-vs-deskbird`.
- **FR-021**: Every individual comparison page MUST include Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA sections.
- **FR-022**: The system MUST generate supporting pages at `/compare/best-coworking-software`, `/compare/best-workspace-management-software`, `/compare/best-desk-booking-software`, `/compare/skedda-alternatives`, `/compare/officernd-alternatives`, and `/compare/nexudus-alternatives`.
- **FR-023**: Supporting pages MUST be generated from the same competitor dataset and normalized feature matrix as individual comparison pages.
- **FR-024**: Every generated page MUST include metadata, Open Graph data, FAQ schema where FAQs are present, structured data appropriate to the visible page content, canonical path, and internal links.
- **FR-025**: Structured data MUST match visible page content and MUST NOT include hidden or unsupported claims.
- **FR-026**: The comparison hub MUST link to all published individual comparison pages and supporting pages.
- **FR-027**: Individual comparison pages MUST link back to the hub, to relevant alternatives pages, and to related competitor pages when the relationship is defined in the shared data.
- **FR-028**: Generated copy MUST use American spelling and grammar.
- **FR-029**: Competitor limitations MUST be written as factual, supportable comparisons and MUST avoid implying unavailable evidence.
- **FR-029a**: Competitor capabilities, strengths, limitations, pricing notes, and integration notes MUST remain unpublished or marked unknown until they have an evidence note or explicit review status.
- **FR-030**: The comparison system MUST support future competitors by updating shared data only, without creating new one-off page templates for each competitor.
- **FR-031**: The comparison system MUST expose a reviewable content inventory that shows every generated page, source data entry, normalized features used, metadata, FAQ entries, and structured data type.
- **FR-032**: The initial Skedular capability baseline MUST include only currently evidenced capabilities, including public workspace discovery; desk, room, parking, event, and resource booking; recurring bookings; private workplace management; coworking/operator product management; marketplace publishing; memberships or recurring subscriptions; billing; invoicing; tax handling; Stripe or Stripe Connect payment setup; Xero integration; Slack; Microsoft Teams; WorkOS/SSO; floor plans; maps; custom domains; operator branding; availability; and analytics where each is backed by current evidence.
- **FR-033**: If evidence for API or webhook capability is incomplete during implementation, the related Developer matrix entry MUST remain unknown or unpublished rather than being marked as supported.
- **FR-034**: Pricing comparison sections MUST distinguish current Skedular pricing model facts from competitor pricing notes and MUST avoid unsupported competitor price claims.
- **FR-035**: The comparison pages MUST provide CTAs suitable for evaluation traffic, including demo or contact paths and relevant pricing or product links.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for generation or retrieval of comparison page data, including page slug and publication status.
- **LOG-002**: Feature MUST emit structured logs when a requested comparison or supporting page is unpublished, missing, or cannot be generated from the shared data.
- **LOG-003**: Feature MUST emit actionable warning/error logs for invalid competitor data, invalid feature matrix references, duplicate page slugs, missing metadata, or structured data validation failures.
- **LOG-004**: Feature logs MUST include correlation context and MUST avoid sensitive data leakage.
- **LOG-005**: Content generation and page rendering logs MUST make it possible for operators to identify which shared data entry produced a given comparison page.

### Key Entities _(include if feature involves data)_

- **Skedular Capability Evidence**: A reviewed capability claim about Skedular, including capability name, category, status, source reference, source freshness, and publication eligibility.
- **Competitor**: A product being compared against Skedular, including category, capabilities, strengths, limitations, integration notes, pricing notes, best-fit audience, FAQ entries, and publication status.
- **Normalized Feature**: A stable feature row used across all comparisons, grouped by category and mapped to Skedular and competitor support states.
- **Feature Support State**: The comparison value for a product-feature pair, such as supported, partially supported, not supported, unknown, or not applicable, with optional explanatory notes.
- **Comparison Page Target**: A generated page definition containing path, primary competitor, page type, title, metadata, sections, related links, FAQ selection, and CTA configuration.
- **Alternative Page Target**: A generated page definition for broader "best software" or "alternatives" searches, including included competitors, category focus, ranking or grouping rules, metadata, FAQs, and CTAs.
- **FAQ Entry**: A question and answer generated from shared data and eligible for FAQ schema when visible on the page.
- **Structured Data Entry**: A page-level structured data definition derived from visible content and associated with the correct generated page.
- **Internal Link Rule**: A relationship that determines links between the hub, comparison pages, supporting pages, product pages, pricing pages, and CTAs.
- **Content Inventory**: A reviewable list of generated pages, source records, matrix rows, metadata, FAQs, and structured data used before publication.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of published Skedular capability claims on comparison pages have current evidence references in the content inventory.
- **SC-002**: The existing one-off Skedda comparison page is removed with no redirect or alias, and Skedular vs Skedda is available only through the generated comparison system.
- **SC-003**: 100% of comparison entries listed on `/compare` navigate to the correct generated comparison page.
- **SC-004**: 100% of required comparison and supporting page URLs render from the shared dataset and normalized feature matrix.
- **SC-004a**: Publication readiness is achieved only when `/compare`, all listed individual comparison pages, and all listed supporting pages pass validation together.
- **SC-005**: Adding a sample future competitor requires only shared data changes and produces a hub entry, comparison page, metadata, FAQ eligibility, structured data inputs, and related links in validation.
- **SC-006**: No generated page contains duplicated hardcoded comparison claims outside the shared comparison data source in review.
- **SC-007**: 100% of individual comparison pages include the required sections: Overview, Feature Matrix, Pricing Comparison, Integration Comparison, Best For, Limitations, Why Teams Choose Skedular, FAQ, and CTA.
- **SC-008**: 100% of generated pages include a unique title, description, canonical path, Open Graph metadata, and internal links.
- **SC-009**: 100% of pages with visible FAQs include matching FAQ schema, and no FAQ schema appears for hidden FAQ content.
- **SC-010**: The comparison hub links to every published competitor page and supporting page in automated validation.
- **SC-010a**: 100% of supporting SEO pages use `/compare` canonical paths in automated validation.
- **SC-011**: At least 90% of test readers can identify within 90 seconds whether Skedular is positioned for private workplace management, coworking/operator management, marketplace discovery, or a combination of those use cases on each relevant page.
- **SC-012**: Content review finds zero unsupported Skedular capability claims and zero unsupported competitor limitation claims before publication.
- **SC-012a**: 100% of published competitor claims have an evidence note or explicit review status in the content inventory.
- **SC-013**: Generated comparison pages are usable on mobile and desktop review without missing primary sections, broken internal links, or duplicate canonical paths.
- **SC-014**: Operators can identify data validation failures, missing page data, and unpublished page requests from structured logs for 100% of tested failure paths.

## Assumptions

- The initial competitor facts supplied in the feature request are accepted as seed data, but publication still requires each generated claim to be represented in the structured dataset with an evidence note or explicit review status.
- The public web app currently has a one-off Skedda comparison page; this feature replaces that page with the generated Skedular vs Skedda page and introduces `/compare` as the comparison index.
- Existing comparison URLs do not need backward compatibility. The feature starts clean rather than preserving or redirecting legacy comparison URLs.
- The comparison section is published as a complete required page set, not as an incremental rollout.
- Current Skedular evidence should prefer active specs, current help content, public-web data files, split app route surfaces, current pricing data, and implemented contracts over older draft strategy documents.
- The comparison experience belongs to the public web/product-discovery surface rather than the signed-in customer, Teams, or Spaces administration apps.
- The first release focuses on English-language public comparison content using American spelling and grammar.
- Competitor pricing can be summarized qualitatively unless current, supportable pricing facts are added to the competitor dataset.
- Competitor data may include unknown states; unknown is preferable to overstating a competitor limitation.
- Skedular's Developer category must stay conservative unless API or webhook capability is explicitly evidenced during implementation.
- Existing product, pricing, help, and CTA destinations remain the source of truth for Skedular product links.
