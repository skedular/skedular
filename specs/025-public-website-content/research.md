# Research: Public Website Content Integration

## Decision: Expand the Existing Astro Public-Web App

**Decision**: Build the expanded public website inside `src/web/apps/public-web` using Astro static site generation.

**Rationale**: The repository already has a static Astro public website with build diagnostics, app-local tests, Cloudflare Pages infrastructure, and a documented deployment path. The feature is content-heavy and does not need backend runtime behavior.

**Alternatives considered**:

- Next.js product-app pattern: rejected because this public site is already Astro and does not need Relay, authentication, or product app runtime dependencies.
- Separate CMS or external content platform: rejected for this slice because the spec requires repository-owned content, full migration inventories, and static deployment.

## Decision: Use Content/Data Files for Pages, Routes, and Inventories

**Decision**: Represent site pages, resources, comparison pages, pricing, redirects, URL destinations, and draft coverage as structured content/data files under the public-web app.

**Rationale**: The source draft is large, and the feature requires complete coverage decisions. Structured content keeps page rendering, inventories, tests, and review checklists aligned without duplicating route metadata across templates.

**Alternatives considered**:

- Hardcoded page content directly in `.astro` pages: rejected because it would make full draft coverage, blog/support migration, and comparison pages harder to test and review.
- Runtime content API: rejected because the public website remains static and does not need an online data dependency.

## Decision: Keep Booking/Search Outbound for This Feature

**Decision**: The public website presents search and booking entry points, but all booking/search actions forward to the separate application website.

**Rationale**: The user clarified that the public website and app website are on separate domains and direct booking is not available on the public website today. Planning direct booking would incorrectly expand scope into product/runtime integration.

**Alternatives considered**:

- Direct public-site booking: rejected because it is not available today and would require application/backend integration beyond this feature.
- Static content only with no search entry point: rejected because the draft positions public workspace discovery as the home page's primary story.

## Decision: Require Three Public Destination URL Variables

**Decision**: Extend the current public URL environment-variable pattern to exactly three required public destination URLs:

- `PUBLIC_SKEDULAR_APP_URL` for app/search/booking destination actions.
- `PUBLIC_SKEDULAR_SIGNUP_URL` for sign-up/login destination actions, preserving the existing variable pattern.
- `PUBLIC_SKEDULAR_DEMO_URL` for demo/contact sales destination actions.

**Rationale**: The user clarified that staging and production use different values and no destination domains should be hardcoded. The existing app already validates `PUBLIC_SKEDULAR_SIGNUP_URL`; the same clear-failure behavior should apply to all required outbound destinations.

**Alternatives considered**:

- One generic CTA URL: rejected because the expanded site has distinct booking/app, login/sign-up, and demo/contact actions.
- Hardcoded staging/production domains: rejected by clarification and would make environment changes require code edits.
- More than three URL variables: rejected for the first implementation because the spec explicitly calls for three required public destination variables.

## Decision: Publish All Current Blog/Support Content in First Implementation

**Decision**: Fully migrate every current public blog and support page into the first implementation, using published pages, rewritten pages, merged published equivalents, or redirects to published replacements.

**Rationale**: The user selected full first-implementation migration. This protects existing search traffic and avoids leaving old public URLs without an intentional destination.

**Alternatives considered**:

- Inventory first and backlog some pages: rejected by user clarification.
- Defer all blog/support content: rejected because it would lose current public website value and conflicts with the clarified scope.

## Decision: Publish Draft Pricing Values and Host Commission Range

**Decision**: Publish the pricing model and suggested values from the draft in the first implementation, while keeping pricing content centralized and easy to revise.

**Rationale**: The user selected publishing the draft's suggested pricing amounts and host commission range. Centralizing pricing content reduces the risk of inconsistent values across pages.

**Alternatives considered**:

- Contact/demo only pricing: rejected by user clarification.
- Hide pricing entirely: rejected because pricing is part of the draft and clarified launch scope.

## Decision: Publish All Draft Comparison Pages

**Decision**: Publish all comparison-page candidates from the draft in the first implementation.

**Rationale**: The user selected first-implementation publication. Comparison pages support high-intent SEO queries and are part of the draft's search strategy.

**Alternatives considered**:

- Prepare structure only and delay publication: rejected by user clarification.
- Exclude comparison pages: rejected because the draft explicitly includes them and the user selected publication.

## Decision: Use Neutral, Reviewed Competitor Positioning

**Decision**: Comparison pages must use neutral positioning and only factual or review-approved competitor claims.

**Rationale**: Publishing comparison pages creates reputational and legal risk if claims are unsupported or aggressive. Neutral language lets the site publish required pages while keeping claims defensible.

**Alternatives considered**:

- Aggressive comparison copy: rejected because it increases risk and conflicts with the friendly professional voice requirement.
- Omitting unverifiable pages: rejected because all draft comparison pages are in first-implementation scope.

## Decision: Treat Future Features as Planning Items Unless Verified Current

**Decision**: Future sections such as community features, native mobile apps, AI recommendations, forecasting, and AI analytics must be inventoried and routed to future planning, not presented as currently available unless separately verified.

**Rationale**: The spec requires factual product claims and forbids publishing future capabilities as current state.

**Alternatives considered**:

- Include all draft items as current capabilities: rejected because it would overstate product readiness.
- Drop future items entirely: rejected because the draft coverage requirement needs explicit decisions for every section.

## Decision: Use Static Metadata and Structured Data

**Decision**: Generate page-level titles, descriptions, canonical paths, social metadata, breadcrumbs, and structured-data candidates from content data.

**Rationale**: The draft requires SEO and AI discoverability, including machine-readable hierarchy and structured data. Static generation keeps output inspectable and deployable on CDN hosting.

**Alternatives considered**:

- Manual per-page metadata only: rejected because the page count is large and drift-prone.
- Runtime SEO generation: rejected because the site should remain static.

## Decision: Validate Content Quality with Automated and Manual Checks

**Decision**: Combine automated checks for route/link/env/build/accessibility basics with manual review for factual accuracy, human-written tone, competitor claims, pricing, and complete draft coverage.

**Rationale**: Some requirements are machine-testable, but human-quality editorial voice and factual claim review require manual approval.

**Alternatives considered**:

- Automated-only validation: rejected because it cannot reliably assess factual accuracy or non-generic human tone.
- Manual-only validation: rejected because route coverage, environment variables, metadata, and accessibility regressions are better caught automatically.
