# Feature Specification: Public Website Content Integration

**Feature Branch**: `025-public-website-content`  
**Created**: 2026-06-05  
**Status**: Draft  
**Input**: User description: "Read `src/web/apps/public-web/public-website-content-draft.md`, review current Skedular public website pages/posts and relevant public website references, then specify how to add the complete drafted public website content into the existing public-web application."

## Clarifications

### Session 2026-06-05

- Q: What should the public website launch with for workspace search and booking? → A: Public site booking actions forward to the separate app website; no booking happens directly on the public website for this feature.
- Q: How should public website outbound destination URLs be configured? → A: Use three required public environment variables following the existing pattern; do not hardcode staging or production domains.
- Q: What is the first-implementation scope for current blog/support migration? → A: Fully migrate and publish every current public blog and support page in the first implementation.
- Q: What pricing detail should the first implementation publish? → A: Publish the draft's suggested pricing amounts and host commission range.
- Q: What is the first-implementation scope for comparison pages? → A: Publish all draft comparison pages in the first implementation.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Visitor Finds and Books Workspace (Priority: P1)

A first-time public visitor arrives at Skedular and wants to find a place to work without learning internal product language. They should immediately understand that Skedular helps them discover and book desks, meeting rooms, event spaces, private offices, and flexible workspaces from available providers.

**Why this priority**: The new draft positions the public website around workspace discovery first. If visitors cannot start a search or understand what can be booked, the website fails its primary growth purpose.

**Independent Test**: A reviewer can visit the home page, see a visible workspace search entry point above the fold, identify supported resource types, and follow the primary booking call-to-action from the public website to the separate application website.

**Acceptance Scenarios**:

1. **Given** a first-time visitor opens the public website, **When** the home page loads, **Then** the first viewport presents Skedular as a way to find and book workspace, not as an internal marketplace concept.
2. **Given** a visitor wants to search, **When** they view the first viewport, **Then** they see fields or entry points for location, date, resource type, and search.
3. **Given** a visitor is comparing options, **When** they review the discovery content, **Then** they can identify desks, meeting rooms, event spaces, and private offices as supported booking categories.
4. **Given** a visitor submits or follows a booking/search action on the public website, **When** the action is completed, **Then** they are forwarded to the separate application website through the configured booking/search destination URL.
5. **Given** a visitor is not ready to search, **When** they scan the home page, **Then** they can still understand featured locations, popular spaces, recently added spaces, or map-based discovery as available discovery paths.

---

### User Story 2 - Organization Buyer Understands Skedular Teams (Priority: P2)

An operations leader, facilities manager, executive assistant, or workplace technology buyer wants to know whether Skedular can manage a private workplace for employees. They need a clear Teams path that explains desk booking, room booking, parking, team attendance, floor plans, analytics, collaboration integrations, and enterprise identity support.

**Why this priority**: The existing public site already emphasizes hybrid teams, desk and meeting-room booking, team visibility, Slack, Microsoft Teams, and analytics. The new site must preserve that value while separating it from public booking and operator-focused messaging.

**Independent Test**: A reviewer can navigate from the main menu to a Teams-focused page and confirm that the page explains the target audience, use cases, supported resources, collaboration features, administration features, and security/identity expectations without relying on public booking terminology.

**Acceptance Scenarios**:

1. **Given** an organization buyer uses the main navigation, **When** they select Teams, **Then** they land on a page clearly framed around private workplace management for organizations.
2. **Given** the buyer reviews the Teams page, **When** they scan the page, **Then** they can identify desk, room, parking, equipment, team attendance, floor plan, analytics, Slack, Microsoft Teams, and SSO capabilities.
3. **Given** the buyer wants a next step, **When** they reach any major Teams section, **Then** they can book a demo or continue to pricing/contact through configured destination URLs without losing context.

---

### User Story 3 - Workspace Operator Understands Skedular Spaces (Priority: P3)

A co-working operator, flexible workspace provider, or shared-office owner wants to know whether Skedular can help them manage resources, package products, collect payments, bill customers, issue invoices, and publish spaces publicly. They need a distinct Spaces path that describes operator workflows and commercial outcomes.

**Why this priority**: The draft introduces Spaces as a major product pillar. Operator-facing content is required so the public website can support the newer marketplace, payments, invoicing, and hosting direction.

**Independent Test**: A reviewer can navigate to a Spaces-focused page and verify that it explains the operator audience, resource management, product catalog, pricing, billing modes, payment methods, invoicing, marketplace publishing, tax, and branding expectations.

**Acceptance Scenarios**:

1. **Given** a workspace operator uses the main navigation, **When** they select Spaces, **Then** they land on a page clearly framed around running and monetizing workspace inventory.
2. **Given** the operator reviews the Spaces page, **When** they scan the page, **Then** they can identify resource management, product management, product pricing, billing, payments, invoicing, tax, marketplace publishing, and branding as supported areas.
3. **Given** the operator wants to assess commercial fit, **When** they view pricing or host information, **Then** they can distinguish location subscription pricing from marketplace commission and public booking fees.

---

### User Story 4 - Prospect Compares Pricing and Next Steps (Priority: P4)

A prospect wants to understand which Skedular product path applies to them and what commercial model to expect before contacting sales or signing up. They need pricing that distinguishes public booking, Teams, Spaces, and host commission without overpromising unapproved prices.

**Why this priority**: The current site includes pricing, and the draft adds a broader pricing strategy. The updated site must avoid confusing buyer types or mixing Teams user pricing with Spaces location pricing.

**Independent Test**: A reviewer can visit Pricing and identify the intended audience, pricing basis, included capabilities, and call-to-action for each product path.

**Acceptance Scenarios**:

1. **Given** a visitor opens Pricing, **When** they compare options, **Then** Teams pricing is described as active-user based, Spaces pricing as location based, public booking as no subscription for bookers, and hosts as commission-based when applicable.
2. **Given** a visitor reviews pricing, **When** the pricing page renders, **Then** it includes the draft's suggested Teams tiers, Spaces tiers, public booking model, and host commission range.
3. **Given** a visitor chooses a pricing path, **When** they use its call-to-action, **Then** the destination matches the selected path: search, sign up, book demo, or contact sales through configured destination URLs.

---

### User Story 5 - Reader and Search Engine Discover Helpful Resources (Priority: P5)

A visitor, customer, or search engine finds Skedular through educational content about hybrid work, workspace planning, payments, invoicing, marketplace features, and product support. Existing public articles and support resources should be fully migrated and published in the first implementation, with corrections and redirects where needed.

**Why this priority**: The current public website includes blog and support pages that may already receive search traffic. Losing them during the migration would reduce discoverability and break external links.

**Independent Test**: A reviewer can inspect the content inventory, confirm each current public blog/support page has a published destination in the first implementation, and verify published resources have titles, descriptions, dates where applicable, canonical URLs, and clear navigation.

**Acceptance Scenarios**:

1. **Given** the current public website has existing pages and posts, **When** the new content is prepared, **Then** each current public blog/support URL has a published destination in the first implementation.
2. **Given** a current post remains relevant, **When** it is published on the new site, **Then** it keeps or improves its topic, title, summary, date, and search discoverability.
3. **Given** a current page or post is outdated, duplicated, or unsafe to publish as-is, **When** migration decisions are reviewed, **Then** it is rewritten, merged into a published equivalent, or redirected to a published replacement in the first implementation.
4. **Given** a search crawler reads the site, **When** it indexes public pages, **Then** each core page has descriptive metadata, machine-readable structure where appropriate, and no blocked critical content.

---

### User Story 6 - Product Team Confirms Full Draft Coverage (Priority: P6)

A product owner needs confidence that the entire attached public website draft and ChatGPT brainstorming output has been reviewed, not only the first-page marketing sections. Every section of the draft should become page content, a feature-page candidate, a resource idea, a technical requirement for planning, a future roadmap note, or an explicit out-of-scope decision.

**Why this priority**: The draft is the source of truth for the public website expansion. Missing later sections such as booking behavior, tax, subscriptions, host model, visibility controls, SEO, AI discoverability, accessibility, performance, competitive positioning, and future features would produce an incomplete plan.

**Independent Test**: A reviewer can compare the source draft section-by-section against the content/source inventory and verify that every heading has a recorded decision and destination.

**Acceptance Scenarios**:

1. **Given** the public website content draft contains a heading or feature item, **When** the content inventory is reviewed, **Then** that heading or item has a keep, rewrite, merge, future, technical-planning, or exclude decision.
2. **Given** a draft section describes a current capability, **When** the new site content is prepared, **Then** the capability is either represented in public copy or documented as intentionally withheld pending verification.
3. **Given** a draft section describes a future capability, **When** the new site content is prepared, **Then** it is not presented as currently available unless approved and verified.
4. **Given** the draft includes technical, SEO, accessibility, AI-discoverability, or performance expectations, **When** planning starts, **Then** those expectations are carried forward as planning constraints rather than lost from the feature.
5. **Given** the draft includes comparison-page candidates, **When** the first implementation is prepared, **Then** each comparison page is published with neutral positioning, reviewed competitor claims, unique metadata, and a relevant call-to-action.

### Edge Cases

- A visitor searches for "marketplace" from old content, but the new public booking journey should use plain "Skedular" booking language unless speaking to hosts/operators.
- A section appears only in the attached draft and not on the current public website; it still needs an inventory decision instead of being ignored.
- A current blog post references a feature name, price, date, integration, or product state that has changed; the migrated content must be corrected before first implementation publication.
- A current URL cannot be reproduced exactly; the migration inventory must define an equivalent destination or a clear removal decision.
- Pricing values need later business approval or change by environment; the published pricing copy must keep values centralized and easy to revise without changing unrelated content.
- Search or discovery data is unavailable for the first release; the home page must still provide a clear search entry point and useful static discovery content, then forward booking/search actions to the separate application website.
- A staging or production URL changes; public website links must update through environment configuration rather than code changes.
- The same capability appears in multiple product paths; copy must explain the buyer-specific value without duplicating unclear or contradictory claims.
- Public pages include user-facing English copy; all visible copy must use American spelling and grammar.
- Public copy sounds stiff, generic, over-polished, repetitive, or obviously machine-generated; it must be rewritten until it reads like a thoughtful human wrote it for real visitors.
- Competitive references influence positioning and page structure, but the site must not copy competitor wording, claims, or proprietary layouts.
- Draft comparison pages are included in first implementation; if a competitor claim cannot be verified, the page must use neutral positioning language rather than omit the page.
- Visitors browse with assistive technology, small screens, slow connections, or blocked scripts; core navigation, content, and calls-to-action must remain accessible and understandable.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The website MUST present Skedular as a workspace operating system that supports public workspace discovery, private workplace management, workspace operator management, resource commerce, payments, billing, invoicing, and team collaboration.
- **FR-002**: The home page MUST prioritize public workspace discovery and booking, with first-viewport copy and actions centered on finding and booking workspace.
- **FR-003**: The home page MUST include a visible search entry point with location, date, resource type, and search action concepts.
- **FR-004**: The website MUST avoid requiring public visitors to understand the term "marketplace" before searching or booking workspace.
- **FR-005**: The main navigation MUST include Home, Teams, Spaces, Pricing, Resources, Book Demo, and Login paths or equivalent user-recognizable labels.
- **FR-006**: The home page MUST include sections for workspace discovery, business product paths, platform value, feature highlights, and social proof readiness.
- **FR-007**: The Teams path MUST describe private workplace management for enterprises, government, hybrid workplaces, and corporate offices.
- **FR-008**: The Teams path MUST cover desk booking, room booking, parking booking, equipment/resource booking, team attendance, floor plans, analytics/reporting, Slack, Microsoft Teams, and enterprise identity.
- **FR-009**: The Spaces path MUST describe workspace management for co-working operators, flexible workspace providers, and shared-office providers.
- **FR-010**: The Spaces path MUST cover resource management, product catalog management, pricing, billing cadence, payments, invoicing, tax handling, marketplace publishing, and branding.
- **FR-011**: The Pricing path MUST distinguish public booking, Teams, Spaces, and host/marketplace commercial models.
- **FR-012**: Public pricing copy MUST publish the draft's suggested Teams tiers, Spaces tiers, public booking model, and host commission range in the first implementation.
- **FR-013**: The Resources path MUST include a migration inventory for current public website pages, posts, support resources, and any draft resources proposed by the content brief.
- **FR-014**: Every current public blog and support page MUST be fully migrated into the first implementation as a published page, rewritten page, merged published equivalent, or redirect to a published replacement.
- **FR-015**: Migrated or rewritten resource content MUST preserve useful search intent from current public content, including hybrid work, office space planning, workspace productivity, payments, invoicing, marketplace features, Slack, and Microsoft Teams topics.
- **FR-016**: The website MUST include clear calls-to-action for searching workspace, booking a demo, signing up or logging in, contacting sales/support, and learning more about Teams or Spaces.
- **FR-017**: Search and booking calls-to-action on the public website MUST forward visitors to the separate application website for the current booking experience; the public website MUST NOT require direct booking, checkout, or availability confirmation in this feature.
- **FR-018**: The public website MUST configure outbound app/search, login/sign-up, and demo/contact destinations through three required public environment variables following the existing `PUBLIC_SKEDULAR_SIGNUP_URL` naming and validation pattern.
- **FR-019**: Public website code, content, tests, and deployment configuration MUST NOT hardcode staging or production destination domains for app/search, login/sign-up, or demo/contact actions.
- **FR-020**: The public website MUST fail clearly during validation or build when any required public destination URL variable is missing or empty.
- **FR-021**: Each primary page MUST include a unique title, summary, canonical destination, and search/social preview metadata.
- **FR-022**: The website MUST expose a sitemap-ready page inventory and avoid orphaning primary pages.
- **FR-023**: Public copy MUST be accurate against current Skedular product capabilities and must not claim future capabilities as currently available.
- **FR-024**: Public copy MUST use American spelling and grammar while preserving technical names, product names, integration names, and legal wording.
- **FR-025**: Public copy MUST read as friendly, professional, specific, and human-written; it MUST avoid generic AI-sounding phrasing, empty superlatives, repetitive sentence structures, and filler claims that do not help visitors make a decision.
- **FR-026**: Public copy MUST be reviewed in context on the page, not only as isolated text, so headings, body copy, labels, and calls-to-action feel natural together for the intended audience.
- **FR-027**: The feature MUST include a section-by-section source coverage inventory for the entire attached public website content draft, including every heading and major bullet list.
- **FR-028**: The source coverage inventory MUST include draft sections for resource types, floor plans, availability, analytics, teams, tags, zones, dynamic product matching, listings, images, amenities, private metadata, maps, opening hours, booking engine, payments, tax, billing, invoicing, Xero, cancellation policies, authentication, Slack, Microsoft Teams, host model, visibility controls, subscriptions, custom domains, future features, competitive positioning, SEO, AI discoverability, accessibility, performance, and final positioning.
- **FR-029**: Draft items describing current capabilities MUST be verified before publication; draft items describing potential future capabilities MUST be labeled for future planning or excluded from public current-state copy.
- **FR-030**: The website MUST publish all draft comparison-page candidates in the first implementation, with reviewed neutral positioning and no unverified competitor claims.
- **FR-031**: The website MUST include search-discovery support for primary keywords, feature-page candidates, published comparison pages, structured data candidates, and machine-readable content hierarchy from the draft.
- **FR-032**: The website MUST meet baseline accessibility expectations for keyboard navigation, semantic headings, alternative text, readable contrast, responsive layout, understandable link text, and the WCAG AA target described by the draft.
- **FR-033**: The website MUST carry the draft's performance expectations into planning, including fast page loading, stable layout, low interaction delay, and high performance/accessibility/search quality scores.
- **FR-034**: The website MUST include privacy-safe analytics readiness so future campaign and page performance measurement can be added without changing content meaning.
- **FR-035**: The website MUST avoid publishing sensitive operational details, private metadata examples that could expose customer secrets, internal system names, or non-public roadmap commitments.
- **FR-036**: The website MUST include a content review checklist for factual accuracy, current-product alignment, pricing approval, legal/privacy safety, accessibility, search metadata, complete draft coverage, competitor-claim review, and human-quality tone before publication.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST produce a reviewable content inventory showing source, decision, destination, owner, and status for each migrated or excluded page/post.
- **LOG-002**: The feature MUST produce a reviewable primary-page inventory showing every public page, page purpose, target audience, primary call-to-action, metadata status, and publication status.
- **LOG-003**: The feature MUST record content branch decisions for ambiguous claims, unapproved pricing, unavailable search data, and excluded legacy content.
- **LOG-004**: The feature MUST produce a reviewable draft coverage inventory mapping every source draft section to a publication, planning, future, merge, or exclude decision.
- **LOG-005**: The feature MUST avoid exposing sensitive values, private access instructions, internal customer data, or unreleased roadmap details in public pages, metadata, logs, or content inventories.

### Key Entities _(include if feature involves data)_

- **Public Page**: A published website page with a purpose, audience, title, summary, navigation placement, metadata, primary call-to-action, and accessibility/search review status.
- **Product Path**: A buyer or user journey for Skedular, Teams, Spaces, public booking, or hosts, including its audience, value proposition, capability groups, and conversion action.
- **Resource Article**: A public educational or support content item with source URL, title, date where applicable, topic, summary, migration decision, publication destination, and review status.
- **Content Source**: An input used to produce the new website content, including the public website content draft, current live website pages/posts, existing in-repository public website content, and reviewed competitive/reference websites.
- **Draft Coverage Item**: A heading or major bullet group from the attached public website content draft, including its source location, decision, destination, and verification status.
- **Migration Decision**: A keep, rewrite, redirect, merge, or exclude decision for current public content, including reason, owner, and destination.
- **Capability Claim**: A public statement that a product supports a feature, integration, commercial model, or workflow; each claim must be verified before publication.
- **Comparison Page**: A public page comparing Skedular with a named alternative from the draft, including neutral positioning, reviewed competitor claims, metadata, and a relevant call-to-action.
- **Call-to-Action**: A visitor action such as Search Workspace, Book Demo, Learn More, Login, Sign Up, Contact Sales, Contact Support, or Join Community, including its intended destination and audience. Search and booking actions currently forward from the public website to the separate application website through configured destination URLs.
- **Public Destination URL**: A required public environment variable value used for an outbound public website action. The feature requires three such values for app/search, login/sign-up, and demo/contact destinations, all configured per environment.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: At least 90% of first-time review participants can identify within 10 seconds that Skedular helps people find and book workspace.
- **SC-002**: At least 90% of organization-buyer review participants can correctly identify Teams as the private workplace management path after viewing the navigation and Teams page.
- **SC-003**: At least 90% of workspace-operator review participants can correctly identify Spaces as the operator/business management path after viewing the navigation and Spaces page.
- **SC-004**: 100% of current public blog and support pages have a published destination or redirect to a published replacement in the first implementation.
- **SC-005**: 100% of published primary pages have unique titles, summaries, canonical destinations, primary calls-to-action, and search/social preview metadata.
- **SC-006**: 100% of pricing model sections from the draft are represented on the pricing page, including Teams tiers, Spaces tiers, public booking terms, and host commission range.
- **SC-007**: Primary navigation allows a reviewer to reach Home, Teams, Spaces, Pricing, Resources, Book Demo, and Login in no more than one interaction from any primary page.
- **SC-008**: Automated and manual accessibility review finds no critical issues across the primary page set before launch.
- **SC-009**: No migrated or newly written public page contains British-only spellings in visible user-facing copy, except where preserved in official names, source titles, legal text, or quoted material.
- **SC-010**: At least 90% of content review participants describe the published page copy as clear, friendly, professional, and natural rather than generic, robotic, or obviously AI-generated.
- **SC-011**: 100% of headings and major bullet groups in the attached public website content draft have a recorded coverage decision before implementation starts.
- **SC-012**: 100% of draft future-feature items are either excluded from current-state public copy or clearly routed into future planning before launch.
- **SC-013**: The launched site preserves or redirects all launch-approved current public URLs so external visitors do not hit avoidable dead ends for migrated content.
- **SC-014**: 100% of public website booking/search actions route visitors to the separate application website until direct public-site booking is explicitly added in a future feature.
- **SC-015**: 100% of public website outbound app/search, login/sign-up, and demo/contact links are sourced from required public environment variables, with no hardcoded staging or production destination domains in source content or code.
- **SC-016**: 100% of comparison-page candidates listed in the draft are published in the first implementation with reviewed competitor claims and unique metadata.

## Assumptions

- The existing public-web application is the target for this content expansion; detailed implementation structure will be decided during planning.
- The content draft at `src/web/apps/public-web/public-website-content-draft.md` is treated as the primary product brief and as the available export of the ChatGPT brainstorming conversation. Any separate ChatGPT conversation that is not present in that file is not available to this spec unless provided later.
- The whole content draft must be reviewed end-to-end during planning, while current live pages and posts are treated as migration inputs that must be preserved, rewritten, redirected, merged, or intentionally excluded.
- "Other websites" means reviewed public references and competitors used for positioning and experience inspiration, not sources to copy verbatim.
- Public workspace discovery is the primary home page story, even if a full live availability search experience requires a later integration step.
- Public website and application website domains are separate. The public website is currently reached through a public marketing domain, while booking and application flows live on a separate application domain.
- The current release must not implement direct booking on the public website. If live inventory/search data is not ready on the public website, the first release may provide a search entry point and static discovery modules that forward visitors to the application website.
- The existing public website app already uses a public environment variable for one app destination URL. This feature should extend that pattern to exactly three required public destination URL variables for app/search, login/sign-up, and demo/contact destinations, with staging and production values supplied by environment-specific deployment configuration.
- Blog/support content migration is in scope as a first-implementation publication requirement for all current public blog and support pages; screenshot capture, customer logo procurement, testimonial approval, and new case-study creation are only in scope if approved assets already exist.
- Existing legal pages, privacy policy, terms, and customer-sensitive support details require owner review before migration or publication.
- Pricing values in the draft are intended for first-implementation publication, including suggested Teams tiers, Spaces tiers, public booking terms, and host commission range. They should remain easy to revise if business approval changes before launch.
- Product capability claims must be verified against current product reality before launch, especially payments, invoicing, Xero, branding, custom domains, WorkOS, SSO, Slack, Microsoft Teams, maps, and marketplace publishing.
- Draft technical architecture notes, SEO targets, structured data ideas, AI discoverability expectations, accessibility targets, and performance targets are planning inputs and quality constraints, not permission to publish inaccurate product claims.
- Comparison pages listed in the draft are first-implementation publication scope. Competitor claims must be verified or rewritten as neutral positioning before launch.
- Public user-facing copy follows American English, but code identifiers, product names, integration names, source article titles, legal terms, and external quoted text are not renamed solely for localization.
- The desired editorial voice is friendly, professional, plainspoken, and specific to Skedular's audiences. Copy should feel written by a human who understands workspace operations, not like generic AI-generated marketing text.
