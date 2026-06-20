# Feature Specification: Skedular Documentation Center

**Feature Branch**: `033-documentation-center`
**Created**: 2026-07-14  
**Status**: Draft  
**Input**: User description: "Create a production-ready Documentation Center in the public website for Skedular Teams, Skedular Spaces, and Skedular Host."

## Clarifications

### Session 2026-07-14

- Q: How should historical help URLs be handled? → A: No legacy links or documentation system exist; no backward compatibility is required.
- Q: Where is the documentation delivered? → A: In the public web app only; not in the Teams, Spaces, or Host applications.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Choose the right product documentation (Priority: P1)

A prospective or current Skedular user can open Documentation from the public navigation, understand the difference between Teams, Spaces, and Host, and reach the product area that matches their work.

**Why this priority**: Users must be able to orient themselves before any individual guide can be useful.

**Independent Test**: From the public navigation, a reader can reach the documentation home page, select the correct product based on a short description, and open that product's landing page without needing a sign-in.

**Acceptance Scenarios**:

1. **Given** a visitor is on any public website page, **When** they select Documentation in the main navigation, **Then** they reach the canonical documentation home page.
2. **Given** a visitor is unsure which product they use, **When** they review the documentation home page, **Then** they can distinguish private workplace coordination (Teams), commercial workspace operations (Spaces), and independent place rental (Host).
3. **Given** a visitor is viewing a product landing page, **When** they select a category, **Then** they see only articles appropriate to that product and category.

---

### User Story 2 - Get a product running (Priority: P1)

A new administrator or host can follow a complete, accurate Getting Started guide for their product and understand the next sensible configuration step.

**Why this priority**: First-use guidance delivers the immediate value of a help center and reduces avoidable support work.

**Independent Test**: A reviewer can follow each product's Getting Started guide from a clean account through its documented setup sequence and verify that every claimed action corresponds to an existing product surface.

**Acceptance Scenarios**:

1. **Given** a new Teams administrator, **When** they read Teams Getting Started, **Then** they can create or join a private organization, set up locations and resources, invite or organize people, and begin private booking.
2. **Given** a new Spaces operator, **When** they read Spaces Getting Started, **Then** they can create a marketplace organization, set up locations and resources, create an offer, prepare customer-facing availability, and understand the next publishing and payment-setup steps.
3. **Given** a new Host, **When** they read Host Getting Started, **Then** they can create a Host organization, add a place, set its pricing and policies, prepare payment setup, and understand the draft-to-published listing journey.

---

### User Story 3 - Find trustworthy feature guidance (Priority: P1)

A user can browse a scalable product-and-category structure, open a clearly labeled article for a supported capability, and move to relevant adjacent guidance without losing context.

**Why this priority**: Documentation must remain navigable as it grows from the initial inventory to hundreds of articles.

**Independent Test**: For every discovered live feature in the approved inventory, a reviewer can find either a published initial article, a clearly labeled placeholder article, or an explicit exclusion/content-gap record; no future or unsupported capability is presented as available.

**Acceptance Scenarios**:

1. **Given** a reader is on an article, **When** they use breadcrumbs, previous/next links, or related articles, **Then** each destination remains within a meaningful product or shared-concept path.
2. **Given** a feature has only enough verified evidence for a short first article, **When** a reader opens it, **Then** the article explains the verified purpose and next step without inventing unverified behavior.
3. **Given** a feature is not present in the current codebase, **When** the inventory is reviewed, **Then** it is omitted from published documentation or explicitly labeled as future work outside the live documentation path.

---

### User Story 4 - Discover documentation through search (Priority: P2)

A person using a search engine or an AI answer engine can discover a documentation page, understand its purpose from the result, and reach the canonical page rather than a duplicate or dead route.

**Why this priority**: Public documentation should help people before they need to contact support and should reinforce accurate product terminology on the web.

**Independent Test**: A reviewer can verify that every published documentation page has a unique title, concise description, canonical address, logical heading hierarchy, crawl eligibility, and a valid route in the public discovery inventory.

**Acceptance Scenarios**:

1. **Given** a documentation page is published, **When** its page source and public discovery outputs are reviewed, **Then** its canonical address, indexability, and page metadata agree.
2. **Given** a reader lands on a deep documentation article from search, **When** they open it, **Then** the title, summary, headings, and internal links make the article's product context clear.
3. **Given** documentation is added or removed, **When** public discovery outputs are generated, **Then** published pages are included and unpublished pages are excluded.

---

### User Story 5 - Use documentation on any device (Priority: P2)

A reader can use the documentation comfortably with keyboard navigation, assistive technology, narrow screens, and either visual color preference.

**Why this priority**: Product guidance is needed in real working conditions, not just on a desktop in one color mode.

**Independent Test**: A reviewer can navigate the documentation home, product landing pages, category lists, and an article by keyboard at narrow and wide viewport sizes, with no loss of essential navigation or reading order.

**Acceptance Scenarios**:

1. **Given** a reader uses a narrow screen, **When** they open navigation, category controls, and an article, **Then** content and wayfinding remain usable without horizontal scrolling.
2. **Given** a reader uses a keyboard or screen reader, **When** they move through documentation navigation and article controls, **Then** labels, current location, heading order, and link purposes are understandable.
3. **Given** a reader changes the website color preference, **When** they browse documentation, **Then** text, controls, and article hierarchy remain legible.

---

### User Story 6 - Maintain accurate content as Skedular evolves (Priority: P3)

A content owner can add, revise, localize, or organize large amounts of documentation without changing established addresses or redesigning the whole documentation center.

**Why this priority**: The initial library is a foundation; it must not become a one-off content campaign.

**Independent Test**: A reviewer can add a representative future article for an API reference, release note, localized page, versioned guide, screenshot, video, and search result without changing existing public article addresses or navigation contracts.

**Acceptance Scenarios**:

1. **Given** a new article is added to an existing product category, **When** it is published, **Then** it appears in relevant navigation, related-content paths, and public discovery outputs without manual restructuring of other articles.
2. **Given** future API references, release notes, media, language variants, versions, or search are introduced, **When** their content model is planned, **Then** they can coexist with the initial product guides without address conflicts.

### Edge Cases

- A page title, slug, or translated title conflicts with an existing article: publication must not replace or create an ambiguous canonical address.
- A feature exists only in one product or in an embedded collaboration experience: its article must say which product and access context it applies to.
- A payment, refund, identity, accounting, or integration flow lacks sufficient public-safe evidence: the article must provide safe high-level guidance and a support path rather than exposing secrets, internal states, or guesswork.
- A content item is withdrawn or no longer verified: it must no longer be indexed. Its already-published address must either redirect to a verified replacement or render a non-indexable retirement page that links to the most relevant available page and the documentation home. This applies only to Documentation Center addresses published by this feature, not to a prior help system.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The public website MUST expose a **Documentation** item in its main navigation that links to `/docs`, and the footer/resource navigation MUST provide an equivalent discoverable path.
- **FR-001a**: The documentation center, its routes, content, navigation, and search-discovery surfaces MUST be delivered in the public web app only; the Teams, Spaces, and Host applications MUST not receive separate documentation route trees as part of this feature.
- **FR-002**: `/docs` MUST be the canonical documentation home page and MUST explain the audience and boundary of Skedular Teams, Skedular Spaces, and Skedular Host using consistent, plain-language terminology.
- **FR-003**: The documentation center MUST use stable, readable, hierarchy-based public addresses beginning with `/docs`, including product landing pages at `/docs/teams`, `/docs/spaces`, and `/docs/host` and category/article paths beneath the owning product.
- **FR-004**: The documentation center MUST retain a source-of-truth article inventory with product, category, address, title, short description, publication state, evidence reference, and related-content relationships for every documentation page.
- **FR-005**: The information architecture MUST support hundreds of pages without requiring a URL or navigation restructure, and MUST reserve non-conflicting extensions for API documentation, release notes, screenshots, videos, language variants, versions, and full-text search.
- **FR-006**: Each product section MUST have a landing page, a Getting Started category, Core Features, Bookings, Settings, Integrations, FAQs, and Best Practices; categories with no applicable live capability MUST explain that boundary rather than imply a feature exists.
- **FR-007**: The initial Teams inventory MUST cover only verified Teams capabilities, including private organizations, locations, resources, zones and floor plans, private bookings, teams and users, workplace availability and analytics, settings and access, Slack, Microsoft Teams, and enterprise sign-in guidance.
- **FR-008**: The initial Spaces inventory MUST cover only verified Spaces capabilities, including marketplace organizations, locations, resources, zones and floor plans, products and pricing, marketplace publishing, bookings, subscriptions, customers, payments and bank-account setup, refunds at a public-safe level, analytics, settings and access, Slack, Microsoft Teams, enterprise sign-in, and Xero accounting guidance at a public-safe level.
- **FR-009**: The initial Host inventory MUST cover only verified Host capabilities, including onboarding and organization setup, place/listing creation, listing details, pricing, availability and booking rules, cancellation policies, media and amenities, draft and publication status, bookings and renters, payment connection, commissions, analytics, settings, and the simplified Host boundary that hides underlying coworking configuration.
- **FR-010**: The first release MUST provide complete, step-by-step Getting Started guides for Teams, Spaces, and Host, with each guide ending in appropriate links to the next relevant live documentation article and product page.
- **FR-011**: Every discovered live capability in the approved source inventory MUST map to a published initial article, a clearly marked placeholder article with verified scope, a documented shared-concept article, or an explicit content-gap/exclusion decision.
- **FR-012**: Placeholder articles MUST be useful rather than empty: each MUST state the feature's verified purpose, applicable product and audience, prerequisites when known, a clear next action, and related articles; they MUST NOT assert unverified steps or future functionality.
- **FR-013**: Documentation MUST clearly distinguish shared concepts from product-specific ones, including the different meanings of locations, bookings, products, subscriptions, teams, users or customers, payments, refunds, analytics, and marketplace publishing.
- **FR-014**: Every published article MUST provide a unique title, concise summary, one primary page heading, logical subordinate headings, a canonical public address, indexability instruction, and internal links that establish product and category context.
- **FR-015**: Every published article MUST provide visible breadcrumbs, Previous/Next navigation where an ordered category exists, and related-article links selected from the same workflow or a clearly labeled cross-product relationship.
- **FR-016**: Documentation pages MUST reuse the public website's established visual language, responsive behavior, accessibility standards, and light/dark mode support.
- **FR-017**: Documentation content MUST use American English, practical founder-style writing, consistent names used by the public website and live product interfaces, short explanatory paragraphs, descriptive headings, and direct user-focused language suitable for both traditional and AI-assisted search.
- **FR-018**: Documentation pages MUST use only verified claims from the current codebase, current public-site evidence, or completed feature artifacts. Planned, incomplete, ambiguous, sensitive, or insufficiently verified work MUST be excluded from public claims or explicitly labeled as future work outside the live guide flow.
- **FR-019**: Documentation about payment setup, refunds, identity, accounting, or third-party integrations MUST avoid secrets, internal implementation details, security-sensitive steps, provider failure internals, and unsupported claims; it MUST direct readers to an appropriate safe next step when detail is unavailable.
- **FR-020**: Where relevant, product landing pages and articles MUST link naturally to the corresponding public product, pricing, comparison, and blog/resource pages without using unrelated promotional links as documentation navigation.
- **FR-021**: Published documentation pages MUST participate in the website's canonical-address, metadata, structured discovery, sitemap, robots, and AI-readable content inventories; unpublished, withdrawn, or future-only pages MUST not be presented as crawlable live documentation.
- **FR-022**: The initial documentation center MUST not add backward-compatibility redirects or migration behavior for prior help systems, because no legacy documentation links or legacy documentation system exist.
- **FR-023**: The content model and publishing workflow MUST allow future additions of screenshots, videos, localized variants, versioned guidance, release notes, API references, and full-text search without changing existing page addresses or weakening page metadata and navigation.
- **FR-024**: The documentation center MUST maintain a terminology and evidence review process so terminology changes in product applications, public product pages, and documentation are reconciled before publication.
- **FR-025**: The feature MUST include content and navigation verification covering all public routes, product landing pages, required categories, Getting Started journeys, metadata, canonical addresses, discovery outputs, responsive behavior, keyboard navigation, and light/dark presentation.
- **FR-026**: Shared-concept articles MUST use a stable `/docs/shared/<category>/<article-slug>` address family, be clearly labeled as cross-product guidance, and remain distinct from the three product landing areas.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Because this is a public content feature without a new business workflow, the feature MUST preserve existing public-site build diagnostics and record documentation build, link, and discovery-output verification results.
- **LOG-002**: The publication workflow MUST record an actionable validation result when an article has a duplicate address, missing required metadata, missing evidence reference, broken internal link, or inconsistent publication state.
- **LOG-003**: Any generated discovery or redirect failure MUST report the affected public path and failure category without exposing environment-specific destination values or sensitive configuration.
- **LOG-004**: Documentation validation records MUST identify the relevant article or build context and MUST avoid sensitive account, payment, identity, or integration data.

### Key Entities

- **Documentation article**: A public, product-scoped or shared-concept guide with a stable address, metadata, content state, evidence reference, and navigation relationships.
- **Documentation category**: A named collection of articles for a product or shared concept that defines its display order and ordered navigation.
- **Capability inventory item**: A verified product capability mapped to documentation coverage, evidence, and a publication or exclusion decision.
- **Evidence reference**: The reviewed code, public content, completed specification, or approved product source supporting a documentation claim.
- **Related-article relationship**: A deliberate navigational connection between articles in the same workflow or an accurately labeled cross-product concept.

### Scope Boundaries

- The first release establishes the public documentation foundation, initial article structure, useful placeholders, and complete Getting Started guidance. It does not claim to provide a complete operations manual for every field or status in every product screen.
- Full-text search, media capture, localization, versioned editions, API references, and release notes are architectural extension points, not required content releases in this initial delivery.
- The retired standalone help projects are not reinstated as separate products; their useful completed inventory and terminology serve as evidence for the unified public documentation center.
- Documentation describes live, verified behavior. It does not change product permissions, billing, booking, marketplace, integration, or support workflows.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of public website navigation contexts provide a working Documentation path to `/docs`, and all three product landing pages are reachable from the documentation home page within two selections.
- **SC-002**: 100% of the approved Teams, Spaces, and Host capability inventory is mapped to an initial article, useful placeholder, shared article, or explicit exclusion/content-gap decision.
- **SC-003**: In a recorded guided usability review of at least 10 new readers, at least 90% can identify the correct product and open its Getting Started guide within 30 seconds.
- **SC-004**: In that review, at least 85% of representative new administrators or hosts can identify the correct next setup action after completing their product's Getting Started guide without asking for help.
- **SC-005**: 100% of published documentation pages pass metadata, canonical-address, heading, breadcrumb, related-content, internal-link, sitemap, robots, and AI-readable discovery checks.
- **SC-006**: 100% of tested documentation routes remain usable at narrow and wide viewport sizes, with keyboard navigation and both color preferences preserving access to primary navigation and article wayfinding.
- **SC-007**: Content review finds zero published claims about unsupported, planned, or unverified product capabilities, and zero contradictions with approved product terminology.
- **SC-008**: A representative new article can be added to an existing product category, and a representative future API, release-note, localized, versioned, media, and search content entry can be modeled, without changing any existing documentation address.

## Assumptions

- The public website is the canonical home for this documentation center and `/docs` is available for the new route family.
- The public web app is the sole delivery surface for this feature; product applications remain documentation subjects, not documentation hosts.
- The current product applications, public product pages, source inventories from completed help work, and completed feature artifacts are the evidence baseline; code and live public content take precedence if they conflict with an older artifact.
- Public documentation remains readable without authentication. Role-specific or sensitive operational detail will be summarized safely and directed to support or future private documentation when necessary.
- The first release may use written descriptions where screenshots or video are not yet verified; media-ready content slots will be added without inventing visual evidence.
- Existing public website SEO and discovery conventions are the baseline for documentation discovery and will be extended rather than replaced.
